using System.Collections.Concurrent;
using Prism.Events;

namespace FriendOrganizer.UI.Event
{
    class AfterDetailClosedEventArgs
    {
        public int Id { get; set; }
        public string ViewModelName { get; set; }
    }
}
