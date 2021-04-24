using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace U_Mod.Custom
{
    /// <summary>
    /// Interaction logic for VideoControl.xaml
    /// </summary>
    public partial class VideoControlLarge : UserControl
    {
        #region Public Properties

        public string VideoSource
        {
            set => VideoPlayer.Source = new Uri(value, UriKind.RelativeOrAbsolute);
        }

        #endregion Public Properties

        #region Private Properties

        private bool VideoPlaying { get; set; }

        #endregion Private Properties

        #region Public Constructors

        public VideoControlLarge()
        {
            InitializeComponent();
            VideoPlayer.LoadedBehavior = MediaState.Manual;
            VideoPlayer.Visibility = Visibility.Hidden;
        }

        #endregion Public Constructors

        #region Private Methods

        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            VideoPlayer.Pause();
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.VideoPlaying)
            {
                VideoPlayer.Pause();
            }
            else
            {
                SetOverlayBackgroundBlack();
                VideoPlayer.Visibility = Visibility.Visible;
                VideoPlayer.Play();
            }

            this.VideoPlaying = !this.VideoPlaying;
        }

        private void ReplayButton_Click(object sender, RoutedEventArgs e)
        {
            VideoPlayer.Stop();
            SetOverlayBackgroundBlack();
            VideoPlayer.Visibility = Visibility.Visible;
            VideoPlayer.Play();
        }

        private void SetOverlayBackgroundBlack()
        {
            OverlayBackground.Background = Brushes.Black;
            OverlayBackground.Margin = new Thickness(20, 15, 10, 40);
            OverlayBackground.Visibility = Visibility.Hidden;
            OverlayBackground.Visibility = Visibility.Visible;
        }

        #endregion Private Methods
    }
}