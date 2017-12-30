using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using FriendOrganizer.Model;
using FriendOrganizer.UI.Data.Lookups;
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
        private FriendPhoneNumberWrapper _selectedPhoneNumber;
        private readonly IEventAggregator _eventAggregator;
        private bool _hasChanges;
        private readonly IMessageDialogService _messageDialogService;
        private readonly IProgrammingLanguageLookUpDataService _programmingLanguageLookUpDataService;

        public FriendDetailViewModel(IFriendRepository friendRepository, IEventAggregator evanAggregator,
            IMessageDialogService messageDialogService, IProgrammingLanguageLookUpDataService programmingLanguageLookUpDataService) {
            _friendRepository = friendRepository;
            _eventAggregator = evanAggregator;
            _messageDialogService = messageDialogService;
            _programmingLanguageLookUpDataService = programmingLanguageLookUpDataService;

            SaveCommand = new DelegateCommand(OnSaveExecute, OnSaveCanExecute);
            DeleteCommand = new DelegateCommand(OnDeleteExecute);
            AddPhoneNumberCommand = new DelegateCommand(OnAddPhoneNumberExecute);
            RemovePhoneNumberCommand = new DelegateCommand(OnRemovePhoneNumberExecute, OnRemovePhoneNumberCanExecute);


            ProgrammingLanguages = new ObservableCollection<LookupItem>();
            PhoneNumbers = new ObservableCollection<FriendPhoneNumberWrapper>();
        }

        public FriendWrapper Friend {
            get => _friend;
            set {
                _friend = value;
                OnPropertyChanged();
            }
        }
        public FriendPhoneNumberWrapper SelectedPhoneNumber {
            get => _selectedPhoneNumber;
            set {
                _selectedPhoneNumber = value;
                OnPropertyChanged();
                ((DelegateCommand)RemovePhoneNumberCommand).RaiseCanExecuteChanged();
            }
        }
        public bool HasChanges {
            get => _hasChanges;
            set {
                if (_hasChanges == value) return;
                _hasChanges = value;
                OnPropertyChanged();
                ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
            }
        }

        public ICommand SaveCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand AddPhoneNumberCommand { get; }
        public ICommand RemovePhoneNumberCommand { get; }

        public ObservableCollection<LookupItem> ProgrammingLanguages { get; }
        public ObservableCollection<FriendPhoneNumberWrapper> PhoneNumbers { get;  }

        public async Task LoadAsync(int? id) {
            var friend = id.HasValue
                ? await _friendRepository.GetByIdAsync(id.Value)
                : CreateNewFriend();

            InitializeFriend(friend);

            InitializeFriendPhoneNumbers(friend.PhoneNumbers);

            await LoadProgrammingLanguagesLookupAsync();
        }

        private void InitializeFriendPhoneNumbers(ICollection<FriendPhoneNumber> phoneNumbers) {
            foreach (var wrapper in PhoneNumbers)
                wrapper.PropertyChanged -= FriendPhoneNumberWrapper_PropertyChanged;

            PhoneNumbers.Clear();
            foreach (var friendPhoneNumber in phoneNumbers) {
                var wrapper = new FriendPhoneNumberWrapper(friendPhoneNumber);
                PhoneNumbers.Add(wrapper);
                wrapper.PropertyChanged += FriendPhoneNumberWrapper_PropertyChanged;
            }
        }

        private void InitializeFriend(Friend friend) {
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

        private Friend CreateNewFriend() {
            var friend = new Friend();
            _friendRepository.Add(friend);
            return friend;
        }

        private async Task LoadProgrammingLanguagesLookupAsync() {
            ProgrammingLanguages.Clear();
            ProgrammingLanguages.Add(new NullLookupItem {DisplayMember = " - "});
            var lookup = await _programmingLanguageLookUpDataService.GetProgrammingLanguageLookupAsync();

            foreach (var lookupItem in lookup) {
                ProgrammingLanguages.Add(lookupItem);
            }
        }

        private void FriendPhoneNumberWrapper_PropertyChanged(object sender, PropertyChangedEventArgs e) {
            if (!HasChanges)
                HasChanges = _friendRepository.HasChanges();

            if (e.PropertyName == nameof(FriendPhoneNumberWrapper.HasErrors))
                ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
        }

        private bool OnSaveCanExecute() {
            return Friend != null
                && !Friend.HasErrors
                && PhoneNumbers.All(pn => !pn.HasErrors)
                && HasChanges;
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

        private async void OnDeleteExecute() {
            var result = _messageDialogService.ShowOkCancelDialog(
                $"Do you really want to delete the friend {Friend.FirstName} {Friend.LastName}?", "Question");

            if (result == MessageDialogResult.Cancel) return;
            _friendRepository.Remove(Friend.Model);
            await _friendRepository.SaveAsync();
            _eventAggregator.GetEvent<AfterFriendDeletedEvent>().Publish(Friend.Id);
        }

        private void OnAddPhoneNumberExecute() {
            var newNumber = new FriendPhoneNumberWrapper(new FriendPhoneNumber());
            newNumber.PropertyChanged += FriendPhoneNumberWrapper_PropertyChanged;
            PhoneNumbers.Add(newNumber);
            Friend.Model.PhoneNumbers.Add(newNumber.Model);
            newNumber.Number = "";
        }

        private void OnRemovePhoneNumberExecute() {
            SelectedPhoneNumber.PropertyChanged -= FriendPhoneNumberWrapper_PropertyChanged;
            _friendRepository.RemovePhoneNumber(SelectedPhoneNumber.Model);
            PhoneNumbers.Remove(SelectedPhoneNumber);
            SelectedPhoneNumber = null;
            HasChanges = _friendRepository.HasChanges();
            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
        }

        private bool OnRemovePhoneNumberCanExecute() {
            return SelectedPhoneNumber != null;
        }
    }
}
