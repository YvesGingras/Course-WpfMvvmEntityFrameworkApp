using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using FriendOrganizer.UI.Data.Lookups;
using FriendOrganizer.UI.Event;
using Prism.Events;

namespace FriendOrganizer.UI.ViewModel
{
    public class NavigationViewModel : ViewModelBase, INavigationViewModel
    {
        private readonly IFriendLookupDataService _friendLookupService;
        private readonly IEventAggregator _eventAggragator;
        private NavigationItemViewModel _selectedFriend;

        public NavigationViewModel(IFriendLookupDataService friendLookupService, IEventAggregator eventAggregator) {
            _friendLookupService = friendLookupService;
            _eventAggragator = eventAggregator;
            Friends = new ObservableCollection<NavigationItemViewModel>();
            _eventAggragator.GetEvent<AfterFriendSavedEvent>().Subscribe(AfterFriendSaved);
        }

        public ObservableCollection<NavigationItemViewModel> Friends { get; set; }

        public NavigationItemViewModel SelectedFriend {
            get => _selectedFriend;
            set {
                _selectedFriend = value;
                OnPropertyChanged();
                if (_selectedFriend != null)
                    _eventAggragator.GetEvent<OpenFriendDetailViewEvent>()
                        .Publish(_selectedFriend.Id);
            }
        }

        private void AfterFriendSaved(AfterFriendSavedEventArgs obj) {
            var lookupItem = Friends.Single(l => l.Id == obj.Id);
            lookupItem.DisplayMember = obj.DisplayMember;
        }

        public async Task LoadAsync() {
            var lookupItems = await _friendLookupService.GetFriendLookupAsync();
            Friends.Clear();
            foreach (var lookupItem in lookupItems)
                Friends.Add(new NavigationItemViewModel(lookupItem.Id, lookupItem.DisplayMember));
        }
    }
}
