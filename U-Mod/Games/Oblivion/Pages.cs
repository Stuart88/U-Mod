using System;
using System.Windows.Controls;
using U_Mod.Enums;
using U_Mod.Pages;

namespace U_Mod.Games.Oblivion.Pages
{
    public static class Pages
    {
        private static Lazy<UserControl> _menu = new Lazy<UserControl>(() => new Oblivion.Pages.MainMenu());
        private static Lazy<UserControl> _optionsMenu = new Lazy<UserControl>(() => new Oblivion.Pages.OblivionOptions());
        private static Lazy<UserControl> _nexusLogin = new Lazy<UserControl>(() => new NexusLoginPage());
        private static Lazy<UserControl> _install1 = new Lazy<UserControl>(() => new Install.Install1PreInstallVid());
        private static Lazy<UserControl> _install2 = new Lazy<UserControl>(() => new Install.Install2SelectGameFolder());
        private static Lazy<UserControl> _install3 = new Lazy<UserControl>(() => new Install.Install3PcSpecs());
        private static Lazy<UserControl> _install4 = new Lazy<UserControl>(() => new Install.Install4ModsList());
        private static Lazy<UserControl> _install5 = new Lazy<UserControl>(() => new Install.Install5DownloadsVideo());
        private static Lazy<UserControl> _install6 = new Lazy<UserControl>(() => new Install.Install6DownloadsList());
        private static Lazy<UserControl> _install7 = new Lazy<UserControl>(() => new Install.Install7_4GBRamPatch());
        private static Lazy<UserControl> _install8 = new Lazy<UserControl>(() => new Install.Install8ModManagerVid());
        private static Lazy<UserControl> _install9 = new Lazy<UserControl>(() => new Install.Install9PostInstallVid());

        public static UserControl GetPage(PagesEnum page, bool refreshInstance)
        {

            return page switch
            {
                PagesEnum.OblivionMainMenu => !refreshInstance ? _menu.Value : new Oblivion.Pages.MainMenu(),
                PagesEnum.NexusLogin => !refreshInstance ? _nexusLogin.Value : new NexusLoginPage(),
                PagesEnum.OblivionInstall1PreInstallVid => !refreshInstance ? _install1.Value : new Install.Install1PreInstallVid(),
                PagesEnum.OblivionInstall2SelectGameFolder => !refreshInstance ? _install2.Value : new Install.Install2SelectGameFolder(),
                PagesEnum.OblivionInstall3PcSpecs => !refreshInstance ? _install3.Value : new Install.Install3PcSpecs(),
                PagesEnum.OblivionInstall4ModsList => !refreshInstance ? _install4.Value : new Install.Install4ModsList(),
                PagesEnum.OblivionInstall5DownloadsVideo => !refreshInstance ? _install5.Value : new Install.Install5DownloadsVideo(),
                PagesEnum.OblivionInstall6DownloadsList => !refreshInstance ? _install6.Value : new Install.Install6DownloadsList(),
                PagesEnum.OblivionInstall7_4GBRamPAtch => !refreshInstance ? _install7.Value : new Install.Install7_4GBRamPatch(),
                PagesEnum.OblivionInstall8ModManager => !refreshInstance ? _install8.Value : new Install.Install8ModManagerVid(),
                PagesEnum.OblivionInstall9PostInstallVid => !refreshInstance ? _install9.Value : new Install.Install9PostInstallVid(),
                PagesEnum.OblivionOptionsMenu => !refreshInstance ? _optionsMenu.Value : new OblivionOptions(),
                _ => throw new NotImplementedException(),
            };
        }
    }
}
