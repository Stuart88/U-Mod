using System;
using System.Collections.Generic;
using System.Text;

namespace U_Mod.Shared.Constants
{
    public static class Constants
    {
        public static class Urls
        {
#if !DEBUG
            public const string BaseUrl = "https://aftermarketgaming.club/";
#else
            public const string BaseUrl = "https://aftermarketgaming.club/";
            //public const string BaseUrl = "https://localhost:5001/";
#endif
            public const string ForgotPassword = "/forgotpassword";
            public const string Account = "/account";
            public const string Login = "/login";
            public const string Register = "/register";
            public const string Index = "/";
            public const string Gamehub = "/gamehub";
            public const string Membership = "/membership";
            public const string Oblivion = "/games/oblivion";
            public const string Fallout = "/games/fallout";
            public const string Community = "/community";
            public const string Support = "/support";
            public const string Help = "/help";
            public const string AdminPanel = "/admin";
        }

#if RELEASE
        public const string StripePublicKey = "pk_live_51IAgrAENDq6ade0v9epGoadmoASrYqnFOBny4mwEjlhWCvrBASiY6lKwX1V5gVlRlBJF5P3ebL3VyOODNUNdyYKv000ePt1PFz";
#else
        public const string StripePublicKey = "pk_test_51IAgrAENDq6ade0vo6PCYKRaJkSKuptKPvjPoXs2mpSze3iWNTbEI0TkNcWBgUXzeTzw1wUOEi1DinjR9TXUMuFY00iRmZXyzm";
#endif

        public const string NexusAppSlug = "vortex";

        public const string GameNameOblivion = "Oblivion";
        public const string GameNameFallout3 = "Fallout 3";
        public const string GameNameNewVegas = "New Vegas";
    }

}
