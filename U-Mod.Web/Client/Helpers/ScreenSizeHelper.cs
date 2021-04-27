using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using BlazorPro.BlazorSize;

namespace U_Mod.Client.Helpers
{
    public static class ScreenSizeHelper
    {
        public static bool IsSmallScreen;

        public static string CalcWidthCss()
        {
            return IsSmallScreen ? "calc(100vw - 64px)" : "calc(100vw - 220px)";
        }

        public static string LeftShiftCss()
        {
            return IsSmallScreen ? "64px" : "220px";
        }

        public static void SetSmallScreen(bool isSmall)
        {
            IsSmallScreen = isSmall;
        }
    }
}
