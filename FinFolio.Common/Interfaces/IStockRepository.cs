using FinFolio.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FinFolio.Core.Interfaces
{
    public interface IStockRepository : IRepository<Stock>
    {
        Task<Stock> CreateAsync(Stock stock);
    }
}
