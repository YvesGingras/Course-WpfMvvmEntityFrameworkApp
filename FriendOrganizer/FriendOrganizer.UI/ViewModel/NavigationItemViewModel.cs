namespace FriendOrganizer.UI.ViewModel
{
    public class NavigationItemViewModel:ViewModelBase
    {
        private string _displayMember;

        public NavigationItemViewModel(int id, string displayMember) {
            Id = id;
            DisplayMember = displayMember;
        }

        public string DisplayMember {
            get => _displayMember;
            set {
                _displayMember = value;
                OnPropertyChanged();
            }
        }

        public int Id { get; }
    }
}
