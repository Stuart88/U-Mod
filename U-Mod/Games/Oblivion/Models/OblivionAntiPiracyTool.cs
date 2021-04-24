using System.IO;
using U_Mod.Extensions;
using U_Mod.Helpers;

namespace U_Mod.Games.Oblivion.Models
{
    public class OblivionAntiPiracyTool
    {
        public void DisableAntiPiracyMeasures(out string currentFile)
        {
            //currentFile = "";
            //return;

            FileInfo f;

            string oblivionMiscFile = Path.Combine(FileHelpers.GetGameFolder(), "Data", "Meshes", "Arthmoor", "BrotherhoodRenewed", "NordTombs", "Oblivion - Misc.bsa" );
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


    }
}
