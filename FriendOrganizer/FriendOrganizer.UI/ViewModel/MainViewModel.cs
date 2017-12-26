using System;
using System.Threading.Tasks;
using System.Windows;
using FriendOrganizer.UI.Event;
using Prism.Events;

namespace FriendOrganizer.UI.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly Func<IFriendDetailViewModel> _friendDetailViewModelCreator;
        private IFriendDetailViewModel _friendDetailViewModel;

        public MainViewModel(INavigationViewModel navigationViewModel,
            Func<IFriendDetailViewModel> friendDetailViewModelCreator,
            IEventAggregator eventAggregator) {
            _eventAggregator = eventAggregator;

            _friendDetailViewModelCreator = friendDetailViewModelCreator;

            _eventAggregator.GetEvent<OpenFriendDetailViewEvent>()
                .Subscribe(OnOpenFriendDetailView);

            NavigationViewModel = navigationViewModel;

        }

        public INavigationViewModel NavigationViewModel { get; }

        public IFriendDetailViewModel FriendDetailViewModel {
            get => _friendDetailViewModel;
            private set {
                _friendDetailViewModel = value;
                OnPropertyChanged();
            }
        }

        public async Task LoadAsync() {
            await NavigationViewModel.LoadAsync();
        }

        private async void OnOpenFriendDetailView(int friendId) {
            if (FriendDetailViewModel != null && FriendDetailViewModel.HasChanges) {
                var result = MessageBox.Show("You've made changes. Navigate away?", "Question",
                    MessageBoxButton.OKCancel);
                if (result == MessageBoxResult.Cancel)
                    return;
            }
            
            FriendDetailViewModel = _friendDetailViewModelCreator();
            await FriendDetailViewModel.LoadAsync(friendId);
        }
    }
}
 