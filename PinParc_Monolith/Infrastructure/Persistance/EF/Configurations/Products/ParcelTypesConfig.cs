using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PinParc_Monolith.Models.Domain;

namespace PinParc_Monolith.Infrastructure.Persistance.EF.Configurations.Products
{
    public class ParcelTypesConfig : IEntityTypeConfiguration<ParcelTypes>
    {
        public void Configure(EntityTypeBuilder<ParcelTypes> entity)
        {
            entity.Property(e => e.Coefficient).HasColumnType("money");

            entity.Property(e => e.Name).HasMaxLength(150);
        }
    }
}
