using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServicesAbstraction;
using ServicesAbstraction.DoctorAbstraction;
using Shared.DTos.AppointmentDTos;
using Shared.DTos.DoctorDTos;
using Shared.DTos.MedicalHistoryDTos;
using Shared.ErrorModels;
using System.Security.Claims;

namespace Presentation.Controllers
{
    [Authorize(Roles = "Doctor")]
    public class DoctorController(IServiceManger _serviceManger) : ApiBaseController
    {
        [HttpGet("GetAllPatients")]
        public async Task<ActionResult<IEnumerable<DoctorPatientDto>>> GetMyPatients()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);


            var result = await _serviceManger.DoctorService.GetAllPatientsAsync(email);

            return Ok(result);
        }
        [HttpPost("AddMedicalHistory")]
        public async Task<ActionResult<MedicalHistoryDetailsDto>> AddMedicalHistory(int patientId, AddMedicalHistoryDto dto)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);


            var result = await _serviceManger.DoctorService.AddMedicalHistoryAsync(email, patientId, dto);

            return Ok(result);
        }

        [HttpGet("GetPatientMedicalHistories")]
        public async Task<ActionResult<IEnumerable<MedicalHistoryDetailsDto>>> GetPatientMedicalHistories(int patientId)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);


            var result = await _serviceManger.DoctorService.GetPatientMedicalHistoriesAsync(email!, patientId);

            return Ok(result);
        }

        [HttpGet("GetMedicalHistoryById")]
        public async Task<ActionResult<MedicalHistoryDetailsDto>> GetPatientMedicalHistoryById(int patientId, int medicalHistoryId)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var result = await _serviceManger.DoctorService.GetPatientMedicalHistoryByIdAsync(email, patientId, medicalHistoryId);
            return Ok(result);
        }

        [HttpPost("AddAvailabilitySlot")]
        public async Task<ActionResult<bool>> AddAvailabilitySlot(AddAvailabilitySlotDto dto)
        {
            var Email = User.FindFirstValue(ClaimTypes.Email);

            if (string.IsNullOrWhiteSpace(Email))
                return Unauthorized();

            var result = await _serviceManger.DoctorService.AddAvailabilitySlotAsync(Email, dto);

            return Ok(result);
        }

        [HttpGet("GetAllAvailbleSlots")]
        public async Task<ActionResult<IEnumerable<AvailabilitySlotDto>>> GetAllAvailbleSlots()
        {
            var Email = User.FindFirstValue(ClaimTypes.Email);

            var result = await _serviceManger.DoctorService.GetMyAvailabilitySlotsAsync(Email!);
            return Ok(result);
        }


        [HttpPut("UpdateAvailabilitySlot")]
        public async Task<ActionResult<ServiceResponse>> UpdateAvailabilitySlot(int SlotId, UpdateAvailabilitySlotDto updateAvailabilitySlot)
        {
            var Email = User.FindFirstValue(ClaimTypes.Email);

            var result = await _serviceManger.DoctorService.UpdateAvailabilitySlotAsync(Email!, SlotId, updateAvailabilitySlot);
            return Ok(result);
        }

        [HttpDelete("DeleteAvailabilitySlot")]
        public async Task<ActionResult<ServiceResponse>> DeleteAvailabilitySlot(int SlotId)
        {
            var Email = User.FindFirstValue(ClaimTypes.Email);

            var result = await _serviceManger.DoctorService.DeleteAvailabilitySlotAsync(Email!, SlotId);
            return Ok(result);
        }
    }
}
