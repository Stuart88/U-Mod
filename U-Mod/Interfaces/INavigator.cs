
using AMGWebsite.Shared.Enums;

namespace U_Mod.Interfaces
{
    public interface INavigator
    {
        public void NavigateToPage(Enums.PagesEnum page, bool refresInstance);
        /// <summary>
        /// This is a reminder to set the game type on creation of new game installer MainWindow
        /// </summary>
        /// <param name="game"></param>
        public void SetGame(GamesEnum game);
    }
}
