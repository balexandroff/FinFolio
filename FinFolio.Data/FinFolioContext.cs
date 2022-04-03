using FinFolio.Core.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace FinFolio.Data
{
    public partial class FinFolioContext :  IdentityDbContext<User, Role, int, UserClaim, UserRole, UserLogin, RoleClaim, UserToken>
    {
        public FinFolioContext()
        {
        }

        public FinFolioContext(DbContextOptions<FinFolioContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Stock> Stocks { get; set; }
        public virtual DbSet<Dividend> Dividends { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Dividend>().HasOne(d => d.Stock)
               .WithMany(p => p.Dividends)
               .HasForeignKey(d => d.StockId)
               .OnDelete(DeleteBehavior.NoAction);

            base.OnModelCreating(modelBuilder);
        }
    }
}
