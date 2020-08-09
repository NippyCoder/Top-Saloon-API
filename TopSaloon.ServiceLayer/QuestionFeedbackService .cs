
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TopSaloon.DTOs.Models;
using TopSaloon.Core;
using TopSaloon.DTOs;
using TopSaloon.DTOs.Enums;
 using TopSaloon.Entities.Models;

namespace TopSaloon.ServiceLayer
{
    public class QuestionFeedbackService
    {
        private readonly UnitOfWork unitOfWork;
        private readonly IConfiguration config;

        public QuestionFeedbackService(UnitOfWork unitOfWork, IConfiguration config)
        {
            this.unitOfWork = unitOfWork;
            this.config = config;
        }
         public async Task<ApiResponse<List<OrderFeedback>>> GetAllOrderFeedback()
        {
            ApiResponse<List<OrderFeedback>> result = new ApiResponse<List<OrderFeedback>>();
            try
            {
                List<OrderFeedback> orderFeedbackQuestionsListToSend = await unitOfWork.OrderFeedBacksManager.GetFeedbackBySubmittedStatus();

                if (orderFeedbackQuestionsListToSend != null)
                {
                    result.Data = orderFeedbackQuestionsListToSend;
                    result.Succeeded = true;
                    return result;
                }
                else
                {
                    result.Succeeded = false;
                    result.Errors.Add("Unable to find any order feedbacks !");
                    return result;
                }
                //End of try . 
            }
            catch (Exception ex)
            {
                result.Succeeded = false;
                result.Errors.Add(ex.Message);
                result.ErrorType = ErrorType.SystemError;
                return result;
            }
        }

    }
}

