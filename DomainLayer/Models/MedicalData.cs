using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Models
{
    public class MedicalData
    {
        public DateOnly? DateOfBirth { get; set; }
        public int? Age { get; set; }

        public string? BloodType { get; set; } = default!;
        public int? Height { get; set; }
        public int? Weight { get; set; }
        public DateOnly? PregnancyStartDate { get; set; }
        public int? PregnancyWeek { get; set; }
        public int? NumberOfPregnancies { get; set; }
    }
}
