using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace U_Mod.Custom
{
    /// <summary>
    /// Interaction logic for FullScreenVideoWindow.xaml
    /// </summary>
    public partial class FullScreenVideoWindow : Window
    {
        private bool IsPlaying { get; set; }

        public string VideoSource
        {
            set => VideoPlayer.Source = new Uri(value, UriKind.RelativeOrAbsolute);
        }

        private ICustomVideoControl ParentControl { get; set; }

        public FullScreenVideoWindow(Uri source, TimeSpan position, ICustomVideoControl parent)
        {
            InitializeComponent();

            this.WindowState = WindowState.Maximized;
            this.ParentControl = parent;

            this.Closing += (s, e) =>
            {
                this.ParentControl.VideoPlayer.Position = this.VideoPlayer.Position;
                this.ParentControl.SetOverlayBackgroundBlack();
                this.ParentControl.VideoPlayer.Visibility = Visibility.Visible;

                this.ParentControl.VideoPlaying = true;
                this.ParentControl.VideoPlayer.Play();
            };

            this.VideoPlayer.PreviewMouseLeftButtonUp += (s, e) =>
            {
                if (this.IsPlaying)
                    this.VideoPlayer.Pause();
                else
                    this.VideoPlayer.Play();

                this.IsPlaying = !this.IsPlaying;
            };

            this.VideoPlayer.LoadedBehavior = MediaState.Manual;
            this.VideoPlayer.Source = source;
            this.VideoPlayer.Position = position;

            this.VideoPlayer.Play();
            this.IsPlaying = true;
            this.StartSliderTimer();

            this.PreviewKeyUp += (s, e) =>
            {
                if (e.Key == Key.Space)
                {
                    if (this.IsPlaying)
                        this.VideoPlayer.Pause();
                    else
                        this.VideoPlayer.Play();

                    this.IsPlaying = !this.IsPlaying;
                }

                if (e.Key == Key.Escape)
                {
                    this.Close();
                }
            };

            this.VideoPlayer.MediaEnded += (s, e) =>
            {
                this.Close();
            };
        }

        private void StartSliderTimer()
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(500);
            timer.Tick += (s, e) =>
            {
                if (this.VideoPlayer.NaturalDuration.HasTimeSpan)
                {
                    this.PositionSlider.Visibility = Visibility.Visible;
                    this.PositionSlider.Maximum = this.VideoPlayer.NaturalDuration.TimeSpan.TotalMilliseconds;
                    this.AutoChangingSlider = true;
                    this.PositionSlider.Value = this.VideoPlayer.Position.TotalMilliseconds;
                    this.AutoChangingSlider = false;
                }
            };
            timer.Start();
        }

        private bool AutoChangingSlider { get; set; }

        private void PositionSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!this.AutoChangingSlider)
            {
                this.VideoPlayer.Position = TimeSpan.FromMilliseconds(this.PositionSlider.Value);
            }
        }
    }
}