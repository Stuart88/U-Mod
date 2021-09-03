using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using U_Mod.Shared.Constants;
using U_Mod.Shared.Enums;
using U_Mod.Shared.Models;

namespace DevTool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Private Fields

        private const string Version = "1.0.0.2";

        #endregion Private Fields

        #region Public Properties

       

        public List<string> GameNames { get; set; } = new List<string>
        {
            "- SELECT -",
            Constants.GameNameOblivion,
            Constants.GameNameFallout3,
            Constants.GameNameNewVegas,
        };

        public MasterList MasterList { get; set; }
        public GameItem SelectedGame { get; set; }
        public Mod SelectedMod { get; set; }
        public ModZipContent SelectedZipContentItem { get; set; }
        public ModZipFile SelectedZipItem { get; set; }

        #endregion Public Properties

        #region Private Properties

        private bool ExtractItemsDropdownHandling { get; set; } = true;

        #endregion Private Properties

        #region Public Constructors

        public MainWindow()
        {
            this.MasterList = new MasterList();

            InitializeComponent();

            this.GameNameInput.ItemsSource = GameNames;

            this.Title = $"Dev Tool v{Version}";

            RedrawGamesList();
        }

        #endregion Public Constructors

        #region Private Methods

        private void AddContentInfoBtn_Click(object sender, RoutedEventArgs e)
        {
            this.SelectedZipItem.Content.Add(new ModZipContent(this.SelectedZipItem.Id)
            {
                InstallOrder = SelectedZipItem.Content.Count() + 1
            });

            //Put empty input box at top
            //this.SelectedZipItem.Content = this.SelectedZipItem.Content.OrderByDescending(i => String.IsNullOrEmpty(i.FileName)).ToList();

            RedrawFileContentsList();
        }

        private void AddContextMenu(FrameworkElement el, Guid id)
        {
            ContextMenu menu = new ContextMenu();
            MenuItem mi = new MenuItem();
            mi.Header = "Delete";
            mi.Click += (e, s) =>
            {
                DeleteItem(id);
            };
            menu.Items.Add(mi);

            el.ContextMenu = menu;
        }

        private void AddFileBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(ZipNameInput.Text) || ContentCol.Visibility == Visibility.Collapsed)
            {
                ModZipFile newFile = new ModZipFile(this.SelectedMod.Id);
                this.SelectedMod.Files.Add(newFile);
                ZipItemClicked(newFile);
                RedrawFileContentsList();
                ZipNameInput.Text = "";
            }
        }

        private void AddGameBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(GameNameInput.SelectedItem as string) || ModsCol.Visibility == Visibility.Collapsed)
            {
                GameItem newGame = new GameItem();
                this.MasterList.Games.Add(newGame);
                GameItemClicked(newGame);
                RedrawGamesList();
                GameNameInput.SelectedItem = GameNames[0];
            }
        }

        private void AddModBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(ModNameInput.Text) || FilesCol.Visibility == Visibility.Collapsed)
            {
                Mod newMod = new Mod(this.SelectedGame.Id);
                this.SelectedGame.Mods.Add(newMod);
                ModItemClicked(newMod);
                RedrawModsList();
                ModNameInput.Text = "";
            }
        }

        /// <summary>
        /// Recursively checks all masterlist items for empty values. If one is found, the value is deleted and function returns false.
        /// Need to run the function repeatedly until it returns true, i.e while(result ==  false){DeleteBlanks();}
        /// </summary>
        /// <returns></returns>
        private bool DeleteBlanks()
        {
            foreach (var g in this.MasterList.Games)
            {
                if (string.IsNullOrEmpty(g.GameName))
                {
                    this.MasterList.RemoveItemWithId(g.Id);
                    return false;
                }
                foreach (var m in g.Mods)
                {
                    if (string.IsNullOrEmpty(m.ModName))
                    {
                        this.MasterList.RemoveItemWithId(m.Id);
                        return false;
                    }
                    foreach (var f in m.Files)
                    {
                        if (string.IsNullOrEmpty(f.FileName))
                        {
                            this.MasterList.RemoveItemWithId(f.Id);
                            return false;
                        }
                        foreach (var c in f.Content)
                        {
                            if (string.IsNullOrEmpty(c.FileName))
                            {
                                this.MasterList.RemoveItemWithId(c.Id);
                                return false;
                            }
                        }
                    }
                }
            }

            return true;
        }

        private void DeleteItem(Guid id)
        {
            if (MessageBox.Show("Delete item?", "Delete", MessageBoxButton.OKCancel, MessageBoxImage.Warning, MessageBoxResult.Cancel) == MessageBoxResult.OK)
            {
                foreach (var g in this.MasterList.Games)
                {
                    if (g.Id == id)
                    {
                        this.MasterList.Games.Remove(g);
                        this.SelectedGame = null;
                        RedrawAll();
                        ResetColVisibility();
                        return;
                    }

                    foreach (var m in g.Mods)
                    {
                        if (m.Id == id)
                        {
                            g.Mods.Remove(m);
                            this.SelectedMod = null;
                            RedrawAll();
                            FilesCol.Visibility = Visibility.Collapsed;
                            ContentCol.Visibility = Visibility.Collapsed;
                            return;
                        }

                        foreach (var f in m.Files)
                        {
                            if (f.Id == id)
                            {
                                m.Files.Remove(f);
                                this.SelectedZipItem = null;
                                RedrawAll();
                                ContentCol.Visibility = Visibility.Collapsed;
                                return;
                            }

                            foreach (var c in f.Content)
                            {
                                if (c.Id == id)
                                {
                                    f.Content.Remove(c);
                                    this.SelectedZipContentItem = null;
                                    RedrawAll();
                                    return;
                                }
                            }
                        }
                    }
                }
            }
        }

        private void ExtractLocationComboBox_DropDownClosed(object sender, EventArgs e)
        {
            if (this.ExtractItemsDropdownHandling)
                HandleExtractItemsDropdown();

            this.ExtractItemsDropdownHandling = true;
        }

        private void ExtractLocationComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cmb = sender as ComboBox;
            this.ExtractItemsDropdownHandling = !cmb.IsDropDownOpen;
            HandleExtractItemsDropdown();
        }

        private void FileContentClicked(ModZipContent c)
        {
            if (this.SelectedZipContentItem?.Id == c.Id)
                return;
            this.SelectedZipContentItem = c;
        }

        private void GameItemClicked(GameItem g)
        {
            if (this.SelectedGame?.Id == g.Id)
                return;

            this.SelectedGame = g;
            GameNameInput.SelectedItem = g.GameName;
            GameVersionInput.Text = g.GameVersion;
            RedrawModsList();

            ModsCol.Visibility = Visibility.Visible;
            FilesCol.Visibility = Visibility.Collapsed;
            ContentCol.Visibility = Visibility.Collapsed;
            this.SelectedMod = null;
            this.SelectedZipItem = null;
            this.SelectedZipContentItem = null;
        }

        private void GameNameInput_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.SelectedGame.GameName = (sender as ComboBox).SelectedItem as string;
            this.SelectedGame.Game = GetSelectedGame();
            AddModBtn.Content = $"Add Mod ({this.SelectedGame.GameName})";
            RedrawGamesList();
        }

        private GamesEnum GetSelectedGame()
        {
            if (this.SelectedGame == null)
                return GamesEnum.Unknown;

            return this.SelectedGame.GameName switch
            {
                Constants.GameNameNewVegas => GamesEnum.NewVegas,
                Constants.GameNameFallout3 => GamesEnum.Fallout,
                Constants.GameNameOblivion => GamesEnum.Oblivion,
                _ => GamesEnum.Unknown
            };
        }

        private string GetSelectedZipFileTypeText()
        {
            return this.SelectedZipItem.ZipFileType switch
            {
                ZipFileType._7z => ".7z",
                ZipFileType.Zip => ".zip",
                ZipFileType.Rar => ".rar",
                ZipFileType.Unknown => "?????",
                _ => throw new NotImplementedException(),
            };
        }

        private void HandleExtractItemsDropdown()
        {
            if (ExtractLocationComboBox.SelectedItem != null)
            {
                string tag = (ExtractLocationComboBox.SelectedItem as ComboBoxItem).Tag as string;

                this.SelectedZipItem.ExtractLocation = tag switch
                {
                    "Base" => ExtractLocation.Base,
                    "Data" => ExtractLocation.Data,
                    "DataTextures" => ExtractLocation.DataTextures,
                    "DataMeshes" => ExtractLocation.DataMeshes,
                    "DataObsePlugins" => ExtractLocation.DataObsePlugins,
                    "DataFosePlugins" => ExtractLocation.DataFosePlugins,
                    "DataMenus" => ExtractLocation.DataMenus,
                    _ => throw new NotImplementedException(),
                };
            }
        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog()
                {
                    Filter = "json file|*.json"
                };
                if (openFileDialog.ShowDialog() == true)
                {
                    this.MasterList = JsonSerializer.Deserialize<MasterList>(File.ReadAllText(openFileDialog.FileName));
                    RedrawGamesList();
                    this.SelectedGame = null;
                    this.ModsFilterText.Text = "";
                    ResetColVisibility();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR");
            }
        }

        private void ModAttributeChecked(object sender, RoutedEventArgs e)
        {
            switch ((sender as CheckBox)?.Tag as string)
            {
                case "Essential":
                    this.SelectedMod.IsEssential = ModEssentialCheck.IsChecked ?? false;
                    return;

                case "Steam":
                    this.SelectedMod.IsSteam = ModIsSteamCheck.IsChecked ?? false;
                    return;

                case "NonSteam":
                    this.SelectedMod.IsNonSteam = ModIsNonSteamCheck.IsChecked ?? false;
                    return;

                case "FullInstall":
                    this.SelectedMod.IsFullInstallOnly = ModFullInstallCheck.IsChecked ?? false;
                    return;

                case "AllDLC":
                    this.SelectedMod.IsAllDlcOnly = ModAllDlcCheck.IsChecked ?? false;
                    return;

                case "NotAllDLC":
                    this.SelectedMod.IsNotAllDlcOnly = ModNotAllDlcCheck.IsChecked ?? false;
                    return;
            }
        }

        private void ModItemClicked(Mod m)
        {
            if (this.SelectedMod?.Id == m.Id)
                return;

            this.SelectedMod = m;
            ModNameInput.Text = m.ModName;
            ModVersionInput.Text = $"{m.Version}";
            CreatorInput.Text = $"{m.CreatedBy}";
            CreatorUrlInput.Text = $"{m.CreatorUrl}";
            ModEssentialCheck.IsChecked = m.IsEssential;
            ModIsSteamCheck.IsChecked = m.IsSteam;
            ModIsNonSteamCheck.IsChecked = m.IsNonSteam;
            ModFullInstallCheck.IsChecked = m.IsFullInstallOnly;
            ModAllDlcCheck.IsChecked = m.IsAllDlcOnly;
            ModNotAllDlcCheck.IsChecked = m.IsNotAllDlcOnly;
            RedrawZipFilesList();

            FilesCol.Visibility = Visibility.Visible;
            ContentCol.Visibility = Visibility.Collapsed;
            this.SelectedZipItem = null;
            this.SelectedZipContentItem = null;
        }

        private void NewButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("This will clear all data from this view. You will not lose any saved json files.", "Clear All?", MessageBoxButton.OKCancel, MessageBoxImage.Warning, MessageBoxResult.Cancel) == MessageBoxResult.OK)
            {
                this.MasterList = new MasterList();
                this.SelectedGame = null;
                this.SelectedMod = null;
                this.SelectedZipItem = null;
                this.SelectedZipContentItem = null;
                RedrawGamesList();
                ResetColVisibility();
            }
        }

        private void RedrawAll()
        {
            RedrawGamesList();
            RedrawModsList();
            RedrawZipFilesList();
            RedrawFileContentsList();
        }

        private void RedrawFileContentsList()
        {
            FileContentList.Children.Clear();

            if (this.SelectedZipItem == null)
                return;

            foreach (var c in this.SelectedZipItem.Content.Where(c => c.ParentId == this.SelectedZipItem?.Id))
            {
                Grid grid = new Grid();
                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });

                TextBox index = new TextBox();
                index.Text = c.InstallOrder.ToString();
                index.TextChanged += (s, e) =>
                {
                    if (int.TryParse(index.Text, out int i))
                    {
                        c.InstallOrder = i;
                    }
                    else if (string.IsNullOrEmpty(index.Text))
                    {
                        c.InstallOrder = 0;
                    }
                    else
                    {
                        index.Text = "";
                    }
                };

                TextBox text = new TextBox();
                text.Text = c.FileName;
                text.TextChanged += (s, e) =>
                {
                    c.FileName = text.Text;
                };
                text.PreviewMouseDown += (s, e) => { FileContentClicked(c); };
                AddContextMenu(text, c.Id);

                grid.Children.Add(index);
                grid.Children.Add(text);
                Grid.SetColumn(index, 0);
                Grid.SetColumn(text, 1);

                FileContentList.Children.Add(grid);
            }
        }

        private void RedrawGamesList()
        {
            GamesList.Children.Clear();

            foreach (var g in this.MasterList.Games)
            {
                TextBlock text = new TextBlock();
                text.Text = $"{g.GameName} (v.{g.GameVersion})";
                text.Style = Application.Current.Resources["ListItem"] as Style;
                text.PreviewMouseDown += (s, e) =>
                {
                    GameItemClicked(g);
                    AddModBtn.Content = $"Add Mod ({this.SelectedGame.GameName})";
                };
                AddContextMenu(text, g.Id);
                GamesList.Children.Add(text);
            }

            switch (GetSelectedGame())
            {
                case GamesEnum.Oblivion:
                    DataFosePlugins.Visibility = Visibility.Collapsed;
                    DataObsePlugins.Visibility = Visibility.Visible;
                    break;

                case GamesEnum.Fallout:
                case GamesEnum.NewVegas:
                    DataFosePlugins.Visibility = Visibility.Visible;
                    DataObsePlugins.Visibility = Visibility.Collapsed;
                    break;

                default:
                    DataFosePlugins.Visibility = Visibility.Collapsed;
                    DataObsePlugins.Visibility = Visibility.Collapsed;
                    break;
            }
        }

        private void RedrawModsList()
        {
            ModsList.Children.Clear();

            if (this.SelectedGame == null)
                return;

            foreach (var mod in this.SelectedGame.Mods.Where(m =>
                m.ParentId == this.SelectedGame?.Id && (string.IsNullOrEmpty(this.ModsFilterText.Text) || (m.ModName?.ToLower() ?? "").Contains(this.ModsFilterText.Text.ToLower()))))
            {
                Grid grid = new Grid();
                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });

                TextBlock text = new TextBlock();
                text.Text = "• " + mod.ModName;
                text.Style = Application.Current.Resources["ListItem"] as Style;
                text.PreviewMouseDown += (s, e) =>
                {
                    ModItemClicked(mod);
                    AddFileBtn.Content = $"Add File ({this.SelectedMod.ModName})";
                };
                AddContextMenu(text, mod.Id);
                ModsList.Children.Add(text);
            }
        }

        private void RedrawZipFilesList()
        {
            FilesList.Children.Clear();

            if (this.SelectedMod == null)
                return;

            foreach (var file in this.SelectedMod.Files.Where(f => f.ParentId == this.SelectedMod?.Id))
            {
                TextBlock text = new TextBlock();
                text.Text = "• " + file.FileName;
                text.Style = Application.Current.Resources["ListItem"] as Style;
                text.PreviewMouseDown += (s, e) =>
                {
                    ZipItemClicked(file);
                    AddContentInfoBtn.Content = $"Add Content Info ({this.SelectedZipItem.FileName})";
                };
                AddContextMenu(text, file.Id);
                FilesList.Children.Add(text);
            }
        }

        private void ResetColVisibility()
        {
            ModsCol.Visibility = Visibility.Collapsed;
            FilesCol.Visibility = Visibility.Collapsed;
            ContentCol.Visibility = Visibility.Collapsed;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                while (!DeleteBlanks())
                {
                    DeleteBlanks();
                }

                string jsonString = JsonSerializer.Serialize(this.MasterList);

                SaveFileDialog dialog = new SaveFileDialog()
                {
                    Filter = "json file|*.json"
                };

                if (dialog.ShowDialog() == true)
                {
                    if (!String.IsNullOrEmpty(dialog.FileName))
                    {
                        File.WriteAllText(dialog.FileName, jsonString);
                        MessageBox.Show("Saved successfully!", "INFO");
                    }
                    else
                    {
                        MessageBox.Show("Not saved! No filename given!", "WARNING");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR");
            }
        }

        private void TextChanged(object sender, TextChangedEventArgs e)
        {
            switch ((sender as TextBox)?.Tag as string)
            {
                case "GameVersion":
                    this.SelectedGame.GameVersion = GameVersionInput.Text.Trim();
                    AddModBtn.Content = $"Add Mod ({this.SelectedGame.GameName})";
                    RedrawGamesList();
                    break;

                case "ModName":
                    if (ModNameInput.Text.Length > 30)
                        ModNameInput.Text = ModNameInput.Text.Substring(0, 30);

                    this.SelectedMod.ModName = ModNameInput.Text;
                    AddFileBtn.Content = $"Add File ({this.SelectedMod.ModName})";
                    RedrawModsList();
                    ModNameInput.CaretIndex = int.MaxValue;
                    break;

                case "CreatedBy":
                    this.SelectedMod.CreatedBy = CreatorInput.Text;
                    RedrawModsList();
                    CreatorInput.CaretIndex = int.MaxValue;
                    break;

                case "CreatorUrl":
                    this.SelectedMod.CreatorUrl = CreatorUrlInput.Text;
                    RedrawModsList();
                    CreatorUrlInput.CaretIndex = int.MaxValue;
                    break;

                case "ModVersion":

                    if (string.IsNullOrEmpty(ModVersionInput.Text))
                        return;

                    if (ModVersionInput.Text.Any(c => !char.IsDigit(c)))
                    {
                        ModVersionInput.Text = "";
                        return;
                    }

                    if (int.TryParse(ModVersionInput.Text, out int v))
                        SelectedMod.Version = v;
                    else
                    {
                        ModVersionInput.Text = "";
                    }

                    break;

                case "FileSize":

                    if (string.IsNullOrEmpty(FileSizeInput.Text))
                        return;

                    if (double.TryParse(FileSizeInput.Text, out double size))
                        SelectedZipItem.SizeinKb = size;
                    else
                    {
                        FileSizeInput.Text = "";
                    }

                    break;

                case "ZipName":
                    this.SelectedZipItem.FileName = ZipNameInput.Text.Trim();
                    AddContentInfoBtn.Content = $"Add Content Info ({this.SelectedZipItem.FileName})";
                    string ext = Path.GetExtension(this.SelectedZipItem.FileName);
                    if (!String.IsNullOrEmpty(ext))
                    {
                        ext = ext.ToLower();
                        this.SelectedZipItem.ZipFileType = ext switch
                        {
                            ".7z" => ZipFileType._7z,
                            ".rar" => ZipFileType.Rar,
                            ".zip" => ZipFileType.Zip,
                            _ => ZipFileType.Unknown
                        };
                        ZipFileTypeLabel.Text = GetSelectedZipFileTypeText();
                    }
                    else
                    {
                        ZipFileTypeLabel.Text = "?????";
                    }

                    RedrawZipFilesList();
                    break;

                case "ManualDownload":
                    this.SelectedZipItem.ManualDownloadUrl = ManualDownloadInput.Text.Trim();
                    DirectDownloadInput.IsEnabled = string.IsNullOrEmpty(ManualDownloadInput.Text);
                    if (!string.IsNullOrEmpty(this.SelectedZipItem.ManualDownloadUrl) && !Uri.IsWellFormedUriString(this.SelectedZipItem.ManualDownloadUrl, UriKind.Absolute))
                    {
                        MessageBox.Show("Badly formed URL!");
                    }
                    if (!this.SelectedZipItem.ManualDownloadUrl.Contains("www.nexusmods.com"))
                    {
                        MessageBox.Show("Manual download links should point to www.nexusmods.com");
                    }
                    break;

                case "DirectDownload":
                    this.SelectedZipItem.DirectDownloadUrl = DirectDownloadInput.Text.Trim();
                    ManualDownloadInput.IsEnabled = string.IsNullOrEmpty(DirectDownloadInput.Text);
                    if (!string.IsNullOrEmpty(this.SelectedZipItem.DirectDownloadUrl) && !Uri.IsWellFormedUriString(this.SelectedZipItem.DirectDownloadUrl, UriKind.Absolute))
                    {
                        MessageBox.Show("Badly formed URL!");
                    }
                    break;

                case "ModsFilter":
                    this.RedrawModsList();
                    break;
            }
        }

        private void ZipItemClicked(ModZipFile f)
        {
            if (this.SelectedZipItem?.Id == f.Id)
                return;

            this.SelectedZipItem = f;
            ZipNameInput.Text = f.FileName;
            FileSizeInput.Text = $"{f.SizeinKb}";
            ManualDownloadInput.Text = f.ManualDownloadUrl;
            DirectDownloadInput.Text = f.DirectDownloadUrl;
            ZipFileTypeLabel.Text = GetSelectedZipFileTypeText();
            foreach (var i in ExtractLocationComboBox.Items)
            {
                if ((((ComboBoxItem)i).Tag as string) == Enum.GetName(typeof(ExtractLocation), f.ExtractLocation))
                    ExtractLocationComboBox.SelectedItem = i;
            }

            RedrawFileContentsList();

            ContentCol.Visibility = Visibility.Visible;
            this.SelectedZipContentItem = null;
        }

        #endregion Private Methods
    }
}