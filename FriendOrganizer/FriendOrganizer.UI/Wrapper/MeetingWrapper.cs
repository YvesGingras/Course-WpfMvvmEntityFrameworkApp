using System;
using FriendOrganizer.Model;

namespace FriendOrganizer.UI.Wrapper
{
    public class MeetingWrapper : ModelWrapper<Meeting>
    {
        public MeetingWrapper(Meeting model) : base(model) { }

        public int Id => Model.Id;

        public string Title {
            get => GetValue<string>();
            set => SetValue(value);
        }

        public DateTime DateFrom {
            get => GetValue<DateTime>();
            set => SetValue(value);
        }

        public DateTime DateTo {
            get => GetValue<DateTime>();
            set => SetValue(value);
        }
    }
}
