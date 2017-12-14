using System;
using System.Data.Entity;
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

        public async Task SaveAsync(Friend friend) {
            using (var context = _contextCreator()) {
                context.Friends.Attach(friend);
                context.Entry(friend).State = EntityState.Modified;
                await context.SaveChangesAsync();
            }
        }
    }
} 
  