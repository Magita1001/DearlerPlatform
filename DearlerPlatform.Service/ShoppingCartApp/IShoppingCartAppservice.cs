using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DearlerPlatform.Core.Repository;
using DearlerPlatform.Domain;
using DearlerPlatform.Service.ShoppingCartApp.Dto;

namespace DearlerPlatform.Service.ShoppingCartApp
{
    public interface IShoppingCartAppservice : IocTag
    {
        Task<ShoppingCart> SetShoppingCart(ShoppingCartInputDto input);
        Task<List<ShoppingCartDto>> GetShoppingCartDto(string customerNo);
        Task<(string, ShoppingCart)> UpdateCartSelected(ShoppingCartSelectedEditDto edit, string customerNo);
        Task<int> GetShoppingCartNum(string customerNo);
    }
}