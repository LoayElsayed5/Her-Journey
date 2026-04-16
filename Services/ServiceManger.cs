using ServicesAbstraction;
using ServicesAbstraction.AuthServices;
using ServicesAbstraction.DoctorAbstraction;
using ServicesAbstraction.IAdminAbstraction;
using ServicesAbstraction.ModelAbstraction;
using ServicesAbstraction.PatientAbstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class ServiceManger(Func<IAccountService> AcountFactory,
                               Func<IDoctorService> DoctorFactory,
                               Func<IAdminService> AdminFactory,
                               Func<IPatientService> PatientFactory,
                               Func<IModelPredictionService> ModelFactory) : IServiceManger
    {
        private IAccountService _accountService;
        private IDoctorService _doctorService;
        private IAdminService _adminService;
        private IPatientService _patientService;
        private IModelPredictionService _modelPredictionService;

        public IAccountService AccountService => _accountService ??= AcountFactory.Invoke();

        public IDoctorService DoctorService => _doctorService ??= DoctorFactory.Invoke();

        public IAdminService AdminService => _adminService ??= AdminFactory.Invoke();

        public IPatientService PatientService => _patientService ??= PatientFactory.Invoke();
        public IModelPredictionService ModelPredictionService => _modelPredictionService ??= ModelFactory.Invoke();
    }
}
