using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using U_Mod.Shared.Enums;
using U_Mod.Shared.Models;
using U_Mod.Shared.Helpers;

namespace U_Mod.Helpers
{
    public static class FileHelpers
    {
        #region Private Fields

        private static string[] ZipTypes = { ".7z", ".zip", ".rar" };

        #endregion Private Fields

        #region Public Methods

        public static void EmptyDirectory(this DirectoryInfo dir)
        {
            foreach (var d in dir.EnumerateDirectories())
            {
                d.Delete(true);
            }

            foreach (var f in dir.EnumerateFiles())
            {
                f.Delete();
            }
        }

        public static string GetUModFolder()
        {
            return Static.StaticData.CurrentGame switch
            {
                GamesEnum.Oblivion => string.IsNullOrEmpty(Static.StaticData.AppData.OblivionGameFolder) ? "" : Path.Combine(Static.StaticData.AppData.OblivionGameFolder, Static.Constants.UMod),
                GamesEnum.Fallout => string.IsNullOrEmpty(Static.StaticData.AppData.FalloutGameFolder) ? "" : Path.Combine(Static.StaticData.AppData.FalloutGameFolder, Static.Constants.UMod),
                _ => throw new NotImplementedException()
            };
        }

        /// <summary>
        /// Gets the directory path of a file. Necessary for moving files to new directorys that do not yet exist.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string GetDirectoryPath(string filePath)
        {
            var pathParts = filePath.Split('\\');
            return string.Join('\\', pathParts[..^1]);
        }

        public static string GetFileExtractionTempFolderPath()
        {
            return Path.Combine(GetGameFolder(), "Temp");
        }

        public static string GetFullExtractionTargetPath(ExtractLocation extractLocation)
        {
            return extractLocation switch
            {
                ExtractLocation.Base => GetGameFolder(),
                ExtractLocation.Data => Path.Combine(GetGameFolder(), "Data"),
                ExtractLocation.DataTextures => Path.Combine(GetGameFolder(), "Data", "Textures"),
                ExtractLocation.DataMeshes => Path.Combine(GetGameFolder(), "Data", "Meshes"),
                ExtractLocation.DataObsePlugins => Path.Combine(GetGameFolder(), "Data", "OBSE", "Plugins"),
                ExtractLocation.DataFosePlugins => Path.Combine(GetGameFolder(), "Data", "FOSE", "Plugins"),
                ExtractLocation.DataMenus => Path.Combine(GetGameFolder(), "Data", "menus"),
                _ => throw new ArgumentOutOfRangeException(nameof(extractLocation), extractLocation, null)
            };
        }

        public static string GetGameFolder()
        {
            return Static.StaticData.CurrentGame switch
            {
                GamesEnum.Oblivion => Static.StaticData.AppData.OblivionGameFolder,
                GamesEnum.Fallout => Static.StaticData.AppData.FalloutGameFolder,
                _ => ""
            };
        }

        public static void SetGameFolder(string path)
        {
            switch (Static.StaticData.CurrentGame)
            {
                case GamesEnum.Oblivion: Static.StaticData.AppData.OblivionGameFolder = path; break;
                case GamesEnum.Fallout: Static.StaticData.AppData.FalloutGameFolder = path; break;
                case GamesEnum.NewVegas: Static.StaticData.AppData.FalloutNewVegasGameFolder = path; break;
                default: throw new NotImplementedException($"AppData does not have GameFolder property for {Enum.GetName(typeof(GamesEnum), Static.StaticData.CurrentGame)}.");
            }
        }

        public static bool IsZip(string path)
        {
            return ZipTypes.Any(z => z == Path.GetExtension(path));
        }

        public static T LoadFile<T>(string path) where T : new()
        {
            if (File.Exists(path))
            {
                return JsonSerializer.Deserialize<T>(File.ReadAllText(path));
            }
            else
            {
                return new T();
            }
        }

        public static void MoveDirectory(string source, string target)
        {
            var sourcePath = source.TrimEnd('\\', ' ');
            var targetPath = target.TrimEnd('\\', ' ');
            var files = Directory.EnumerateFiles(sourcePath, "*", SearchOption.AllDirectories)
                .GroupBy(Path.GetDirectoryName);
            foreach (var folder in files)
            {
                var targetFolder = folder.Key.Replace(sourcePath, targetPath);
                Directory.CreateDirectory(targetFolder);
                foreach (var file in folder)
                {
                    var targetFile = Path.Combine(targetFolder, Path.GetFileName(file));
                    if (File.Exists(targetFile)) File.Delete(targetFile);
                    File.Move(file, targetFile);
                }
            }
            
        }

        public static string RemoveFileExtension(string path)
        {
            return path.ReplaceLastOccurrenceOf(Path.GetExtension(path), "");
        }

        public static void SaveAsJson<T>(T item, string savePath)
        {
            try
            {
                string jsonString = JsonSerializer.Serialize(item);
                File.WriteAllText(savePath, jsonString);
            }
            catch (Exception e)
            {
                GeneralHelpers.ShowExceptionMessageBox(e);
                Logging.Logger.LogException("SaveAsJson",e);
            }
        }

        #endregion Public Methods
    }
}