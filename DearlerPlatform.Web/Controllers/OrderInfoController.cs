using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DearlerPlatform.Core.Consts;
using DearlerPlatform.Service.CustomerApp;
using DearlerPlatform.Service.OrderApp;
using DearlerPlatform.Service.OrderApp.Dto;
using DearlerPlatform.Web.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DearlerPlatform.Web.Controllers
{
    [Authorize]
    [CtmAuthorizationFilter]
    public class OrderInfoController : BaseController
    {
        public OrderInfoController(IOrderService orderService, ICustomerService customerService)
        {
            OrderService = orderService;
            CustomerService = customerService;
        }

        public IOrderService OrderService { get; }
        public ICustomerService CustomerService { get; }

        [HttpGet]
        public async Task<SaleOrderDto> GetSaleOrderDto(string orderNo)
        {
            // var customerNo = HttpContext.Items[HttpContextItemKeyName.CUSTOMER_NO];
            var orderDto = await OrderService.GetOrderInfoByOrderNo(orderNo);
            return orderDto;
        }
        [HttpGet]
        public async Task<bool> BuyAgain(string orderGuid)
        {
            var res = await OrderService.BuyAgain(orderGuid);
            return res;
        }
    }
}