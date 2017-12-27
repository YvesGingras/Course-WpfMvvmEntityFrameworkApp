using System.Threading.Tasks;
using System.Windows.Input;
using FriendOrganizer.Model;
using FriendOrganizer.UI.Data.Repositories;
using FriendOrganizer.UI.Event;
using FriendOrganizer.UI.View.Services;
using FriendOrganizer.UI.Wrapper;
using Prism.Commands;
using Prism.Events;

namespace FriendOrganizer.UI.ViewModel
{
    public class FriendDetailViewModel : ViewModelBase, IFriendDetailViewModel
    {
        private readonly IFriendRepository _friendRepository;
        private FriendWrapper _friend;
        private readonly IEventAggregator _eventAggregator;
        private bool _hasChanges;
        private IMessageDialogService _messageDialogService;

        public FriendDetailViewModel(IFriendRepository friendRepository, IEventAggregator evanAggregator, IMessageDialogService messageDialogService) {
            _friendRepository = friendRepository;
            _eventAggregator = evanAggregator;
            _messageDialogService = messageDialogService;
            SaveCommand = new DelegateCommand(OnSaveExecute, OnSaveCanExecute);
            DeleteCommand = new DelegateCommand(OnDeleteExecute);
        }

        public FriendWrapper Friend {
            get => _friend;
            set {
                _friend = value;
                OnPropertyChanged();
            }
        }

        public ICommand SaveCommand { get; }
        public ICommand DeleteCommand { get;}

        public bool HasChanges {
            get => _hasChanges;
            set {
                if (_hasChanges == value) return;
                _hasChanges = value;
                OnPropertyChanged();
                ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
            }
        }

        public async Task LoadAsync(int? friendId)
        {
            var friend = friendId.HasValue
                ? await _friendRepository.GetByIdAsync(friendId.Value)
                : CreateNewFriend() ;
            Friend = new FriendWrapper(friend);

            Friend.PropertyChanged += (s, e) => {
                if (!HasChanges)
                    HasChanges = _friendRepository.HasChanges();
                if (e.PropertyName == nameof(Friend.HasErrors))
                    ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
            };

            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
            if (Friend.Id == 0)
                //Little trick to trigger the validation. :-(
                Friend.FirstName = "";
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

        private Friend CreateNewFriend() {
            var friend = new Friend();
            _friendRepository.Add(friend);
            return friend;
        }

        private async void OnDeleteExecute() {
            var result = _messageDialogService.ShowOkCancelDialog(
                $"Do you really want to delete the friend {Friend.FirstName} {Friend.LastName}?", "Question");

            if (result == MessageDialogResult.Cancel) return;
            _friendRepository.Remove(Friend.Model);
            await _friendRepository.SaveAsync();
            _eventAggregator.GetEvent<AfterFriendDeletedEvent>().Publish(Friend.Id);
        }
    }
}
