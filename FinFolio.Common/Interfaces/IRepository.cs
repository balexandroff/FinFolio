using FinFolio.Core.Entities;
using System.Linq;
using System.Threading.Tasks;

namespace FinFolio.Core.Interfaces
{
    public interface IRepository { }
    public interface IRepository<T> : IRepository where T : AuditableEntity
    {
        public Task<T> GetAsync(int id);

        public IQueryable<T> GetAll();

        public Task<T> AddAsync(T entity);

        public void Update(T entity);

        public void Delete(int id);

        public Task<int> SaveChangesAsync();
    }
}
