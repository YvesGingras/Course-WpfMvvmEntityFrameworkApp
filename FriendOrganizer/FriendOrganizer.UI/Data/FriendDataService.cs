using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
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

        public async Task<Friend> GetByIdAsync(int friendId) {
            using (var context = _contextCreator()) {
                return await context.Friends.AsNoTracking().SingleAsync(f => f.Id == friendId);
            }
        }
    }
} 
  