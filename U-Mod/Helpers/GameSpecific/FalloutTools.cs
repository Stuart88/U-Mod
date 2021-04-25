using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace U_Mod.Helpers.GameSpecific
{
    public static class FalloutTools
    {
        public static bool CopyCustomIniFiles()
        {
            try
            {
                FileInfo falloutDefault = new FileInfo(Path.Combine(Directory.GetCurrentDirectory(), "Assets/Fallout/IniFiles/Fallout_default.ini"));
                falloutDefault.CopyTo(Path.Combine(FileHelpers.GetGameFolder(), "Fallout_default.ini"), overwrite: true);

                FileInfo falloutIni = new FileInfo(Path.Combine(Directory.GetCurrentDirectory(), "Assets/Fallout/IniFiles/Fallout.ini"));
                falloutIni.CopyTo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "My Games", "Fallout3", "Fallout.ini"), overwrite: true);

                FileInfo falloutPrefs = new FileInfo(Path.Combine(Directory.GetCurrentDirectory(), "Assets/Fallout/IniFiles/FalloutPrefs.ini"));
                falloutPrefs.CopyTo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "My Games", "Fallout3", "FalloutPrefs.ini"), overwrite: true);

                return true;
            }
            catch (Exception e)
            {
                Logging.Logger.LogException("CopyCustomIniFiles (Fallout)", e);
                GeneralHelpers.ShowExceptionMessageBox(e);
                return false;
            }
        }
    }
}
