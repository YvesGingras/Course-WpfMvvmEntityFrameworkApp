using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using FriendOrganizer.Model;
using FriendOrganizer.UI.Data.Repositories;
using FriendOrganizer.UI.View.Services;
using FriendOrganizer.UI.Wrapper;
using Prism.Commands;
using Prism.Events;

namespace FriendOrganizer.UI.ViewModel
{
    public class MeetingDetailViewModel : DetailViewModelBase, IMeetingDetailViewModel
    {
        private readonly IMessageDialogService _messageDialogService;
        private readonly IMeetingRepository _meetingRepository;
        private MeetingWrapper _meetingWrapper;
        private Friend _selectedAddedFriend;
        private Friend _selectedAvailableFriend;

        public MeetingDetailViewModel(IEventAggregator eventAggregator,
            IMessageDialogService messageDialogService,
            IMeetingRepository meetingRepository) : base(eventAggregator) {
            _messageDialogService = messageDialogService;
            _meetingRepository = meetingRepository;

            AddedFriends = new ObservableCollection<Friend>();
            AvailableFriends = new ObservableCollection<Friend>();
            AddFriendCommand = new DelegateCommand(OnAddFriendExecute,OnAddFriendCanExecute);
            RemoveFriendCommand = new DelegateCommand(OnRemoveFriendExecute,OnRemoveFriendCanExecute);
            
            
        }

        
        public MeetingWrapper Meeting {
            get => _meetingWrapper;
            set {
                _meetingWrapper = value;
                OnPropertyChanged();
            }
        }
        public Friend SelectedAvailableFriend {
            get => _selectedAvailableFriend;
            set {
                _selectedAvailableFriend = value;
                OnPropertyChanged();
                ((DelegateCommand)AddFriendCommand).RaiseCanExecuteChanged();
            }
        }
        public Friend SelectedAddedFriend {
            get => _selectedAddedFriend;
            set {
                _selectedAddedFriend = value;
                OnPropertyChanged();
                ((DelegateCommand)AddFriendCommand).RaiseCanExecuteChanged();
            }
        }

        public ObservableCollection<Friend> AddedFriends { get; }
        public ObservableCollection<Friend> AvailableFriends { get; }

        public ICommand AddFriendCommand { get; }
        public ICommand RemoveFriendCommand { get; }

        public override async Task LoadAsync(int? meetingId) {
            var meeting = meetingId.HasValue
                ? await _meetingRepository.GetByIdAsync(meetingId.Value)
                : CreateNewMeeting();

            InitializeMeeting(meeting);

            // todo: Load the friends for the picklist.
        }

        protected override void OnDeleteExecute() {
            var result = _messageDialogService.ShowOkCancelDialog($"Do you really want to delete the meeting {Meeting.Title}?", "Question");
            if (result == MessageDialogResult.Ok) {
                _meetingRepository.Remove(Meeting.Model);
                _meetingRepository.SaveAsync();
                RaiseDetailDeletedEvent(Meeting.Id);
            }
        }

        protected override bool OnSaveCanExecute() {
            return Meeting != null && !Meeting.HasErrors && HasChanges;
        }

        protected override void OnSaveExecute() {
            _meetingRepository.SaveAsync();
            HasChanges = _meetingRepository.HasChanges();
            RaiseDetailSavedEvent(Meeting.Id,Meeting.Title);
        }

        private Meeting CreateNewMeeting()
        {
            var meeting = new Meeting
            {
                DateFrom = DateTime.Now.Date,
                DateTo = DateTime.Now.Date
            };

            _meetingRepository.Add(meeting);
            return meeting;
        }

        private void InitializeMeeting(Meeting meeting)
        {
            Meeting = new MeetingWrapper(meeting);

            Meeting.PropertyChanged += (s, e) => {
                if (!HasChanges)
                    HasChanges = _meetingRepository.HasChanges();

                if (e.PropertyName == nameof(Meeting.HasErrors))
                    ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
            };
            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();

            //Trick to validate the validation.
            if (Meeting.Id == 0)
                Meeting.Title = "";
        }

        private bool OnRemoveFriendCanExecute() {
            return SelectedAddedFriend != null;
        }

        private void OnRemoveFriendExecute() {
            // todo: Implement remove logic.
        }

        private bool OnAddFriendCanExecute() {
            return SelectedAvailableFriend != null;
        }

        private void OnAddFriendExecute() {
            // todo: Implement add logic.
        }

    }
}
