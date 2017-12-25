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
        private bool _hasChanges;

        public FriendDetailViewModel(IFriendRepository friendRepository, IEventAggregator evanAggregator) {
            _friendRepository = friendRepository;
            _eventAggregator = evanAggregator;

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

        public bool HasChanges {
            get => _hasChanges;
            set {
                if (_hasChanges == value) return;
                _hasChanges = value;
                OnPropertyChanged();
                ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
            }
        }

        public async Task LoadAsync(int friendId)
        {
            var friend = await _friendRepository.GetByIdAsync(friendId);
            Friend = new FriendWrapper(friend);

            Friend.PropertyChanged += (s, e) => {
                if (!HasChanges)
                    HasChanges = _friendRepository.HasChanges();
                if (e.PropertyName == nameof(Friend.HasErrors))
                    ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
            };

            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
        }

        private bool OnSaveCanExecute() { 
            return Friend!=null && !Friend.HasErrors && HasChanges;
        }

        private async void OnSaveExecute() {
            await _friendRepository.SaveAsync();
            HasChanges = _friendRepository.HasChanges();

            _eventAggregator.GetEvent<AfterFriendSavedEvent>().Publish(
                new AfterFriendSavedEventArgs {
                    Id = Friend.Id,
                    DisplayMember = $"{Friend.FirstName} {Friend.LastName}"
                });
        } 
    }
}
