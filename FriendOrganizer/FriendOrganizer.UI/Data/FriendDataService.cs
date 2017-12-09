using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using FriendOrganizer.DataAccess;
using FriendOrganizer.Model;

namespace FriendOrganizer.UI.Data
{
    public class FriendDataService : IFriendDataService
    {
        private readonly Func<FriendOrganizerDbContext> _contextCreator;

        public FriendDataService(Func<FriendOrganizerDbContext> contextCreator) {
            _contextCreator = contextCreator;
        }

        public IEnumerable<Friend> GetAll() {
  
            /*before dependency injection in the constructor and autofac*/
            //using (var context = new FriendOrganizerDbContext()) {
            //    return context.Friends.AsNoTracking().ToList();
            //}

            using (var context = _contextCreator())
            {
                return context.Friends.AsNoTracking().ToList();
            }

        }
    }
}
  