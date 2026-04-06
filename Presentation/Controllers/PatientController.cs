using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServicesAbstraction;
using Shared.DTos.PatientDTos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.Controllers
{
    [Authorize(Roles = "Patient")]
    public class PatientController(IServiceManger _serviceManger) : ApiBaseController
    {
        [HttpPut("CompleteMedicalProfile")]
        public async Task<ActionResult<bool>> CompleteProfile(CompleteMedicalProfileDto completeMedicalProfileDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();
            var result = await _serviceManger.PatientService.CompleteProfileAsync(userId, completeMedicalProfileDto);
            return Ok(result);
        }
    }
}
