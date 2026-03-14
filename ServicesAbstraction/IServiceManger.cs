using ServicesAbstraction.AuthServices;
using ServicesAbstraction.DoctorAbstraction;
using ServicesAbstraction.IAdminAbstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesAbstraction
{
    public interface IServiceManger
    {
        IAccountService AccountService { get; }
        IDoctorService DoctorService { get; }
        IAdminService AdminService { get; }
    }
}
