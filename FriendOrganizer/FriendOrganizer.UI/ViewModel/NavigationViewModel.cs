using System.Collections.ObjectModel;
using System.Threading.Tasks;
using FriendOrganizer.Model;
using FriendOrganizer.UI.Data;
using FriendOrganizer.UI.Event;
using Prism.Events;

namespace FriendOrganizer.UI.ViewModel
{
    public class NavigationViewModel : ViewModelBase, INavigationViewModel
    {
        private IFriendLookupDataService _friendLookupService;
        private IEventAggregator _eventAggragator;
        private LookupItem _selectedFriend;

        public NavigationViewModel(IFriendLookupDataService friendLookupService, IEventAggregator eventAggregator) {
            _friendLookupService = friendLookupService;
            _eventAggragator = eventAggregator;
            Friends = new ObservableCollection<LookupItem>();
        }

        public ObservableCollection<LookupItem> Friends { get; set; }

        /*Used by NavigationView.xml*/
        public LookupItem SelectedFriend {
            get => _selectedFriend;
            set {
                _selectedFriend = value;
                OnPropertyChanged();
                if (_selectedFriend != null)
                    _eventAggragator.GetEvent<OpenFriendDetailViewEvent>()
                        .Publish(_selectedFriend.Id);
            }
        }

        public async Task LoadAsync() {
            var lookupItems = await _friendLookupService.GetFriendLookupAsync();
            Friends.Clear();
            foreach (var lookupItem in lookupItems) Friends.Add(lookupItem);
        }
    }
}
