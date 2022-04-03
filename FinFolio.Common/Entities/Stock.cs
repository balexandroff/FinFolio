using FinFolio.Core.Entities;
using System.Collections.Generic;

namespace FinFolio.Core.Entities
{
    public class Stock : AuditableEntity
    {
        public string Ticker { get; set; }

        public string Name { get; set; }

        public virtual ICollection<Dividend> Dividends { get; set; }
    }
}
