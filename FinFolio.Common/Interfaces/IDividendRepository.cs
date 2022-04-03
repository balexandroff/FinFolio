using FinFolio.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FinFolio.Core.Interfaces
{
    public interface IDividendRepository : IRepository<Dividend>
    {
        Task CreateAsync(int stockId, IEnumerable<Dividend> divideds);
    }
}
