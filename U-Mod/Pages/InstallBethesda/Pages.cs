using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using U_Mod.Enums;

namespace U_Mod.Pages.InstallBethesda
{
    public static class Pages
    {
        private static Lazy<UserControl> _oblivionMenu = new Lazy<UserControl>(() => new Games.Oblivion.Pages.MainMenu());
        private static Lazy<UserControl> _falloutMenu= new Lazy<UserControl>(() => new Games.Fallout.Pages.MainMenu());
        private static Lazy<UserControl> _nexusLogin = new Lazy<UserControl>(() => new NexusLoginPage());
        private static Lazy<UserControl> _1GameFolderSelect = new Lazy<UserControl>(() => new _1GameFolderSelect());
        private static Lazy<UserControl> _2ModList = new Lazy<UserControl>(() => new _2ModList());
        private static Lazy<UserControl> _3ManualDownload = new Lazy<UserControl>(() => new _3ManualDownload());
        private static Lazy<UserControl> _4PatchAndModManager = new Lazy<UserControl>(() => new _4_PatchAndModManager());
        private static Lazy<UserControl> _options = new Lazy<UserControl>(() => new General.Options());

        public static UserControl GetPage(PagesEnum page, bool refreshInstance)
        {

            return page switch
            {
                PagesEnum.OblivionMainMenu => !refreshInstance ? _oblivionMenu.Value : new Games.Oblivion.Pages.MainMenu(),
                PagesEnum.FalloutMainMenu => !refreshInstance ? _falloutMenu.Value : new Games.Fallout.Pages.MainMenu(),
                PagesEnum.GameFolderSelect => !refreshInstance ? _1GameFolderSelect.Value : new _1GameFolderSelect(),
                PagesEnum.ModsSelect => !refreshInstance ? _2ModList.Value : new _2ModList(),
                PagesEnum.ManualDownload => !refreshInstance ? _3ManualDownload.Value : new _3ManualDownload(),
                PagesEnum.AutoDownload => !refreshInstance ? _nexusLogin.Value : new NexusLoginPage(),
                PagesEnum.PatchAndModManager => !refreshInstance ? _4PatchAndModManager.Value : new _4_PatchAndModManager(),
                PagesEnum.Options => !refreshInstance? _options.Value : new General.Options(),

                _ => throw new NotImplementedException(),
            };
        }
    }
}
