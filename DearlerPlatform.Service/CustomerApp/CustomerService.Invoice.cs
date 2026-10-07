using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DearlerPlatform.Core.Repository;
using DearlerPlatform.Domain;
using DearlerPlatform.Service.CustomerApp.Dto;
using DearlerPlatform.Service.OrderApp.Dto;

namespace DearlerPlatform.Service.CustomerApp
{
    public partial class CustomerService
    {
        //事件总线
        private async Task LocalEventHandler(SaleOrderDto t)
        {
            t.CustomerInvoice = await GetCustomerInvoiceByInvoiceNo(t.InvoiceNo);
        }

        public async Task<List<InvoiceOfOrderConfirmDto>> GetInvoicesByCustomerNo(string customerNo)
        {
            var invoices = await CustomerInvoiceRepo.GetListAsync(m => m.CustomerNo == customerNo);

            return Mapper.Map<List<CustomerInvoice>, List<InvoiceOfOrderConfirmDto>>(invoices);
        }

        public async Task<CustomerInvoice> GetCustomerInvoiceByInvoiceNo(string invoiceNo)
        {
            return await CustomerInvoiceRepo.GetAsync(m => m.InvoiceNo == invoiceNo);
        }
    }
}