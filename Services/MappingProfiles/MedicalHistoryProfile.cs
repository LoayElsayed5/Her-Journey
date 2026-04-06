using AutoMapper;
using DomainLayer.Models;
using Shared.DTos.MedicalHistoryDTos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.MappingProfiles
{
    public class MedicalHistoryProfile : Profile
    {
        public MedicalHistoryProfile()
        {
            CreateMap<PreScription, PreScriptionDto>();

            CreateMap<MedicalHistory, MedicalHistoryDetailsDto>();
        }
    }
}
