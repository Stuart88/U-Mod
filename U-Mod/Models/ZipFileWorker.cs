using U_Mod.Extensions;
using U_Mod.Helpers;
using SevenZipExtractor;
using SharpCompress.Archives;
using SharpCompress.Common;
using SharpCompress.Readers;
using System;
using System.IO;
using System.Linq;
using U_Mod.Shared.Models;

namespace U_Mod.Models
{
    public class FileExtractResult
    {
        #region Public Properties

        public bool DownloadOk { get; set; } = true;
        public string FilePath { get; set; }
        public string ManualDownloadUrl { get; set; }
        public string Message { get; set; }
        public ModZipFile ModZipFile { get; set; }
        public ZipFileWorker.FileExtractResultEnum Result { get; set; } = ZipFileWorker.FileExtractResultEnum.Ok;
        public bool TransferOk { get; set; } = true;
        public bool UnzipOk { get; set; } = true;

        #endregion Public Properties
    }

    public class UnzipProgressEventArgs : EventArgs
    {
        #region Public Properties

        public int Count { get; set; }
        public string Text { get; set; }
        public int Total { get; set; }

        #endregion Public Properties
    }

    /// <summary>
    /// The worker unit to handling the extraction and processing of mod files
    /// </summary>
    public class ZipFileWorker
    {
        #region Private Fields

        private int _loopCount = 0;
        private object zipLock = new object();

        #endregion Private Fields

        #region Private Properties

        private string ExtractDirectory { get; set; }
        private FileExtractResult ExtractResult { get; set; }
        private ModZipFile ModZipFile { get; set; }
        private bool ShouldSendProgressEvents { get; set; }
        private string ZipPath { get; set; }

        #endregion Private Properties

        #region Public Enums

        public enum FileExtractResultEnum
        {
            Ok,
            Fail,
            AlreadyDone,
            Unknown
        }

        #endregion Public Enums

        #region Public Constructors

        public ZipFileWorker(string path, ModZipFile zip)
        {
            this.ZipPath = path;
            this.ModZipFile = zip;
            this.ExtractResult = new FileExtractResult()
            {
                Result = FileExtractResultEnum.Unknown,
                FilePath = this.ZipPath,
                ModZipFile = this.ModZipFile
            };

            try
            {
                this.ExtractDirectory = FileHelpers.GetFileExtractionTempFolderPath();

                if (!Directory.Exists(this.ExtractDirectory))
                {
                    Directory.CreateDirectory(this.ExtractDirectory);
                }
            }
            catch (Exception e)
            {
                Logging.Logger.LogException("ZipFileWorker (constructor)", e);
            }
        }

        #endregion Public Constructors

        #region Public Events

        public event EventHandler UnzipProgressed;

        #endregion Public Events

        #region Public Methods

        /// <summary>
        /// Unzips file and places in the Temp folder for the selected game
        /// </summary>
        public FileExtractResult BeginUnzip()
        {
            try
            {
                this.ExtractResult.UnzipOk = ExtractFile(this.ZipPath);
            }
            catch (Exception e)
            {
                Logging.Logger.LogException("BeginUnzip (1)", e);
                this.ExtractResult.Result = FileExtractResultEnum.Fail;
                this.ExtractResult.Message = $"File extraction failure for:\nFile: {this.ZipPath}\n\nError: {e.Message}";
                this.ExtractResult.UnzipOk = false;
                this.ExtractResult.TransferOk = false;
                return this.ExtractResult;
            }

            if (this.ExtractResult.Result == FileExtractResultEnum.Ok)
            {
                try
                {
                    TransferToFinalLocation(this.ModZipFile);
                }
                catch (Exception e)
                {
                    Logging.Logger.LogException("BeginUnzip (2)", e);

                    this.ExtractResult.Result = FileExtractResultEnum.Fail;
                    this.ExtractResult.Message = $"Extraction okay but file transfer failed for:\nFile: {this.ZipPath}\n\nError: {e.Message}";
                    this.ExtractResult.TransferOk = false;
                    return this.ExtractResult;
                }
            }

            return this.ExtractResult;
        }

