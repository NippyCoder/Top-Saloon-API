using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using TopSaloon.Core;
using TopSaloon.DTOs;
using TopSaloon.DTOs.Enums;
using TopSaloon.DTOs.Models;
using TopSaloon.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;

namespace TopSaloon.ServiceLayer
{
    public class QueueService
    {
        private readonly UnitOfWork unitOfWork;
        private readonly IConfiguration config;
        private IMapper mapper;

        public QueueService(UnitOfWork unitOfWork, IConfiguration config, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.config = config;
            this.mapper = mapper;
        }

        public async Task<ApiResponse<BarberQueueDTO>> GetBarberQueueByBarberId(int barberId)
        {
            //Return Barber Name, barber status, barber queue status, total waiting time (orders)

            var result = new ApiResponse<BarberQueueDTO>();

           
            BarberQueueDTO Queue = new BarberQueueDTO();

            try
            {
                var BarberQueue = await unitOfWork.BarbersQueuesManager.GetBarberQueueByBarberId(barberId);
                if (BarberQueue != null)
                {
                    Queue.QueueId = BarberQueue.Id;
                    Queue.BarberId = BarberQueue.BarberId;
                    Queue.QueueStatus = BarberQueue.QueueStatus;
                    
                    var Barber = await unitOfWork.BarbersManager.GetByIdAsync(barberId);
                    if (Barber != null)
                    {
                        Queue.BarberName = Barber.Name;
                        Queue.BarberStatus = Barber.Status;
                        var QueueTotalTime = await unitOfWork.OrdersManager.GetOrderByBarberQueue(Queue.QueueId);
                        Queue.WaitingTimeInMinutes = QueueTotalTime;

                        result.Data = Queue;
                        result.Succeeded = true;
                        return result;
                    }
                    else
                    {
                        result.Succeeded = false;
                        result.Errors.Add("Barber not found!");
                        return result;
                    }  
                }
                else
                {
                    result.Succeeded = false;
                    result.Errors.Add("Barber queue not found");
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


        public async Task<ApiResponse<string>> GetBarberQueueWaitingTime(int QueueId)
        {
            //Return Barber Name, barber status, barber queue status, total waiting time (orders)

            var result = new ApiResponse<string>();
            QueueTimeHandlerModel handler = new QueueTimeHandlerModel();

            try
            {
                var barberQueue = await unitOfWork.BarbersQueuesManager.GetByIdAsync(QueueId);

                if (barberQueue != null)
                {
                    handler.QueueId = barberQueue.Id;

                    var queueOrders = await unitOfWork.OrdersManager.GetOrdersViaBarberQueues(handler.QueueId);
                    List<OrderTimeDTO> Orders = new List<OrderTimeDTO>();
                    //Check for queue availability
                    if (queueOrders != null)
                    {
                        List<OrderService> OrderService = new List<OrderService>();
                        for(int i=0; i<queueOrders.Count; i++)
                        {
                            var queueservices = await unitOfWork.OrderServicesManager.GetOrderServices(queueOrders[i].Id);
                            if(queueservices != null)
                            {
                                for (int k = 0; k < queueservices.Count; k++) 
                                {
                                    queueOrders[i].OrderServices[k] = queueservices[k];
                                } 
                               // queueOrders[i].OrderServices = ;
                            }
                        }
                        
                        handler.Orders = mapper.Map<List<OrderTimeDTO>>(queueOrders);
                        string json = System.Text.Json.JsonSerializer.Serialize(handler);
                        json = json.Replace(@"\", "");
                        result.Data = json;
                        result.Succeeded = true;
                        return result;                       
                    }
                    else
                    {
                        result.Succeeded = false;
                        return result;
                    }
                }
                else
                {
                    result.Succeeded = false;
                    result.Errors.Add("Barber Queue not found");
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

        public async Task<ApiResponse<bool>> ReassignOrderToDifferentQueue(int orderId, int newBarberQueue)
        {
            //Return Barber Name, barber status, barber queue status, total waiting time (orders)

            var result = new ApiResponse<bool>();


            Order orderToUpdate = new Order();

            try
            {
                var order = await unitOfWork.OrdersManager.GetByIdAsync(orderId);

                if (order != null)
                {
                    orderToUpdate.Id = order.Id;
                    orderToUpdate.OrderDate = order.OrderDate;
                    orderToUpdate.OrderFeedback = order.OrderFeedback;
                    orderToUpdate.OrderIdentifier = order.OrderIdentifier;
                    orderToUpdate.OrderServices = order.OrderServices;
                    orderToUpdate.OrderTotal = order.OrderTotal;
                    orderToUpdate.Status = order.Status;
                    orderToUpdate.WaitingTimeInMinutes = order.WaitingTimeInMinutes;
                    orderToUpdate.BarberQueueId = newBarberQueue;


                    bool isUpdated = await unitOfWork.OrdersManager.UpdateAsync(orderToUpdate);
                    if (isUpdated)
                    {

                        result.Data = true;
                        result.Succeeded = true;
                        return result;
                    }
                    else
                    {
                        result.Succeeded = false;
                        result.Errors.Add("Error changing Order queue");
                        return result;
                    }
                }
                else
                {
                    result.Succeeded = false;
                    result.Errors.Add("Error fetching order");
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

