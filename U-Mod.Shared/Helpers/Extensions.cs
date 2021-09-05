using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using U_Mod.Shared.Enums;
using U_Mod.Shared.Models;

namespace U_Mod.Shared.Helpers
{
    public static class Extensions
    {
        /// <summary>
        /// Removes ExtractLocations that are not relevant to game
        /// </summary>
        /// <returns></returns>
        public static List<ExtractLocation> FilterForGame(this List<ExtractLocation> list, GamesEnum game)
        {
            switch (game)
            {
                case GamesEnum.Fallout:
                    list.Remove(ExtractLocation.DataObsePlugins);
                    list.Remove(ExtractLocation.DataNvsePlugins);
                    return list;
                case GamesEnum.NewVegas:
                    list.Remove(ExtractLocation.DataObsePlugins);
                    list.Remove(ExtractLocation.DataFosePlugins);
                    return list;

                case GamesEnum.Oblivion:
                    list.Remove(ExtractLocation.DataFosePlugins);
                    return list;

                default:
                    return list;
            }
        }
    }
}
