#nullable enable

using U_Mod.Enums;
using U_Mod.Helpers;
using U_Mod.Pages.BaseClasses;
using System.Collections.ObjectModel;
using System.Windows;

namespace U_Mod.Games.Fallout.Pages.InstallFallout
{
    /// <summary>
    /// Interaction logic for Install4.xaml
    /// </summary>
    public partial class Install4ModsList : ModsListBase
    {
        #region Public Constructors

        public Install4ModsList()
        {
            InitializeComponent();

            base.TopCirclesBase = this.TopCirclesBase;
            base.BottomCirclesBase = this.BottomCircles;
        }

        #endregion Public Constructors

        #region Private Methods

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (Static.StaticData.UserDataStore.CurrentUserData.IsUpdating)
                Navigation.NavigateToPage(PagesEnum.FalloutMainMenu);
            else
                Navigation.NavigateToPage(PagesEnum.FalloutInstall3PcSpecs);
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            this.ListData = new ObservableCollection<ModListItem>(ListData); // force refresh!
            Navigation.NavigateToPage(PagesEnum.FalloutInstall5DownloadsVideo);
        }

        private void QuitButton_Click(object sender, RoutedEventArgs e)
        {
            Navigation.NavigateToPage(PagesEnum.FalloutMainMenu);
        }

        #endregion Private Methods
    }
}