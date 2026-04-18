using Shared.DTos.AppointmentDTos;
using Shared.DTos.MedicalTestDTos;
using Shared.DTos.PatientDTos;
using Shared.ErrorModels;
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

        public Task<IEnumerable<AvailabilitySlotDto>> GetAllSlotsAsync(string Email);

        Task<MedicalTestDto> UploadMedicalTestAsync(string userId, UploadMedicalTestDto dto);

        Task<IEnumerable<MedicalTestDto>> GetMyMedicalTestsAsync(string userId);
        Task<ServiceResponse> DeleteMedicalTestAsync(string userId, int medicalTestId);
    }
}
