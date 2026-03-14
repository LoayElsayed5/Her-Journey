using DomainLayer.Contracts;
using DomainLayer.Models;
using Services.Specifications.DoctorSpecifications;
using ServicesAbstraction.DoctorAbstraction;
using Shared.DTos.DoctorDTos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DoctorServices
{
    public class DoctorService(IUnitOfWork _unitOfWork) : IDoctorService
    {
        
    }
}
