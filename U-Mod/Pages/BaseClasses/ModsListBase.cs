using U_Mod.Shared.Models;
using U_Mod.Extensions;
using U_Mod.Helpers;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace U_Mod.Pages.BaseClasses
{

    public class ModListItem : INotifyPropertyChanged
    {
        #region Private Fields

        private bool _isChecked;
        private bool _isDownloaded;

        #endregion Private Fields

        #region Public Properties

        public int Index { get; set; }

        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                _isChecked = value;
                OnPropertyChanged();
            }
        }

        public bool IsDirectDownloadOnly { get; set; }

        public bool IsDownloaded
        {
            get => _isDownloaded;
            set
            {
                _isDownloaded = value;
                OnPropertyChanged();
            }
        }

        public bool IsInstalled { get; set; }
        public Mod Mod { get; set; }

        public string IsEssentialText => (this.Mod.IsEssential && !this.IsInstalled) ? " (essential)" : "";
        public string IsInstalledText => this.IsInstalled ? " (installed)" : "";

        public bool ShouldDisable => this.Mod.IsEssential || this.IsInstalled;

        public bool IsIntalled => Static.StaticData.UserDataStore.CurrentUserData.InstalledModIds.Any(m => m.Id == this.Mod.Id);

        #endregion Public Properties

        #region Public Constructors

        public ModListItem() // Do not delete. Required for binding in XAML data
        {
        }

        #endregion Public Constructors

        #region Public Events

        public event PropertyChangedEventHandler? PropertyChanged;

        #endregion Public Events

        #region Protected Methods

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion Protected Methods
    }

    public abstract partial class ModsListBase : UserControl, INotifyPropertyChanged
    {
        #region Private Fields

        private ObservableCollection<ModListItem> _listData = new ObservableCollection<ModListItem>();

        #endregion Private Fields

        #region Public Properties

        public StackPanel BottomCirclesBase { get; set; } = new StackPanel();

        public ObservableCollection<ModListItem> ListData
        {
            get { return _listData; }
            set
            {
                _listData = value;
                OnPropertyChanged();
            }
        }

        public List<Mod> ModData { get; set; }

        public StackPanel TopCirclesBase { get; set; } = new StackPanel();

        #endregion Public Properties

        #region Public Constructors

        public ModsListBase()
        {
            var userData = Static.StaticData.UserDataStore.CurrentUserData;

            if (!userData.IsUpdating)
            {
                //if updating, this was set on menu page already.
                userData.SelectedToInstall = new List<ModListItem>();
            }

            this.ModData = Static.StaticData.MasterList.GetAvailableModsList(true, true);

            this.InitModList();
            this.DataContext = this;
        }

        #endregion Public Constructors

        #region Public Events

        public event PropertyChangedEventHandler? PropertyChanged;

        #endregion Public Events

        #region Public Methods

        public void Grid_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (((Grid)sender).Tag is int index)
            {
                ModListItem? updating = this.ListData.FirstOrDefault(x => x.Index == index);

                if (updating == null || updating.ShouldDisable)
                {
                    return;
                }

                if (updating.IsInstalled) // cannot uninstall! So don't allow unchecking
                    return;

                updating.IsChecked = !updating.IsChecked;

                if (updating.IsChecked)
                {
                    Static.StaticData.UserDataStore.CurrentUserData.SelectedToInstall.Add(updating);
                }
                else
                {
                    Static.StaticData.UserDataStore.CurrentUserData.SelectedToInstall.Remove(updating);
                }
            }
        }

        public void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scv = (ScrollViewer)sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            e.Handled = true;
        }

        public void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            TopCirclesBase.Visibility = e.VerticalOffset < 10 ? Visibility.Hidden : Visibility.Visible;
            BottomCirclesBase.Visibility = e.ExtentHeight - e.VerticalOffset < 400 ? Visibility.Hidden : Visibility.Visible;

            e.Handled = true;
        }

        #endregion Public Methods

        #region Internal Methods

        internal void InitModList()
        {
            int index = 0;
            this.ListData = new ObservableCollection<ModListItem>();
            foreach (Mod m in this.ModData)
            {
                bool isInstalled = ModHelpers.IsInstalled(m);
                var adding = new ModListItem
                {
                    Index = ++index,
                    Mod = m,
                    IsChecked = true,
                    IsInstalled = isInstalled
                };
                this.ListData.Add(adding);

                // All items should start checked, so add to 'Selected to Install' list.
                if (!isInstalled)
                    Static.StaticData.UserDataStore.CurrentUserData.SelectedToInstall.Add(adding);
            }
        }

        #endregion Internal Methods

        #region Protected Methods

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion Protected Methods
    }
}