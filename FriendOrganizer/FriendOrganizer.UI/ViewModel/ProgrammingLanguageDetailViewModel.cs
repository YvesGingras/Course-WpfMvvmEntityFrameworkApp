using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using FriendOrganizer.Model;
using FriendOrganizer.UI.Data.Lookups;
using FriendOrganizer.UI.Data.Repositories;
using FriendOrganizer.UI.View.Services;
using FriendOrganizer.UI.Wrapper;
using Prism.Commands;
using Prism.Events;

namespace FriendOrganizer.UI.ViewModel
{
    public class ProgrammingLanguageDetailViewModel : DetailViewModelBase
    {
        private readonly IProgrammingLanguageRepository _programmingLanguageRepository;
        private ProgrammingLanguageWrapper _selectedProgrammingLanguage;

        public ProgrammingLanguageDetailViewModel(IEventAggregator eventAggregator,
            IMessageDialogService messageDialogService,
            IProgrammingLanguageRepository programmingLanguageRepository)
            : base(eventAggregator, messageDialogService) {

            _programmingLanguageRepository = programmingLanguageRepository;
            Title = "Programming Languages";
            ProgrammingLanguages = new ObservableCollection<ProgrammingLanguageWrapper>();

            AddCommand = new DelegateCommand(OnAddExecute);
            RemoveCommand = new DelegateCommand(OnRemoveExecute, OnRemoveCanExecute);
        }

        public ICommand RemoveCommand { get; set; }
        public ICommand AddCommand { get; set; }

        public ProgrammingLanguageWrapper SelectedProgrammingLanguage {
            get => _selectedProgrammingLanguage;
            set {
                _selectedProgrammingLanguage = value;
                OnPropertyChanged();
                ((DelegateCommand)RemoveCommand).RaiseCanExecuteChanged();
            }
        }

        public ObservableCollection<ProgrammingLanguageWrapper> ProgrammingLanguages { get; set; }
        public override async Task LoadAsync(int id) {
            Id = id;
            foreach (var wrapper in ProgrammingLanguages) {
                wrapper.PropertyChanged -= Wrapper_PropertyChanged;
            }
            ProgrammingLanguages.Clear();

            var languages = await _programmingLanguageRepository.GetAllAsync();

            foreach (var model in languages) {
                var wrapper = new ProgrammingLanguageWrapper(model);
                wrapper.PropertyChanged += Wrapper_PropertyChanged;
                ProgrammingLanguages.Add(wrapper);
            }
        }
        protected override bool OnSaveCanExecute() {
            return HasChanges && ProgrammingLanguages.All(p => !p.HasErrors);
        }
        protected override void OnDeleteExecute()
        {
            throw new System.NotImplementedException();
        }
        protected override async void OnSaveExecute() {
            try {
                await _programmingLanguageRepository.SaveAsync();
                HasChanges = _programmingLanguageRepository.HasChanges();
                RaiseCollectionSavedEvent();
            }
            catch (Exception ex) {
                while (ex.InnerException != null)
                    ex = ex.InnerException;

                    await MessageDialogService.ShowInfoDialogAsync("Error while saving the entities, the data will be reloaded. Details: "
                                                                   + ex.Message);
                await LoadAsync(Id);
            }
        }
        private void OnAddExecute() {
            var wrapper = new ProgrammingLanguageWrapper(new ProgrammingLanguage());
            wrapper.PropertyChanged += Wrapper_PropertyChanged;
            _programmingLanguageRepository.Add(wrapper.Model);
            ProgrammingLanguages.Add(wrapper);

            //trigger the validation
            wrapper.Name = "";
        }
        private async void OnRemoveExecute() {
            var isReferenced = await _programmingLanguageRepository.IsRererencedByFriendAsync(SelectedProgrammingLanguage.Id);
            if (isReferenced) {
                await MessageDialogService.ShowInfoDialogAsync($"The language {SelectedProgrammingLanguage.Name}"
                                                               + " cannot be removed, as it is referenced by at least one friend.");
                return;
            }

            SelectedProgrammingLanguage.PropertyChanged -= Wrapper_PropertyChanged;
            _programmingLanguageRepository.Remove(SelectedProgrammingLanguage.Model);
            ProgrammingLanguages.Remove(SelectedProgrammingLanguage);
            SelectedProgrammingLanguage = null;
            HasChanges = _programmingLanguageRepository.HasChanges();
            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
        }
        private bool OnRemoveCanExecute() {
            return SelectedProgrammingLanguage != null;
        }
        private void Wrapper_PropertyChanged(object sender, PropertyChangedEventArgs e) {
            if (!HasChanges)
                HasChanges = _programmingLanguageRepository.HasChanges();

            if (e.PropertyName == nameof(ProgrammingLanguageWrapper.HasErrors))
                ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
        }
    }
}
 