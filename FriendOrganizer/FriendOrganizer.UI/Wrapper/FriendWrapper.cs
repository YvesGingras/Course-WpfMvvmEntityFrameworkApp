using System;
using System.Runtime.Serialization;
using FriendOrganizer.Model;

namespace FriendOrganizer.UI.Wrapper
{
    public class FriendWrapper : ModelWrapper<Friend>
    {
        public FriendWrapper(Friend model) : base(model) { }

        public int Id => Model.Id;

        public string FirstName {
            get => GetValue<string>();
            set {
                SetValue(value);
                ValidiateProperty(nameof(FirstName));
            }
        }

        public string LastName {
            get => GetValue<string>(nameof(LastName));
            set => SetValue(value);
        }

        public string Email {
            get => GetValue<string>();
            set => SetValue(value);
        }

        private void ValidiateProperty(string propertyName) {
            ClearErrors(propertyName);
            switch (propertyName) {
                case nameof(FirstName):
                    if (string.Equals(FirstName, "Robot", StringComparison.CurrentCultureIgnoreCase))
                        AddError(propertyName, "Robots are not valid friends.");
                    break;
            }
        }
    }
}
