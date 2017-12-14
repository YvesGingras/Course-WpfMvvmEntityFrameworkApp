using System.Threading.Tasks;
using System.Windows.Input;
using FriendOrganizer.Model;
using FriendOrganizer.UI.Data;
using FriendOrganizer.UI.Event;
using Prism.Commands;
using Prism.Events;

namespace FriendOrganizer.UI.ViewModel
{
    public class FriendDetailViewModel : ViewModelBase, IFriendDetailViewModel
    {
        private readonly IFriendDataService _dataService;
        private Friend _friend;
        private IEventAggregator _eventAggregator;

        public FriendDetailViewModel(IFriendDataService dataService, IEventAggregator evanAggregator) {
            _dataService = dataService;
            _eventAggregator = evanAggregator;
            _eventAggregator.GetEvent<OpenFriendDetailViewEvent>()
                .Subscribe(OnOpenFriendDetailView);

            SaveCommand = new DelegateCommand(OnSaveExecute, OnSaveCanExecute);
        }

        /*Leave public*/
        public Friend Friend {
            get => _friend;
            set {
                _friend = value;
                OnPropertyChanged();
            }
        }

        /*Leave public*/
        public ICommand SaveCommand { get; }

        private bool OnSaveCanExecute() { 
            // todo: Check if friend is valid.
            return true;
        }

        private async void OnSaveExecute() {
            await _dataService.SaveAsync(Friend);
            _eventAggregator.GetEvent<AfterFriendSavedEvent>().Publish(
                new AfterFriendSavedEventArgs {
                    Id = Friend.Id,
                    DisplayMember = $"{Friend.FirstName} {Friend.LastName}"
                });
        } 

        private async void OnOpenFriendDetailView(int friendId) {
            await LoadAsync(friendId);
        }


        public async Task LoadAsync(int friendId) {
            Friend = await _dataService.GetByIdAsync(friendId);
        }
    }
}
