using AMGWebsite.Shared.Enums;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using U_Mod.Enums;
using U_Mod.Static;

namespace U_Mod
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<SideMenuOption> SideMenuOptions { get; set; }
        
        public enum MenuItem
        {
            Home,
            Oblivion,
            Fallout3
        }

        public MainWindow()
        {
            StaticData.LoadAppData();

            this.SideMenuOptions = new ObservableCollection<SideMenuOption>
            {
                new SideMenuOption("Home", true, GamesEnum.None, MenuItem.Home),
                new SideMenuOption("Oblivion", false, GamesEnum.None, MenuItem.Oblivion),
                new SideMenuOption("Fallout 3", false, GamesEnum.None, MenuItem.Fallout3),
            };

            InitializeComponent();

            this.SideMenu.ItemsSource = this.SideMenuOptions;
        }

        public class SideMenuOption
        {
            public SideMenuOption(string name, bool active, GamesEnum game, MenuItem menuItem)
            {
                this.Name = name;
                this.Active = active;
                this.Game = game;
                this.MenuItem = menuItem;
            }
            public Image Icon { get; set; }
            public string Name { get; set; }
            public bool Active { get; set; }
            public bool Enabled { get; set; }
            public GamesEnum Game { get; set; }
            public MenuItem MenuItem { get; set; }
        }

        private void MenuItemClicked(object sender, MouseButtonEventArgs e)
        {
            if (((Grid)sender).Tag is MenuItem item)
            {
                Static.StaticData.CurrentGame = item switch
                {
                    MenuItem.Home => GamesEnum.None,
                    MenuItem.Oblivion => GamesEnum.Oblivion,
                    MenuItem.Fallout3 => GamesEnum.Fallout,
                    _ => GamesEnum.None,
                };

                StaticData.LoadGameData();

                this.MainContent.Content = item switch
                {
                    MenuItem.Home => Pages.Pages.GetPage(PagesEnum.Home, false),
                    MenuItem.Oblivion => Pages.Pages.GetPage(PagesEnum.MainMenu, false),
                    MenuItem.Fallout3 => Pages.Pages.GetPage(PagesEnum.MainMenu, false),
                    _ => throw new NotImplementedException(),
                };
            }
        }

        public void NavigateToPage(Enums.PagesEnum page, bool refreshInstance = false)
        {
            this.MainContent.Content = Pages.Pages.GetPage(page, refreshInstance);
        }
    }
}
