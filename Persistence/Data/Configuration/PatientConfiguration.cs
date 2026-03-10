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
    public class PatientConfiguration : IEntityTypeConfiguration<Patient>
    {
        public void Configure(EntityTypeBuilder<Patient> builder)
        {
            builder.HasOne(P => P.User)
                .WithOne(U => U.Patient)
                .HasForeignKey<Patient>(P => P.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(P => P.Doctor)
                .WithMany(D => D.Patients)
                .HasForeignKey(P => P.DoctorID)
                .OnDelete(DeleteBehavior.Restrict);


            builder.Property(p => p.BloodType)
                .HasMaxLength(5);
        }
    }
}
