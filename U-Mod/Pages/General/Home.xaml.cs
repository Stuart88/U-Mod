using System;
using System.Collections.Generic;
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

namespace U_Mod.Pages.General
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Home : UserControl
    {
        public Home()
        {
            Static.StaticData.CurrentGame = Shared.Enums.GamesEnum.None;

            InitializeComponent();
        }

        private void OptionsBtn_Click(object sender, RoutedEventArgs e)
        {
            Navigation.NavigateToPage(PagesEnum.Options, true);
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
