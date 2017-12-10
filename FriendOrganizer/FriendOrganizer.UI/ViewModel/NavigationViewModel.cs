using System.Collections.ObjectModel;
using System.Threading.Tasks;
using FriendOrganizer.Model;
using FriendOrganizer.UI.Data;

namespace FriendOrganizer.UI.ViewModel
{
    public class NavigationViewModel : INavigationViewModel
    {
        private IFriendLookupDataService _friendLookupService;

        public NavigationViewModel(IFriendLookupDataService friendLookupService) {
            _friendLookupService = friendLookupService;
            Friends = new ObservableCollection<LookupItem>();
        }

        private ObservableCollection<LookupItem> Friends { get; set; }
        
        public async Task LoadAsync() {
            var lookupItems = await _friendLookupService.GetFriendLookupAsync();
            Friends.Clear();
            foreach (var lookupItem in lookupItems) {
                Friends.Add(lookupItem);
            }
        }


    }


    
}
