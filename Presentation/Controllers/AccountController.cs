using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ServicesAbstraction.AuthServices;
using Shared.DTos.IdentityModuleDTo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController(IAccountService _accountService) : ControllerBase
    {
        [HttpPost("RegisterDoctor")]
        public async Task<ActionResult<UserDto>> RegisterDoctor(RegisterDoctorDto dto)
        {
            var result = await _accountService.RegisterDoctorAsync(dto);
            return Ok(result);
        }

        [HttpPost("RegisterPatient")]
        public async Task<ActionResult<UserDto>> RegisterPatient(RegisterPatientDto dto)
        {
            var result = await _accountService.RegisterPatientAsync(dto);

            return Ok(result);
        }


 [HttpGet]
        public IActionResult Get()
        {
            return Ok(new {
                message = "Backend Dockerized and working!",
                time = DateTime.Now
            });
        }

        [HttpPost("ConfirmEmail")]
        public async Task<ActionResult<IdentityResult>> ConfirmEmail(ConfirmEmailDto dto)
        {
            var result = await _accountService.ConfirmEmailAsync(dto);

            if (result.Succeeded)
                return Ok(new { message = "Account confirmed and password set successfully! You can login now." });

            return BadRequest(result.Errors);
        }


        [HttpPost("Login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto dto)
        {
            var result =await _accountService.LoginAsync(dto);
            return Ok(result);
        }
    }
}
