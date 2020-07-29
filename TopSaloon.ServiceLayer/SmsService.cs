
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TopSaloon.Core;
using TopSaloon.DTOs;
using TopSaloon.DTOs.Enums;
using TopSaloon.DTOs.Models;
using TopSaloon.Entities.Models;
using TopSaloon.ServiceLayer;

namespace TopSaloon.ServiceLayer
{
    public class SmsService
    {
        private readonly UnitOfWork unitOfWork;
        private readonly IConfiguration config;

        public SmsService(UnitOfWork unitOfWork,IConfiguration config)
        {
            this.unitOfWork = unitOfWork;
            this.config = config;
        }


        public async Task<ApiResponse<String>> getSmsById(int smsId)
        {
            ApiResponse<string> result = new ApiResponse<string>();
            SMS smsValue = await unitOfWork.SMSManager.GetByIdAsync(smsId);
            try
            {
                if (result != null)
                {
                    result.Data = smsValue.Body;
                    result.Succeeded = true;

                    return result;

                }
                else
                {
                    result.Succeeded = false;
                    result.Errors.Add("Invalid input value");
                    return result;
                }


            }
            catch (Exception ex)
            {
                result.Succeeded = false;
                result.Errors.Add(ex.Message);
                return result;
            }

        }
        public async Task<ApiResponse<bool>> editSmSbyId(SMSDTO model)
        {
            ApiResponse<bool> result = new ApiResponse<bool>();


            try
            {
                SMS value = await unitOfWork.SMSManager.GetByIdAsync(model.Id);

                value.Body = model.Body;
                var result1 = await unitOfWork.SaveChangesAsync();

                if (result1 == true)
                {
                    result.Data = true;
                    result.Succeeded = true;
                    return result;
                }


                else
                {
                    result.Succeeded = false;
                    result.Errors.Add("Invalid input value");
                    return result;
                }


            }
            catch (Exception ex)
            {
                result.Succeeded = false;
                result.Errors.Add(ex.Message);
                return result;
            }

        }
    }
}


