using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using U_Mod.Enums;
using U_Mod.Helpers;
using U_Mod.Pages.BaseClasses;

namespace U_Mod.Pages.InstallBethesda
{
    /// <summary>
    /// Interaction logic for _2ModList.xaml
    /// </summary>
    public partial class _2ModList : ModsListBase
    {
        public _2ModList()
        {
            InitializeComponent();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (Static.StaticData.UserDataStore.CurrentUserData.IsUpdating)
                Navigation.NavigateToPage(PagesEnum.MainMenu, true);
            else
                Navigation.NavigateToPage(PagesEnum.GameFolderSelect);
        }

        private void QuitButton_Click(object sender, RoutedEventArgs e)
        {
            Navigation.NavigateToPage(PagesEnum.MainMenu, true);
        }

        private void AutoBtn_Click(object sender, RoutedEventArgs e)
        {
            Navigation.NavigateToPage(PagesEnum.AutoDownload, true);
        }

        private void ManualBtn_Click(object sender, RoutedEventArgs e)
        {
            Navigation.NavigateToPage(PagesEnum.ManualDownload, true);
        }
    }
}
