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

        protected MainWindowBase(GamesEnum game)
        {
            Static.StaticData.CurrentGame = game;
        }

        protected override void OnInitialized(EventArgs e)
        {
            StaticData.LoadAppData();

            StaticData.LoadGameData();

            base.OnInitialized(e);
        }



    }
}
