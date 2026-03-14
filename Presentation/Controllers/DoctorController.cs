using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServicesAbstraction.DoctorAbstraction;
using Shared.DTos.DoctorDTos;

namespace Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorController(IDoctorService _doctorService) : ControllerBase
    {
        



    }
}
