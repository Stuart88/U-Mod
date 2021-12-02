using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using U_Mod.Enums;
using U_Mod.Helpers;
using U_Mod.Models;
using U_Mod.Pages;
using U_Mod.Shared.Enums;
using U_Mod.Shared.Models;
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
            NewVegas,
            Skyrim
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
                new SideMenuOption("Skyrim", false, GamesEnum.Skyrim, MenuItem.Skyrim),
            };

            InitializeComponent();

            this.SideMenu.ItemsSource = this.SideMenuOptions;

            this.MainContent.Content = Pages.Pages.GetPage(PagesEnum.Home, false);

            FetchAppVersionAndMasterList();
        }

        public async void FetchAppVersionAndMasterList()
        {
            Mouse.OverrideCursor = System.Windows.Input.Cursors.AppStarting;
            this.IsEnabled = false;

            StaticData.InstallerInfo = await HttpHelpers.Fetch<SoftwareVersion>(Constants.SoftwareVersionLink, null);

#if DEV
            await GetMasterList();
#else
            if (StaticData.InstallerInfo.Version == Constants.DefaultSoftwareVersion)
            {
                Helpers.GeneralHelpers.ShowMessageBox("This software requires an internet connection to work.");
                Application.Current.Shutdown();
                return;
            }

            if (!StaticData.InstallerInfo.SoftwareUpToDate)
            {
                if (MessageBoxHelpers.OkCancel("Software update available. Download now?", "Please Update", MessageBoxImage.Information))
                {
                    Application.Current.MainWindow.IsEnabled = false;
                    new UpdateWindow().Show();
                }
            }

            await GetMasterList();
#endif

            this.IsEnabled = true;
            Mouse.OverrideCursor = System.Windows.Input.Cursors.Arrow;
        }

        public async Task GetMasterList()
        {
#if DEV || DEBUG
            //StaticData.MasterList = Helpers.FileHelpers.LoadFile<MasterList>("masterList.json");
            StaticData.MasterList = await Helpers.HttpHelpers.Fetch<MasterList>(Static.Constants.MasterListLink, null);
#else
            StaticData.MasterList = await Helpers.HttpHelpers.Fetch<MasterList>(Static.Constants.MasterListLink, null);
#endif

            AssignMasterListOrderValues();
        }

        public void AssignMasterListOrderValues()
        {
            // TODO: DELETE THIS LATER WHEN ORDER VALUES ARE DONE ON DB

            for (int i = 0; i < Static.StaticData.MasterList.Games.Count; i++)
            {
                for (int j = 0; j < Static.StaticData.MasterList.Games[i].Mods.Count; j++)
                {
                    Static.StaticData.MasterList.Games[i].Mods[j].InstallOrder = j;

                    for (int k = 0; k < Static.StaticData.MasterList.Games[i].Mods[j].Files.Count; k++)
                    {
                        Static.StaticData.MasterList.Games[i].Mods[j].Files[k].InstallOrder = k;

                        for (int l = 0; l < Static.StaticData.MasterList.Games[i].Mods[j].Files[k].Content.Count; l++)
                        {
                            Static.StaticData.MasterList.Games[i].Mods[j].Files[k].Content[l].InstallOrder = l;
                        }
                    }
                }
            }
        }

        public class SideMenuOption : INotifyPropertyChanged
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
                foreach (var opt in this.SideMenuOptions)
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
                    MenuItem.Skyrim => GamesEnum.Skyrim,
                    _ => GamesEnum.None,
                };

                StaticData.LoadGameData();

                this.MainContent.Content = item switch
                {
                    MenuItem.Home => Pages.Pages.GetPage(PagesEnum.Home, false),
                    MenuItem.Oblivion => Pages.Pages.GetPage(PagesEnum.MainMenu, true),
                    MenuItem.Fallout3 => Pages.Pages.GetPage(PagesEnum.MainMenu, true),
                    MenuItem.NewVegas => Pages.Pages.GetPage(PagesEnum.MainMenu, true),
                    MenuItem.Skyrim => Pages.Pages.GetPage(PagesEnum.MainMenu, true),
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