using System.Data.Entity;
using System.Threading.Tasks;
using FriendOrganizer.DataAccess;
using FriendOrganizer.Model;

namespace FriendOrganizer.UI.Data.Repositories
{
    public class FriendRepository : IFriendRepository
    {
        private readonly FriendOrganizerDbContext _context;

        public FriendRepository(FriendOrganizerDbContext context) {
            _context = context;
        }

        public async Task<Friend> GetByIdAsync(int friendId) {
            return await _context.Friends.SingleAsync(f => f.Id == friendId);
        }

        public void Add(Friend friend) {
            _context.Friends.Add(friend);
        }

        public void Remove(Friend model) {
            _context.Friends.Remove(model);
        }

        public async Task SaveAsync() {
            await _context.SaveChangesAsync();
        }

        public bool HasChanges() {
            return _context.ChangeTracker.HasChanges();
        }
    }
} 
   