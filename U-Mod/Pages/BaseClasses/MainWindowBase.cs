using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using U_Mod.Static;
using AMGWebsite.Shared.Models.ApiModels;
using AMGWebsite.Shared;
using AMGWebsite.Shared.Helpers;
using System.Threading.Tasks;
using AMGWebsite.Shared.Enums;

namespace U_Mod.Pages.BaseClasses
{
    public abstract partial class MainWindowBase : Page
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string LoginResultMessage { get; set; }

        protected MainWindowBase(GamesEnum game)
        {
            Static.StaticData.CurrentGame = game;
        }

        protected override void OnInitialized(EventArgs e)
        {
            StaticData.LoadAppData();

            StaticData.LoadGameData();

            SetDynamicResources();

            SetWindowTitle();

            base.OnInitialized(e);
        }

        private void SetDynamicResources()
        {
            switch (Static.StaticData.CurrentGame)
            {
                //case GamesEnum.Oblivion:
                //    Application.Current.Resources["ScrollSlider"] = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/Assets/Images/Scroll Slider.png", UriKind.Absolute)));
                //    Application.Current.Resources["ScrollBackground"] = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/Assets/Images/Scroll Bar.png", UriKind.Absolute)));
                //    Application.Current.Resources["MediaReplayButton"] = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/Assets/Images/Buttons/Replay Button.png", UriKind.Absolute)));
                //    Application.Current.Resources["MediaReplayButtonHover"] = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/Assets/Images/Buttons/Replay Button(HL).png", UriKind.Absolute)));
                //    Application.Current.Resources["MediaPauseButton"] = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/Assets/Images/Buttons/Pause Button.png", UriKind.Absolute)));
                //    Application.Current.Resources["MediaPauseButtonHover"] = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/Assets/Images/Buttons/Pause Button(HL).png", UriKind.Absolute)));
                //    Application.Current.Resources["MediaPlayButton"] = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/Assets/Images/Buttons/Video Play Button.png", UriKind.Absolute)));
                //    Application.Current.Resources["MediaPlayButtonHover"] = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/Assets/Images/Buttons/Video Play Button(HL).png", UriKind.Absolute)));
                //    Application.Current.Resources["VideoPlayerFrame"] = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/Assets/Images/Video-Player.png", UriKind.Absolute)));
                //    Application.Current.Resources["MediaFullScreenButton"] = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/Assets/Images/Buttons/video play pop-out.png", UriKind.Absolute)));
                //    Application.Current.Resources["MediaFullScreenButtonHover"] = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/Assets/Images/Buttons/video play pop-out(HL).png", UriKind.Absolute)));
                //    break;

                //case GamesEnum.Fallout:
                //    Application.Current.Resources["ScrollSlider"] = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/Assets/Fallout/Assets/Terminal/Scroll Slider.png", UriKind.Absolute)));
                //    Application.Current.Resources["ScrollBackground"] = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/Assets/Fallout/Assets/Terminal/Scroll Bar.png", UriKind.Absolute)));

                //    Application.Current.Resources["MediaReplayButton"] = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/Assets/Fallout/Buttons/Replay.png", UriKind.Absolute)));
                //    Application.Current.Resources["MediaReplayButtonHover"] = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/Assets/Fallout/Buttons/Replay.png", UriKind.Absolute)));
                //    Application.Current.Resources["MediaPauseButton"] = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/Assets/Fallout/Buttons/Pause.png", UriKind.Absolute)));
                //    Application.Current.Resources["MediaPauseButtonHover"] = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/Assets/Fallout/Buttons/Pause.png", UriKind.Absolute)));
                //    Application.Current.Resources["MediaPlayButton"] = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/Assets/Fallout/Buttons/Play.png", UriKind.Absolute)));
                //    Application.Current.Resources["MediaPlayButtonHover"] = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/Assets/Fallout/Buttons/Play.png", UriKind.Absolute)));
                //    Application.Current.Resources["VideoPlayerFrame"] = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/Assets/Fallout/Assets/Fallout Video Player.png", UriKind.Absolute)));

                //    Application.Current.Resources["MediaFullScreenButton"] = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/Assets/Fallout/Buttons/Pop-Out.png", UriKind.Absolute)));
                //    Application.Current.Resources["MediaFullScreenButtonHover"] = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/Assets/Fallout/Buttons/Pop-Out.png", UriKind.Absolute)));
                //    break;
            }
        }

