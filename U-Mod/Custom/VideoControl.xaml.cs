using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace U_Mod.Custom
{
    /// <summary>
    /// Interaction logic for VideoControl.xaml
    /// </summary>
    public partial class VideoControl : UserControl, ICustomVideoControl
    {
        #region Public Properties

        public MediaElement VideoPlayer { get; set; }

        public bool VideoPlaying { get; set; }

        public string VideoSource
        {
            set => ThisVideoPlayer.Source = new Uri(value, UriKind.RelativeOrAbsolute);
        }

        #endregion Public Properties

        #region Private Properties

        private FullScreenVideoWindow FullScreenWindow { get; set; }

        #endregion Private Properties

        #region Public Constructors

        public VideoControl()
        {
            InitializeComponent();
            VideoPlayer = ThisVideoPlayer;
            ThisVideoPlayer.LoadedBehavior = MediaState.Manual;
            ThisVideoPlayer.Visibility = Visibility.Hidden;

            this.ThisVideoPlayer.MediaEnded += (s, e) =>
            {
                this.ThisVideoPlayer.Pause();
                this.ThisVideoPlayer.Position = TimeSpan.Zero;
            };
        }

        #endregion Public Constructors

        #region Public Methods

        public void PlayVideo()
        {
            if (this.VideoPlaying)
            {
                ThisVideoPlayer.Pause();
            }
            else
            {
                SetOverlayBackgroundBlack();
                ThisVideoPlayer.Visibility = Visibility.Visible;
                ThisVideoPlayer.Play();
            }

            this.VideoPlaying = !this.VideoPlaying;
        }

        public void SetOverlayBackgroundBlack()
        {
            OverlayBackground.Background = Brushes.Black;
            OverlayBackground.Margin = new Thickness(20, 0, 20, 80);
            OverlayBackground.Visibility = Visibility.Hidden;
            OverlayBackground.Visibility = Visibility.Visible;
        }

        #endregion Public Methods

        #region Private Methods

        private void FullScreenButton_Click(object sender, RoutedEventArgs e)
        {
            PlayVideo(); // Temporarily play to ensure correctly initialised.
            ThisVideoPlayer.Pause();
            this.VideoPlaying = false;

            if (FullScreenWindow == null)
            {
                FullScreenWindow = new FullScreenVideoWindow(this.ThisVideoPlayer.Source, this.ThisVideoPlayer.Position, this);
                FullScreenWindow.Show();
                FullScreenWindow.Topmost = true;
                FullScreenWindow.Closed += (s, e) =>
                {
                    FullScreenWindow = null;
                };
            }
            else
            {
                FullScreenWindow.Activate();
            }
        }

        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            ThisVideoPlayer.Pause();
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            PlayVideo();
        }

        /// <summary>
        /// Currently not in use. Replay button hidden.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReplayButton_Click(object sender, RoutedEventArgs e)
        {
            ThisVideoPlayer.Stop();
            SetOverlayBackgroundBlack();
            ThisVideoPlayer.Visibility = Visibility.Visible;
            ThisVideoPlayer.Play();
        }

        #endregion Private Methods
    }
}