using Shared.DTos.AppointmentDTos;
using Shared.DTos.DashBoardDTos;
using Shared.DTos.DoctorDTos;
using Shared.DTos.MedicalHistoryDTos;
using Shared.ErrorModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesAbstraction.DoctorAbstraction
{
    public interface IDoctorService
    {
        public Task<IEnumerable<DoctorPatientDto>> GetAllPatientsAsync(string Email);

        public Task<MedicalHistoryDetailsDto> AddMedicalHistoryAsync(string Email,int PatientId,AddMedicalHistoryDto addMedicaldto);
        public Task<IEnumerable<MedicalHistoryDetailsDto>> GetPatientMedicalHistoriesAsync(string Email, int PatientId);
        public Task<MedicalHistoryDetailsDto> GetPatientMedicalHistoryByIdAsync(string Email, int PatientId,int MedicalHistoryId);

        public Task<bool> AddAvailabilitySlotAsync(string Email, AddAvailabilitySlotDto addAvailabilitySlot);

        public Task<IEnumerable<AvailabilitySlotDto>> GetMyAvailabilitySlotsAsync(string Email);

        public Task<ServiceResponse> UpdateAvailabilitySlotAsync(string Email, int SlotId, UpdateAvailabilitySlotDto updateAvailabilitySlot);

        public  Task<ServiceResponse> DeleteAvailabilitySlotAsync(string email, int slotId);
    }
}
