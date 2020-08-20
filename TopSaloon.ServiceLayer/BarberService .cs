using Microsoft.AspNetCore.Identity;
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
                    barberProfilePhoto.Path = model.BarberProfilePhotoPath;

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
                var Barbers = await unitOfWork.BarbersManager.GetAsync(b => b.Status == "Available" || b.Status == "Busy", includeProperties: "BarberProfilePhoto,BarberQueue");

                if (Barbers != null)
                {
                    result.Data = mapper.Map<List<BarberDTO>>(Barbers.ToList());
                    result.Succeeded = true;
                    return result;
                }
                else
                {
                    result.Errors.Add("Unable to fetch barber list !");
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

    }
}

