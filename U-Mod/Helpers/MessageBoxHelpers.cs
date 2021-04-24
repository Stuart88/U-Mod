using System.Windows;

namespace U_Mod.Helpers
{
    public static class MessageBoxHelpers
    {
        #region Public Methods

        public static bool OkCancel(string message, string boxTitle, MessageBoxImage messageBoxImage)
        {
            return (MessageBox.Show(message, boxTitle, MessageBoxButton.OKCancel, messageBoxImage, MessageBoxResult.Cancel) == MessageBoxResult.OK);
        }

        public static bool YesNo(string message, string boxTitle)
        {
            return (MessageBox.Show(message, boxTitle, MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Cancel) == MessageBoxResult.Yes);
        }

        #endregion Public Methods
    }
}