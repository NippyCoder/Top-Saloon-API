using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TopSalon.DTOs.Models;
using TopSaloon.API.Controllers.Common;
using TopSaloon.DTOs.Models;
using TopSaloon.ServiceLayer;

namespace TopSaloon.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BarberController : BaseResultHandlerController<BarberService>
    {
        public BarberController(BarberService _service) : base(_service)
        {

        }
        [HttpGet("GetNumberOfAvailableBarbers")]
        public async Task<IActionResult> GetNumberOfAvailableBarbers()
        {
            return await AddItemResponseHandler(async () => await service.GetNumberOfAvailableBarbers());
        }

        [HttpPost("BarberTotalNumberOfHandledCustomer/{id}")]
        public async Task<IActionResult> BarberTotalNumberOfHandledCustomer(int BarberId)
        {
            return await AddItemResponseHandler(async () => await service.BarberTotalNumberOfHandledCustomer(BarberId));
        }

        [HttpGet("GetBarberDetailsReports/{id}")]
        public async Task<IActionResult> GetBarberDetailsReports(int BarberId)
        {
            return await AddItemResponseHandler(async () => await service.GetBarberDetailsReports(BarberId));
        }


    }
}
