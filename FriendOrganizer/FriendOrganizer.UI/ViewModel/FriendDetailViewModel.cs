using System.Threading.Tasks;
using FriendOrganizer.Model;
using FriendOrganizer.UI.Data;

namespace FriendOrganizer.UI.ViewModel
{
    public class FriendDetailViewModel : ViewModelBase, IFriendDetailViewModel
    {
        private readonly IFriendDataService _dataService;
        private Friend _friend;

        public FriendDetailViewModel(IFriendDataService dataService) {
            _dataService = dataService;
        }

        public Friend Friend {
            get => _friend;
            set {
                _friend = value;
                OnPropertyChanged();
            }
        }

        public async Task LoadAsync(int friendId) {
            Friend = await _dataService.GetByIdAsync(friendId);
        }
    }
}
