using U_Mod.Enums;
using U_Mod.Helpers;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace U_Mod.Games.Fallout.Pages.InstallFallout
{
    /// <summary>
    /// Interaction logic for Install1.xaml
    /// </summary>
    public partial class Install1PreInstallVid : UserControl
    {
        #region Public Constructors

        private Button NextButton { get; set; } = new Button();
        private Button BackButton { get; set; } = new Button();

        public Install1PreInstallVid()
        {
            InitializeComponent();   

#if DEV
            NextButton.IsEnabled = true;
            NextButton.Opacity = 1;
#else
            NextButton.IsEnabled = false;
            NextButton.Opacity = 0.6;
#endif

            BackButton.Margin = new Thickness(20, 0, 0, 40);
            BackButton.Click += BackButton_Click;
            BackButton.Height = 30;
            BackButton.Width = 75;
            BackButton.HorizontalAlignment = HorizontalAlignment.Center;
            //BackButton.Style = Application.Current.Resources["MenuBackButtonFalloutSmall"] as Style;

            NextButton.Margin = new Thickness(0, 0, 55, 40);
            NextButton.Click += NextButton_Click;
            NextButton.Height = 30;
            NextButton.Width = 75;
            NextButton.HorizontalAlignment = HorizontalAlignment.Right;
            //NextButton.Style = Application.Current.Resources["MenuNextButtonFalloutSmall"] as Style;


        }

        #endregion Public Constructors

        #region Private Methods

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            Navigation.NavigateToPage(PagesEnum.FalloutInstall2SelectGameFolder);
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Navigation.NavigateToPage(PagesEnum.FalloutMainMenu, true);
        }

        #endregion Private Methods
    }
}