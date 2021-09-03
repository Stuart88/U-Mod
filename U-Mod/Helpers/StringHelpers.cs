using System;
using System.Collections.Generic;
using U_Mod.Shared.Enums;

namespace U_Mod.Helpers
{
    public static class StringHelpers
    {
        #region Public Methods

       
        public static string VideoUri(string videoName)
        {
            return Static.StaticData.CurrentGame switch
            { 
                GamesEnum.Oblivion => $"Assets/Oblivion/Video/{videoName}",
                GamesEnum.Fallout => $"Assets/Fallout/Video/{videoName}",
                GamesEnum.NewVegas => $"Assets/NewVegas/Video/{videoName}",
#if DEV
                _ => "Assets/giphy.mp4"
#else
                _ => ""
#endif
            };
        }

        public static string ThumbnailResourceString(string fileName)
        {

            return Static.StaticData.CurrentGame switch
            {
                GamesEnum.Oblivion => $"pack://application:,,,/Assets/Oblivion/Video/Thumbnails/{fileName}",
                GamesEnum.Fallout => $"pack://application:,,,/Assets/Fallout/Assets/Thumbnails/{fileName}",
                GamesEnum.NewVegas => $"pack://application:,,,/Assets/NewVegas/Assets/Thumbnails/{fileName}",
#if DEV
                _ => $"pack://application:,,,/Assets/Oblivion/Video/Thumbnails/1 Pre install video.png"
#else
                _ => ""
#endif
            };
        }

        public static string GoogleDriveVideoUri(string uriCode)
        {
            return $"https://drive.google.com/uc?export=download&id={uriCode}";
        }


        #endregion Public Methods
    }
}