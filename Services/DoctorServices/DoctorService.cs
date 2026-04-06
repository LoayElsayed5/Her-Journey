using AutoMapper;
using DomainLayer.Contracts;
using DomainLayer.Exceptions;
using DomainLayer.Models;
using Services.Specifications.DoctorSpecifications;
using Services.Specifications.MedicalHistorySpecification;
using Services.Specifications.PatientSpecifications;
using ServicesAbstraction.DoctorAbstraction;
using Shared.DTos.AppointmentDTos;
using Shared.DTos.DashBoardDTos;
using Shared.DTos.DoctorDTos;
using Shared.DTos.MedicalHistoryDTos;
using Shared.ErrorModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static System.Reflection.Metadata.BlobBuilder;

namespace Services.DoctorServices
{
    public class DoctorService(IUnitOfWork _unitOfWork, IMapper _mapper) : IDoctorService
    {
        public async Task<bool> AddAvailabilitySlotAsync(string Email, AddAvailabilitySlotDto addAvailabilitySlot)
        {
            var DRepo = _unitOfWork.GetRepository<Doctor>();
            var SlotRepo = _unitOfWork.GetRepository<AvailabilitySlot>();

            var spec = new DoctorDetailsSpecification(Email);
            var doctor = await DRepo.GetByIdAsync(spec);

            if (doctor == null)
                throw new DoctorNotFoundException("Doctor not found.");

            if (addAvailabilitySlot.StartAt <= DateTime.Now)
                throw new BadRequestException(new List<string> { "Start time must be in the future." });

            if (addAvailabilitySlot.DurationInMinutes <= 0)
                throw new BadRequestException(new List<string> { "Duration must be greater than zero." });

            var newEnd = addAvailabilitySlot.StartAt.AddMinutes(addAvailabilitySlot.DurationInMinutes);

            var slotSpec = new DoctorAvailabilitySlotsSpecification(doctor.Id);
            var existingSlots = await SlotRepo.GetAllAsync(slotSpec);

            var hasOverlap = existingSlots.Any(slot =>
                addAvailabilitySlot.StartAt < slot.StartAt.Add(slot.Duration) &&
                newEnd > slot.StartAt);

            if (hasOverlap)
                throw new BadRequestException(new List<string>
                {
                    "This availability slot overlaps with another existing slot."
                });

            var availabilitySlot = new AvailabilitySlot
            {
                DoctorId = doctor.Id,
                StartAt = addAvailabilitySlot.StartAt,
                Duration = TimeSpan.FromMinutes(addAvailabilitySlot.DurationInMinutes),
                Type = (DomainLayer.Models.AppointmentType)addAvailabilitySlot.Type
            };
            await SlotRepo.AddAsync(availabilitySlot);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }


        public async Task<IEnumerable<AvailabilitySlotDto>> GetMyAvailabilitySlotsAsync(string Email)
        {
            var DRepo = _unitOfWork.GetRepository<Doctor>();
            var SlotRepo = _unitOfWork.GetRepository<AvailabilitySlot>();

            var spec = new DoctorDetailsSpecification(Email);
            var doctor = await DRepo.GetByIdAsync(spec);

            if (doctor == null)
                throw new DoctorNotFoundException("Doctor not found.");

            var slotSpec = new DoctorAvailabilitySlotsSpecification(doctor.Id);
            var Slots = await SlotRepo.GetAllAsync(slotSpec);


            return Slots.Select(slot => new AvailabilitySlotDto
            {
                Id = slot.Id,
                StartAt = slot.StartAt,
                DurationInMinutes = (int)slot.Duration.TotalMinutes,
                Type = (Shared.DTos.AppointmentDTos.AppointmentType)slot.Type,
                IsBooked = slot.Appointment is not null
            });
        }

        public async Task<ServiceResponse> UpdateAvailabilitySlotAsync(string Email, int SlotId, UpdateAvailabilitySlotDto dto)
        {
            var DRepo = _unitOfWork.GetRepository<Doctor>();
            var SlotRepo = _unitOfWork.GetRepository<AvailabilitySlot>();

            var doctor = await DRepo.GetByIdAsync(new DoctorDetailsSpecification(Email));
            if (doctor == null)
                throw new DoctorNotFoundException("Doctor not found.");
            var slotspec = new DoctorAvailabilitySlotByIdSpecification(SlotId, doctor.Id);

            var slot = await SlotRepo.GetByIdAsync(slotspec);
            if (slot == null)
                throw new BadRequestException(new List<string> { "Availability slot not found." });

            if (slot.Appointment is not null)
                throw new BadRequestException(new List<string> { "Booked slot cannot be updated." });

            if (dto.StartAt <= DateTime.Now)
                throw new BadRequestException(new List<string> { "Start time must be in the future." });

            if (dto.DurationInMinutes <= 0)
                throw new BadRequestException(new List<string> { "Duration must be greater than zero." });

            var newEnd = dto.StartAt.AddMinutes(dto.DurationInMinutes);

            var existingSlots = await SlotRepo.GetAllAsync(new DoctorAvailabilitySlotsSpecification(doctor.Id));

            var hasOverlap = existingSlots
                .Where(s => s.Id != SlotId)
                .Any(s =>
                    dto.StartAt < s.StartAt.Add(s.Duration) &&
                    newEnd > s.StartAt);

            if (hasOverlap)
                throw new BadRequestException(new List<string>
                {
                    "This availability slot overlaps with another existing slot."
                });

            slot.StartAt = dto.StartAt;
            slot.Duration = TimeSpan.FromMinutes(dto.DurationInMinutes);
            slot.Type = (DomainLayer.Models.AppointmentType)dto.Type;

            SlotRepo.Update(slot);
            return await _unitOfWork.SaveChangesAsync() > 0 ? new ServiceResponse { Status = true, Message = "Availability slot updated successfully." }
                                                            : new ServiceResponse { Status = true, Message = "No changes were made." };
        }

