using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DearlerPlatform.Core.Consts;
using DearlerPlatform.Service.CustomerApp;
using DearlerPlatform.Service.CustomerApp.Dto;
using DearlerPlatform.Web.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DearlerPlatform.Web.Controllers
{
    [Authorize]
    [CtmAuthorizationFilter]
    public class CustomerController : BaseController
    {
        public CustomerController(ICustomerService customerService)
        {
            CustomerService = customerService;
        }

        public ICustomerService CustomerService { get; }

        [HttpGet] 
        public async Task<List<InvoiceOfOrderConfirmDto>> GetInvoice()
        {
            var customerNo = HttpContext.Items[HttpContextItemKeyName.CUSTOMER_NO]?.ToString();
            var invoices = await CustomerService.GetInvoicesByCustomerNo(customerNo);

            return invoices;
        }
    }
}