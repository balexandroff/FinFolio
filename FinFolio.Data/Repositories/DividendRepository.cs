using FinFolio.Core.Interfaces;
using FinFolio.Core.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinFolio.Data.Repositories
{
    public class DividendRepository : BaseRepository<Dividend>, IDividendRepository
    {
        public DividendRepository(FinFolioContext context) : base(context) { }

        public async Task CreateAsync(int stockId, IEnumerable<Dividend> divideds)
        {
            var dividendEntities = this._context.Dividends.Where(d => d.StockId == stockId).ToList();
            this._context.Dividends.RemoveRange(dividendEntities);

            await base.SaveChangesAsync();

            foreach (var dividend in divideds)
            {
                dividend.CreatedDate = System.DateTime.Now;
                dividend.StockId = stockId;

                await this._context.Dividends.AddAsync(dividend);
            }

            await base.SaveChangesAsync();
        }
    }
}