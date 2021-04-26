using U_Mod.Enums;
using U_Mod.Helpers;
using U_Mod.Models;
using U_Mod.Pages.BaseClasses;
using System;
using System.Windows;
using System.Windows.Input;
using U_Mod.Static;
using U_Mod.Shared.Models;
using System.Threading.Tasks;
using System.Windows.Media;

namespace U_Mod.Pages
{
    /// <summary>
    /// Interaction logic for NexusLoginPage.xaml
    /// </summary>
    public partial class NexusLoginPage : ModDownloaderBase
    {
        private NexusSocket NexusSocket { get; set; }

        #region Public Constructors

        public NexusLoginPage()
        {
            this.DataContext = this;

#if DEBUG
            this.ApiKey = Constants.NexusApiKey;
#else
            this.ApiKey = "Waiting for connection...";
#endif

            InitializeComponent();

            NexusSocket = new NexusSocket()
            {
                Uuid = Guid.NewGuid(),
                ConnectionToken = Static.StaticData.AppData.NexusLoginToken
            };

            NexusSocket.ExceptionHappened += (s, e) =>
            {
                Dispatcher.Invoke(() =>
                {
                    Logging.Logger.LogException("NexusLoginPage Constructor: NexusSocket.ExceptionHappened.", ((NexusExceptionArgs)e).ex);
                    GeneralHelpers.ShowExceptionMessageBox(((NexusExceptionArgs)e).ex);
                    
                    Static.StaticData.AppData.NexusLoginToken = "";
                    StaticData.AppData.NexusUuid = Guid.Empty;
                    Static.StaticData.SaveAppData();
                });
            };
            NexusSocket.GotApiKey += (s, e) =>
            {
                Dispatcher.Invoke(() =>
                {
                    this.ApiKey = ((NexusResponseEventArgs)e).ApiKey;
                    this.ApiKeyInput.Text = "READY";
                    this.ApiKeyInput.Background = new SolidColorBrush(Colors.Green);
                    this.ApiKeyInput.Foreground = new SolidColorBrush(Colors.White);
                    InstallBtn.IsEnabled = true;
                    InstallBtn.Opacity = 1;
                });
            };
            NexusSocket.GotConnectionToken += (s, e) => 
            {
                Dispatcher.Invoke(() =>
                {
                    this.NexusConnectionToken = ((NexusResponseEventArgs)e).ConnectionToken;
                    NexusLoginBtn.IsEnabled = true;
                    NexusLoginBtn.Opacity = 1;
                    Static.StaticData.AppData.NexusLoginToken = this.NexusConnectionToken;
                    StaticData.AppData.NexusUuid = NexusSocket.Uuid;
                    Static.StaticData.SaveAppData();
                });
            };

            Task.Run(() =>
            {
                NexusSocket.ConnectToSocket();
            });

        }

        #endregion Public Constructors

        #region Public Methods

        public override void BeginInstallProcess()
        {
            Application.Current.MainWindow.IsEnabled = false;

            CreateDirectDownloadsList(true);
            this.FilesToProcess = this.DirectDownloadsList.Count;
            this.FileToAutomaticallyDownload = this.DirectDownloadsList.Count;

            if (this.DirectDownloadsList.Count > 0)
            {
                UpdateProgressText("Downloading...");
                UpdateProgressBar(0);
                //Starts with first download, then when download is done, it is popped from list and if there are more urls, it moves onto the next one
                // (See end of DownloadFileCompleted)
                DirectDownload(this.DirectDownloadsList[0]);
            }
            else
            {
                ProcessFiles();
            }
        }

        public override void UpdateProgressBar(double value)
        {
            ProcessingProgressBar.Value = value;
        }

        public override void UpdateProgressText(string value)
        {
            SecondaryProgressGrid.Visibility = Visibility.Visible;
            LoadingSpinner.Visibility = Visibility.Hidden;

            if (value.StartsWith("DOWNLOADING"))
            {
                NexusStatusText.Text = "Downloading...";
                value = value.Replace("DOWNLOADING", ""); //Remove as it's just marker text for this if block to catch
            }
            else if (value.StartsWith("Preparing") || value.StartsWith("Creating"))
            {
                NexusStatusText.Text = "";
                SecondaryProgressGrid.Visibility = Visibility.Collapsed;
                LoadingSpinner.Visibility = Visibility.Visible;

            }
            else
                NexusStatusText.Text = "Installing...";

            InstallingInfo.Text = value;
           
        }

        public override void UpdateSecondaryProgressBar(string text, int value, int max)
        {
            SecondaryProgressGrid.Visibility = Visibility.Visible;
            SecondaryProcessingProgressBar.Maximum = max;
            SecondaryProcessingProgressBar.Value = value;
            SecondaryInstallingInfo.Text = text;
        }

        #endregion Public Methods

        #region Private Methods

        private void GetApiKeyBtn_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Helpers.ProcessHelpers.OpenInBrowser("https://www.nexusmods.com/users/myaccount?tab=api%20access");
        }

        private void GoBackClickArea_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Navigation.NavigateToPage(PagesEnum.ModsSelect);
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            Navigation.NavigateToPage(PagesEnum.ModsSelect);
        }

        private async void InstallBtn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(this.ApiKey))
                return;

            try
            {
                InstallBtn.IsEnabled = false;

                var resp = await Helpers.HttpHelpers.FetchFromNexus<NexusUserData>("https://api.nexusmods.com/v1/users/validate.json", null, this.ApiKey);

                if (resp.Ok)
                {
                    if (resp.Data.IsPremium)
                    {
                        ApiDetailsGrid.Visibility = Visibility.Collapsed;
                        DownloadProgressGrid.Visibility = Visibility.Visible;
                        GoBackClickArea.Visibility = Visibility.Hidden;
                        BeginInstallProcess();
                    }
                    else
                    {
                        Helpers.GeneralHelpers.ShowMessageBox("Auto downloads require a premium Nexus account.");
                    }
                }
                else
                {
                    Helpers.GeneralHelpers.ShowMessageBox("Internet connection error! " + resp.ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                GeneralHelpers.ShowExceptionMessageBox(ex);
                Logging.Logger.LogException("InstallBtn_Click (Nexus Login)", ex);
            }
            finally
            {
                InstallBtn.IsEnabled = true;
            }
        }

        private void RegisterBtn_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Helpers.ProcessHelpers.OpenInBrowser("https://users.nexusmods.com/register");
        }

        #endregion Private Methods

        private void NexusLoginBtn_Click(object sender, RoutedEventArgs e)
        {
            Helpers.ProcessHelpers.OpenInBrowser($"https://www.nexusmods.com/sso?id={this.NexusSocket.Uuid}&application={NexusSocket.ApplicationSlug}");
        }

       
    }
}