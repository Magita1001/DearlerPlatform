using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using DearlerPlatform.Domain;
using DearlerPlatform.Service.OrderApp.Dto;
using DearlerPlatform.Service.ShoppingCartApp.Dto;

namespace DearlerPlatform.Service.OrderApp
{
    public partial class OrderService
    {
        public async Task<bool> AddOrder(string customerNo, OrderMasterInputDto input, List<ShoppingCartDto> carts)
        {
            //启动事务
            using TransactionScope ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            try
            {
                //添加主订单
                DateTime inputDate = DateTime.Now;
                string orderNo = Guid.NewGuid().ToString();

                SaleOrderMaster master = new SaleOrderMaster()
                {
                    CustomerNo = customerNo,
                    DeliveryDate = input.DeliveryDate,
                    EditUserNo = customerNo,
                    InputDate = inputDate,
                    InvoiceNo = input.Invoice,
                    Remark = input.Remark,
                    SaleOrderNo = orderNo,
                    StockNo = ""
                };

                await OrderMasterRepo.InsertAsync(master);

                //往 订单进度表里 添加状态
                await AddProgress(orderNo, inputDate);

                //添加 订单详情
                await AddOrderDetail(carts, customerNo, orderNo, inputDate);

                //提交事务
                ts.Complete();

                //删除 Redis 中的购物车数据
                foreach (var item in carts)
                {
                    RedisWorker.RemoveKey($"cart:{item.CartGuid}:{customerNo}");
                }
                return true;
            }
            catch (System.Exception)
            {
                ts.Dispose();
                throw;
            }

        }
    }
}