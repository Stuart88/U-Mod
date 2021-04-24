using AMGWebsite.Shared.Enums;
using U_Mod.Games;

namespace U_Mod.Models
{
    public class UserDataStore
    {
        #region Public Properties

        public Games.Oblivion.Models.UserData OblivionUserData { get; set; } = new Games.Oblivion.Models.UserData();
        public Games.Fallout.Models.UserData FalloutUserData { get; set; } = new Games.Fallout.Models.UserData();

        public UserDataBase CurrentUserData => Static.StaticData.CurrentGame switch
        {
            GamesEnum.Oblivion => this.OblivionUserData,
            GamesEnum.Fallout => this.FalloutUserData,
            _ => null
        };

        #endregion Public Properties
    }
}