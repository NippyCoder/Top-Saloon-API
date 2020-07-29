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
    public class ServiceController : BaseResultHandlerController<ServiceService>
    {
        public ServiceController(ServiceService _service) : base(_service)
        {

        }

        [HttpGet("getAllServices")]
        public async Task<IActionResult> GetAllServices()
        {
            return await GetResponseHandler(async () => await service.GetAllServices());
        }

    }
}
