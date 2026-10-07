using DearlerPlatform.Domain;
using DearlerPlatform.Service.CustomerApp.Dto;

namespace DearlerPlatform.Service.CustomerApp
{
    public interface ICustomerService : IocTag
    {
        Task<bool> CheckPassword(CustomerLoginDto dto);
        Task<Customer> GetCustomer(string customerNo);
        Task<List<InvoiceOfOrderConfirmDto>> GetInvoicesByCustomerNo(string customerNo);
        Task<CustomerInvoice> GetCustomerInvoiceByInvoiceNo(string invoiceNo);
    }
}