        public async Task<ServiceResponse> DeleteAvailabilitySlotAsync(string email, int slotId)
        {
            var DRepo = _unitOfWork.GetRepository<Doctor>();
            var SlotRepo = _unitOfWork.GetRepository<AvailabilitySlot>();

            var spec = new DoctorDetailsSpecification(email);
            var doctor = await DRepo.GetByIdAsync(spec);
            if (doctor == null)
                throw new DoctorNotFoundException("Doctor not found.");

            var slotspec = new DoctorAvailabilitySlotByIdSpecification(slotId, doctor.Id);
            var slot = await SlotRepo.GetByIdAsync(slotspec);
            if (slot == null)
                throw new BadRequestException(new List<string> { "Availability slot not found." });

            if (slot.Appointment is not null)
                throw new BadRequestException(new List<string> { "Booked slot cannot be deleted." });

            SlotRepo.Remove(slot);
            return await _unitOfWork.SaveChangesAsync() > 0 ? new ServiceResponse { Status = true, Message = "Availability slot Delete successfully." }
                                                            : new ServiceResponse { Status = false, Message = "Availability slot was not deleted." };
        }

        public async Task<IEnumerable<DoctorPatientDto>> GetAllPatientsAsync(string Email)
        {
            var DRepo = _unitOfWork.GetRepository<Doctor>();
            var Prepo = _unitOfWork.GetRepository<Patient>();

            var spec = new DoctorDetailsSpecification(Email);
            var doctor = await DRepo.GetByIdAsync(spec);
            if (doctor == null)
                throw new DoctorNotFoundException("Doctor not found.");


            var Pspec = new PatientsBelongToSpecifcDoctor(doctor.Id);

            var patients = await Prepo.GetAllAsync(Pspec);

            return _mapper.Map<IEnumerable<DoctorPatientDto>>(patients);
        }

        public async Task<MedicalHistoryDetailsDto> AddMedicalHistoryAsync(string Email, int PatientId, AddMedicalHistoryDto dto)
        {
            if (string.IsNullOrWhiteSpace(Email))
                throw new UnauthorizedException();


            if (dto is null)
                throw new BadRequestException(new List<string> { "Medical history data is required." });

            var DRepo = _unitOfWork.GetRepository<Doctor>();
            var PRepo = _unitOfWork.GetRepository<Patient>();
            var MRepo = _unitOfWork.GetRepository<MedicalHistory>();

            var DocotrSpec = new DoctorDetailsSpecification(Email);
            var doctor = await DRepo.GetByIdAsync(DocotrSpec);
            if (doctor is null)
                throw new DoctorNotFoundException("Doctor not found.");


            var PatientSpec = new PatientsBelongToSpecifcDoctor(PatientId, doctor.Id);
            var patient = await PRepo.GetByIdAsync(PatientSpec);

            if (patient is null)
                throw new BadRequestException(new List<string> { "Patient not found or does not belong to this doctor." });

            var medicalHistory = new MedicalHistory
            {
                PatientId = PatientId,
                CreatedByDoctorId = doctor.Id,
                Diagnosis = dto.Diagnosis,
                VitalSigns = dto.VitalSigns,
                Notes = dto.Notes,
                CreatedAt = DateTime.UtcNow,
                PreScriptions = dto.PreScriptions?
                    .Where(p => !string.IsNullOrWhiteSpace(p.MedicationName))
                    .Select(p => new PreScription
                    {
                        MedicationName = p.MedicationName,
                        Dosage = p.Dosage,
                        Duration = p.Duration,
                        Instructions = p.Instructions,
                        CreatedAt = DateTime.UtcNow
                    }).ToList() ?? new List<PreScription>()
            };
            await MRepo.AddAsync(medicalHistory);

            return await _unitOfWork.SaveChangesAsync() > 0 ? _mapper.Map<MedicalHistoryDetailsDto>(medicalHistory) 
                                                            : throw new BadRequestException(new List<string> { "Failed to add medical history." }); ;
        }

        public async Task<IEnumerable<MedicalHistoryDetailsDto>> GetPatientMedicalHistoriesAsync(string Email, int PatientId)
        {
            if (string.IsNullOrWhiteSpace(Email))
                throw new UnauthorizedException();


            var DRepo = _unitOfWork.GetRepository<Doctor>();
            var PRepo = _unitOfWork.GetRepository<Patient>();
            var MRepo = _unitOfWork.GetRepository<MedicalHistory>();

            var DocotrSpec = new DoctorDetailsSpecification(Email);
            var doctor = await DRepo.GetByIdAsync(DocotrSpec);
            if (doctor is null)
                throw new DoctorNotFoundException("Doctor not found.");


            var PatientSpec = new PatientsBelongToSpecifcDoctor(PatientId, doctor.Id);
            var patient = await PRepo.GetByIdAsync(PatientSpec);

            if (patient is null)
                throw new BadRequestException(new List<string> { "Patient not found or does not belong to this doctor." });


            var MedicalHistorySpec = new PatientMedicalHistoriesSpecification(PatientId);
            var medicalhistories =await MRepo.GetAllAsync(MedicalHistorySpec);

            return _mapper.Map<IEnumerable<MedicalHistoryDetailsDto>>(medicalhistories);
        }
    }
}
