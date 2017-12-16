using FriendOrganizer.Model;
using FriendOrganizer.UI.ViewModel;

namespace FriendOrganizer.UI.Wrapper
{
    public class FriendWrapper:ViewModelBase
    {
        public FriendWrapper(Friend model) {
            Model = model;
        }

        public Friend Model { get; }

        public int Id => Model.Id;

        public string FirstName {
            get => Model.FirstName;
            set {
                Model.FirstName = value;
                OnPropertyChanged();
            }
        }

        public string LastName {
            get => Model.LastName;
            set {
                Model.LastName = value;
                OnPropertyChanged();
            }
        }

        public string Email {
            get => Model.Email;
            set {
                Model.Email = value;
                OnPropertyChanged();
            } 
        }
    }
}
