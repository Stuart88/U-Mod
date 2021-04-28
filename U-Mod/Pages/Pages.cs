using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using U_Mod.Enums;
using U_Mod.Pages.InstallBethesda;

namespace U_Mod.Pages
{
    public static class Pages
    {

        private static Lazy<UserControl> _mainMenu = new Lazy<UserControl>(() => new General.MainMenu());
        private static Lazy<UserControl> _home = new Lazy<UserControl>(() => new General.Home());
        private static Lazy<UserControl> _nexusLogin = new Lazy<UserControl>(() => new NexusLoginPage());
        private static Lazy<UserControl> _1GameFolderSelect = new Lazy<UserControl>(() => new _1GameFolderSelect());
        private static Lazy<UserControl> _2ModList = new Lazy<UserControl>(() => new _2ModList());
        private static Lazy<UserControl> _3ManualDownload = new Lazy<UserControl>(() => new _3ManualDownload());
        private static Lazy<UserControl> _4PatchAndModManager = new Lazy<UserControl>(() => new _4_PatchAndModManager());
        private static Lazy<UserControl> _options = new Lazy<UserControl>(() => new General.Options());

        public static UserControl GetPage(PagesEnum page, bool refreshInstance)
        {
            switch (page)
            {
                case PagesEnum.MainMenu:
                    if (refreshInstance)
                        _mainMenu = new Lazy<UserControl>(() => new General.MainMenu());
                    return _mainMenu.Value;

                case PagesEnum.Home:
                    if (refreshInstance)
                        _home = new Lazy<UserControl>(() => new General.Home());
                    return _home.Value;

                case PagesEnum.AutoDownload:
                    if (refreshInstance)
                        _nexusLogin = new Lazy<UserControl>(() => new NexusLoginPage());
                    return _nexusLogin.Value;

                case PagesEnum.GameFolderSelect:
                    if (refreshInstance)
                        _1GameFolderSelect = new Lazy<UserControl>(() => new _1GameFolderSelect());
                    return _1GameFolderSelect.Value;

                case PagesEnum.ModsSelect:
                    if (refreshInstance)
                        _2ModList = new Lazy<UserControl>(() => new _2ModList());
                    return _2ModList.Value;

                case PagesEnum.ManualDownload:
                    if (refreshInstance)
                        _3ManualDownload = new Lazy<UserControl>(() => new _3ManualDownload());
                    return _3ManualDownload.Value;

                case PagesEnum.PatchAndModManager:
                    if (refreshInstance)
                        _4PatchAndModManager = new Lazy<UserControl>(() => new _4_PatchAndModManager());
                    return _4PatchAndModManager.Value;

                case PagesEnum.Options:
                    if (refreshInstance)
                        _options = new Lazy<UserControl>(() => new General.Options());
                    return _options.Value;

                default:
                    throw new NotImplementedException("Pages.GetPage() - Page not found!");
            }

        }
    }
}
