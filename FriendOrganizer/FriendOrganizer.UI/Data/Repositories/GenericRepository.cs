using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;

namespace FriendOrganizer.UI.Data.Repositories
{
    public class GenericRepository<TEntity, TConttext> : IGenericRepository<TEntity>
        where TEntity : class
        where TConttext : DbContext
    {
        protected readonly TConttext Context;

        protected GenericRepository(TConttext context) {
            Context = context;
        }

        public void Add(TEntity model) {
            Context.Set<TEntity>().Add(model);
        }

        public virtual async Task<TEntity> GetByIdAsync(int id) {
            return await Context.Set<TEntity>().FindAsync(id);
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync() {
            return await Context.Set<TEntity>().ToListAsync();
        }

        public async Task SaveAsync() {
            await Context.SaveChangesAsync();
        }

        public bool HasChanges() {
            return Context.ChangeTracker.HasChanges();
        }

        public void Remove(TEntity model) {
            Context.Set<TEntity>().Remove(model);
        }
    }
}