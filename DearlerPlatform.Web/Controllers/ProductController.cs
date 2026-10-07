using DearlerPlatform.Core.Global;
using DearlerPlatform.Service.ProductApp;
using DearlerPlatform.Service.ProductApp.Dto;
using Microsoft.AspNetCore.Mvc;

namespace DearlerPlatform.Web.Controllers
{
    // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ProductController : BaseController
    {
        public ProductController(IProductService productService)
        {
            ProductService = productService;
        }

        public IProductService ProductService { get; }

        [HttpGet]
        public async Task<List<ProductDto>> GetProductDtoAsync(
        string? searchText,
        string? productType,
        string sysNo,
        string? productProps,
        string? sort,
        int pageIndex = 1,
        int pageSize = 30,
        OrderType orderType = OrderType.Acs)
        {
            Dictionary<string, string> dicProductProps = new Dictionary<string, string>();
            // if (productProps != null)
            // {
            var arrProductProps = productProps?.Split("^") ?? [];
            foreach (var item in arrProductProps)
            {
                var key = item.Split("_")[0];
                var value = item.Split("_")[1];
                dicProductProps.Add(key, value);
            }
            // }

            sort ??= "ProductName";
            return (await ProductService.GetProductDto(searchText, productType, sysNo, dicProductProps, new PageWithSortDto
            {
                Sort = sort,
                PageIndex = pageIndex,
                PageSize = pageSize,
                OrderType = orderType
            })).ToList();
        }

        [HttpGet]
        public async Task<IEnumerable<ProductTypeDto>> GetProductTypeDtoAsync(string sysNo)
        {
            return await ProductService.GetProductType(sysNo);
        }

        [HttpGet]
        public async Task<Dictionary<string, IEnumerable<string>>> GetProdctProps(string? typeNo, string sysNo = "1")
        {
            return await ProductService.GetProdctProps(sysNo, typeNo);
        }

        [HttpGet]
        public async Task<List<BelongTypeDto>> GetBelongType()
        {
            return await ProductService.GetBelongTypeDto();
        }
    }
}