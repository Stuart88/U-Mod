using U_Mod.Shared.Enums;

namespace U_Mod.Models
{
    public class UserDataStore
    {
        #region Public Properties

        public UserDataBase OblivionUserData { get; set; } = new UserDataBase();
        public UserDataBase FalloutUserData { get; set; } = new UserDataBase();
        public UserDataBase NewVegasUserData { get; set; } = new UserDataBase();
        public UserDataBase SkyrimUserData { get; set; } = new UserDataBase();

        public UserDataBase CurrentUserData => Static.StaticData.CurrentGame switch
        {
            GamesEnum.Oblivion => this.OblivionUserData,
            GamesEnum.Fallout => this.FalloutUserData,
            GamesEnum.NewVegas => this.NewVegasUserData,
            GamesEnum.Skyrim => this.SkyrimUserData,
            _ => null
        };

        #endregion Public Properties
    }
}