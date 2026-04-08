using DomainLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Specifications.MedicalHistorySpecification
{
    class PatientMedicalHistoriesSpecification : BaseSpecifications<MedicalHistory>
    {
        public PatientMedicalHistoriesSpecification(int patientId) : base(M => M.PatientId == patientId)
        {
            AddInclude(M => M.PreScriptions);
        }
        public PatientMedicalHistoriesSpecification(int patientId,int medicalhistoryId) : 
            base(M => M.PatientId == patientId &&M.Id == medicalhistoryId )
        {
            AddInclude(M => M.PreScriptions);
        }
    }
}
