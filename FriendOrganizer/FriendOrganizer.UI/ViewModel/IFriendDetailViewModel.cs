using System.Threading.Tasks;

namespace FriendOrganizer.UI.ViewModel {
    public interface IFriendDetailViewModel {
        Task LoadAsync(int friendId);
    }
}