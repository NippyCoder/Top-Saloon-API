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
    public class SmsController : BaseResultHandlerController<SmsService>
    {
        public SmsController(SmsService _service) : base(_service)
        {

        }

        [HttpGet("GetSMS")]
        public async Task<IActionResult> GetSMS()
        {
            return await GetResponseHandler(async () => await service.GetSMS());
        }

        [HttpPost("EditSMS")]
        public async Task<IActionResult> EditSMS(SmsDTO model)
        {
            return await EditItemResponseHandler(async () => await service.EditSMS(model));
        }

    }
}
