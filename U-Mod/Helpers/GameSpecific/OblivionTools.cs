using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using U_Mod.Extensions;

namespace U_Mod.Helpers.GameSpecific
{
    public class OblivionTools
    {
        #region Anti-Piracy

        public void DisableAntiPiracyMeasures(out string currentFile)
        {
            //currentFile = "";
            //return;

            FileInfo f;

            string oblivionMiscFile = Path.Combine(FileHelpers.GetGameFolder(), "Data", "Meshes", "Arthmoor", "BrotherhoodRenewed", "NordTombs", "Oblivion - Misc.bsa");
            currentFile = Path.Combine(FileHelpers.GetGameFolder(), "Data", "Oblivion - Misc.bsa");

            if (!File.Exists(currentFile) && File.Exists(oblivionMiscFile)) // if file exists, this move has already been done!
            {
                f = new FileInfo(oblivionMiscFile);
                f.MoveTo(currentFile);
            }

            string obseFile = Path.Combine(FileHelpers.GetGameFolder(), "obse_1_2_41.dll");
            currentFile = Path.Combine(FileHelpers.GetGameFolder(), "obse_1_2_416.dll");

            if (!File.Exists(currentFile) && File.Exists(obseFile))// if file exists, this move has already been done!
            {
                f = new FileInfo(obseFile);
                f.MoveTo(currentFile);
            }

            string texturesFile = Path.Combine(FileHelpers.GetGameFolder(), "Data", "Oblivion - Textures.bsa");
            currentFile = Path.Combine(FileHelpers.GetGameFolder(), "Data", "Oblivion - Textures - Compressed.bsa");

            if (!File.Exists(currentFile) && File.Exists(texturesFile))// if file exists, this move has already been done!
            {
                f = new FileInfo(texturesFile);
                f.MoveTo(currentFile);
            }
        }

        public void ObseIniFileDisableAntiPiracyEdit(out string currentFile)
        {
            //currentFile = "";
            //return;

            string ini = "sr_Oblivion_Stutter_Remover.ini";
            string iniFileLocation = Path.Combine(FileHelpers.GetGameFolder(), "Data", "OBSE", "Plugins", ini);
            currentFile = iniFileLocation;
            string tempLocation = Path.Combine(FileHelpers.GetFileExtractionTempFolderPath(), ini);

            if (File.Exists(iniFileLocation)) // enbseries is part of non-essential mod so might not exist
            {
                if (File.Exists(tempLocation))
                    File.Delete(tempLocation);

                using (StreamWriter sw = File.CreateText(tempLocation))
                {
                    foreach (var line in File.ReadAllLines(iniFileLocation))
                    {
                        string lineText = line switch
                        {
                            { } s when s.ToIniEditString().StartsWith("fMaximumFPS=") => "	fMaximumFPS =  60",
                            _ => line
                        };

                        sw.WriteLine(lineText);
                    }
                }

                File.Delete(iniFileLocation);
                File.Move(tempLocation, iniFileLocation);
            }
        }

        public void EnableAntiPiracyMeasures(out string currentFile)
        {
            currentFile = "";
            return;

            FileInfo f;

            string oblivionMiscFile = Path.Combine(FileHelpers.GetGameFolder(), "Data", "Oblivion - Misc.bsa");
            string nordTombsDir = Path.Combine(FileHelpers.GetGameFolder(), "Data", "Meshes", "Arthmoor", "BrotherhoodRenewed", "NordTombs");
            currentFile = Path.Combine(FileHelpers.GetGameFolder(), nordTombsDir, "Oblivion - Misc.bsa");

            if (!File.Exists(currentFile) && File.Exists(oblivionMiscFile)) // if file exists, this move has already been done!
            {
                if (!Directory.Exists(nordTombsDir))
                    Directory.CreateDirectory(nordTombsDir);

                f = new FileInfo(oblivionMiscFile);
                f.MoveTo(currentFile);
            }

            if (Static.StaticData.UserDataStore.OblivionUserData.IsSteamGame)
            {
                string obseFile = Path.Combine(FileHelpers.GetGameFolder(), "obse_1_2_416.dll");
                currentFile = Path.Combine(FileHelpers.GetGameFolder(), "obse_1_2_41.dll");

                if (!File.Exists(currentFile) && File.Exists(obseFile))// if file exists, this move has already been done!
                {
                    f = new FileInfo(obseFile);
                    f.MoveTo(currentFile);
                }
            }


            string texturesFile = Path.Combine(FileHelpers.GetGameFolder(), "Data", "Oblivion - Textures - Compressed.bsa");
            currentFile = Path.Combine(FileHelpers.GetGameFolder(), "Data", "Oblivion - Textures.bsa");

            if (!File.Exists(currentFile) && File.Exists(texturesFile))// if file exists, this move has already been done!
            {
                f = new FileInfo(texturesFile);
                f.MoveTo(currentFile);
            }

        }

        public void ObseIniFileEnableAntiPiracyEdit(out string currentFile)
        {
            currentFile = "";
            return;

            if (Static.StaticData.UserDataStore.OblivionUserData.IsSteamGame)
            {
                string ini = "sr_Oblivion_Stutter_Remover.ini";
                string iniFileLocation = Path.Combine(FileHelpers.GetGameFolder(), "Data", "OBSE", "Plugins", ini);
                currentFile = iniFileLocation;
                string tempLocation = Path.Combine(FileHelpers.GetFileExtractionTempFolderPath(), ini);

                if (File.Exists(iniFileLocation)) // enbseries is part of non-essential mod so might not exist
                {
                    if (File.Exists(tempLocation))
                        File.Delete(tempLocation);

                    using (StreamWriter sw = File.CreateText(tempLocation))
                    {
                        foreach (var line in File.ReadAllLines(iniFileLocation))
                        {
                            string lineText = line switch
                            {
                                { } s when s.ToIniEditString().StartsWith("fMaximumFPS=") => "	fMaximumFPS =  5",
                                _ => line
                            };

                            sw.WriteLine(lineText);
                        }
                    }

                    File.Delete(iniFileLocation);
                    File.Move(tempLocation, iniFileLocation);
                }
            }

        }

        #endregion

        #region Post Install File Edits

        public string MovingToDir { get; internal set; }

        public void PerformFileEdits(out string currentFile)
        {

            string dataDirectory = Path.Combine(FileHelpers.GetGameFolder(), "Data");

            string bookMenuFile = Path.Combine(dataDirectory, "book_menu (vanilla-or-BTmod).xml");
            currentFile = Path.Combine(dataDirectory, "book_menu.xml");

            if (File.Exists(bookMenuFile))
            {
                FileInfo f = new FileInfo(bookMenuFile);
                f.MoveTo(currentFile, true);
            }


            string akaviriFile = Path.Combine(dataDirectory, "Akaviri imports.esp");
            currentFile = Path.Combine(dataDirectory, "Akaviri_imports.esp");

            if (File.Exists(akaviriFile))
            {
                FileInfo f = new FileInfo(akaviriFile);
                f.MoveTo(currentFile, true);
            }
        }


        #endregion
    }
}
