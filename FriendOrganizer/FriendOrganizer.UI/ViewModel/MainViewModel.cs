using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Autofac.Features.Indexed;
using FriendOrganizer.UI.Event;
using FriendOrganizer.UI.View.Services;
using Prism.Commands;
using Prism.Events;

namespace FriendOrganizer.UI.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private readonly IEventAggregator _eventAggregator;
        private IDetailViewModel _selectedDetailViewModel;
        private readonly IMessageDialogService _messageDialogService;
        private IIndex<string, IDetailViewModel> _detailViewModelCreator;

        public MainViewModel(INavigationViewModel navigationViewModel,
            IIndex<string,IDetailViewModel> detailViewModelCreator,
            IEventAggregator eventAggregator, IMessageDialogService messageDialogService) {
            _eventAggregator = eventAggregator;
            _detailViewModelCreator = detailViewModelCreator;
            _messageDialogService = messageDialogService;
            _eventAggregator.GetEvent<OpenDetailViewEvent>()
                .Subscribe(OnOpenDetailView);
            _eventAggregator.GetEvent<AfterDetailDeletedEvent>().Subscribe(AfterDetailDeleted);
            _eventAggregator.GetEvent<AfterDetailClosedEvent>().Subscribe(AfterDetailClosed);

            DetailViewModels = new ObservableCollection<IDetailViewModel>();
            CreateNewDetailCommand = new DelegateCommand<Type>(OnCreateNewDetailExecute);
            OpenSingleDetailViewCommand = new DelegateCommand<Type>(OnOpenSingleOpenViewExcecute);
            NavigationViewModel = navigationViewModel;
        }

        public ICommand CreateNewDetailCommand { get; }
        public ICommand OpenSingleDetailViewCommand { get; }
        public INavigationViewModel NavigationViewModel { get; }

        public ObservableCollection<IDetailViewModel> DetailViewModels { get; }

        public IDetailViewModel SelectedDetailViewModel {
            get => _selectedDetailViewModel;
            set {
                _selectedDetailViewModel = value;
                OnPropertyChanged();
            }
        }

        public async Task LoadAsync() {
            await NavigationViewModel.LoadAsync();
        }

        private async void OnOpenDetailView(OpenDetailViewEventArgs args) {
            var detailViewModel = DetailViewModels
                .SingleOrDefault(vm => vm.Id == args.Id && vm.GetType().Name == args.ViewModelName);

            if (detailViewModel == null) {
                    detailViewModel = _detailViewModelCreator[args.ViewModelName];
                try {
                    await detailViewModel.LoadAsync(args.Id);
                }
                catch { //was deleted
                    _messageDialogService.ShowInfoDialog("Could not load the entity, "
                                                         + "maybe it was deleted in the meantime by another user. "
                                                         + "The navigation is refreshed for you.");
                    await NavigationViewModel.LoadAsync();
                    return;
                }
                DetailViewModels.Add(detailViewModel);

            }
            SelectedDetailViewModel = detailViewModel;
        }

        private int _nextNewItemId = 0;
        private void OnCreateNewDetailExecute(Type viewModelType) {
            OnOpenDetailView(new OpenDetailViewEventArgs {
                Id = _nextNewItemId--,
                ViewModelName = viewModelType.Name
            });
        }

        private void OnOpenSingleOpenViewExcecute(Type viewModelType) {
            OnOpenDetailView(new OpenDetailViewEventArgs {
                Id = -1,
                ViewModelName = viewModelType.Name
            });
        }

        private void AfterDetailDeleted(AfterDetailDeletedEventArgs args) {
            RemoveDetailViewModel(args.Id,args.ViewModelName);
        }

        private void AfterDetailClosed(AfterDetailClosedEventArgs args) {
            RemoveDetailViewModel(args.Id,args.ViewModelName);
        }

        private void RemoveDetailViewModel(int id, string viewModelName)
        {
            var detailViewModel = DetailViewModels
                .SingleOrDefault(vm => vm.Id == id && vm.GetType().Name == viewModelName);

            if (detailViewModel != null)
                DetailViewModels.Remove(detailViewModel);
        }
    }
}