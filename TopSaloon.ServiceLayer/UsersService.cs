
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
    public class UsersService
    {
        private readonly UnitOfWork unitOfWork;
        private readonly IConfiguration config;

        public UsersService(UnitOfWork unitOfWork, IConfiguration config)
        {
            this.unitOfWork = unitOfWork;
            this.config = config;
        }
        public async Task<ApiResponse<bool>> CreateRole(string roleName)
        {
            ApiResponse<bool> result = new ApiResponse<bool>();
            try
            {
                bool x = await unitOfWork.RoleManager.RoleExistsAsync(roleName);
                if (!x)
                {
                    var role = new IdentityRole();
                    role.Name = roleName;
                    
                    var res = await unitOfWork.RoleManager.CreateAsync(role);

                    if (res.Succeeded)
                    {
                        result.Data = true;
                        result.Succeeded = true;
                        return result;
                    }
                    result.Succeeded = false;
                    foreach (var error in res.Errors)
                    {
                        result.Errors.Add(error.Description);
                    }
                    return result;
                }
                result.Succeeded = false;
                result.Errors.Add("Unable to create role !");
                return result;
            }
            catch (Exception ex)
            {
                result.Succeeded = false;
                result.Errors.Add(ex.Message);
                return result;
            }

        }
        public async Task<ApiResponse<bool>> AssignRole(string userId, string roleName)
        {
            ApiResponse<bool> result = new ApiResponse<bool>();

            try
            {
                var user = await unitOfWork.UserManager.FindByIdAsync(userId);
                if (user != null)
                {
                    var roleresult = await unitOfWork.UserManager.AddToRoleAsync(user, roleName);

                    if (roleresult.Succeeded)
                    {
                        result.Data = true;
                        result.Succeeded = true;
                        return result;
                    }
                    result.Succeeded = false;
                    foreach (var error in roleresult.Errors)
                    {
                        result.Errors.Add(error.Description);
                    }
                    return result;
                }
                result.Succeeded = false;
                result.Errors.Add("Unable to find user !");
                return result;
            }
            catch (Exception ex)
            {
                result.Succeeded = false;
                result.Errors.Add(ex.Message);
                return result;
            }
        }

        public async Task<ApiResponse<bool>> CreateAdmin(AdminCreationModel model)
        {
            ApiResponse<bool> result = new ApiResponse<bool>();
            try
            {
                ApplicationUser user = new ApplicationUser
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    UserName = model.FirstName + model.LastName
                };

                var res = await unitOfWork.UserManager.CreateAsync(user, model.Password);


                await unitOfWork.SaveChangesAsync();

                if (res.Succeeded)
                {

                    var roleresult = await unitOfWork.UserManager.AddToRoleAsync(user, "Administrator");

                    Administrator adminToCreate = new Administrator();
                    adminToCreate.UserId = user.Id;
                    adminToCreate.ShopId = 1;
                    var admin = await unitOfWork.AdministratorsManager.CreateAsync(adminToCreate);

                    var res2 = await unitOfWork.SaveChangesAsync();

                    if (res2 == true)
                    {
                        result.Data = true;
                        result.Succeeded = true;
                        return result;
                    }
                    else
                    {
                        result.Succeeded = false;
                        result.Errors.Add("Failed To Create Adminstrator");
                        result.ErrorType = ErrorType.LogicalError;
                        return result;
                    }
                }
                result.Succeeded = false;
                foreach (var error in res.Errors)
                {
                    result.Errors.Add(error.Description);
                }
                result.ErrorType = ErrorType.LogicalError;
                return result;
            }
            catch (Exception ex)
            {
                result.Succeeded = false;
                result.Errors.Add(ex.Message);
                result.ErrorType = ErrorType.SystemError;
                return result;
            }
        }
        public async Task<ApiResponse<bool>> DeleteAdmin(string AdminId)
        {
            ApiResponse<bool> result = new ApiResponse<bool>();
            try
            {
                int adminId = Int32.Parse(AdminId);
                var admin = await unitOfWork.AdministratorsManager.GetByIdAsync(adminId);
                var user = await unitOfWork.UserManager.FindByIdAsync(admin.UserId);
                var rolesForUser = await unitOfWork.UserManager.GetRolesAsync(user);

                foreach (var item in rolesForUser.ToList())
                {
                    // item should be the name of the role
                    var res = await unitOfWork.UserManager.RemoveFromRoleAsync(user, "Administrator");
                }

                var userResult = await unitOfWork.UserManager.DeleteAsync(user);
                var adminResult = await unitOfWork.AdministratorsManager.RemoveAsync(admin);

                if (adminResult == true && userResult.Succeeded == true)
                {
                    var res2 = await unitOfWork.SaveChangesAsync();
                    if (res2)
                    {
                        result.Data = true;
                        result.Succeeded = true;
                        return result;
                    }
                    else
                    {
                        result.Succeeded = false;
                        result.Errors.Add("Error Deleting Administrator !");
                        return result;
                    }
                }
                result.Succeeded = false;
                foreach (var error in userResult.Errors)
                {
                    result.Errors.Add(error.Description);
                }
                result.ErrorType = ErrorType.LogicalError;
                return result;
            }
            catch (Exception ex)
            {
                result.Succeeded = false;
                result.Errors.Add(ex.Message);
                result.ErrorType = ErrorType.SystemError;
                return result;
            }
        }
        public async Task<ApiResponse<AdministratorDTO>> LoginAsync(AdminLoginModel model)
        {
            ApiResponse<AdministratorDTO> result = new ApiResponse<AdministratorDTO>();
            try
            {
                var user = await unitOfWork.UserManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    bool res = await unitOfWork.UserManager.CheckPasswordAsync(user, model.Password);
                    if (res)
                    {
                        var admin = unitOfWork.AdministratorsManager.GetAdminByUserId(user.Id);
                        result.Data = admin;
                        result.Succeeded = true;
                        return result;
                    }
                    else
                    {
                        result.Succeeded = false;
                        result.Errors.Add("Invalid login attempt.");
                        result.ErrorType = ErrorType.LogicalError;
                        return result;
                    }

                }
                else
                {
                    result.Succeeded = false;
                    result.Errors.Add("Invalid login attempt.");
                    result.ErrorType = ErrorType.LogicalError;
                    return result;
                }

            }
            catch (Exception ex)
            {
                result.Succeeded = false;
                result.Errors.Add(ex.Message);
                result.ErrorType = ErrorType.SystemError;
                return result;
            }

        }


        public async Task<ApiResponse<float>> GetUserDailyEarningPerTime(DateTime Start , DateTime End)
        {
            ApiResponse<float> result = new ApiResponse<float>();
            try
            {
                float Total = await unitOfWork.OrdersManager.GetUserDailyEarning(Start , End); 
                if (Total != 0f)
                {
                        result.Data = Total;  
                        result.Succeeded = true;
                         return result;
                     
                }
                else
                {
                    result.Succeeded = false;
                    result.Errors.Add("Invalid login attempt.");
                    result.ErrorType = ErrorType.LogicalError;
                    return result;
                }

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

