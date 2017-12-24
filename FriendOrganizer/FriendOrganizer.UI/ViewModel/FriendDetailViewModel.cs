using System.Threading.Tasks;
using System.Windows.Input;
using FriendOrganizer.UI.Data.Repositories;
using FriendOrganizer.UI.Event;
using FriendOrganizer.UI.Wrapper;
using Prism.Commands;
using Prism.Events;

namespace FriendOrganizer.UI.ViewModel
{
    public class FriendDetailViewModel : ViewModelBase, IFriendDetailViewModel
    {
        private readonly IFriendRepository _friendRepository;
        private FriendWrapper _friend;
        private IEventAggregator _eventAggregator;

        public FriendDetailViewModel(IFriendRepository friendRepository, IEventAggregator evanAggregator) {
            _friendRepository = friendRepository;
            _eventAggregator = evanAggregator;
            _eventAggregator.GetEvent<OpenFriendDetailViewEvent>()
                .Subscribe(OnOpenFriendDetailView);

            SaveCommand = new DelegateCommand(OnSaveExecute, OnSaveCanExecute);
        }
         
        public FriendWrapper Friend {
            get => _friend;
            set {
                _friend = value;
                OnPropertyChanged();
            }
        }

        public ICommand SaveCommand { get; }

        private bool OnSaveCanExecute() { 
            // todo: Check in addition if friend has changes.
            return Friend!=null && !Friend.HasErrors;
        }

        private async void OnSaveExecute() {
            await _friendRepository.SaveAsync();
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
            var friend = await _friendRepository.GetByIdAsync(friendId);
            Friend = new FriendWrapper(friend);
            Friend.PropertyChanged += (s, e) => {
                if(e.PropertyName==nameof(Friend.HasErrors))
                    ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
            };

            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
        }
    }
}
