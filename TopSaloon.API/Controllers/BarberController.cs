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
    public class BarberController : BaseResultHandlerController<BarberService>
    {
        public BarberController(BarberService _service) : base(_service)
        {

        }

        [HttpGet("GetAllBarbers")]
        public async Task<IActionResult> GetAllBarbers()
        {
            return await GetResponseHandler(async () => await service.GetAllBarbers());
        }
        [HttpGet("GetNumberOfAvailableBarbers")]
        public async Task<IActionResult> GetNumberOfAvailableBarbers()
        {
            return await GetResponseHandler(async () => await service.GetNumberOfAvailableBarbers());
        }
        
        [HttpGet("GetAllAvailableBarbers")]
        public async Task<IActionResult> GetAvailableBarbers()
        {
            return await GetResponseHandler(async () => await service.GetAvailableBarbers());
        }

        [HttpPost("BarberTotalNumberOfHandledCustomer/{id}")]
        public async Task<IActionResult> BarberTotalNumberOfHandledCustomer(int id)
        {
            return await AddItemResponseHandler(async () => await service.BarberTotalNumberOfHandledCustomer(id));
        }

        [HttpPost("CreateBarber")]
        public async Task<IActionResult> CreateBarber(CreateBarberDTO model)
        {
            return await AddItemResponseHandler(async () => await service.CreateBarber(model));
        }

        [HttpGet("GetBarberDetailsReports/{id}")]
        public async Task<IActionResult> GetBarberDetailsReports(int id)
        {
            return await AddItemResponseHandler(async () => await service.GetBarberDetailsReports(id));
        }

        [HttpGet("DeleteBarberById/{id}")]
        public async Task<IActionResult> DeleteBarberById(int id)
        {
            return await AddItemResponseHandler(async () => await service.DeleteBarberById(id));
        }

    }
}

