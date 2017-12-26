using System.Windows;

namespace FriendOrganizer.UI.View.Services
{
    public class MessageDialogService : IMessageDialogService
    {
        public MessageDialogResult ShowOkCancelDialog(string text, string title) {
            var result =  MessageBox.Show(text, title, MessageBoxButton.OKCancel);
            return result == MessageBoxResult.OK
                ? MessageDialogResult.Ok
                : MessageDialogResult.Cancel;
        }
    }

    public enum MessageDialogResult
    {
        Ok = 0,
        Cancel = 1
    }
}
