using System;
using System.Runtime.Serialization;
using FriendOrganizer.Model;

namespace FriendOrganizer.UI.Wrapper
{
    public class FriendWrapper:NotifyDataErrorBase
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
                ValdiateProperty(nameof(FirstName));
            }
        }

        private void ValdiateProperty(string propertyName) {
            ClearErrors(propertyName);
            switch (propertyName) {
                case nameof(FirstName):
                    if (string.Equals(FirstName, "Robot", StringComparison.CurrentCultureIgnoreCase))
                        AddError(propertyName, "Robots are not valid friends.");
                    break;
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
