using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DearlerPlatform.Domain;
using DearlerPlatform.Service.OrderApp.Dto;
using DearlerPlatform.Service.ShoppingCartApp.Dto;

namespace DearlerPlatform.Service.OrderApp
{
    public interface IOrderService : IocTag
    {
        Task<bool> AddOrder(string customerNo, OrderMasterInputDto input, List<ShoppingCartDto> carts);
        Task AddOrderDetail(List<ShoppingCartDto> carts,
         string customerNo, string orderNo, DateTime inputTime);
        Task AddProgress(string orderNo, DateTime stepTime);
        Task<SaleOrderDto> GetOrderInfoByOrderNo(string orderNo);
        Task<bool> BuyAgain(string saleOrderNo);
    }
}