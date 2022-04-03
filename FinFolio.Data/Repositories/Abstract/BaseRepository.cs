using FinFolio.Core.Entities;
using FinFolio.Core.Interfaces;
using System.Linq;
using System.Threading.Tasks;

namespace FinFolio.Data.Repositories
{
    public abstract class BaseRepository : IRepository { }
    public abstract class BaseRepository<T> : BaseRepository, IRepository<T> where T : AuditableEntity
    {
        protected readonly FinFolioContext _context;

        public BaseRepository(FinFolioContext context)
        {
            this._context = context;
        }

        public async Task<T> GetAsync(int id)
        {
            return await this._context.FindAsync<T>(id);   
        }

        public IQueryable<T> GetAll()
        {
            return this._context.Set<T>().Select(e => e);
        }

        public async Task<T> AddAsync(T entity)
        {
           await this._context.AddAsync<T>(entity);

           return entity;
        }

        public void Update(T entity)
        {
            this._context.Update(entity);
        }

        public void Delete(int id)
        {
            this._context.Remove(id);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await this._context.SaveChangesAsync();
        }
    }
}