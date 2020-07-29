

using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TopSalon.DTOs.Models;
using TopSaloon.Core;
using TopSaloon.DTOs;
using TopSaloon.DTOs.Enums;
using TopSaloon.DTOs.Models;
using TopSaloon.Entities.Models;
namespace TopSaloon.ServiceLayer
{
    public class ServiceService
    {
        private readonly UnitOfWork unitOfWork;
        private readonly IConfiguration config;
        private readonly IMapper mapper;

        public ServiceService(UnitOfWork unitOfWork, IConfiguration config, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.config = config;
            this.mapper = mapper; 
        }

        public async Task<ApiResponse<List<ServiceDTO>>> GetAllServices()
        {
            ApiResponse<List<ServiceDTO>> result = new ApiResponse<List<ServiceDTO>>();

            try
            {
               var services  = await unitOfWork.ServicesManager.GetAsync(b => b.Id !=null, 0, 0, null, includeProperties: "FeedBackQuestions");

                if (services != null)
                {
                    result.Data = mapper.Map<List<ServiceDTO>>(services.ToList());
                    
                    result.Succeeded = true;
                    return result;
                }
                else
                {
                    result.Errors.Add("Unable to get list !");
                    result.Succeeded = false;
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
