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
        private readonly IMeetingLookupDataSevice _meetingLookupService;

        public NavigationViewModel(IFriendLookupDataService friendLookupService, IEventAggregator eventAggregator, IMeetingLookupDataSevice meetingLookupService) {
            _friendLookupService = friendLookupService;
            _meetingLookupService = meetingLookupService;
            _eventAggragator = eventAggregator;
            Friends = new ObservableCollection<NavigationItemViewModel>();
            Meetings = new ObservableCollection<NavigationItemViewModel>();
            _eventAggragator.GetEvent<AfterDetailSavedEvent>().Subscribe(AfterDetailSaved);
            _eventAggragator.GetEvent<AfterDetailDeletedEvent>().Subscribe(AfterDetailDeleted);
        }

        public ObservableCollection<NavigationItemViewModel> Friends { get; set; }
        public ObservableCollection<NavigationItemViewModel> Meetings { get; set; }

        public async Task LoadAsync() {
            var lookupItems = await _friendLookupService.GetFriendLookupAsync();
            Friends.Clear();
            foreach (var lookupItem in lookupItems)
                Friends.Add(new NavigationItemViewModel(lookupItem.Id, lookupItem.DisplayMember,
                    nameof(FriendDetailViewModel), _eventAggragator));

            lookupItems = await _meetingLookupService.GetMeetingLookupAsync();
            Meetings.Clear();
            foreach (var lookupItem in lookupItems)
                Meetings.Add(new NavigationItemViewModel(lookupItem.Id, lookupItem.DisplayMember,
                    nameof(MeetingDetailViewModel), _eventAggragator));
        }

        private void AfterDetailSaved(AfterDetailSavedEventArgs args) {
            switch (args.ViewModelName) {
                case nameof(FriendDetailViewModel):
                    AfterDetailSaved(Friends,args);
                    break;
                case nameof(MeetingDetailViewModel):
                    AfterDetailSaved(Meetings, args);
                    break;
            }
        }

        private void AfterDetailSaved(ObservableCollection<NavigationItemViewModel> items, AfterDetailSavedEventArgs args) {
            var lookupItem = items.SingleOrDefault(l => l.Id == args.Id);

            if (lookupItem == null)
                items.Add(new NavigationItemViewModel(args.Id, args.DisplayMember,
                    args.ViewModelName, _eventAggragator));
            else
                lookupItem.DisplayMember = args.DisplayMember;
            { }
        }

        private void AfterDetailDeleted(AfterDetailDeletedEventArgs args) {
            switch (args.ViewModelName) {
                case nameof(FriendDetailViewModel):
                    AfterDetailDeleted(Friends,args);
                    break;
                case nameof(MeetingDetailViewModel):
                    AfterDetailDeleted(Meetings, args);
                    break;
            }
        }

        private void AfterDetailDeleted(ObservableCollection<NavigationItemViewModel> items, AfterDetailDeletedEventArgs args) {
            var item = items.SingleOrDefault(f => f.Id == args.Id);
            if (item != null)
                items.Remove(item);
        }
    }
}
 