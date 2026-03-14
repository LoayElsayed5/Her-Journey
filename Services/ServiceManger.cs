using ServicesAbstraction;
using ServicesAbstraction.AuthServices;
using ServicesAbstraction.DoctorAbstraction;
using ServicesAbstraction.IAdminAbstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class ServiceManger(Func<IAccountService> AcountFactory,
                               Func<IDoctorService> DoctorFactory,
                               Func<IAdminService> AdminFactory) : IServiceManger
    {
        private IAccountService _accountService;
        private IDoctorService _doctorService;
        private IAdminService _adminService;

        public IAccountService AccountService => _accountService ??= AcountFactory.Invoke();

        public IDoctorService DoctorService => _doctorService ??= DoctorFactory.Invoke();

        public IAdminService AdminService => _adminService ??= AdminFactory.Invoke();
    }
}
