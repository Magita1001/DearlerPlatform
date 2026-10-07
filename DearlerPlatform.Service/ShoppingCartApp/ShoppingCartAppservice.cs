using AutoMapper;
using DearlerPlatform.Common.EventBusHelper;
using DearlerPlatform.Common.RedisModule;
using DearlerPlatform.Core;
using DearlerPlatform.Core.Repository;
using DearlerPlatform.Domain;
using DearlerPlatform.Service.ShoppingCartApp.Dto;
using Microsoft.EntityFrameworkCore;

namespace DearlerPlatform.Service.ShoppingCartApp
{


    public partial class ShoppingCartAppservice : IShoppingCartAppservice
    {
        public ShoppingCartAppservice(
            IRepository<ShoppingCart> cartRepo,
            IMapper mapper,
            LocalEventBus<List<ShoppingCartDto>> localEvnetBusShoppingCartDto,
            DealerPlatformContext context,
            IRedisWorker redisWorker)
        {
            CartRepo = cartRepo;
            Mapper = mapper;
            LocalEvnetBusShoppingCartDto = localEvnetBusShoppingCartDto;
            Context = context;
            RedisWorker = redisWorker;
        }

        public IRepository<ShoppingCart> CartRepo { get; }
        public IMapper Mapper { get; }
        public LocalEventBus<List<ShoppingCartDto>> LocalEvnetBusShoppingCartDto { get; }
        public DealerPlatformContext Context { get; }
        public IRedisWorker RedisWorker { get; }

        public async Task<ShoppingCart> SetShoppingCart(ShoppingCartInputDto input)
        {
            ShoppingCart shoppingCartRes = null;

            // var shoppingCartUpdate = await CartRepo.GetAsync(m => m.ProductNo == input.ProductNo);
            var shoppingCart =
            RedisWorker.GetHashMemory<ShoppingCart>($"cart:*:{input.CustomerNo}")
            .FirstOrDefault(m => m.ProductNo == input.ProductNo);

            if (shoppingCart != null)
            {
                shoppingCart.ProductNum++;
                //21-4 改为从 redis 读取数据
                // shoppingCartRes = await CartRepo.UpdateAsync(shoppingCartUpdate);
                //redis没有更新一说，key相同就可以直接覆盖
                RedisWorker.SetHashMemory($"cart:{shoppingCart.CartGuid}:{shoppingCart.CustomerNo}", shoppingCart);
            }
            else
            {
                var tempShoppingCart = Mapper.Map<ShoppingCartInputDto, ShoppingCart>(input);

                tempShoppingCart.CartGuid = Guid.NewGuid().ToString();
                tempShoppingCart.CartSelected = true;

                //21-4 改为向 redis 里存入数据
                // shoppingCartRes = await CartRepo.InsertAsync(shoppingCart);
                RedisWorker.SetHashMemory($"cart:{tempShoppingCart.CartGuid}:{tempShoppingCart.CustomerNo}", tempShoppingCart);
            }


            return shoppingCartRes;
        }

        /// <summary>
        /// 根据 customerNo 从 Redis 里获取 购物车内的物品
        /// </summary>
        /// <param name="customerNo"></param>
        /// <returns></returns>
        public async Task<List<ShoppingCartDto>> GetShoppingCartDto(string customerNo)
        {
            //改为从 Redis 里获取
            // var cart = await CartRepo.GetListAsync(m => m.CustomerNo == customerNo);
            var cart = RedisWorker.GetHashMemory<ShoppingCart>($"cart:*:{customerNo}");

            var dtos = Mapper.Map<List<ShoppingCart>, List<ShoppingCartDto>>(cart);

            // 从事件总线里 获取 Product 物品详情信息
            await LocalEvnetBusShoppingCartDto.Publish(dtos);

            return dtos;
        }

        /// <summary>
        /// 定向更新发生变化的字段
        /// </summary>
        /// <param name="cartGuid"></param>
        /// <param name="cartSelected"></param>
        /// <returns></returns>
        public async Task<(string, ShoppingCart)> UpdateCartSelected(ShoppingCartSelectedEditDto edit, string customerNo)
        {
            #region 原方案 于22-2 更改
            // try
            // {
            //     bool isSuccess = false;
            //     ShoppingCart cart = new ShoppingCart();
            //     foreach (var cartGuid in edit.CartGuids)
            //     {
            //         cart = new ShoppingCart
            //         {
            //             CartGuid = cartGuid,
            //             CartSelected = edit.CartSelected,
            //             ProductNum = edit.ProductNum
            //         };

            //         Context.Attach(cart);
            //         Context.Entry(cart).Property(m => m.CartSelected).IsModified = true;
            //         Context.Entry(cart).Property(m => m.ProductNum).IsModified = true;
            //     }
            //     isSuccess = await Context.SaveChangesAsync() > 0;
            //     await Context.Entry(cart).ReloadAsync();
            //     return (isSuccess, cart);

            // }
            // catch (System.Exception)
            // {
            //     return (false, new ShoppingCart());
            //     throw;
            // }
            #endregion

            //购物车数量小于0时删除数据
            if (edit.ProductNum <= 0)
            {
                RedisWorker.RemoveKey($"cart:{edit.CartGuids[0]}:{customerNo}");
                return ("Remove", new ShoppingCart());
            }

            var shoppingCart =
            RedisWorker.GetHashMemory<ShoppingCart>($"cart:{edit.CartGuids[0]}:*")
            .FirstOrDefault();

            shoppingCart.CartSelected = edit.CartSelected;
            shoppingCart.ProductNum = edit.ProductNum;

            RedisWorker.SetHashMemory($"cart:{edit.CartGuids[0]}:{customerNo}", shoppingCart);

            var res = RedisWorker.GetHashMemory<ShoppingCart>($"cart:{edit.CartGuids[0]}:{customerNo}")[0];
            return ("Update", res);
        }

        /// <summary>
        /// 获取购物车数量
        /// </summary>
        /// <param name="customerNo"></param>
        /// <returns></returns>
        public async Task<int> GetShoppingCartNum(string customerNo)
        {
            //21-4 改为从Redis里获取购物车数量
            // var carts = await CartRepo.GetListAsync(m => m.CustomerNo == customerNo && m.CartSelected);
            var carts = RedisWorker.GetHashMemory<ShoppingCart>($"cart:*:{customerNo}");

            var currentCartNum = 0;
            foreach (var item in carts)
            {
                currentCartNum += item.ProductNum;
            }
            return currentCartNum;
        }
    }
}