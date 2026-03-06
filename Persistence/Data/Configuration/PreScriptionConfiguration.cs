using DomainLayer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Data.Configuration
{
    public class PreScriptionConfiguration : IEntityTypeConfiguration<PreScription>
    {
        public void Configure(EntityTypeBuilder<PreScription> builder)
        {
            builder.HasOne(P=>P.MedicalHistory)
                .WithMany(M=>M.PreScriptions)
                .HasForeignKey(P=>P.MedicalHistoryId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
