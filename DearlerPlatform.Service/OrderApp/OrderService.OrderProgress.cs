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
        public async Task AddProgress(string orderNo, DateTime stepTime)
        {
            SaleOrderProgress progress = new SaleOrderProgress()
            {
                ProgressGuid = Guid.NewGuid().ToString(),
                SaleOrderNo = orderNo,
                StepName = "下单",
                StepSn = 1,
                StepTime = stepTime
            };
            await OrderProgressRepo.InsertAsync(progress);
        }

        public async Task<List<SaleOrderProgress>> GetProgressByOrderNos(params string[] orderNos)
        {
            var progress = await OrderProgressRepo.GetListAsync(m => orderNos.Contains(m.SaleOrderNo));
            return progress;
        }
    }
}