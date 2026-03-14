using DomainLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Specifications.DoctorSpecifications
{
     class DoctorsConfirmedSpecification :BaseSpecifications<Doctor>
    {
        public DoctorsConfirmedSpecification() : base(D => D.User.EmailConfirmed == true)
        {
            AddInclude(d => d.User);
        }
    }
}
