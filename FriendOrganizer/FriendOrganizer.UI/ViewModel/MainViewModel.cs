using System.Collections.ObjectModel;
using FriendOrganizer.Model;
using FriendOrganizer.UI.Data;

namespace FriendOrganizer.UI.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private readonly IFriendDataService _friendDataService;
        private Friend _selectedFriend;

        public MainViewModel(IFriendDataService friendDataService) {
            Friends = new ObservableCollection<Friend>();
            _friendDataService = friendDataService;
        }

        public ObservableCollection<Friend> Friends { get; set; }

        public Friend SelectedFriend {
            get => _selectedFriend;
            set {
                _selectedFriend = value;
                OnPropertyChanged();
            }
        }

        public void Load() {
            var friends = _friendDataService.GetAll();

            Friends.Clear();
            foreach (var friend in friends) {
                Friends.Add(friend);
            }
        }
    }
}
