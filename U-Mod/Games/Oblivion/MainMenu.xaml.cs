using AMGWebsite.Shared.Enums;
using U_Mod.Enums;
using U_Mod.Extensions;
using U_Mod.Helpers;
using U_Mod.Pages.BaseClasses;
using System.Linq;
using System.Windows;
using Application = System.Windows.Application;

namespace U_Mod.Games.Oblivion.Pages
{
    /// <summary>
    /// Interaction logic for MainMenu.xaml
    /// </summary>
    public partial class MainMenu : MainMenuBase
    {
        #region Public Constructors

        public MainMenu() : base()
        {
            InitializeComponent();

            base.ActionButton = this.ActionButton;
            base.OptionsButton = this.OptionsButton;

            InitComponentState();
        }

        #endregion Public Constructors

        #region Private Methods
       

        public override void ActionButton_Click(object sender, RoutedEventArgs e)
        {
            switch (this.ModState)
            {
                case ModState.Install:

                    if (Static.StaticData.UserDataStore.OblivionUserData.On4GbRamPatch)
                        Navigation.NavigateToPage(PagesEnum.OblivionInstall7_4GBRamPAtch);
                    else if (Static.StaticData.UserDataStore.OblivionUserData.OnModManagerPage)
                        Navigation.NavigateToPage(PagesEnum.OblivionInstall8ModManager);
                    else
                        Navigation.NavigateToPage(PagesEnum.GameFolderSelect, true);
                    break;

                case ModState.Play:
                    PlayGame();
                    break;

                case ModState.Update:

                    //Auto select all mods requiring update

                    Static.StaticData.UserDataStore.OblivionUserData.SelectedToInstall = Static.StaticData.MasterList.GetModUpdates()
                        .Select(m => new ModListItem
                        {
                            IsDownloaded = false,
                            IsInstalled = false,
                            IsDirectDownloadOnly = m.Files.All(f => !string.IsNullOrEmpty(f.DirectDownloadUrl)),
                            Mod = m
                        })
                        .ToList();

                    Static.StaticData.UserDataStore.OblivionUserData.IsUpdating = true;
                    Static.StaticData.UserDataStore.OblivionUserData.InstallationComplete = false;

                    if (Static.StaticData.UserDataStore.OblivionUserData.SelectedToInstall.Any(m => !m.Mod.IsEssential || ModHelpers.IsNewMod(m.Mod)))
                    {
                        //Some mods are new and are optional, so go to mod selection list
                        Navigation.NavigateToPage(PagesEnum.ModsSelect);
                    }
                    else
                    {
                        //Nothing to choose from, updates are just updates and/or essential, so go straight to download/process steps.
                        Navigation.NavigateToPage(PagesEnum.ModsSelect);

                        //NOTE both blocks are now the same. Not sure if this will work out or not..
                    }

                    break;
            }
        }
        public override void OptionsButton_Click(object sender, RoutedEventArgs e)
        {
            Navigation.NavigateToPage(PagesEnum.Options, true);
        }

        private async void SupportButton_Click(object sender, RoutedEventArgs e)
        {
        }

        #endregion Private Methods
    }
}