using DearlerPlatform.Core.Global;
using DearlerPlatform.Domain;
using DearlerPlatform.Service.ProductApp.Dto;

namespace DearlerPlatform.Service.ProductApp
{
    public interface IProductService : IocTag
    {
        // Task<IEnumerable<ProductDto>> GetProductDto(string Sort = null, int pageIndex = 1, int pageSize = 30);
        Task<IEnumerable<ProductDto>> GetProductDto(string searchText, string productType, string sysNo, Dictionary<string, string> dicProductProps, PageWithSortDto pageWithSortDto);

        Task<List<ProductSale>> GetProductSalesByProductNo(params string[] productNos);
        Task<List<ProductPhoto>> GetProductPhotosByProductNo(params string[] productNos);


        Task<IEnumerable<ProductTypeDto>> GetProductType(string belongTypeName);
        Task<List<BelongTypeDto>> GetBelongTypeDto();
        Task<Dictionary<string, IEnumerable<string>>> GetProdctProps(string belongTypeName, string typeNo);
    }
}