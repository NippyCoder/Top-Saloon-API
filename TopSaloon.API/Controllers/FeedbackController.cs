using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TopSaloon.API.Controllers.Common;
using TopSaloon.DTOs.Models;
using TopSaloon.ServiceLayer;

namespace TopSaloon.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeedbackController : BaseResultHandlerController<FeedbackService>
    {
        public FeedbackController(FeedbackService _service) : base(_service)
        {

        }

        [HttpPost("AddServiceFeedbackQuestion")]
        public async Task<IActionResult> AddServiceFeedbackQuestion(AddServiceFeedbackQuestionDTO model)
        {
            return await AddItemResponseHandler(async () => await service.AddServiceFeedbackQuestion(model));
        }

        [HttpPost("RemoveServiceFeedbackQuestion")]
        public async Task<IActionResult> RemoveServiceFeedbackQuestion(string questionId)
        {
            return await AddItemResponseHandler(async () => await service.RemoveServiceFeedbackQuestion(questionId));
        }

        [HttpPost("EditServiceFeedbackQuestion")]
        public async Task<IActionResult> EditServiceFeedbackQuestion(EditServiceFeedbackQuestionDTO model)
        {
            return await EditItemResponseHandler(async () => await service.EditServiceFeedbackQuestion(model));
        }

        [HttpGet("GetAllOrderFeedbackQuestions")]
        public async Task<IActionResult> GetAllOrderFeedbackQuestions()
        {
            return await GetResponseHandler(async () => await service.GetAllOrderFeedbackQuestions());
        }
         [HttpGet("GetOrderFeedbackQuestionsById")]
        public async Task<IActionResult> GetOrderFeedbackQuestionsById(int Id)
        {
            return await GetResponseHandler(async () => await service.GetOrderFeedbackQuestionsById(Id));
        }

        [HttpGet("GetFeedbackById")]
        public async Task<IActionResult> GetFeedbackById(string id)
        {
            return await GetResponseHandler(async () => await service.GetOrderFeedbackById(id));
        }
        [HttpGet("GetAllOrderFeedbacks")]
        public async Task<IActionResult> GetAllOrderFeedback()
        {
            return await GetResponseHandler(async () => await service.GetAllOrderFeedback());
        }

    }
}
