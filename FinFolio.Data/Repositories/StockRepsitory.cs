using FinFolio.Core.Interfaces;
using FinFolio.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinFolio.Data.Repositories
{
    public class StockRepository : BaseRepository<Stock>, IStockRepository
    {
        public StockRepository(FinFolioContext context) : base(context) { }

        public async Task<Stock> CreateAsync(Stock stock)
        {
            var entity = await base._context.Stocks.FirstOrDefaultAsync(s => s.Ticker == stock.Ticker);

            if (entity == null)
            {
                stock.CreatedDate = System.DateTime.Now;
                await this._context.Stocks.AddAsync(stock);
            }

            await base.SaveChangesAsync();

            return await base._context.Stocks.Where(s => s.Ticker == stock.Ticker).FirstOrDefaultAsync();
        }
    }
}