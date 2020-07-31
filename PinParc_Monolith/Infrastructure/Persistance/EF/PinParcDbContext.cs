using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PinParc_Monolith.Models.Domain;

namespace PinParc_Monolith.Infrastructure.EF
{

    public partial class PinParcDbContext : DbContext
    {
        public PinParcDbContext()
        {
        }

        public PinParcDbContext(DbContextOptions<PinParcDbContext> options)
            : base(options)
        {

        }

        public virtual DbSet<Order> Order { get; set; }
        public virtual DbSet<ParcelTypes> ParcelTypes { get; set; }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {

            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.6-servicing-10079");

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(PinParcDbContext).Assembly);
        }
    }
}
