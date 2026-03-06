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
    public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
    {
        public void Configure(EntityTypeBuilder<Appointment> builder)
        {
            builder.HasOne(A => A.Patient)
                .WithMany(P => P.Appointments)
                .HasForeignKey(A => A.PatientId)
                .OnDelete(DeleteBehavior.Restrict);



            builder.HasOne(A => A.Doctor)
                .WithMany(D => D.Appointments)
                .HasForeignKey(A => A.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);


            builder.HasOne(A => A.AvailabilitySlot)
                .WithOne(AS => AS.Appointment)
                .HasForeignKey<Appointment>(A => A.AvailabilitySlotId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(a => a.AvailabilitySlotId); // isUniqe malhash lazma 3ashan el relation aslan ( 1 to 1 ) 
        }

    }
}
