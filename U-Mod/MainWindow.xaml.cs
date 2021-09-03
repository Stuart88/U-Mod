using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
using U_Mod.Shared.Enums;
using U_Mod.Static;

namespace U_Mod
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {

        private ObservableCollection<SideMenuOption> _sideMenuOptions { get; set; }
        public ObservableCollection<SideMenuOption> SideMenuOptions 
        {
            get => _sideMenuOptions;
            set
            {
                _sideMenuOptions = value;
                OnPropertyChanged();
            }
        }
        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public enum MenuItem
        {
            Home,
            Oblivion,
            Fallout3,
            NewVegas
        }

        public MainWindow()
        {
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;

            StaticData.LoadAppData();
            StaticData.CurrentGame = GamesEnum.None;

            this.SideMenuOptions = new ObservableCollection<SideMenuOption>
            {
                new SideMenuOption("Home", true, GamesEnum.None, MenuItem.Home),
                new SideMenuOption("Oblivion", false, GamesEnum.Oblivion, MenuItem.Oblivion),
                new SideMenuOption("Fallout 3", false, GamesEnum.Fallout, MenuItem.Fallout3),
                new SideMenuOption("New Vegas", false, GamesEnum.NewVegas, MenuItem.NewVegas),
            };

            InitializeComponent();

            this.SideMenu.ItemsSource = this.SideMenuOptions;

            this.MainContent.Content = Pages.Pages.GetPage(PagesEnum.Home, false);
        }

        public class SideMenuOption :  INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler? PropertyChanged;
            protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }

            public SideMenuOption(string name, bool active, GamesEnum game, MenuItem menuItem)
            {
                this.Name = name;
                this.Active = active;
                this.Game = game;
                this.MenuItem = menuItem;
            }
            public Image Icon { get; set; }
            public string Name { get; set; }
            private bool _active { get; set; }
            public bool Active
            {
                get => _active;
                set
                {
                    _active = value;
                    OnPropertyChanged();
                }
            }
            public bool Enabled { get; set; }
            public GamesEnum Game { get; set; }
            public MenuItem MenuItem { get; set; }
        }

        private void MenuItemClicked(object sender, RoutedEventArgs e)
        {
            if (((Button)sender).Tag is MenuItem item)
            {
                foreach(var opt in this.SideMenuOptions)
                {
                    opt.Active = opt.MenuItem == item;
                }

                OnPropertyChanged(nameof(this.SideMenuOptions));
                
                StaticData.CurrentGame = item switch
                {
                    MenuItem.Home => GamesEnum.None,
                    MenuItem.Oblivion => GamesEnum.Oblivion,
                    MenuItem.Fallout3 => GamesEnum.Fallout,
                    MenuItem.NewVegas => GamesEnum.NewVegas,
                    _ => GamesEnum.None,
                };

                StaticData.LoadGameData();

                this.MainContent.Content = item switch
                {
                    MenuItem.Home => Pages.Pages.GetPage(PagesEnum.Home, false),
                    MenuItem.Oblivion => Pages.Pages.GetPage(PagesEnum.MainMenu, true),
                    MenuItem.Fallout3 => Pages.Pages.GetPage(PagesEnum.MainMenu, true),
                    MenuItem.NewVegas => Pages.Pages.GetPage(PagesEnum.MainMenu, true),
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
