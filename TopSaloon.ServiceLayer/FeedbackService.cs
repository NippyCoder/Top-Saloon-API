using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopSaloon.Core;
using TopSaloon.DTOs;
using TopSaloon.DTOs.Enums;
using TopSaloon.DTOs.Models;
using TopSaloon.Entities.Models;

namespace TopSaloon.ServiceLayer
{
    public class FeedbackService
    {
        private readonly UnitOfWork unitOfWork;
        private readonly IConfiguration config;

        public FeedbackService(UnitOfWork unitOfWork, IConfiguration config)
        {
            this.unitOfWork = unitOfWork;
            this.config = config;
        }

        public async Task<ApiResponse<ServiceFeedBackQuestion>> AddServiceFeedbackQuestion(AddServiceFeedbackQuestionDTO questionToAdd)
        {

            ApiResponse<ServiceFeedBackQuestion> result = new ApiResponse<ServiceFeedBackQuestion>();
            try
            {
                ServiceFeedBackQuestion serviceFeedBackQuestionToAdd = new ServiceFeedBackQuestion();
                serviceFeedBackQuestionToAdd.Question = questionToAdd.Question;
                serviceFeedBackQuestionToAdd.ServiceId = questionToAdd.ServiceId;
                serviceFeedBackQuestionToAdd = await unitOfWork.ServiceFeedBackQuestionsManager.CreateAsync(serviceFeedBackQuestionToAdd);

                var res = await unitOfWork.SaveChangesAsync();

                if (res == true)
                {
                    result.Data = serviceFeedBackQuestionToAdd;
                    result.Succeeded = true;
                    return result;
                }
                else
                {
                    result.Succeeded = false;
                    result.Errors.Add("Error while adding service feedback question");
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

        public async Task<ApiResponse<bool>> RemoveServiceFeedbackQuestion(string questionId)
        {

            ApiResponse<bool> result = new ApiResponse<bool>();
            try
            {

                var serviceFeedbackQuestionToRemove = await unitOfWork.ServiceFeedBackQuestionsManager.GetByIdAsync(Int32.Parse(questionId));

                var res = await unitOfWork.ServiceFeedBackQuestionsManager.RemoveAsync(serviceFeedbackQuestionToRemove);

                if (res == true)
                {
                    var res2 = await unitOfWork.SaveChangesAsync();
                    if (res2 == true)
                    {
                        result.Data = true;
                        result.Succeeded = true;
                        return result;
                    }
                    result.Succeeded = false;
                    result.Errors.Add("Error while updating database !");
                    return result;

                }
                else
                {
                    result.Succeeded = false;
                    result.Errors.Add("Error while removing service feedback question !");
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

        public async Task<ApiResponse<bool>> EditServiceFeedbackQuestion(EditServiceFeedbackQuestionDTO questionToEdit)
        {
            ApiResponse<bool> result = new ApiResponse<bool>();
            try
            {
                var serviceFeedbackQuestionToEdit = await unitOfWork.ServiceFeedBackQuestionsManager.GetByIdAsync(questionToEdit.Id);

                if (serviceFeedbackQuestionToEdit != null)
                {
                    serviceFeedbackQuestionToEdit.Question = questionToEdit.Question;
                    var res2 = await unitOfWork.SaveChangesAsync();
                    if (res2 == true)
                    {
                        result.Data = true;
                        result.Succeeded = true;
                        return result;
                    }
                    result.Succeeded = false;
                    result.Errors.Add("Error while updating database !");
                    return result;
                }
                else
                {
                    result.Succeeded = false;
                    result.Errors.Add("Unable to find service feedback question !");
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

        public async Task<ApiResponse<List<ServiceFeedBackQuestion>>> GetAllServiceFeedbackQuestionsByServiceId(string serviceId)
        {
            ApiResponse<List<ServiceFeedBackQuestion>> result = new ApiResponse<List<ServiceFeedBackQuestion>>();
            try
            {
                List<ServiceFeedBackQuestion> serviceFeedbackQuestionsListToSend = await unitOfWork.ServiceFeedBackQuestionsManager.GetServiceFeedBackQuestionsByServiceId(serviceId);

                if (serviceFeedbackQuestionsListToSend != null)
                {
                    result.Data = serviceFeedbackQuestionsListToSend;
                    result.Succeeded = true;
                    return result;
                }
                else
                {
                    result.Succeeded = false;
                    result.Errors.Add("Unable to find service feedback question !");
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

        public async Task<ApiResponse<DataItemsResult<OrderFeedback>>> GetOrderFeedbackById(string id)
        {
            ApiResponse<DataItemsResult<OrderFeedback>> result = new ApiResponse<DataItemsResult<OrderFeedback>>();

            result.Data = new DataItemsResult<OrderFeedback>();

            int OrderFeedbackId = Int32.Parse(id);

            try
            {
                IQueryable<OrderFeedback> res = await unitOfWork.OrderFeedBacksManager.GetAsync(c => c.Id == OrderFeedbackId , 0, 0,null,"OrderFeedbackQuestions");

                result.Data.Items = res.ToList();
                result.Succeeded = true;
                return result;

                //else
                //{
                //    result.Succeeded = false;
                //    result.Errors.Add("Unable to find order feedback !");
                //    return result;
                //}
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
