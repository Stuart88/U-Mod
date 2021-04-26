using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using U_Mod.Extensions;
using U_Mod.Helpers;
using U_Mod.Models;

namespace U_Mod.Pages
{
    /// <summary>
    /// Interaction logic for UpdateWindow.xaml
    /// </summary>
    public partial class UpdateWindow : Window
    {
        public UpdateWindow()
        {
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;

            this.MouseDown += (s, e) =>
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    this.DragMove(); // DragMove only works for left button clicked. Right click throws exception
                }
            };

            InitializeComponent();
            BeginUpdate();
        }


        private string UpdateExe => Path.Combine(Static.Constants.AppDataFolder, "U_Mod-Update.exe");

        private void BeginUpdate()
        {
            try
            {
                Thread thread = new Thread(async () =>
                {
                    MyWebClient client = new MyWebClient(40000);
                    client.DownloadFileCompleted += (s, e) =>
                    {
                        if (File.Exists(UpdateExe))
                        {
                            Tools.InstallUpdate(UpdateExe);

                           
                            Dispatcher.Invoke(() => Application.Current.Shutdown());
                        }
                        else
                        {
                            throw new Exception("Error downloading software update. File does not exist!");
                        }
                    };
                    client.DownloadProgressChanged += (s, e) =>
                    {
                        Dispatcher.Invoke(() =>
                        {
                            if (e.ProgressPercentage > 0)
                            {
                                UpdateProgressFrme.Visibility = Visibility.Visible;
                                UpdateProgress.Value = e.ProgressPercentage;
                            }
                        });
                    };

                    client.DownloadFileAsync(new Uri(Static.Constants.UpdateUrl), UpdateExe);

                 
                });
                thread.Start();
            }
            catch (Exception e)
            {
                Logging.Logger.LogException("BeginUpdate", e);
                MessageBox.Show("Update failed! " + Shared.Helpers.StringHelpers.ErrorMessage(e));
                Application.Current.Shutdown();
            }
        }
    }
}
