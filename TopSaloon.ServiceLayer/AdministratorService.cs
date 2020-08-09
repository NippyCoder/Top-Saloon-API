using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;
using TopSaloon.Core;
using TopSaloon.DTOs;
using TopSaloon.DTOs.Models;
using TopSaloon.Entities.Models;
namespace TopSaloon.ServiceLayer
{
    public class AdministratorService
    {
        private readonly UnitOfWork unitOfWork;
        private readonly IConfiguration config;

        public AdministratorService(UnitOfWork unitOfWork, IConfiguration config)
        {
            this.unitOfWork = unitOfWork;
            this.config = config;
        }

        public async Task<ApiResponse<AdministratorDTO>> GetAdminByID(int adminId)
        {
            ApiResponse<AdministratorDTO> result = new ApiResponse<AdministratorDTO>();
            try
            {
                Administrator adminValue = await unitOfWork.AdministratorsManager.GetByIdAsync(adminId);

                if (adminValue != null)
                {

                    var userdata = await unitOfWork.UserManager.FindByIdAsync(adminValue.UserId);

                    if (userdata != null)
                    {
                        AdministratorDTO returnobject = new AdministratorDTO();
                        returnobject.Id = adminValue.Id;
                        returnobject.UserId = adminValue.UserId;
                        AdminCreationModel adminstratorModel = new AdminCreationModel();

                        adminstratorModel.FirstName = userdata.FirstName;
                        adminstratorModel.LastName = userdata.LastName;
                        adminstratorModel.Email = userdata.Email;
                        adminstratorModel.PhoneNumber = userdata.PhoneNumber;

                        returnobject.adminModel = adminstratorModel;

                        result.Data = returnobject;
                        result.Succeeded = true;
                        return result;
                    }
                    else
                    {
                        result.Succeeded = false;
                        result.Errors.Add("User not found");
                        return result;
                    }
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

        public async Task<ApiResponse<bool>> EditAdminById(AdministratorDTO adminDto)
        {
            ApiResponse<bool> result = new ApiResponse<bool>();
            try
            {
                Administrator adminValue = await unitOfWork.AdministratorsManager.GetByIdAsync(adminDto.Id);
                if (adminValue != null)
                {
                    var userdata = await unitOfWork.UserManager.FindByIdAsync(adminValue.UserId);

                    AdminCreationModel adminCreationModel = new AdminCreationModel();


                    userdata.Email = adminDto.adminModel.Email;
                    userdata.FirstName = adminDto.adminModel.FirstName;
                    userdata.LastName = adminDto.adminModel.LastName;
                    userdata.PhoneNumber = adminDto.adminModel.PhoneNumber;

                    var result1 = await unitOfWork.UserManager.UpdateAsync(userdata);

                    if (result1.Succeeded)
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
                else
                {
                    result.Succeeded = false;
                    result.Errors.Add("opsss");
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
