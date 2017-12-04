using System.Collections.Generic;
using FriendOrganizer.Model;

namespace FriendOrganizer.UI.Data {
    public interface IFriendDataService {
        IEnumerable<Friend> GetAll();
    }
}