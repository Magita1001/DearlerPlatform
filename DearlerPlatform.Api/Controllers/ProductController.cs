using DearlerPlatform.Service.ProductApp;
using DearlerPlatform.Service.ProductApp.Dto;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DearlerPlatform.Api.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ProductController : BaseController
    {
        public ProductController(IProductService productService)
        {
            ProductService = productService;
        }

        public IProductService ProductService { get; }

        [HttpGet]
        public async Task<List<ProductDto>> GetProductDtoAsync(string? sort)
        {
            // return (await ProductService.GetProductDto()).ToList();
            return new List<ProductDto>();
        }
    }
}