using DearlerPlatform.Common;
using DearlerPlatform.Common.TokenModule.Model;
using DearlerPlatform.Service.CustomerApp;
using DearlerPlatform.Service.CustomerApp.Dto;
using Microsoft.AspNetCore.Mvc;

namespace DearlerPlatform.Web.Controllers
{
    public class LoginController : BaseController
    {
        public LoginController(ICustomerService customerService, IConfiguration configuration)
        {
            CustomerService = customerService;
            Configuration = configuration;
        }

        public ICustomerService CustomerService { get; }
        public IConfiguration Configuration { get; }

        [HttpPost]
        public async Task<string> CheckLogin(CustomerLoginDto dto)
        {
            //对前端数据判空
            if (string.IsNullOrWhiteSpace(dto.CustomerNo) && string.IsNullOrWhiteSpace(dto.Password))
            {
                HttpContext.Response.StatusCode = 400;
                return "NonLoginInfo";
            }

            //验证密码并返回Token
            var isSuccess = await CustomerService.CheckPassword(dto);
            if (isSuccess)
            {
                //TODO:获取真实用户数据
                var customer = await CustomerService.GetCustomer(dto.CustomerNo);
                return GetToken(customer.Id, customer.CustomerNo, customer.CustomerName);
            }
            else
            {
                HttpContext.Response.StatusCode = 403;
                return "NonUser";
            }
        }
        private string GetToken(int userId, string customerNo, string customerName)
        {
            var token = Configuration.GetSection("Jwt").Get<JwtTokenModel>();
            token.Id = userId;
            token.CustomerNo = customerNo;
            token.CustomerName = customerName;

            return TokenHelper.CreatToken(token);
        }
    }
}