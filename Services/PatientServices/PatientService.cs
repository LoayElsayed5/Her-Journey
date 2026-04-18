using AutoMapper;
using DomainLayer.Contracts;
using DomainLayer.Exceptions;
using DomainLayer.IdentityModule;
using DomainLayer.Models;
using Microsoft.AspNetCore.Identity;
using Services.Specifications.DoctorSpecifications;
using Services.Specifications.MedicalTestSpecifications;
using Services.Specifications.PatientSpecifications;
using ServicesAbstraction.Common;
using ServicesAbstraction.PatientAbstraction;
using Shared.DTos.AppointmentDTos;
using Shared.DTos.MedicalTestDTos;
using Shared.DTos.PatientDTos;
using Shared.ErrorModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.PatientServices
{
    public class PatientService(IUnitOfWork _unitOfWork, IMapper _mapper, IFileStorageService _fileStorageService) : IPatientService
    {
        public async Task<bool> CompleteProfileAsync(string userId, CompleteMedicalProfileDto profileDto)
        {
            var psepc = new PatientByIdSpecification(userId);
            var prepo = _unitOfWork.GetRepository<Patient>();
            var patient = await prepo.GetByIdAsync(psepc);

            if (patient == null) throw new PatientNotFoundException(userId);

            _mapper.Map(profileDto, patient.MedicalInfo);
            prepo.Update(patient);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<AvailabilitySlotDto>> GetAllSlotsAsync(string Email)
        {
            if (string.IsNullOrWhiteSpace(Email))
                throw new UnauthorizedException();

            var PRepo = _unitOfWork.GetRepository<Patient>();
            var DRepo = _unitOfWork.GetRepository<Doctor>();
            var SlotRepo = _unitOfWork.GetRepository<AvailabilitySlot>();
            var spec = new PatientDetailsSpecification(Email);
            var patient = await PRepo.GetByIdAsync(spec);

            var doctor = await DRepo.GetByIdAsync(patient.DoctorID);
            if (doctor == null)
                throw new DoctorNotFoundException("Doctor not found.");

            var slotSpec = new DoctorAvailabilitySlotsSpecification(doctor.Id);
            var Slots = await SlotRepo.GetAllAsync(slotSpec);
            return _mapper.Map<IEnumerable<AvailabilitySlotDto>>(Slots);

        }


        public async Task<MedicalTestDto> UploadMedicalTestAsync(string userId, UploadMedicalTestDto dto)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new UnauthorizedException();

            if (dto is null || dto.File is null || dto.File.Length == 0)
                throw new BadRequestException("Medical test file is required.");

            var allowedExtensions = new[] { ".pdf", ".jpg", ".jpeg", ".png" };
            var extension = Path.GetExtension(dto.File.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
                throw new BadRequestException("Only pdf, jpg, jpeg, and png files are allowed.");

            const long maxFileSize = 10 * 1024 * 1024;
            if (dto.File.Length > maxFileSize)
                throw new BadRequestException("File size must not exceed 10 MB.");

            var patientRepo = _unitOfWork.GetRepository<Patient>();
            var medicalTestRepo = _unitOfWork.GetRepository<MedicalTest>();

            var patient = await patientRepo.GetByIdAsync(new PatientByIdSpecification(userId));
            if (patient == null)
                throw new PatientNotFoundException(userId);

            var objectName = BuildMedicalTestObjectName(patient.Id, dto.File.FileName);

            string? uploadedObjectName = null;

            try
            {
                uploadedObjectName = await _fileStorageService.UploadFileAsync(dto.File, objectName);

                var medicalTest = new MedicalTest
                {
                    FileName = dto.File.FileName,
                    FilePath = uploadedObjectName,
                    UploadedAt = DateTime.Now,
                    PatientId = patient.Id
                };

                await medicalTestRepo.AddAsync(medicalTest);

                return await _unitOfWork.SaveChangesAsync() > 0
                    ? _mapper.Map<MedicalTestDto>(medicalTest)
                    : throw new BadRequestException("Failed to save medical test.");
            }
            catch
            {
                if (!string.IsNullOrWhiteSpace(uploadedObjectName))
                {
                    try
                    {
                        await _fileStorageService.DeleteFileAsync(uploadedObjectName);
                    }
                    catch
                    {
                    }
                }

                throw;
            }
        }

        public async Task<IEnumerable<MedicalTestDto>> GetMyMedicalTestsAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new UnauthorizedException();

            var patientRepo = _unitOfWork.GetRepository<Patient>();
            var medicalTestRepo = _unitOfWork.GetRepository<MedicalTest>();

            var patient = await patientRepo.GetByIdAsync(new PatientByIdSpecification(userId));
            if (patient == null)
                throw new PatientNotFoundException(userId);

            var tests = await medicalTestRepo.GetAllAsync(new PatientMedicalTestsSpecification(patient.Id));

            return _mapper.Map<IEnumerable<MedicalTestDto>>(tests.OrderByDescending(m => m.UploadedAt));
        }

        public async Task<ServiceResponse> DeleteMedicalTestAsync(string userId, int medicalTestId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new UnauthorizedException();

            var patientRepo = _unitOfWork.GetRepository<Patient>();
            var medicalTestRepo = _unitOfWork.GetRepository<MedicalTest>();

            var patient = await patientRepo.GetByIdAsync(new PatientByIdSpecification(userId));
            if (patient == null)
                throw new PatientNotFoundException(userId);

            var medicalTest = await medicalTestRepo.GetByIdAsync(new PatientMedicalTestsSpecification(patient.Id, medicalTestId));
            if (medicalTest == null)
                throw new BadRequestException("Medical test not found.");

            if (!string.IsNullOrWhiteSpace(medicalTest.FilePath))
            {
                await _fileStorageService.DeleteFileAsync(medicalTest.FilePath);
            }

            medicalTestRepo.Remove(medicalTest);

            return await _unitOfWork.SaveChangesAsync() > 0
                ? new ServiceResponse { Status = true, Message = "Medical test deleted successfully." }
                : new ServiceResponse { Status = false, Message = "Medical test was not deleted." };
        }


        private static string BuildMedicalTestObjectName(int patientId, string fileName)
        {
            var extension = Path.GetExtension(fileName);
            var originalName = Path.GetFileNameWithoutExtension(fileName);

            var safeName = string.Concat(originalName
                .Where(c => char.IsLetterOrDigit(c) || c == '-' || c == '_'))
                .Trim();

            if (string.IsNullOrWhiteSpace(safeName))
                safeName = "medical-test";

            var now = DateTime.Now;
            var uniqueFileName = $"{Guid.NewGuid()}-{safeName}{extension}";

            return $"medical-tests/patients/{patientId}/{now:yyyy}/{now:MM}/{uniqueFileName}";
        }
    }
}
