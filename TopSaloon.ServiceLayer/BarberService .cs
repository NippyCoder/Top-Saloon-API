﻿using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage.Internal;
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
using AutoMapper;
using Microsoft.VisualBasic;

namespace TopSaloon.ServiceLayer
{
    public class BarberService
    {
        private readonly UnitOfWork unitOfWork;
        private readonly IConfiguration config;
        private readonly IMapper mapper;

        public BarberService(UnitOfWork unitOfWork, IConfiguration config, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.config = config;
            this.mapper = mapper;


        }
        public async Task<ApiResponse<int>> GetNumberOfAvailableBarbers()
        {
            ApiResponse<int> result = new ApiResponse<int>();
            try
            {
                result.Data =  await unitOfWork.BarbersManager.GetNumberOfAvailableBarber();

                result.Succeeded = true;
            }
            catch (Exception ex)
            {
                result.Succeeded = false;
                result.Errors.Add(ex.Message);
            }
            return result;
        }
        public async Task<ApiResponse<int>> BarberTotalNumberOfHandledCustomer(int BarberId)
        {
            ApiResponse<int> result = new ApiResponse<int>();

            try
            {
                Barber barber = await unitOfWork.BarbersManager.GetByIdAsync(BarberId);

                if (barber != null)
                {
                    result.Data = (int)barber.NumberOfCustomersHandled;
                    result.Succeeded = true;
                    return result;
                }
                result.Succeeded = false;
                result.Errors.Add("It May Be A New Barber !");
                result.ErrorType = ErrorType.LogicalError;
                return result;
            }
            catch (Exception ex)
            {
                result.Succeeded = false;
                result.Errors.Add(ex.Message);
                return result;
            }
        }
        public async Task<ApiResponse<BarberDTO>> GetBarberDetailsReports(int BarberId)
        {
            ApiResponse<BarberDTO> result = new ApiResponse<BarberDTO>();
            try
            {
                var barber = await unitOfWork.BarbersManager.GetAsync(A=>A.Id==BarberId, includeProperties: "BarberQueue");
                 List<Barber> barberData = barber.ToList();

                if (barber != null)
                {
                    result.Data = mapper.Map<BarberDTO>(barberData[0]);
                    result.Succeeded = true;
                   

                    return result;
                }
                result.Succeeded = false;
                result.Errors.Add("It May Be A New Barber !");
                result.ErrorType = ErrorType.LogicalError;
                return result;
            }
            catch (Exception ex)
            {
                result.Succeeded = false;
                result.Errors.Add(ex.Message);
                return result;
            }
        }
        public async Task<ApiResponse<BarberDTO>> CreateBarber(CreateBarberDTO model)
        {
            ApiResponse<BarberDTO> result = new ApiResponse<BarberDTO>();
            try
            {
                var shops = await unitOfWork.ShopsManager.GetAsync();

                Shop shop = shops.FirstOrDefault();
                Barber barberToAdd = new Barber();
                barberToAdd.NameAR = model.NameAR;
                barberToAdd.NameEN = model.NameEN;
                barberToAdd.ShopId = shop.Id;
                barberToAdd.NumberOfCustomersHandled = 0;
                barberToAdd.Status = "Unavailable";
                var barberResult = await unitOfWork.BarbersManager.CreateAsync(barberToAdd);

                await unitOfWork.SaveChangesAsync();

                if(barberResult != null)
                {
                    BarberProfilePhoto barberProfilePhoto = new BarberProfilePhoto();
                    barberProfilePhoto.BarberId = barberResult.Id;
                    barberProfilePhoto.AdminPath = model.BarberProfilePhotoPathAdmin;
                    barberProfilePhoto.UserPath = model.BarberProfilePhotoPathUser;

                    var barberProfilePhotoResult = await unitOfWork.BarberProfilePhotosManager.CreateAsync(barberProfilePhoto);

                    await unitOfWork.SaveChangesAsync();

                    if (barberProfilePhotoResult != null)
                    {
                        BarberQueue barberQueue = new BarberQueue();

                        barberQueue.BarberId = barberResult.Id;

                        barberQueue.QueueStatus = "idle";

                        barberQueue.QueueWaitingTime = 0;

                        var barberQueueResult = await unitOfWork.BarbersQueuesManager.CreateAsync(barberQueue);

                        await unitOfWork.SaveChangesAsync();

                        if (barberQueueResult != null)
                        {

                            var barbers = await unitOfWork.BarbersManager.GetAsync(b => b.Id == barberResult.Id, includeProperties:"BarberQueue,BarberProfilePhoto");

                            Barber barberToReturn = barbers.FirstOrDefault();

                            if(barberToReturn != null)
                            {
                                result.Succeeded = true;
                                result.Data = mapper.Map<BarberDTO>(barberToReturn);
                                result.Errors.Add("Failed to create barber !");
                                return result;
                            } 
                            else
                            {
                                result.Succeeded = false;
                                result.Errors.Add("Error creating barber !");
                                return result;
                            }
                        }
                        else
                        {
                            result.Succeeded = false;
                            result.Errors.Add("Error creating barber queue !");
                            return result;
                        }
                    }
                    else
                    {
                        result.Succeeded = false;
                        result.Errors.Add("Failed to create barber profile photo !");
                        return result;
                    }

                }
                else
                {
                    result.Succeeded = false;
                    result.Errors.Add("Failed to create barber !");
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
        public async Task<ApiResponse<bool>> EditBarber(EditBarberDTO model)
        {
            ApiResponse<bool> result = new ApiResponse<bool>();
            try
            {
                Barber BarberToEdit = await unitOfWork.BarbersManager.GetByIdAsync(model.Id);

                if(BarberToEdit != null)
                {
                    BarberProfilePhoto barberProfilePhotoToEdit = await unitOfWork.BarberProfilePhotosManager.GetProfilePhotoByBarberId(BarberToEdit.Id);

                    if(barberProfilePhotoToEdit != null)
                    {
                        BarberToEdit.NameAR = model.NameAR;
                        BarberToEdit.NameEN = model.NameEN;

                        barberProfilePhotoToEdit.AdminPath = model.BarberProfilePhotoPathAdmin;
                        barberProfilePhotoToEdit.UserPath = model.BarberProfilePhotoPathUser;

                        var barberResult = await unitOfWork.BarbersManager.UpdateAsync(BarberToEdit);

                        var barberProfilePhotoResult = await unitOfWork.BarberProfilePhotosManager.UpdateAsync(barberProfilePhotoToEdit);

                        

                        if(barberResult == true && barberProfilePhotoResult == true)
                        {

                            await unitOfWork.SaveChangesAsync();

                            result.Succeeded = true;
                            result.Data = true;
                            return result;

                        }
                        else
                        {
                            result.Succeeded = false;
                            result.Errors.Add("Failed to update barber information ! ");
                            return result;
                        }
                    }
                    else
                    {
                        result.Succeeded = false;
                        result.Errors.Add("Unable to find barber profile photo with specified barber id ! ");
                        return result;
                    }
                }
                else
                {
                    result.Succeeded = false;
                    result.Errors.Add("Unable to find barber with specified id ! ");
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
        public async Task<ApiResponse<List<BarberDTO>>> GetAllBarbers()
        {
            ApiResponse<List<BarberDTO>> result = new ApiResponse<List<BarberDTO>>();

            try
            {
                var barbersList = await unitOfWork.BarbersManager.GetAsync(includeProperties: "BarberProfilePhoto");

                List <Barber> barberListToReturn = barbersList.ToList();


                if (barberListToReturn != null)
                {
                    result.Data = mapper.Map<List<BarberDTO>>(barberListToReturn);
                    result.Succeeded = true;
                    return result;
                }
                else
                {
                    result.Errors.Add("Unable to retreive barbers list !");
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
        public async Task<ApiResponse<List<BarberDTO>>> GetAvailableBarbers()
        {
            ApiResponse<List<BarberDTO>> result = new ApiResponse<List<BarberDTO>>();

            try
            {
                var Barbers = await unitOfWork.BarbersManager.GetAsync(b => b.Status == "Available", includeProperties: "BarberProfilePhoto,BarberQueue");

                if (Barbers != null)
                {
                    result.Data = mapper.Map<List<BarberDTO>>(Barbers.ToList());
                    result.Succeeded = true;
                    return result;
                }
                else
                {
                    result.Errors.Add("Unable to fetch available barbers list !");
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
        public async Task<ApiResponse<bool>> DeleteBarberById(int id)
        {
            ApiResponse<bool> result = new ApiResponse<bool>();

            try
            {
                var barberToDelete = await unitOfWork.BarbersManager.GetByIdAsync(id);

                if(barberToDelete != null)
                {
                    var RemoveBarberResult = await unitOfWork.BarbersManager.RemoveAsync(barberToDelete);

                    await unitOfWork.SaveChangesAsync();

                    if(RemoveBarberResult == true)
                    {
                        result.Data = true;
                        result.Succeeded = true;
                        result.Errors.Add("Barber deleted successfully !");
                        return result;
                    }
                    else
                    {
                        result.Succeeded = false;
                        result.Errors.Add("Failed to delete barber !");
                        return result;
                    }

                }
                else
                {
                    result.Succeeded = false;
                    result.Errors.Add("Failed to fetch barber with specified id !");
                    return result;
                }         
            }
            catch(Exception ex)
            {
                result.Succeeded = false;
                result.Errors.Add(ex.Message);
                return result;
            }
        }
        public async Task<ApiResponse<List<CompleteOrder>>> GetBarberAllCustomersHandled(int id)
        {
            ApiResponse<List<CompleteOrder>> result = new ApiResponse<List<CompleteOrder>>();

            try
            {
                var barbersList = await unitOfWork.CompleteOrdersManager.GetAsync(A=>A.BarberId==id);

                List<CompleteOrder> barberListToReturn = barbersList.ToList();


                if (barberListToReturn != null)
                {
                    result.Data = mapper.Map<List<CompleteOrder>>(barberListToReturn);
                    result.Succeeded = true;
                    return result;
                }
                else
                {
                    result.Errors.Add("Unable to retreive barbers list !");
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
        public async Task<ApiResponse<BarberDTO>> ChangeBarberStatus(int id)
        {
            ApiResponse<BarberDTO> result = new ApiResponse<BarberDTO>();
            try
            {
                Barber barberToEdit = await unitOfWork.BarbersManager.GetByIdAsync(id);

                if(barberToEdit != null)
                {
                    if(barberToEdit.Status == "Available")
                    {
                        barberToEdit.Status = "Unavailable";
                    }
                    else
                    {
                        barberToEdit.Status = "Available";
                    }

                    var res = await unitOfWork.BarbersManager.UpdateAsync(barberToEdit);

                    await unitOfWork.SaveChangesAsync();

                    if (res == true)
                    {
                        result.Succeeded = true;
                        result.Data = mapper.Map<BarberDTO>(barberToEdit);
                        return result;
                    }
                    else
                    {
                        result.Succeeded = false;
                        result.Errors.Add("Failed to update barber status !");
                        return result;
                    }

                }
                else
                {
                    result.Succeeded = false;
                    result.Errors.Add("Failed to find specified barber !");
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
        public async Task<ApiResponse<bool>> SignInBarber(int id)
        {
            ApiResponse<bool> result = new ApiResponse<bool>();
            try
            {
                Barber barber = await unitOfWork.BarbersManager.GetByIdAsync(id);

                if (barber != null)
                {
                    var barberLoginResult = await unitOfWork.BarberLoginsManager.GetAsync(b => b.BarberId == id && b.LoginDateTime.Value.Date == DateTime.Now.Date);

                    if(barberLoginResult.FirstOrDefault() == null)
                    {

                        BarberLogin newLogin = new BarberLogin();

                        newLogin.BarberId = barber.Id;

                        newLogin.LoginDateTime = DateTime.Now;

                        var res = await unitOfWork.BarberLoginsManager.CreateAsync(newLogin);

                        await unitOfWork.SaveChangesAsync();

                        if(res != null)
                        {
                            result.Succeeded = true;
                            result.Data = true;
                            return result;
                        }
                        else
                        {
                            result.Succeeded = false;
                            result.Data = false;
                            result.Errors.Add("Failed to sign in barber !");
                            return result;
                        }

                    }
                    else
                    {
                        result.Succeeded = false;
                        result.Data = false;
                        result.Errors.Add("Barber already signed in today !");
                        return result;
                    }
       
                }
                else
                {
                    result.Succeeded = false;
                    result.Errors.Add("Failed to find specified barber !");
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
        public async Task<ApiResponse<bool>> SignOutBarber(int id)
        {
            ApiResponse<bool> result = new ApiResponse<bool>();

            try
            {
                Barber barber = await unitOfWork.BarbersManager.GetByIdAsync(id);

                if (barber != null)
                {
                    var barberLoginResult = await unitOfWork.BarberLoginsManager.GetAsync(b => b.BarberId == id && b.LoginDateTime.Value.Date == DateTime.Now.Date);

                    if (barberLoginResult.FirstOrDefault() == null)
                    {
                        result.Succeeded = false;
                        result.Data = false;
                        result.Errors.Add("Barber hasn't signed in today , Barber needs to sign in first in order to be able to sign out !");
                        return result;
                    }
                    else
                    {
                        BarberLogin barberLoginToEdit = barberLoginResult.FirstOrDefault();

                        barberLoginToEdit.logoutDateTime = DateTime.Now;

                        var res = await unitOfWork.BarberLoginsManager.UpdateAsync(barberLoginToEdit);

                        await unitOfWork.SaveChangesAsync();

                        if(res == true)
                        {
                            result.Succeeded = true;
                            result.Data = true;
                            return result;
                        }
                        else
                        {
                            result.Succeeded = false;
                            result.Data = false;
                            result.Errors.Add("Failed to sign out barber !");
                            return result;
                        } 
                    }

                }
                else
                {
                    result.Succeeded = false;
                    result.Errors.Add("Failed to find specified barber !");
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

