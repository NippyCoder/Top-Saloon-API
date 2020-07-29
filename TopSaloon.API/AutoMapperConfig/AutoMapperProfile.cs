using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TopSaloon.DTOs.Models;
using TopSaloon.Entities.Models;

namespace TopSaloon.API.AutoMapperConfig
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<ApplicationUser, UserDTO>().ReverseMap();
            CreateMap<Administrator, AdministratorDTO>().ReverseMap();
            CreateMap<Barber, BarberDTO>().ForMember(dest => dest.Shop, opt => opt.Ignore()).ReverseMap();
            CreateMap<BarberProfilePhoto, BarberProfilePhotoDTO>().ForMember(dest => dest.Barber, opt => opt.Ignore()).ReverseMap();
            CreateMap<BarberQueue, BarberQueueDTO>().ForMember(dest => dest.Barber, opt => opt.Ignore()).ReverseMap();
            CreateMap<Customer, CustomerDTO>().ReverseMap();
            CreateMap<DailyReport, DailyReportDTO>().ReverseMap();
            CreateMap<Order, OrderDTO>()
                .ForMember(dest => dest.BarberQueue, opt => opt.Ignore())
                .ForMember(dest => dest.Customer, opt => opt.Ignore())
                .ReverseMap();
            CreateMap<OrderFeedback, OrderFeedbackDTO>().ForMember(dest => dest.Order, opt => opt.Ignore()).ReverseMap();
            CreateMap<OrderFeedbackQuestion, OrderFeedbackQuestionDTO>().ForMember(dest => dest.OrderFeedback, opt => opt.Ignore()).ReverseMap();
            CreateMap<OrderService, OrderServiceDTO>().ForMember(dest => dest.Order, opt => opt.Ignore()).ReverseMap();
            CreateMap<Service, ServiceDTO>().ReverseMap();
            CreateMap<ServiceFeedBackQuestion, ServiceFeedbackQuestionDTO>().ForMember(dest => dest.Service, opt => opt.Ignore()).ReverseMap();
            CreateMap<Shop, ShopDTO>().ReverseMap();
            CreateMap<SMS, SMSDTO>().ReverseMap();
            CreateMap<Barber, BarberDTO>().ForMember(dest => dest.Shop, opt=> opt.Ignore()).ReverseMap();
            CreateMap<Order, OrderDTO>().ForMember(dest => dest.OrderFeedback, opt => opt.Ignore()).ReverseMap();
            CreateMap<OrderService, OrderServiceDTO>().ForMember(dest => dest.Order, opt => opt.Ignore()).ReverseMap();
            CreateMap<OrderFeedback, OrderFeedbackDTO>().ForMember(dest => dest.Order, opt => opt.Ignore()).ReverseMap();

        }
    }
}
