using System.IO;
using U_Mod.Helpers;

namespace U_Mod.Games.Oblivion.Models
{
    public class OblivionPostInstallFileEditor
    {
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

        
    }
}
