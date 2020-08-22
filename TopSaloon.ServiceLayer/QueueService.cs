using AutoMapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TopSaloon.Core;
using TopSaloon.DTOs;
using TopSaloon.DTOs.Models;
using TopSaloon.Entities.Models;

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

            try
            {
                var barberQueueToFetch = await unitOfWork.BarbersQueuesManager.GetAsync(b=> b.BarberId == barberId, includeProperties: "Orders");
                var BarberQueue = barberQueueToFetch.ToList().FirstOrDefault();
                if (BarberQueue != null)
                {
                    result.Data = mapper.Map<BarberQueueDTO>(BarberQueue);
                    result.Succeeded = true;
                    return result;
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

        //Barber Queue order addition
        public async Task<ApiResponse<bool>> AddOrderToQueue(OrderToAddDTO order)
        {
            var result = new ApiResponse<bool>();
            BarberQueue Queue = new BarberQueue();
            Order currentOrder = new Order();
            order.TotalServicesWaitingTime = 0;
            order.OrderTotal = 0;
            for (int i=0; i<order.OrderServices.Count; i++)
            {
                order.TotalServicesWaitingTime += order.OrderServices[i].Time;
                order.OrderTotal += order.OrderServices[i].Price;
                order.OrderServices[i].IsConfirmed = false;
            }
            try
            {
                var BarberQueue = await unitOfWork.BarbersQueuesManager.GetAsync(b => b.Id == order.BarberQueueId, 0, 0, null, includeProperties: "Orders");
                Queue = BarberQueue.FirstOrDefault();
                if (Queue != null)
                {
                    // var QueueOrders = await unitOfWork.OrdersManager.GetAsync(b => b.BarberQueueId == QueueId, 0, 0, null, includeProperties: "OrderServices");

                    if (Queue.Orders.Count == 0 && Queue.QueueStatus == "idle") // Empty queue 
                    {
                        currentOrder.BarberQueueId = order.BarberQueueId;
                        currentOrder.OrderDate = DateTime.Now;
                        currentOrder.FinishTime = DateTime.Now.AddMinutes(Convert.ToDouble(order.TotalServicesWaitingTime));
                        currentOrder.OrderTotal = order.OrderTotal;
                        currentOrder.WaitingTimeInMinutes = order.TotalServicesWaitingTime;
                        currentOrder.TotalServicesWaitingTime = order.TotalServicesWaitingTime;
                        currentOrder.Status = "inprogress";
                        currentOrder.OrderServices = mapper.Map<List<OrderService>>(order.OrderServices.ToList());
                        var CreationResult = await unitOfWork.OrdersManager.CreateAsync(currentOrder);
            
                        if (CreationResult != null)
                        {
                            OrderService ServiceCreationResult;
                            for(int i=0; i<CreationResult.OrderServices.Count; i++)
                            {
                                ServiceCreationResult = await unitOfWork.OrderServicesManager.CreateAsync(currentOrder.OrderServices[i]);
                            }
                            Queue.QueueStatus = "busy";
                            var BarberUpdateResult = await unitOfWork.BarbersQueuesManager.UpdateAsync(Queue);
                            var FinalRes = await unitOfWork.SaveChangesAsync();
                            if (FinalRes)
                            {
                                result.Succeeded = true;
                                result.Data = true;
                                return result;
                            }
                            else
                            {
                                result.Succeeded = false;
                                result.Errors.Add("Unable to save changes");
                                return result;
                            }
                        }
                        else
                        {
                            result.Succeeded = false;
                            return result;
                        }
                    }
                    else
                    {
                        currentOrder.OrderDate = DateTime.Now;
                        currentOrder.OrderTotal = order.OrderTotal;
                        currentOrder.Status = "pending";
                        currentOrder.WaitingTimeInMinutes = order.TotalServicesWaitingTime;
                        currentOrder.TotalServicesWaitingTime = order.TotalServicesWaitingTime;
                        currentOrder.CustomerId = order.CustomerId;
                        for (int i = 0; i < Queue.Orders.Count; i++)
                        {
                            if(i == Queue.Orders.Count - 1)
                            {
                                currentOrder.FinishTime = Queue.Orders[i].FinishTime.Value.AddMinutes(Convert.ToDouble(currentOrder.TotalServicesWaitingTime));
                            }
                        }
                        currentOrder.BarberQueueId = order.BarberQueueId;
                       // currentOrder.CustomerId = order.CustomerId;
                        currentOrder.OrderServices = mapper.Map<List<OrderService>>(order.OrderServices.ToList());
                        var CreationResult = await unitOfWork.OrdersManager.CreateAsync(currentOrder);

                        if (CreationResult != null)
                        {
                            OrderService ServiceCreationResult;
                            for (int i = 0; i < CreationResult.OrderServices.Count; i++)
                            {
                                ServiceCreationResult = await unitOfWork.OrderServicesManager.CreateAsync(currentOrder.OrderServices[i]);
                            }
                            var FinalRes = await unitOfWork.SaveChangesAsync();
                            if (FinalRes)
                            {
                                var queueAdjustmentResult = await SetQueueWaitingTimes();
                                if (queueAdjustmentResult.Data == true)
                                {
                                    result.Succeeded = true;
                                    result.Data = true;
                                    return result;
                                }
                                else
                                {
                                    result.Succeeded = false;
                                    result.Data = false;
                                    result.Errors.Add("Unable to adjust queue times !");
                                    return result;
                                }
                            }
                            else
                            {
                                result.Succeeded = false;
                                result.Errors.Add("Unable to save changes");
                                return result;
                            }
                        }
                        else
                        {
                            result.Succeeded = false;
                            return result;
                        }
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

        public async Task<ApiResponse<QueueTimeHandlerModel>> SetBarberQueueWaitingTime(int QueueId)
        {
            //Return Barber Name, barber status, barber queue status, total waiting time (orders)

            var result = new ApiResponse<QueueTimeHandlerModel>();
            QueueTimeHandlerModel handler = new QueueTimeHandlerModel();

            try
            {
                var barberQueue = await unitOfWork.BarbersQueuesManager.GetByIdAsync(QueueId);

                if (barberQueue != null)
                {
                    handler.QueueId = barberQueue.Id;

                    var queueOrders = await unitOfWork.OrdersManager.GetAsync(b => b.BarberQueueId == handler.QueueId, 0,0,null, includeProperties: "OrderServices");

                    //Check for queue availability

                    if (queueOrders != null)
                    {
                        TimeSpan? CalculatedDateTime;
                        handler.Orders = mapper.Map<List<OrderDTO>>(queueOrders.ToList());
                        if (handler.Orders.Count == 0)
                        {
                            handler.QueueEstimatedFinishTime = null;
                            handler.QueueEstimatedWaitingTime = 0;
                            handler.QueueId = QueueId;
                            handler.Orders = null;
                            result.Data = handler;
                            result.Succeeded = true;
                            return result;
                        }
                        else
                        {
                            for (int i = 0; i < handler.Orders.Count; i++)
                            {
                                if (i == 0)
                                {
                                    handler.QueueEstimatedFinishTime = handler.Orders[i].FinishTime;
                                    handler.QueueEstimatedWaitingTime = 0;
                                }
                                else
                                {
                                    handler.QueueEstimatedFinishTime = handler.QueueEstimatedFinishTime.Value.AddMinutes(Convert.ToDouble(handler.Orders[i].WaitingTimeInMinutes.Value));
                                    CalculatedDateTime = (handler.QueueEstimatedFinishTime - handler.Orders[i].OrderDate);
                                    handler.QueueEstimatedWaitingTime = CalculatedDateTime.Value.TotalMinutes;
                                    //Calculated DateTime: time difference between General estimate finish time and current order creation date.
                                }

                            }
                            CalculatedDateTime = (handler.QueueEstimatedFinishTime - DateTime.Now);          
                            handler.QueueEstimatedWaitingTime = CalculatedDateTime.Value.TotalMinutes;
                            if(handler.QueueEstimatedWaitingTime < 0)
                            {
                                handler.QueueEstimatedWaitingTime = 0;
                            }
                            result.Data = handler;
                            result.Succeeded = true;
                            return result;
                        }

                        //end of else .

                    }
                    else
                    {
                        result.Succeeded = false;
                        result.Errors.Add("Error fetching queue orders!");
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

        public async Task<ApiResponse<bool>> SetQueueWaitingTimes()
        {
            var result = new ApiResponse<bool>();
            try
            {
                var barberQueues = await unitOfWork.BarbersQueuesManager.GetAsync();
                List<BarberQueue> barberQueuesList = barberQueues.ToList();
                if(barberQueuesList != null)
                {
                    int countErrors = 0;

                    for (int i = 0; i < barberQueuesList.Count; i++)
                    {
                        // Calculating waiting queue waiting time . 
                        QueueTimeHandlerModel handler = new QueueTimeHandlerModel();
                        handler.QueueId = barberQueuesList[i].Id;
                        var queueOrders = await unitOfWork.OrdersManager.GetAsync(b => b.BarberQueueId == handler.QueueId, 0, 0, null, includeProperties: "OrderServices");
                        BarberQueue barberQueueToUpdate = await unitOfWork.BarbersQueuesManager.GetByIdAsync(barberQueuesList[i].Id);

                        if (queueOrders != null)
                        {
                            
                            TimeSpan? CalculatedDateTime;
                            handler.Orders = mapper.Map<List<OrderDTO>>(queueOrders.ToList());

                            if (handler.Orders.Count == 0)
                            {
                                handler.QueueEstimatedFinishTime = null;
                                handler.QueueEstimatedWaitingTime = 0;
                                barberQueueToUpdate.QueueWaitingTime = 0;
                            }
                            else
                            {
                                for (int y = 0; y < handler.Orders.Count; y++)
                                {
                                    if (y == 0)
                                    {
                                        handler.QueueEstimatedFinishTime = handler.Orders[y].FinishTime;
                                        handler.QueueEstimatedWaitingTime = 0;
                                        barberQueueToUpdate.QueueWaitingTime = 0;
                                    }
                                    else
                                    {
                                        handler.QueueEstimatedFinishTime = handler.QueueEstimatedFinishTime.Value.AddMinutes(Convert.ToDouble(handler.Orders[y].WaitingTimeInMinutes.Value));
                                        CalculatedDateTime = (handler.QueueEstimatedFinishTime - handler.Orders[y].OrderDate);
                                        handler.QueueEstimatedWaitingTime = CalculatedDateTime.Value.TotalMinutes;
                                        barberQueueToUpdate.QueueWaitingTime = Convert.ToInt32(CalculatedDateTime.Value.TotalMinutes);
                                        //Calculated DateTime: time difference between General estimate finish time and current order creation date.
                                    }
                                }
                                CalculatedDateTime = (handler.QueueEstimatedFinishTime - DateTime.Now);
                                handler.QueueEstimatedWaitingTime = CalculatedDateTime.Value.TotalMinutes;
                                barberQueueToUpdate.QueueWaitingTime = Convert.ToInt32(CalculatedDateTime.Value.TotalMinutes);
                                if (barberQueueToUpdate.QueueWaitingTime < 0)
                                {
                                    barberQueueToUpdate.QueueWaitingTime = 0;
                                }
                            }

                           var res = await  unitOfWork.SaveChangesAsync();  
                        }
                        else
                        {
                            barberQueueToUpdate.QueueWaitingTime = 0;

                            await unitOfWork.SaveChangesAsync();
                        } 
                        //End of calculating queue waiting time .
                     }
                    
                    if(countErrors > 0 )
                    {
                            result.Succeeded = false;
                            result.Errors.Add("Error fetching Barber Queues!");
                            return result;
                     }
                    else
                    {
                        result.Data = true;
                        result.Succeeded = true;
                        return result;
                    }
                    

                }//End OF MAIN IF  . 
                else
                {
                    result.Succeeded = false;
                    result.Errors.Add("Error fetching Barber Queues");
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

