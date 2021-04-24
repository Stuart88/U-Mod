using System;
using System.Windows.Controls;
using U_Mod.Enums;
using U_Mod.Pages;

namespace U_Mod.Games.Fallout.Pages
{
    public static class Pages
    {
        private static Lazy<UserControl> _menu = new Lazy<UserControl>(() => new Fallout.Pages.MainMenu());
        private static Lazy<UserControl> _optionsMenu = new Lazy<UserControl>(() => new Fallout.Pages.FalloutOptions());
        private static Lazy<UserControl> _nexusLogin = new Lazy<UserControl>(() => new NexusLoginPage());
        private static Lazy<UserControl> _install1 = new Lazy<UserControl>(() => new InstallFallout.Install1PreInstallVid());
        private static Lazy<UserControl> _install2 = new Lazy<UserControl>(() => new InstallFallout.Install2SelectGameFolder());
        private static Lazy<UserControl> _install3 = new Lazy<UserControl>(() => new InstallFallout.Install3PcSpecs());
        private static Lazy<UserControl> _install4 = new Lazy<UserControl>(() => new InstallFallout.Install4ModsList());
        private static Lazy<UserControl> _install5 = new Lazy<UserControl>(() => new InstallFallout.Install5DownloadsVideo());
        private static Lazy<UserControl> _install6 = new Lazy<UserControl>(() => new InstallFallout.Install6DownloadsList());
        private static Lazy<UserControl> _install7 = new Lazy<UserControl>(() => new InstallFallout.Install7_4GBRamPatch());
        private static Lazy<UserControl> _install8 = new Lazy<UserControl>(() => new InstallFallout.Install8ModManagerVid());
        private static Lazy<UserControl> _install9 = new Lazy<UserControl>(() => new InstallFallout.Install9PostInstallVid());

        public static UserControl GetPage(PagesEnum page, bool refreshInstance)
        {

            return page switch
            {
                PagesEnum.FalloutMainMenu => !refreshInstance ? _menu.Value : new Fallout.Pages.MainMenu(),
                PagesEnum.NexusLogin => !refreshInstance ? _nexusLogin.Value : new NexusLoginPage(),
                PagesEnum.FalloutInstall1PreInstallVid => !refreshInstance ? _install1.Value : new InstallFallout.Install1PreInstallVid(),
                PagesEnum.FalloutInstall2SelectGameFolder => !refreshInstance ? _install2.Value : new InstallFallout.Install2SelectGameFolder(),
                PagesEnum.FalloutInstall3PcSpecs => !refreshInstance ? _install3.Value : new InstallFallout.Install3PcSpecs(),
                PagesEnum.FalloutInstall4ModsList => !refreshInstance ? _install4.Value : new InstallFallout.Install4ModsList(),
                PagesEnum.FalloutInstall5DownloadsVideo => !refreshInstance ? _install5.Value : new InstallFallout.Install5DownloadsVideo(),
                PagesEnum.FalloutInstall6DownloadsList => !refreshInstance ? _install6.Value : new InstallFallout.Install6DownloadsList(),
                PagesEnum.FalloutInstall7_4GBRamPAtch => !refreshInstance ? _install7.Value : new InstallFallout.Install7_4GBRamPatch(),
                PagesEnum.FalloutInstall8ModManager => !refreshInstance ? _install8.Value : new InstallFallout.Install8ModManagerVid(),
                PagesEnum.FalloutInstall9PostInstallVid => !refreshInstance ? _install9.Value : new InstallFallout.Install9PostInstallVid(),
                PagesEnum.FalloutOptionsMenu => !refreshInstance ? _optionsMenu.Value : new FalloutOptions(),
                _ => throw new NotImplementedException(),
            };
        }
    }
}
