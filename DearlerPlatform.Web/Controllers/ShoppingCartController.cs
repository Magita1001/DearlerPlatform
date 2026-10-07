using DearlerPlatform.Domain;
using DearlerPlatform.Service.ProductApp;
using DearlerPlatform.Service.ShoppingCartApp;
using DearlerPlatform.Service.ShoppingCartApp.Dto;
using DearlerPlatform.Web.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DearlerPlatform.Web.Controllers
{
    [Authorize]
    [CtmAuthorizationFilter]
    public class ShoppingCartController : BaseController
    {
        public ShoppingCartController(IShoppingCartAppservice shoppingCartservice, IProductService productService)
        {
            ShoppingCartservice = shoppingCartservice;
        }

        public IShoppingCartAppservice ShoppingCartservice { get; }

        [HttpPost]
        public async Task<ShoppingCart> SetShoppingCart(ShoppingCartInputDto input)
        {
            var customerNo = HttpContext.Items["CustomerNo"]?.ToString();
            input.CustomerNo = customerNo;
            var res = await ShoppingCartservice.SetShoppingCart(input);
            return res;
        }

        [HttpGet]
        public async Task<dynamic> GetShoppingCartDtosAsync()
        // public async Task<dynamic> GetShoppingCartDtosAsync(string customerNo)
        {
            var customerNo = HttpContext.Items["CustomerNo"]?.ToString();
            var carts = await ShoppingCartservice.GetShoppingCartDto(customerNo);
            var productDtos = carts.Select(m => m.ProductDto);
            var types = productDtos?.Select(m => new { m?.TypeNo, m?.TypeName, TypeSelected = false }).Distinct();
            return new { carts, types };
        }

        [HttpPost]
        public async Task<object> UpdateCartSelected(ShoppingCartSelectedEditDto edit)
        {
            var customerNo = HttpContext.Items["CustomerNo"]?.ToString();
            var (isSuccess, data) = await ShoppingCartservice.UpdateCartSelected(edit, customerNo);
            return new { isSuccess, data };
        }
        [HttpGet]
        public async Task<int> GetShoppingCartNum()
        // public async Task<int> GetShoppingCartNum(string customerNo)
        {
            var customerNo = HttpContext.Items["CustomerNo"]?.ToString();
            return await ShoppingCartservice.GetShoppingCartNum(customerNo);
        }
    }
}