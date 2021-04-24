using AMGWebsite.Shared.Enums;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace U_Mod.Custom
{
    /// <summary>
    /// Interaction logic for CustomMessageWindow.xaml
    /// </summary>
    public partial class CustomMessageWindow : Window
    {
        public string MessageText { get; set; }
        public CustomMessageWindow(string message)
        {
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

            //this.OkButton.Style = Static.StaticData.CurrentGame switch
            //{
            //    GamesEnum.Oblivion => Application.Current.Resources["MenuBackButton"] as Style,
            //    GamesEnum.Fallout => Application.Current.Resources["MenuBackButtonFallout"] as Style,
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

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
