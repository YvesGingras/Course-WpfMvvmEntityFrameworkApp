using System.Collections.Generic;
using FriendOrganizer.Model;

namespace FriendOrganizer.UI.Data
{
    public class FriendDataService : IFriendDataService
    {
        public IEnumerable<Friend> GetAll() {
            //Todo: Load data from real database.
            yield return new Friend {FirstName = "Thomas", LastName = "Huber"};
            yield return new Friend {FirstName = "Andreas", LastName = "Boelher"};
            yield return new Friend {FirstName = "Julia", LastName = "Huber"};
            yield return new Friend {FirstName = "Chrissi", LastName = "Egin"};
        }
    }
}
 