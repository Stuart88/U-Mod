using U_Mod.Shared.Enums;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Threading;
using U_Mod.Enums;
using U_Mod.Helpers;
using U_Mod.Static;

namespace U_Mod.Pages.InstallBethesda
{
    /// <summary>
    /// Interaction logic for _4_PatchAndModManager.xaml
    /// </summary>
    public partial class _4_PatchAndModManager : UserControl
    {
        private bool PatchInstalled { get; set; } = true;
        private bool ModManagerInstalled { get; set; } = true;
        public _4_PatchAndModManager()
        {
            InitializeComponent();

            SetText();

            Static.StaticData.UserDataStore.CurrentUserData.On4GbRamPatch = true;
            Static.StaticData.SaveAppData();

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(100);
            timer.Tick += (s, e) =>
            {
                CheckPatchInstalled();
                //CheckModManagerInstalled();
                if (this.PatchInstalled && this.ModManagerInstalled)
                {
                    timer.Stop();
                    this.FinishBtn.IsEnabled = true;

                    //Need to reset these because if user reinstalls again, without having closed U-Mod,
                    //this page will still show 4GB and ModManager installed.
                    this.PatchInstalled = true;
                    this.ModManagerInstalled = true;
                }
            };
            timer.Start();
        }


        private void SetText()
        {

            (ModManagerTitle.Text, ModManagerInfoName.Text, ModManagerBtnTextName.Text, ModManagerClickHereName.Text) 
                = Static.StaticData.CurrentGame switch
            {
                GamesEnum.Oblivion => (Constants.OblivionModManager, Constants.Obmm, Constants.Obmm, Constants.Obmm),
                var x when 
                    x == GamesEnum.Fallout || 
                    x == GamesEnum.Skyrim ||
                    x == GamesEnum.NewVegas => (Constants.ModOrganizer2, Constants.ModOrganizer2, Constants.ModOrganizer2, Constants.ModOrganizer2),
                _ => throw new NotImplementedException()
            };
        }

        private void RamPatchBtn_Click(object sender, RoutedEventArgs e)
        {
            Tools.Run4GbPatch(true);
        }

        private void CheckPatchInstalled()
        {
            if (Static.StaticData.CurrentGame == GamesEnum.None)
                return;

            string fileName = Static.StaticData.CurrentGame switch
            {
                GamesEnum.Oblivion => "Oblivion.exe.Backup",
                GamesEnum.Fallout => "Fallout3.exe.Backup",
                GamesEnum.NewVegas => "FalloutNV_backup.exe", 
                GamesEnum.Skyrim => "",
                _ => throw new Exception("CheckPatchInstalled(): game enum unknown!")
            };
            if (string.IsNullOrEmpty(fileName) || File.Exists(System.IO.Path.Combine(FileHelpers.GetGameFolder(), fileName)))
            {
                _4gbOkInfo.Visibility = Visibility.Visible;
                this.RamPatchBtn.IsEnabled = false;
                this.PatchInstalled = true;

                Static.StaticData.UserDataStore.CurrentUserData.On4GbRamPatch = true;
                Static.StaticData.SaveAppData();
            }
        }

        private void CheckModManagerInstalled()
        {
            if (this.ModManagerInstalled)
                return;

            string programName = "";

            switch (StaticData.CurrentGame)
            {
                case GamesEnum.Fallout:
                case GamesEnum.NewVegas:
                    programName = "Mod Organizer";
                    break;
                default:
                    this.ModManagerInstalled = true;
                    break;
            }

            //string programName = Static.StaticData.CurrentGame switch
            //{
            //    GamesEnum.Oblivion => "OBMM",
            //    GamesEnum.Fallout => "Mod Organizer"
            //};

            if (!string.IsNullOrEmpty(programName) && File.Exists(ProcessHelpers.GetModOrganizerExePath()))
            {
                ModOrganizerInstalledInfo.Visibility = Visibility.Visible;
                this.ModManagerBtn.IsEnabled = false;
                this.ModManagerInstalled = true;
            }
        }

        private void ManualPatchToolLink_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ProcessHelpers.OpenGameFolder();
        }

        private void ModManagerBtn_Click(object sender, RoutedEventArgs e)
        {
            Tools.LaunchModManager();
        }

        private void ModManagerInstructionsLink_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Tools.OpenModManagerHelpInBrowser();
        }

        private void FinishBtn_Click(object sender, RoutedEventArgs e)
        {
            EndInstaller();
        }

        private void EndInstaller()
        {
            Static.StaticData.UserDataStore.CurrentUserData.On4GbRamPatch = false;
            Static.StaticData.UserDataStore.CurrentUserData.GameVersion = Static.StaticData.GetCurrentGame().GameVersion;
            Static.StaticData.UserDataStore.CurrentUserData.InstallationComplete = true;
            Static.StaticData.UserDataStore.CurrentUserData.IsUpdating = false; // this might have been false anyway, but reset now just in case user was updating.
            Static.StaticData.SaveAppData();
            Navigation.NavigateToPage(PagesEnum.MainMenu, true);
        }

    }
}
