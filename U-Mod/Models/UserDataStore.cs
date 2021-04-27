using U_Mod.Shared.Enums;

namespace U_Mod.Models
{
    public class UserDataStore
    {
        #region Public Properties

        public UserDataBase OblivionUserData { get; set; } = new UserDataBase();
        public UserDataBase FalloutUserData { get; set; } = new UserDataBase();

        public UserDataBase CurrentUserData => Static.StaticData.CurrentGame switch
        {
            GamesEnum.Oblivion => this.OblivionUserData,
            GamesEnum.Fallout => this.FalloutUserData,
            _ => null
        };

        #endregion Public Properties
    }
}