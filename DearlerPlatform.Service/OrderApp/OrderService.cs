using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using AutoMapper;
using DearlerPlatform.Common.EventBusHelper;
using DearlerPlatform.Common.RedisModule;
using DearlerPlatform.Core.Repository;
using DearlerPlatform.Domain;
using DearlerPlatform.Service.OrderApp.Dto;
using DearlerPlatform.Service.ShoppingCartApp.Dto;

namespace DearlerPlatform.Service.OrderApp
{
    public partial class OrderService : IOrderService
    {
        public OrderService(
            IRepository<SaleOrderMaster> saleOrderMasterRepo,
            IRepository<SaleOrderDetail> saleOrderDetailRepo,
            IRepository<SaleOrderProgress> saleOrderProgressRepo,
            IRedisWorker redisWorker,
            IMapper mapper,

            LocalEventBus<SaleOrderDto> saleOrderDtoLocalEventBus
        )
        {
            OrderMasterRepo = saleOrderMasterRepo;
            OrderDetailRepo = saleOrderDetailRepo;
            OrderProgressRepo = saleOrderProgressRepo;
            RedisWorker = redisWorker;
            Mapper = mapper;
            SaleOrderDtoLocalEventBus = saleOrderDtoLocalEventBus;
        }

        public IRepository<SaleOrderMaster> OrderMasterRepo { get; }
        public IRepository<SaleOrderDetail> OrderDetailRepo { get; }
        public IRepository<SaleOrderProgress> OrderProgressRepo { get; }
        public IRedisWorker RedisWorker { get; }
        public IMapper Mapper { get; }
        public LocalEventBus<SaleOrderDto> SaleOrderDtoLocalEventBus { get; }

        /// <summary>
        /// 获得订单详情
        /// </summary>
        /// <param name="orderNo"></param>
        /// <returns></returns>
        public async Task<SaleOrderDto> GetOrderInfoByOrderNo(string orderNo)
        {
            //获取主订单
            var orderMaster = (await OrderMasterRepo.GetListAsync(m => m.SaleOrderNo == orderNo)).FirstOrDefault();
            //转成Dto
            var saleOrderDto = Mapper.Map<SaleOrderDto>(orderMaster);
            //获取订单流程
            saleOrderDto.OrderProgress = (await GetProgressByOrderNos(saleOrderDto.SaleOrderNo)).FirstOrDefault();
            //获取订单详情
            saleOrderDto.OrderDetails = await GetOrderDetailsByOrderNo(saleOrderDto.SaleOrderNo);

            //获取开票人信息 通过事件总线 从 CustomerService 里调用方法把 invoice 装到 Dto 里
            await SaleOrderDtoLocalEventBus.Publish(saleOrderDto);

            return saleOrderDto;
        }


        public async Task<bool> BuyAgain(string saleOrderNo)
        {
            using TransactionScope ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            // try
            // {
            var currentTime = DateTime.Now;
            var newSaleOrderNo = Guid.NewGuid().ToString();

            var master = await OrderMasterRepo.GetAsync(m => m.SaleOrderNo == saleOrderNo);
            var details = await OrderDetailRepo.GetListAsync(m => m.SaleOrderNo == saleOrderNo);

            //TODO：现在的逻辑由于数据库自增列问题，数据插不进去 明天再改
            var newMaster = Mapper.Map<SaleOrderMaster>(master);
            newMaster.Id = 0;
            newMaster.SaleOrderNo = newSaleOrderNo;
            newMaster.InputDate = currentTime;
            newMaster.DeliveryDate = currentTime.AddDays(1);
            await OrderMasterRepo.InsertAsync(master);


            foreach (var item in details)
            {
                item.Id = 0;
                item.SaleOrderNo = newSaleOrderNo;
                item.SaleOrderGuid = Guid.NewGuid().ToString();
                item.InputDate = currentTime;
                await OrderDetailRepo.InsertAsync(item);
            }

            var progress = new SaleOrderProgress()
            {
                ProgressGuid = Guid.NewGuid().ToString(),
                SaleOrderNo = newSaleOrderNo,
                StepName = "下单",
                StepSn = 1,
                StepTime = currentTime
            };
            await OrderProgressRepo.InsertAsync(progress);

            ts.Complete();

            return true;
            // }
            // catch (System.Exception)
            // {
            //     ts.Dispose();
            //     return false;
            //     throw;
            // }
        }
    }
}