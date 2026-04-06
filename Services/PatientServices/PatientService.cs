using AutoMapper;
using DomainLayer.Contracts;
using DomainLayer.Exceptions;
using DomainLayer.IdentityModule;
using DomainLayer.Models;
using Microsoft.AspNetCore.Identity;
using Services.Specifications.PatientSpecifications;
using ServicesAbstraction.PatientAbstraction;
using Shared.DTos.PatientDTos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.PatientServices
{
    public class PatientService(IUnitOfWork _unitOfWork,IMapper _mapper) : IPatientService
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
    }
}
