using FinFolio.Core.Entities;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinFolio.Core.Entities
{
    public class Dividend : AuditableEntity
    {
        public int StockId { get; set; }

        public DateTime Date { get; set; }

        public decimal Amount { get; set; }

        public virtual Stock Stock { get; set; }
    }
}
