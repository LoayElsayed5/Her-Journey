using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Services.AdminServices;
using Services.AuthServices;
using Services.DoctorServices;
using Services.PatientServices;
using ServicesAbstraction;
using ServicesAbstraction.AuthServices;
using ServicesAbstraction.DoctorAbstraction;
using ServicesAbstraction.IAdminAbstraction;
using ServicesAbstraction.PatientAbstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public static class ApplicationServicesRegisteration
    {
        public static IServiceCollection ApplicationServices(this IServiceCollection Services)
        {

            Services.AddScoped<IServiceManger, ServiceManger>();


            Services.AddScoped<IAccountService, AccountService>();
            Services.AddScoped<Func<IAccountService>>(Provider =>
            () => Provider.GetRequiredService<IAccountService>());
            
            
            Services.AddScoped<IDoctorService, DoctorService>();
            Services.AddScoped<Func<IDoctorService>>(Provider =>
            () => Provider.GetRequiredService<IDoctorService>());
            
            Services.AddScoped<IAdminService, AdminService>();
            Services.AddScoped<Func<IAdminService>>(Provider =>
            () => Provider.GetRequiredService<IAdminService>());
            
            Services.AddScoped<IPatientService, PatientService>();
            Services.AddScoped<Func<IPatientService>>(Provider =>
            () => Provider.GetRequiredService<IPatientService>());


            Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            return Services;
        }
    }
}
