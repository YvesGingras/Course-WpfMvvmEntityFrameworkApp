using System;
using System.Collections.Generic;
using FriendOrganizer.Model;

namespace FriendOrganizer.UI.Wrapper
{
    public class FriendWrapper : ModelWrapper<Friend>
    {
        public FriendWrapper(Friend model) : base(model) { }

        public int Id => Model.Id;

        public string FirstName {
            get => GetValue<string>();
            set => SetValue(value);
        }

        public string LastName {
            get => GetValue<string>(nameof(LastName));
            set => SetValue(value);
        }

        public string Email {
            get => GetValue<string>();
            set => SetValue(value);
        }

        public int? FavoriteLanguageId {
            get => GetValue<int?>();
            set => SetValue(value);
        }

        protected override IEnumerable<string> ValidateProperty(string propertyName) {
            switch (propertyName) {
                case nameof(FirstName):
                    if (string.Equals(FirstName, "Robot", StringComparison.CurrentCultureIgnoreCase))
                        yield return "Robots are not valid friends.";
                    break;
            }
        }
    }
}
