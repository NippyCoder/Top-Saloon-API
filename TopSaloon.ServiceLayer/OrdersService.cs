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

namespace TopSaloon.ServiceLayer
{
    public class OrdersService
    {
        private readonly UnitOfWork unitOfWork;
        private readonly IConfiguration config;
        private IMapper mapper;

        public OrdersService(UnitOfWork unitOfWork, IConfiguration config, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.config = config;
            this.mapper = mapper;
        }

        public async Task<ApiResponse<List<OrderService>>> GetOrderServicesViaOrderId(int orderId)
        {

            ApiResponse<List<OrderService>> result = new ApiResponse<List<OrderService>>();

            try
            {
                var orderServicesToFetch = await unitOfWork.OrderServicesManager.GetAsync(b => b.OrderId == orderId);
                var orderServices = orderServicesToFetch.ToList();
                if (orderServices != null)
                {            
                    result.Data = orderServices;
                    result.Succeeded = true;
                    return result;
                }
                else
                {
                    result.Succeeded = false;
                    result.Errors.Add("Could not fetch order services");
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


        public async Task<ApiResponse<CompleteOrderDTO>> GetCompleteOrderById(int id)
        {

            ApiResponse<CompleteOrderDTO> result = new ApiResponse<CompleteOrderDTO>();

            try
            {
                var completeOrder = await unitOfWork.CompleteOrdersManager.GetByIdAsync(id);

                if (completeOrder != null)
                {
                    result.Data = mapper.Map<CompleteOrderDTO>(completeOrder);
                    result.Succeeded = true;
                    return result;
                }
                else
                {
                    result.Succeeded = false;
                    result.Errors.Add("Unable to retreive complete order !");
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


        public async Task<ApiResponse<bool>> SetOrderService(int orderId)
        {

            ApiResponse<bool> result = new ApiResponse<bool>();

            try
            {
                var orderServices = await unitOfWork.OrderServicesManager.GetByIdAsync(orderId);
                
                if (orderServices != null)
                {
                    orderServices.IsConfirmed = true;

                    var isUpdated = await unitOfWork.OrderServicesManager.UpdateAsync(orderServices);
                    if (isUpdated)
                    {
                        result.Data = true;
                        result.Succeeded = true;
                        return result;
                    }
                    else
                    {
                        result.Succeeded = false;
                        result.Errors.Add("Error updating order service !");
                        return result;
                    }
                }
                else
                {
                    result.Succeeded = false;
                    result.Errors.Add("Could not fetch order service");
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

        //----------------------------- Cancel Order ----------------------------------------------//
        public async Task<ApiResponse<string>> CancelOrder(string orderId, string customerId)
        {
            //Cancel Order: set order status to cancelled, pop order from corresponding queue. 
            ApiResponse<string> result = new ApiResponse<string>();

            try
            {
                var order = await unitOfWork.OrdersManager.GetAsync(b => b.Id == Int32.Parse(orderId), 0, 0, null, includeProperties: "OrderServices");
                Order OrderToUpdate = order.FirstOrDefault();
                CompleteOrder OrderHistory = new CompleteOrder();
                OrderHistory.OrderServicesList = "";
                if (OrderToUpdate != null)
                {
                    var customer = await unitOfWork.CustomersManager.GetByIdAsync(Int32.Parse(customerId));
                    if(customer != null)
                    {
                        OrderToUpdate.Status = "Cancelled";
                        var isUpdated_OrderService = false;
                        OrderHistory.OrderDateTime = OrderToUpdate.OrderDate;
                        OrderHistory.OrderFinishTime = OrderToUpdate.FinishTime;
                        OrderHistory.OrderTotalAmount = OrderToUpdate.OrderTotal;
                        OrderHistory.CustomerWaitingTimeInMinutes = OrderToUpdate.WaitingTimeInMinutes;
                        OrderHistory.Status = OrderToUpdate.Status;
                        OrderHistory.CustomerId = customer.Id;
                        for (int i = 0; i < OrderToUpdate.OrderServices.Count; i++)
                        {
                            OrderToUpdate.OrderServices[i].IsConfirmed = false;
                            OrderHistory.OrderServicesList = OrderHistory.OrderServicesList + OrderToUpdate.OrderServices[i].ServiceId + ","; // orderservices[i].ServiceId
                            isUpdated_OrderService = true;
                        }
                        var OrderUpdateResult = false;
                        if (isUpdated_OrderService)
                        {
                            OrderUpdateResult = await unitOfWork.OrdersManager.UpdateAsync(OrderToUpdate);
                        }
                        //= await unitOfWork.SaveChangesAsync();
                        if (OrderUpdateResult)
                        {
                            var barberQueue = await unitOfWork.BarbersQueuesManager.GetAsync(b => b.Id == OrderToUpdate.BarberQueueId, 0, 0, null, includeProperties: "Orders");
                            if (barberQueue != null)
                            {
                                BarberQueue QueueToUpdate = barberQueue.FirstOrDefault();
                                for (int i = 0; i < QueueToUpdate.Orders.Count; i++)
                                {
                                    if (QueueToUpdate.Orders[i].Id == Int32.Parse(orderId))
                                    {
                                        QueueToUpdate.Orders.Remove(QueueToUpdate.Orders[i]);
                                    }
                                }
                                if (QueueToUpdate.Orders.Count == 0)
                                {
                                    QueueToUpdate.QueueStatus = "idle";
                                }
                                else //Re-Adjust Queue Finish Time For Remaining Orders
                                {
                                    for(int i=0; i<QueueToUpdate.Orders.Count; i++)
                                    {
                                        if (i == 0)
                                        {
                                            QueueToUpdate.Orders[i].FinishTime =
                                                QueueToUpdate.Orders[i].OrderDate.Value.AddMinutes
                                                (Convert.ToDouble(QueueToUpdate.Orders[i].TotalServicesWaitingTime));
                                        }
                                        else
                                        {
                                            QueueToUpdate.Orders[i].FinishTime =
                                                QueueToUpdate.Orders[i - 1].FinishTime.Value.AddMinutes
                                                (Convert.ToDouble(QueueToUpdate.Orders[i].TotalServicesWaitingTime));
                                        }
                                    }
                                }
                                OrderHistory.BarberId = QueueToUpdate.BarberId;
                                await unitOfWork.BarbersQueuesManager.UpdateAsync(QueueToUpdate);
                                await unitOfWork.CompleteOrdersManager.CreateAsync(OrderHistory);
                                var finalres = await unitOfWork.SaveChangesAsync();
                                if (finalres)
                                {
                                    await SetQueueWaitingTimes();
                                    result.Data = "Order cancelled successfully.";
                                    result.Succeeded = true;
                                    return result;
                                }
                                else
                                {
                                    result.Data = "Cancellation error.";
                                    result.Succeeded = false;
                                    return result;
                                }
                            }
                            else
                            {
                                result.Data = "Error";
                                result.Errors.Add("Failed to fetch barber Queue !");
                                return result;
                            }
                        }
                        else
                        {
                            result.Data = "Error.";
                            result.Errors.Add("Error cancelling order !");
                            result.Succeeded = false;
                            return result;
                        }
                    }
                    else
                    {
                        result.Data = "Error.";
                        result.Errors.Add("Error fetching customer details !");
                        result.Succeeded = false;
                        return result;

                    }
                }
                else
                {
                    result.Data = "Error";
                    result.Succeeded = false;
                    result.Errors.Add("Could not fetch order service");
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

        //--------------------------------- Confirm Order Services ----------------------------------------//
        public async Task<ApiResponse<string>> ConfirmOrderServices(List<OrderServiceDTO> orderServices)
        {
            ApiResponse<string> result = new ApiResponse<string>();
            try
            {
                int iteratorErrorCounter = 0;
                for(int i=0; i<orderServices.Count; i++)
                {
                    var serviceToFetch = await unitOfWork.OrderServicesManager.GetAsync(b => b.Id == orderServices[i].Id);
                    var service = serviceToFetch.ToList().FirstOrDefault();
                    service.IsConfirmed = orderServices[i].IsConfirmed;
                    var updateRes = await unitOfWork.OrderServicesManager.UpdateAsync(service);
                    if (updateRes)
                    {
                    }
                    else
                    {
                        iteratorErrorCounter++;
                    }
                }
                if(iteratorErrorCounter > 0)
                {
                    result.Data = "Error updating one or more order services status !";
                    result.Succeeded = false;
                    return result;
                }
                else
                {
                    var orderToFetch = await unitOfWork.OrdersManager.GetAsync(b => b.Id == orderServices[0].OrderId);
                    var order = orderToFetch.FirstOrDefault();
                    var completionRes = await FinalizeOrder(order.Id, order.CustomerId);
                    if (completionRes.Succeeded)
                    {

                        var queueUpdateRes = await SetQueueWaitingTimes();
                        if (queueUpdateRes.Data)
                        {
                            result.Data = "Order services status updates successfully.";
                            result.Succeeded = true;
                            return result;
                        }
                        else
                        {
                            result.Data = "Error adjusting Queue waiting time! ";
                            result.Succeeded = false;
                            return result;
                        }

                    }
                    else
                    {
                        result.Data = "Error finalizing order !";
                        result.Succeeded = true;
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                result.Succeeded = false;
                result.Errors.Add(ex.Message);
                return result;
            }
        }
            //--------------------------------- Finalize Order ------------------------------------------------//

            public async Task<ApiResponse<string>> FinalizeOrder(int orderId, int customerId)
            {
                ApiResponse<string> result = new ApiResponse<string>();

                try
                {
                    var order = await unitOfWork.OrdersManager.GetAsync(b => b.Id == orderId, 0, 0, null, includeProperties: "OrderServices");
                    Order OrderToUpdate = order.FirstOrDefault();
                    CompleteOrder OrderHistory = new CompleteOrder();
                    OrderHistory.OrderServicesList = "";
                    if (OrderToUpdate != null)
                    {
                        var customer = await unitOfWork.CustomersManager.GetByIdAsync(customerId);
                        if (customer != null)
                        {
                            OrderToUpdate.Status = "Done";
                            var isUpdated_OrderService = false;
                            OrderHistory.OrderDateTime = OrderToUpdate.OrderDate;
                            OrderHistory.OrderFinishTime = OrderToUpdate.FinishTime;
                            OrderHistory.OrderTotalAmount = OrderToUpdate.OrderTotal;
                            OrderHistory.CustomerWaitingTimeInMinutes = OrderToUpdate.WaitingTimeInMinutes;
                            OrderHistory.Status = OrderToUpdate.Status;
                            OrderHistory.CustomerId = customer.Id;
                            for (int i = 0; i < OrderToUpdate.OrderServices.Count; i++)
                            {
                                if (OrderToUpdate.OrderServices[i].IsConfirmed.HasValue)
                                {
                                    OrderHistory.OrderServicesList = OrderHistory.OrderServicesList + OrderToUpdate.OrderServices[i].ServiceId + ","; // orderservices[i].ServiceId
                                    isUpdated_OrderService = true;
                                }
                            }
                            var OrderUpdateResult = false;
                            if (isUpdated_OrderService)
                            {
                                OrderUpdateResult = await unitOfWork.OrdersManager.UpdateAsync(OrderToUpdate);
                            }
                            //= await unitOfWork.SaveChangesAsync();
                            if (OrderUpdateResult)
                            {
                                var barberQueue = await unitOfWork.BarbersQueuesManager.GetAsync(b => b.Id == OrderToUpdate.BarberQueueId, 0, 0, null, includeProperties: "Orders");
                                if (barberQueue != null)
                                {
                                    BarberQueue QueueToUpdate = barberQueue.FirstOrDefault();
                                    for (int i = 0; i < QueueToUpdate.Orders.Count; i++)
                                    {
                                        if (QueueToUpdate.Orders[i].Id == orderId)
                                        {
                                            QueueToUpdate.Orders.Remove(QueueToUpdate.Orders[i]);
                                        }
                                    }
                                    if (QueueToUpdate.Orders.Count == 0)
                                    {
                                        QueueToUpdate.QueueStatus = "idle";
                                    }
                                    else //Re-Adjust Queue Finish Time For Remaining Orders
                                    {
                                        for (int i = 0; i < QueueToUpdate.Orders.Count; i++)
                                        {
                                            if (i == 0)
                                            {
                                                QueueToUpdate.Orders[i].FinishTime =
                                                    QueueToUpdate.Orders[i].OrderDate.Value.AddMinutes
                                                    (Convert.ToDouble(QueueToUpdate.Orders[i].TotalServicesWaitingTime));
                                            }
                                            else
                                            {
                                                QueueToUpdate.Orders[i].FinishTime =
                                                    QueueToUpdate.Orders[i - 1].FinishTime.Value.AddMinutes
                                                    (Convert.ToDouble(QueueToUpdate.Orders[i].TotalServicesWaitingTime));
                                            }
                                        }
                                    }
                                    OrderHistory.BarberId = QueueToUpdate.BarberId;
                                    await unitOfWork.BarbersQueuesManager.UpdateAsync(QueueToUpdate);
                                    await unitOfWork.CompleteOrdersManager.CreateAsync(OrderHistory);
                                    var finalres = await unitOfWork.SaveChangesAsync();
                                    if (finalres)
                                    {
                                        result.Data = "Order successfully Finalized.";
                                        result.Succeeded = true;
                                        return result;
                                    }
                                    else
                                    {
                                        result.Data = "Error finalizing order.";
                                        result.Succeeded = false;
                                        return result;
                                    }
                                }
                                else
                                {
                                    result.Data = "Error";
                                    result.Errors.Add("Failed to fetch barber Queue !");
                                    return result;
                                }
                            }
                            else
                            {
                                result.Data = "Error.";
                                result.Errors.Add("Error Finalizing order !");
                                result.Succeeded = false;
                                return result;
                            }
                        }
                        else
                        {
                            result.Data = "Error.";
                            result.Errors.Add("Error fetching customer details !");
                            result.Succeeded = false;
                            return result;

                        }
                    }
                    else
                    {
                        result.Data = "Error";
                        result.Succeeded = false;
                        result.Errors.Add("Could not fetch order service");
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
                if (barberQueuesList != null)
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

                            var res = await unitOfWork.SaveChangesAsync();
                        }
                        else
                        {
                            barberQueueToUpdate.QueueWaitingTime = 0;

                            await unitOfWork.SaveChangesAsync();
                        }
                        //End of calculating queue waiting time .
                    }

                    if (countErrors > 0)
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
            catch (Exception ex)
            {
                result.Succeeded = false;
                result.Errors.Add(ex.Message);
                return result;
            }
        }
        public async Task<ApiResponse<List<CompleteOrderDTO>>> getAllCompleteOrderByDate(DateTime date)
        {
            var result = new ApiResponse<List<CompleteOrderDTO>>();
            try
            {
                var Complete = await unitOfWork.CompleteOrdersManager.GetAsync(a=>a.OrderDateTime==date);
                List<CompleteOrder> completeorderlist = Complete.ToList();
                if (completeorderlist != null)
                {
                    result.Data = mapper.Map<List<CompleteOrderDTO>>(completeorderlist);
                    result.Succeeded = true;
                    return result;
                }
                else
                {
                    result.Succeeded = false;
                    result.Errors.Add("Error fetching Barber Queues");
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

//