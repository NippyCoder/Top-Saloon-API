using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TopSaloon.API.Controllers.Common;
using TopSaloon.DTOs.Models;
using TopSaloon.ServiceLayer;

namespace TopSaloon.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : BaseResultHandlerController<CustomerService>
    {
        public CustomerController(CustomerService _service) : base(_service)
        {

        }

        [HttpPost("AddNewCustomer")]
        public async Task<IActionResult> AddNewCustomer(AddCustomerModel model)
        {
            return await AddItemResponseHandler(async () => await service.AddNewCustomer(model));
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(string phoneNumber)
        {
            return await AddItemResponseHandler(async () => await service.Login(phoneNumber));
        }

        [HttpPost("GetCustomerTotalNumberOfVisit/{id}")]
        public async Task<IActionResult> GetCustomerTotalNumberOfVisit(int UserId)
        {
            return await AddItemResponseHandler(async () => await service.GetCustomerTotalNumberOfVisit(UserId));
        }
        [HttpGet("GetCustomerVisitDetails/{id}")]
        public async Task<IActionResult> GetCustomerVisitDetails(int CustomerID)
        {
            return await GetResponseHandler(async () => await service.GetCustomerVisitDetails(CustomerID));
        }

    }
}
