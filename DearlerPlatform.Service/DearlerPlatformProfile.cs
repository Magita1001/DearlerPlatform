using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DearlerPlatform.Domain;
using DearlerPlatform.Service.CustomerApp.Dto;
using DearlerPlatform.Service.OrderApp.Dto;
using DearlerPlatform.Service.ProductApp.Dto;
using DearlerPlatform.Service.ShoppingCartApp.Dto;
using Newtonsoft.Json;

namespace DearlerPlatform.Service
{
    public class DearlerPlatformProfile : Profile
    {
        public DearlerPlatformProfile()
        {
            CreateMap<Product, ProductDto>().ReverseMap();
            //映射时忽略 Id 字段
            // CreateMap<ProductSale, ProductDto>().ForMember(dest => dest.Id, opt => opt.Ignore()).ReverseMap();
            // CreateMap<ProductPhoto, ProductDto>().ForMember(dest => dest.Id, opt => opt.Ignore()).ReverseMap();
            // CreateMap<ProductSaleAreaDiff, ProductDto>().ForMember(dest => dest.Id, opt => opt.Ignore()).ReverseMap();
            CreateMap<ProductSale, ProductDto>().ReverseMap();
            CreateMap<ProductPhoto, ProductDto>().ReverseMap();
            CreateMap<ProductSaleAreaDiff, ProductDto>().ReverseMap();
            CreateMap<ShoppingCart, ShoppingCartInputDto>().ReverseMap();
            CreateMap<ShoppingCart, ShoppingCartDto>().ReverseMap();
            CreateMap<CustomerInvoice, InvoiceOfOrderConfirmDto>().ReverseMap();
            CreateMap<SaleOrderMaster, SaleOrderDto>().ReverseMap();

            //将 ProductDto 中的引用类型在向Cto转换时 序列化成json
            CreateMap<ProductDto, ProductCto>()
            .ForMember(cto => cto.ProductPhoto, dto => dto.MapFrom(dto => JsonConvert.SerializeObject(dto.ProductPhoto)))
            .ForMember(cto => cto.ProductSale, dto => dto.MapFrom(dto => JsonConvert.SerializeObject(dto.ProductSale)));
            //CTO 转 DTO
            CreateMap<ProductCto, ProductDto>()
            .ForMember(cto => cto.ProductPhoto, dto => dto.MapFrom(dto => JsonConvert.DeserializeObject<ProductPhoto>(dto.ProductPhoto)))
            .ForMember(cto => cto.ProductSale, dto => dto.MapFrom(dto => JsonConvert.DeserializeObject<ProductSale>(dto.ProductSale)));

        }
    }
}