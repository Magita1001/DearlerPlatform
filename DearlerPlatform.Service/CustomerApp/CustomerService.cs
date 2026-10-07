using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DearlerPlatform.Common.EventBusHelper;
using DearlerPlatform.Core.Repository;
using DearlerPlatform.Domain;
using DearlerPlatform.Service.CustomerApp.Dto;
using DearlerPlatform.Service.OrderApp.Dto;

namespace DearlerPlatform.Service.CustomerApp
{
    //使用部分类来对所有Customer类进行操作
    public partial class CustomerService : ICustomerService
    {
        public CustomerService(
            IRepository<Customer> customerRepo,
            IRepository<CustomerInvoice> customerInvoiceRepo,
            IRepository<CustomerPwd> customerPwdRepo,
            IMapper mapper,
            LocalEventBus<SaleOrderDto> saleOrderDtoLocalEventBus)
        {
            //Repo -> Repository -> 仓储
            CustomerRepo = customerRepo;
            CustomerInvoiceRepo = customerInvoiceRepo;
            CustomerPwdRepo = customerPwdRepo;
            Mapper = mapper;

            saleOrderDtoLocalEventBus.localEventHandler+= LocalEventHandler;
        }


        public IRepository<Customer> CustomerRepo { get; }
        public IRepository<CustomerInvoice> CustomerInvoiceRepo { get; }
        public IRepository<CustomerPwd> CustomerPwdRepo { get; }
        public IMapper Mapper { get; }

        public async Task<Customer> GetCustomer(string customerNo)
        {
            return await CustomerRepo.GetAsync(m=>m.CustomerNo==customerNo);
        }
    }
}