using System;
using System.Windows;

namespace U_Mod.Custom
{
    /// <summary>
    /// Interaction logic for CustomYesNoMessage.xaml
    /// </summary>
    public partial class CustomYesNoMessage : Window
    {
        public string MessageText { get; set; }
        public event EventHandler YesClicked;
        protected virtual void OnYesClicked(EventArgs e)
        {
            EventHandler handler = YesClicked;
            handler?.Invoke(this, e);
        }
        public CustomYesNoMessage(string message)
        {
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;

            this.MessageText = message;

            this.DataContext = this;
            InitializeComponent();
            SetStyles();
            this.Title = "U_Mod Message";
        }

        private void SetStyles()
        {

            //this.Background = Static.StaticData.CurrentGame switch
            //{
            //    GamesEnum.Oblivion => Application.Current.Resources["OblivionPopupBackground"] as ImageBrush,
            //    GamesEnum.Fallout => Application.Current.Resources["FalloutPopupBackground"] as ImageBrush,
            //    _ => null
            //};

            //this.YesButton.Style = Static.StaticData.CurrentGame switch
            //{
            //    GamesEnum.Oblivion => Application.Current.Resources["YesButton"] as Style,
            //    GamesEnum.Fallout => Application.Current.Resources["YesButtonFallout"] as Style,
            //    _ => null
            //};

            //this.NoButton.Style = Static.StaticData.CurrentGame switch
            //{
            //    GamesEnum.Oblivion => Application.Current.Resources["NoButton"] as Style,
            //    GamesEnum.Fallout => Application.Current.Resources["NoButtonFallout"] as Style,
            //    _ => null
            //};

            //this.MessageTextBlock.FontFamily = Static.StaticData.CurrentGame switch
            //{
            //    GamesEnum.Oblivion => Application.Current.Resources["OblivionFont"] as FontFamily,
            //    GamesEnum.Fallout => Application.Current.Resources["FalloutFont"] as FontFamily,
            //    _ => null
            //};

            //this.MessageTextBlock.Foreground = Static.StaticData.CurrentGame switch
            //{
            //    GamesEnum.Oblivion => Application.Current.Resources["Black"] as SolidColorBrush,
            //    GamesEnum.Fallout => Application.Current.Resources["FalloutFontColour"] as SolidColorBrush,
            //    _ => null
            //};
        }

        private void NoButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void YesButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            this.OnYesClicked(e);
        }
    }
}
