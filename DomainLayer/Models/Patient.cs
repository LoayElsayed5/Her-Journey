using DomainLayer.IdentityModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Models
{
    public class Patient
    {
        public int Id { get; set; }


        public DateOnly DateOfBirth { get; set; }


        public string BloodType { get; set; } = default!;
        public int Height { get; set; }
        public int Age { get; set; }
        public int Weight { get; set; }
        public int PregnancyWeek { get; set; }
        public int NumberOfPregnancies { get; set; }

        public string UserId { get; set; }

        public ApplicationUser User { get; set; } = default!;

        public int DoctorID { get; set; }

        public Doctor Doctor { get; set; } = default!;

        public ICollection<MedicalHistory>? MedicalHistory { get; set; } = [];

        public ICollection<Appointment>? Appointments { get; set; } = [];

        public ICollection<MedicalTest>? MedicalTests { get; set; } = [];

    }
}
