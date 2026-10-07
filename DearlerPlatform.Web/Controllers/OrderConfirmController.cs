using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DearlerPlatform.Core.Consts;
using DearlerPlatform.Core.Repository;
using DearlerPlatform.Service.OrderApp;
using DearlerPlatform.Service.OrderApp.Dto;
using DearlerPlatform.Service.ProductApp;
using DearlerPlatform.Service.ShoppingCartApp;
using DearlerPlatform.Service.ShoppingCartApp.Dto;
using DearlerPlatform.Web.Filters;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DearlerPlatform.Web.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [CtmAuthorizationFilter]
    public class OrderConfirmController : BaseController
    {
        public OrderConfirmController(IShoppingCartAppservice shoppingCartAppservice,
        IProductService productService, IOrderService orderService)
        {
            ShoppingCartAppservice = shoppingCartAppservice;
            ProductService = productService;
            OrderService = orderService;
        }
        public IShoppingCartAppservice ShoppingCartAppservice { get; }
        public IProductService ProductService { get; }
        public IOrderService OrderService { get; }


        /// <summary>
        /// 获取购物车内选中的物品
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IEnumerable<ShoppingCartDto>> Get()
        {
            var customerNo = HttpContext.Items[HttpContextItemKeyName.CUSTOMER_NO].ToString();
            var carts = (await ShoppingCartAppservice.GetShoppingCartDto(customerNo)).Where(m => m.CartSelected == true);

            return carts;
        }

        [HttpPost]
        public async Task<bool> Add(OrderMasterInputDto input)
        {
            var customerNo = HttpContext.Items[HttpContextItemKeyName.CUSTOMER_NO].ToString();
            var carts = (await ShoppingCartAppservice.GetShoppingCartDto(customerNo)).Where(m => m.CartSelected == true);

            return await OrderService.AddOrder(customerNo, input, carts.ToList());
        }
    }
}