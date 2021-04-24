using System.Windows;
using U_Mod.Enums;
using U_Mod.Pages.BaseClasses;

namespace U_Mod.Helpers
{
    public static class Navigation
    {
        public static void NavigateToPage(PagesEnum page, bool refreshInstance = false)
        {
            ((MainWindow)Application.Current.MainWindow).NavigateToPage(page, refreshInstance);
        }
    }
}
