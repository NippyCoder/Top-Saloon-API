using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TopSaloon.ServiceLayer;

namespace TopSaloon.API.Extensions
{
    public static class ServiceExtensions
    {
        public static void AddBusinessServices(this IServiceCollection caller)
        {
             caller.AddScoped<UsersService>();
             caller.AddScoped<BarberService>();
            caller.AddScoped<CustomerService>();
            caller.AddScoped<QuestionFeedbackService>();


        }
    }
}
