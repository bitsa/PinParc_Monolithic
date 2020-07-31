using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PinParc_Monolith.Models.Domain;

namespace PinParc_Monolith.Infrastructure.Persistance.EF.Configurations.Orders
{
    public class OrderConfig : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> entity)
        {
            entity.Property(e => e.Date).HasMaxLength(150);


            entity.Property(e => e.DestFrom).HasMaxLength(500);

            entity.Property(e => e.DestTo).HasMaxLength(500);

            entity.Property(e => e.Time).HasMaxLength(150);

            entity.Property(e => e.Price).HasColumnType("money");

            entity.HasOne(d => d.ParcelType)
                .WithMany(p => p.Order)
                .HasForeignKey(d => d.ParcelTypeID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Order_ParcelTypes");
        }
    }
}