        /// <summary>
        /// Primary file extraction method
        /// </summary>
        /// <param name="inFile"></param>
        /// <param name="outDirectory"></param>
        public void UncompressZip(string inFile, string outDirectory)
        {
            // Not sure if this lock will help but something somewhere is causing very strange phantom directories and files called "**" to be written to
            // VS solution root. No idea why. Trying everything.
            try
            {
                if (ZipHelpers.IsExe(inFile))
                {
                    //Nothing to unzip so just move to outDirectory
                    File.Move(inFile, Path.Combine(outDirectory, Path.GetFileName(inFile)));
                    return;
                }

                lock (zipLock)
                {
                    using var archive = ArchiveFactory.Open(inFile);
                    IReader reader = archive.ExtractAllEntries();
                    reader.EntryExtractionProgress += Reader_EntryExtractionProgress;

                    while (reader.MoveToNextEntry())
                    {
                        string targetPath = "";

                        if (!reader.Entry.IsDirectory)
                        {
                            if (ZipHelpers.HasSubDirectory(reader.Entry.Key))
                            {
                                var subfolders = Path.Combine(outDirectory, reader.Entry.Key.Replace('/', '\\')).Split('\\');

                                targetPath = string.Join('\\', subfolders[..^1]);
                            }
                            else
                            {
                                targetPath = outDirectory;
                            }

                            if (ZipHelpers.HasSubDirectory(reader.Entry.Key) && !Directory.Exists(targetPath))
                                // If key has '/' it represents a directory, so need to make it.
                                // Otherwise it's zip root don't make dir
                                Directory.CreateDirectory(targetPath);

                            reader.WriteEntryToDirectory(targetPath, new ExtractionOptions() { ExtractFullPath = false, Overwrite = true });
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logging.Logger.LogException("UncompressZip (only worry about this if the next logged exception is UncompressZipFallback!)", e);
                UncompressZipFallback(inFile, outDirectory);
            }
        }

        /// <summary>
        /// Slower but more reliable unzipper, use as secondary option if the main unzipper fails
        /// </summary>
        /// <param name="inFile"></param>
        /// <param name="outDirectory"></param>
        public void UncompressZipFallback(string inFile, string outDirectory)
        {
            _loopCount = 0;
            // Not sure if this lock will help but something somewhere is causing very strange phantom directories and files called "**" to be written to
            // VS solution root. No idea why. Trying everything.
            lock (zipLock)
            {
                using var archive = new ArchiveFile(inFile);

                int total = archive.Entries.Count;
                for (int i = 0; i < total; i++)
                {
                    var entry = archive.Entries[i];
                    _loopCount++;

                    try
                    {
                        if (ShouldSendProgressEvents)
                        {
                            OnUnzipProgressed(new UnzipProgressEventArgs
                            {
                                Text = $"{GetProgressBarTextFallbackZipper()}",
                                Count = i,
                                Total = total
                            });
                        }

                        entry.Extract(Path.Combine(outDirectory, entry.FileName));
                    }
                    catch (Exception e)
                    {
                        Logging.Logger.LogException("UncompressZipFallback", e);
                    }
                }
            }
        }

        #endregion Public Methods

        #region Protected Methods

        protected virtual void OnUnzipProgressed(UnzipProgressEventArgs e)
        {
            EventHandler handler = UnzipProgressed;
            handler?.Invoke(this, e);
        }

        #endregion Protected Methods

        #region Private Methods

        private bool ExtractFile(string zipPath)
        {
            string extractionFolderName = ZipHelpers.GetExtractedZipFolderName(zipPath);

            string extractTo = Path.Combine(this.ExtractDirectory, extractionFolderName);

            //Assuming this is a bad idea because an existing directory does not necessarily mean all required files exist
            //if (Directory.Exists(extractTo)) //Extraction already done?
            //{
            //    this.ExtractResult.Result = FileExtractResultEnum.AlreadyDone;
            //    //return true;
            //}

            _ = Directory.CreateDirectory(extractTo);

            //if (Path.GetExtension(zipPath).ToLower() == ".zip")
            //{
            //    // Use standard Micrisoft zip extraction
            //    ZipFile.ExtractToDirectory(zipPath, extractTo);
            //}
            //else
            //{
            //    ShouldSendProgressEvents = true;
            //    UncompressZip(zipPath, extractTo);
            //}

            ShouldSendProgressEvents = true;
            UncompressZip(zipPath, extractTo);
            this.ExtractResult.Result = FileExtractResultEnum.Ok;
            return true;
        }

        private string GetProgressBarText(string zipEntry)
        {
            // Gets folder name
            string s = zipEntry;

            //Get folder path. Cuts off individual file names because they appear too quickly in progress bar text.

            if (zipEntry.Contains('\\') && zipEntry.Split('\\').Last().Contains('.')) // if it's an individual file the last part of the parth will have a dot in it somewhere...
            {
                s = string.Join('\\', zipEntry.Split('\\')[..^1]);
            }
            if (zipEntry.Contains('/') && zipEntry.Split('/').Last().Contains('.'))
            {
                s = string.Join('/', zipEntry.Split('/')[..^1]);
            }

            return s.Length > 30
                ? $"...{s.Substring(s.Length - 30, 30)}"
                : s;
        }

        //private string GetProgressBarText
        private string GetProgressBarTextFallbackZipper()
        {
            string fileName = FileHelpers.RemoveFileExtension(this.ModZipFile.FileName).LettersAndSpacesOnly();
            string progressBarText = fileName.Length > 30
                ? fileName.Substring(0, 27) + "..."
                : fileName;

            return progressBarText;
        }

        private void Reader_EntryExtractionProgress(object sender, ReaderExtractionEventArgs<IEntry> e)
        {
            OnUnzipProgressed(new UnzipProgressEventArgs
            {
                Text = $"{GetProgressBarText(e.Item.Key)}",
                Count = e.ReaderProgress?.PercentageRead ?? 0,
                Total = 100
            });
        }

        private void TransferToFinalLocation(ModZipFile f)
        {
            string targetFolder = FileHelpers.GetFullExtractionTargetPath(f.ExtractLocation);
            string tempExtractedFolder = Path.Combine(FileHelpers.GetFileExtractionTempFolderPath(), ZipHelpers.GetExtractedZipFolderName(f.FileName));

            foreach (ModZipContent c in f.Content.OrderBy(item => item.InstallOrder))
            {
                // Masterlist entries take the form "folder\subfolder\item.ext"
                // and some paths contain zips, e.g. "folder\subfolder.zip\item.ext"
                // So first clean up and convert to useable path
                string contentFileName = c.FileName.Replace('>', '\\');
                string[] cleaning = contentFileName.Split('\\');
                string cleanedFilepath = "";
                for (int i = 0; i < cleaning.Length; i++)
                {
                    cleaning[i] = cleaning[i].Trim();
                    cleanedFilepath = Path.Combine(cleanedFilepath, cleaning[i]);

                    //May need to unzip another zip file along the way..
                    // e.g. if extraction location is folder/zippedFolder.zip/otherFolder/file.ext
                    if (FileHelpers.IsZip(cleanedFilepath))
                    {
                        string zipLocation = Path.Combine(tempExtractedFolder, cleanedFilepath);
                        string extractLocation = FileHelpers.RemoveFileExtension(zipLocation);

                        if (!Directory.Exists(extractLocation))
                            Directory.CreateDirectory(extractLocation);

                        ShouldSendProgressEvents = false;
                        UncompressZip(zipLocation, extractLocation);
                        //extracted here, so now can lose zip extension and use directory name instead

                        cleanedFilepath = FileHelpers.RemoveFileExtension(cleanedFilepath);
                    }
                }

                // "*" for the path means it's the root folder and should just take all.
                string tempPath = cleanedFilepath == "*"
                    ? tempExtractedFolder
                    : Path.Combine(tempExtractedFolder, cleanedFilepath);

                if (File.Exists(tempPath))
                {
                    // it's a file
                    string fileName = cleanedFilepath.Split('\\').Last();
                    string targetFilePath = Path.Combine(targetFolder, fileName);
                    MoveFile(Path.Combine(tempExtractedFolder, cleanedFilepath), targetFilePath);
                    Directory.Delete(tempExtractedFolder, true);
                }
                else if (Directory.Exists(tempPath))
                {
                    // it's a folder

                    //e.g.  if tempPath is folder/subFolder/Textures
                    //      we're moving it to Oblivion/Data/Textures ... so get "Textures" from end of tempPath and append to targetFolder path.
                    string targetDirectory = Path.Combine(targetFolder, tempPath.Split('\\').Last());
                    FileHelpers.MoveDirectory(tempPath, targetDirectory);
                    Directory.Delete(tempPath, true);
                }
                else
                {
                    this.ExtractResult.UnzipOk = false;
                    this.ExtractResult.Result = FileExtractResultEnum.Fail;
                    this.ExtractResult.Message = $"Given path is not a known file or directory! {tempPath}";
                    return;
                }

                void MoveFile(string source, string target)
                {
                    string targetDir = FileHelpers.GetDirectoryPath(target);

                    if (!Directory.Exists(targetDir))
                        Directory.CreateDirectory(targetDir);

                    if (File.Exists(target))
                        File.Delete(target);

                    File.Move(source, target);
                }
            }

            
        }

        #endregion Private Methods
    }
}