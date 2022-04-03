using System;
using System.ComponentModel.DataAnnotations;

namespace FinFolio.Core.Entities
{
    public abstract class AuditableEntity
    {
        [Key]
        public int Id { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }
    }
}
