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

        public NavigationViewModel(IFriendLookupDataService friendLookupService, IEventAggregator eventAggregator) {
            _friendLookupService = friendLookupService;
            _eventAggragator = eventAggregator;
            Friends = new ObservableCollection<NavigationItemViewModel>();
            _eventAggragator.GetEvent<AfterFriendSavedEvent>().Subscribe(AfterFriendSaved);
            _eventAggragator.GetEvent<AfterFriendDeletedEvent>().Subscribe(AfterFriendDeleted);
        }

        
        public ObservableCollection<NavigationItemViewModel> Friends { get; set; }


        public async Task LoadAsync() {
            var lookupItems = await _friendLookupService.GetFriendLookupAsync();
            Friends.Clear();
            foreach (var lookupItem in lookupItems)
                Friends.Add(new NavigationItemViewModel(lookupItem.Id, lookupItem.DisplayMember, _eventAggragator));
        }

        private void AfterFriendSaved(AfterFriendSavedEventArgs obj) {
            var lookupItem = Friends.SingleOrDefault(l => l.Id == obj.Id);

            if (lookupItem == null)
                Friends.Add(new NavigationItemViewModel(obj.Id, obj.DisplayMember, _eventAggragator));
            else
                lookupItem.DisplayMember = obj.DisplayMember;
        }

        private void AfterFriendDeleted(int friendId) {
            var friend = Friends.SingleOrDefault(f => f.Id == friendId);
            if (friend != null)
                Friends.Remove(friend);
        }

    }
}
