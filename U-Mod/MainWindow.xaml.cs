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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
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

            StaticData.LoadGameData();

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


                this.MainContent.Content = item switch
                {
                    MenuItem.Home => throw new NotImplementedException(),
                    MenuItem.Oblivion => Games.Oblivion.Pages.Pages.GetPage(Enums.PagesEnum.OblivionMainMenu, false),
                    MenuItem.Fallout3 => Games.Fallout.Pages.Pages.GetPage(Enums.PagesEnum.FalloutMainMenu, false),
                    _ => throw new NotImplementedException(),
                };
            }
        }

        public void NavigateToPage(Enums.PagesEnum page, bool refreshInstance = false)
        {
            this.MainContent.Content = Static.StaticData.CurrentGame switch
            {
                GamesEnum.Oblivion => Games.Oblivion.Pages.Pages.GetPage(page, refreshInstance),
                GamesEnum.Fallout => Games.Fallout.Pages.Pages.GetPage(page, refreshInstance),
            };
        }
    }
}
