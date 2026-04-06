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



            builder.OwnsOne(p => p.MedicalInfo, medicalBuilder =>
            {
                medicalBuilder.Property(m => m.PregnancyWeek)
                              .HasComputedColumnSql("(DATEDIFF(DAY, [PregnancyStartDate], GETDATE()) / 7) + 1", stored: false);

                medicalBuilder.Property(p => p.Age)
                   .HasComputedColumnSql("DATEDIFF(YEAR, [DateOfBirth], GETDATE()) " +
                   "- CASE WHEN DATEADD(YEAR, DATEDIFF(YEAR, [DateOfBirth], GETDATE()), [DateOfBirth]) > GETDATE() THEN 1 ELSE 0 END"
                   , stored: false);

                medicalBuilder.Property(m => m.Age).HasColumnName("Age");
                medicalBuilder.Property(m => m.DateOfBirth).HasColumnName("DateOfBirth");
                medicalBuilder.Property(m => m.Height).HasColumnName("Height");
                medicalBuilder.Property(m => m.Weight).HasColumnName("Weight");
                medicalBuilder.Property(m => m.BloodType).HasColumnName("BloodType").HasMaxLength(5);
                medicalBuilder.Property(m => m.NumberOfPregnancies).HasColumnName("NumberOfPregnancies");
                medicalBuilder.Property(m => m.PregnancyStartDate).HasColumnName("PregnancyStartDate");
                medicalBuilder.Property(m => m.PregnancyWeek).HasColumnName("PregnancyWeek");
            });
        }
    }
}
