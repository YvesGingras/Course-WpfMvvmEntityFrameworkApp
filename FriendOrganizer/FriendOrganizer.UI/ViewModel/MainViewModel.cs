using System.Threading.Tasks;

namespace FriendOrganizer.UI.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        
        public MainViewModel(INavigationViewModel navigationViewModel) {
            NavigationViewModel = navigationViewModel;
        }

        public INavigationViewModel NavigationViewModel { get; }

        public async Task LoadAsync() {
            await NavigationViewModel.LoadAsync();
        }
    }
}
 