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
    public class QuestionFeedbackController : BaseResultHandlerController<QuestionFeedbackService>
    {
        public QuestionFeedbackController(QuestionFeedbackService _service) : base(_service)
        {

        }
        [HttpGet("GetAllOrderFeedbacks")]
        public async Task<IActionResult> GetAllOrderFeedback()
        {
            return await GetResponseHandler(async () => await service.GetAllOrderFeedback());
        }


    }
}
