using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using ByteSizeLib;
using SevenZipExtractor;
using AmgShared.Helpers;
using AmgShared.Models;
using SharpCompress.Archives;
using SharpCompress.Readers;
using AMGWebsite.Shared.Models;

namespace U_Mod.Helpers
{
    public static class ZipHelpers
    {
        #region Public Methods

        /// <summary>
        /// Gets the folder name that a zip would be automatically extracted to. e.g. /folder/subfolder/MyMod.zip would extract to a folder named MyMod
        /// </summary>
        /// <param name="zipPath"></param>
        /// <returns></returns>
        public static string GetExtractedZipFolderName(string zipPath)
        {
            // e.g. this turns
            // c:/folder/subfolder/modName.zip
            // into
            // c:/folder/subfolder/modName
            string filePathNoExt = zipPath.ReplaceLastOccurrenceOf(Path.GetExtension(zipPath), "");

            // Then take last part of path to get folder name to extract zip file into
            // i.e. from c:/folder/subfolder/modName
            // you get "modName"
            string extractionFolderName = filePathNoExt.Split('\\').Last();

            return extractionFolderName;
        }

        public static bool Is7Zip(string path)
        {
            return path.EndsWith("7z");
        }

        public static bool IsRar(string path)
        {
            return path.EndsWith("rar");
        }

        public static bool IsZip(string path)
        {
            return path.EndsWith("zip");
        }

        public static ZipFileType GetZipFileType(string path)
        {
            if (Is7Zip(path))
                return ZipFileType._7z;

            if (IsRar(path))
                return ZipFileType.Rar;

            if (IsZip(path))
                return ZipFileType.Zip;

            return ZipFileType.Unknown;
        }



        #endregion Public Methods

        #region Private Methods

        public static bool HasSubDirectory(string key)
        {
            return key.Contains('/') || key.Contains('\\');
        }

        public static bool VerifiyZip(string zipPath, ModZipFile modZipFile, out string message)
        {
            try
            {
                message = "Verifying...";

                // Basic existence check...
                if(!File.Exists(zipPath))
                {
                    message = "File not found!";
                    return false;
                }

                try
                {
                    message = "On File size Check";
                    //Now check against file size in masterlist
                    FileInfo fileInfo = new FileInfo(zipPath);
                    ByteSize sizeInKb = ByteSize.FromBytes(fileInfo.Length);

                    double displaySize = Math.Ceiling(sizeInKb.KibiBytes); // File explorer rounds up, so this value will match what was seen when making masterlist

                    if (Math.Abs(displaySize - modZipFile.SizeinKb) > 0.01 * modZipFile.SizeinKb
                        && modZipFile.SizeinKb > 0) // also bypass if expected file size is 0
                    {
                        // Downloaded file is not within 99% tolerance of expected file size
                        message = $"File not fully downloaded! (Size {displaySize}kb; Expected {modZipFile.SizeinKb}kb)";
                        return false;
                    }
                }
                catch (Exception e)
                {
                    // Let this section safely fail and move on as it's only one part of the check. Second part can still do the job.
                    // This is being bypased because one user seems to be crashing on zip verification... Not sure which part...
                    Logging.Logger.LogException("VerifiyZip (1)", e);
                }


                switch (GetZipFileType(zipPath))
                {
                    case ZipFileType.Zip:
                    {
                        message = "On Zip";
                        using var zip = ZipFile.OpenRead(zipPath);

                        //this first check accounts for folders in folders, and the rar zip-in-zip situation
                        foreach (var entry in zip.Entries)
                        {
                            if (modZipFile.Content.Any(c => c.FileName.ToLower().StartsWith(entry.FullName.ToLower().Replace('/', '\\'))))
                            {
                                message = "";
                                return true;
                            }
                        }
                        //Now just check against all entries
                        if (zip.Entries.Any(e => modZipFile.Content.Any(c => c.FileName.ToLower().StartsWith(e.Name.ToLower()))))
                        {
                            message = "";
                            return true;
                        }
                        else
                        {
                            message = "No entries matched! (Zip)";
                            return false;
                        }
                    }

                    case ZipFileType.Rar:
                    {
                        message = "On Rar";
                            // this block would also work for 7z but it's not quite as reliable, so let's just use for the occasional rar files

                            using var archive = ArchiveFactory.Open(zipPath);
                        IReader reader = archive.ExtractAllEntries();

                        while (reader.MoveToNextEntry())
                        {
                            if (reader.Entry.Key.Replace('/', '\\').Split('\\').Any(e => modZipFile.Content.Any(c => c.FileName.ToLower().StartsWith(e.ToLower()))))
                            {
                                message = "";
                                return true;
                            }
                        }

                        message = "No entries matched! (Rar)";
                        return false;
                    }

                    case ZipFileType._7z:
                    {
                        message = "On 7z"; 
                        using var archive = new ArchiveFile(zipPath);
                        if (archive.Entries.Any(e => modZipFile.Content.Any(c => c.FileName.ToLower().StartsWith(e.FileName.ToLower()))))
                        {
                            message = "";
                            return true;
                        }
                        else
                        {
                            message = "No entries matched!";
                            return false;
                        }
                    }

                    default:
                        message = "Unknown zip type! (7z)";
                        return false;
                }
            }
            catch (Exception e)
            {
                Logging.Logger.LogException("VerifiyZip (2)", e);
                message = AmgShared.Helpers.StringHelpers.ErrorMessage(e);
                return false;
            }
        }

        #endregion Private Methods
    }
}