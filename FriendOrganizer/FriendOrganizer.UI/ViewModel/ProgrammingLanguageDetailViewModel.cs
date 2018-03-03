using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
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

        public ProgrammingLanguageDetailViewModel(IEventAggregator eventAggregator,
            IMessageDialogService messageDialogService,
            IProgrammingLanguageRepository programmingLanguageRepository)
            : base(eventAggregator, messageDialogService) {

            _programmingLanguageRepository = programmingLanguageRepository;
            Title = "Programming Languages";
            ProgrammingLanguages = new ObservableCollection<ProgrammingLanguageWrapper>();
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
        protected override async void OnSaveExecute()
        {
            await _programmingLanguageRepository.SaveAsync();
            HasChanges = _programmingLanguageRepository.HasChanges();
            RaiseCollectionSavedEvent();    
        }

        private void Wrapper_PropertyChanged(object sender, PropertyChangedEventArgs e) {
            if (!HasChanges)
                HasChanges = _programmingLanguageRepository.HasChanges();

            if (e.PropertyName == nameof(ProgrammingLanguageWrapper.HasErrors))
                ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
        }
    }
}
