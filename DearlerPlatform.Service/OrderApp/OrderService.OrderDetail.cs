using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DearlerPlatform.Domain;
using DearlerPlatform.Service.ShoppingCartApp.Dto;

namespace DearlerPlatform.Service.OrderApp
{
    public partial class OrderService
    {
        public async Task AddOrderDetail(List<ShoppingCartDto> carts,
         string customerNo, string orderNo, DateTime inputTime)
        {
            foreach (var item in carts)
            {
                SaleOrderDetail detail = new SaleOrderDetail()
                {
                    SaleOrderGuid = Guid.NewGuid().ToString(),
                    SaleOrderNo = orderNo,
                    ProductNo = item.ProductNo,
                    ProductName = item.ProductDto.ProductName,
                    ProductPhotoUrl = item.ProductDto.ProductPhoto?.ProductPhotoUrl,
                    CustomerNo = customerNo,
                    InputDate = inputTime,
                    OrderNum = item.ProductNum,
                    BasePrice = item.ProductDto.ProductSale?.SalePrice ?? 0,
                    DiffPrice = 0,
                    SalePrice = item.ProductDto.ProductSale?.SalePrice ?? 0
                };
                await OrderDetailRepo.InsertAsync(detail);
            }
        }

        public async Task<List<SaleOrderDetail>> GetOrderDetailsByOrderNo(string orderNo)
        {
            return await OrderDetailRepo.GetListAsync(m => m.SaleOrderNo == orderNo);
        }
    }
}