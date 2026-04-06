using Shared.DTos.PatientDTos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesAbstraction.PatientAbstraction
{
    public interface IPatientService
    {
        public Task<bool> CompleteProfileAsync(string UserId, CompleteMedicalProfileDto profileDto);
    }
}
