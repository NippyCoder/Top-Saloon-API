using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TopSaloon.API.Controllers.Common;
using TopSaloon.DTOs.Models;
using TopSaloon.Entities.Models;
using TopSaloon.ServiceLayer;

namespace TopSaloon.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdministratorController : BaseResultHandlerController<AdministratorService>
    {
        public AdministratorController(AdministratorService _service) : base(_service)
        {

        }

        [HttpGet("getAdministrator")]
        public async Task<IActionResult> getAdminByid(int adminId)
        {
            return await GetResponseHandler(async () => await service.GetAdminByID(adminId));
        }
        [HttpPut("EditAdministrator")]
        public async Task<IActionResult> EditAdminById(AdministratorDTO admin)
        {
            return await EditItemResponseHandler(async () => await service.EditAdminById(admin));
        }

    }
}