        public void SetWindowTitle()
        {
            this.Title = Static.StaticData.CurrentGame switch
            {
                //GamesEnum.Oblivion => "U_Mod - Oblivion",
                _ => ""
            };
        }

        public virtual void NavigateToPage(Enums.PagesEnum page, bool refreshInstance = false) { }

        public async Task<bool> LoginButton_ClickBase(object sender, RoutedEventArgs e)
        {
            try
            {
                LoginResultMessage = "";

                System.Net.Http.HttpClient http = new System.Net.Http.HttpClient();

                LoginResponse resp = await http.FetchAsync<LoginResponse>($"{AMGWebsite.Shared.Constants.Urls.BaseUrl}user/login/{false}/{true}", Username, Password);

                if (resp.Ok && resp.LoginSuccess)
                {
                    Security.U_ModHasher hasher = new Security.U_ModHasher();

                    if (resp.MemberData.MachineID != null)
                    {
                        if (!hasher.CheckMachineId(resp.MemberData.MachineID))
                        {
                            SetUserOffline();
                            LoginResultMessage = "This account is locked to another machine.";
                            return false;
                        }
                    }

                    StaticData.AppData.MemberData = resp.MemberData;
                    StaticData.AppData.MemberData.LoginToken = resp.LoginToken;
                       

                    if (StaticData.AppData.MemberData.MachineID == null)
                    {
                        StaticData.AppData.MemberData.MachineID = hasher.MachineIdHash;

                        ApiResponse updateResponse = await http.SendAsync<ApiResponse, AmgMember>($"{AMGWebsite.Shared.Constants.Urls.BaseUrl}member", Static.StaticData.AppData.MemberData, AuthNames.LoginToken, resp.LoginToken);
                    }

                    StaticData.SaveAppData();

                    StaticData.IsLoggedIn = true;
                    
                    return true;
                }
                else if (resp.Ok && !resp.LoginSuccess)
                {
                    LoginResultMessage = resp.Message;
                    SetUserOffline();
                    return false;
                }
                else
                {
                    SetUserOffline();
                    throw new Exception($"SERVER RESPONSE: {resp.Message}");
                }
            }
            catch (Exception ex)
            {
                Logging.Logger.LogException("LoginButton_ClickBase", ex);
                LoginResultMessage = "An exception occurred. Please try restarting the U_Mod.";
                return false;
            }
        }

        private void SetUserOffline()
        {
            StaticData.IsLoggedIn = false;
            StaticData.AppData.MemberData.LoginToken = "";
            StaticData.SaveAppData();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="membershipExpiredBox">The area in which to tell user their membership has expired</param>
        /// <returns></returns>
        public async Task<bool> TryAutoLogin(StackPanel membershipExpiredBox)
        {
            try
            {
                if (StaticData.IsLoggedIn)
                    return true;

                if (string.IsNullOrEmpty(StaticData.AppData.MemberData.LoginToken))
                    return false;

                System.Net.Http.HttpClient http = new System.Net.Http.HttpClient();

                LoginResponse resp = await http.FetchAsync<LoginResponse>($"{AMGWebsite.Shared.Constants.Urls.BaseUrl}user/login/token/{true}", AuthNames.LoginToken, StaticData.AppData.MemberData.LoginToken);

                if (resp.Ok && resp.LoginSuccess)
                {
                    Security.U_ModHasher hasher = new Security.U_ModHasher();

                    if (resp.MemberData.MachineID != null)
                    {
                        if (!hasher.CheckMachineId(resp.MemberData.MachineID))
                        {
                            SetUserOffline();
                            LoginResultMessage = "This account is locked to another machine.";
                            return false;
                        }
                    }

                    StaticData.IsLoggedIn = true;

                    return true;
                }
                else if (resp.Ok && !resp.LoginSuccess)
                {
                    SetUserOffline();
                    if (resp.Message.ToLower().Contains("expired"))
                    {
                        membershipExpiredBox.Visibility = Visibility.Visible;
                    }
                    return false; // Auto login is background task so fail silently
                }
                else
                {
                    SetUserOffline();
                    throw new Exception(resp.Message);
                }

            }
            catch (Exception ex)
            {
                Logging.Logger.LogException("TryAutoLogin", ex);
                return false;
            }
        }
    }
}
