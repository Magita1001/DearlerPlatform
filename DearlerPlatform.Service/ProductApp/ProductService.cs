using System.Linq.Dynamic.Core;
using AutoMapper;
using DearlerPlatform.Common.EventBusHelper;
using DearlerPlatform.Common.RedisModule;
using DearlerPlatform.Core;
using DearlerPlatform.Core.Consts;
using DearlerPlatform.Core.Global;
using DearlerPlatform.Core.Repository;
using DearlerPlatform.Domain;
using DearlerPlatform.Service.ProductApp.Dto;
using DearlerPlatform.Service.ShoppingCartApp.Dto;

namespace DearlerPlatform.Service.ProductApp
{
    public partial class ProductService : IProductService
    {
        public ProductService(
            IRepository<Product> productRepo,
            IRepository<ProductSale> productSaleRepo,
            IRepository<ProductSaleAreaDiff> productSaleAreaDiffRepo,
            IRepository<ProductPhoto> productPhotoRepo,

            IMapper mapper,
            DealerPlatformContext context,
            LocalEventBus<List<ShoppingCartDto>> localEvnetBusShoppingCartDto,
            IRedisWorker redisWorker
            )
        {
            ProductRepo = productRepo;
            ProductSaleRepo = productSaleRepo;
            ProductSaleAreaDiffRepo = productSaleAreaDiffRepo;
            ProductPhotoRepo = productPhotoRepo;
            Mapper = mapper;
            Context = context;
            RedisWorker = redisWorker;
            localEvnetBusShoppingCartDto.localEventHandler += LocalEventHandler;
        }
        //触发事件，根据ProductNo寻找product
        public async Task LocalEventHandler(List<ShoppingCartDto> dtos)
        {
            var nos = dtos.Select(d => d.ProductNo);

            //22-3 更改为试着先从 Redis 中获取
            // var productDtos = await GetProductByProductNos(nos.ToArray());
            var productDtos = await GetProductByProductNosInCache(nos.ToArray());

            dtos.ForEach(dtos =>
            {
                var productDto = productDtos.FirstOrDefault(m => m.ProductNo == dtos.ProductNo);
                dtos.ProductDto = productDto;
            });
        }
        public IRepository<Product> ProductRepo { get; }
        public IRepository<ProductSale> ProductSaleRepo { get; }
        public IRepository<ProductSaleAreaDiff> ProductSaleAreaDiffRepo { get; }
        public IRepository<ProductPhoto> ProductPhotoRepo { get; }
        public IMapper Mapper { get; }
        public DealerPlatformContext Context { get; }
        public IRedisWorker RedisWorker { get; }

        public async Task<IEnumerable<ProductDto>> GetProductDto(string searchText, string productType, string sysNo, Dictionary<string, string> productProps, PageWithSortDto pageWithSortDto)
        {
            pageWithSortDto.Sort ??= "ProductName";

            // int skipNum = (pageIndex - 1) * pageSize;

            #region 其他查询写法
            //Linq 查询语法写法
            // var products = (from p in await ProductRepo.GetListAsync()
            //                 orderby p.GetType().GetProperty(sort).GetValue(p)  //倒叙查找在这里加一个 descending
            //                 select p).Skip(skipNum).Take(pageSize).ToList();
            //方法语法
            // var products = (await ProductRepo.GetListAsync())
            //                 .OrderBy(p => p.GetType().GetProperty(sort).GetValue(p)).Skip(skipNum).Take(pageSize);

            // var products = await ProductRepo.GetListAsync(pageWithSortDto);
            #endregion

            // 筛选属性
            // var bzgg = productProps.ContainsKey("ProductBzgg") ? productProps["ProductBzgg"] : null;
            // var bzgg = productProps.TryGetValue("ProductBzgg",out var bzgg );
            var bzgg = productProps.GetValueOrDefault("ProductBzgg");
            var cd = productProps.GetValueOrDefault("ProductCd");
            var cz = productProps.GetValueOrDefault("ProductCz");
            var dj = productProps.GetValueOrDefault("ProductDj");
            var gg = productProps.GetValueOrDefault("ProductGg");
            var gy = productProps.GetValueOrDefault("ProductGy");
            var hb = productProps.GetValueOrDefault("ProductHb");
            var hd = productProps.GetValueOrDefault("ProductHd");
            var hs = productProps.GetValueOrDefault("ProductHs");
            var mc = productProps.GetValueOrDefault("ProductMc");
            var pp = productProps.GetValueOrDefault("ProductPp");
            var xh = productProps.GetValueOrDefault("ProductXh");
            var ys = productProps.GetValueOrDefault("ProductYs");

            var skip = (pageWithSortDto.PageIndex - 1) * pageWithSortDto.PageSize;

            var products = ProductRepo.GetQueryble()
               .Where(m => m.SysNo.ToLower() == sysNo.ToLower()
                && (m.TypeNo == productType || string.IsNullOrWhiteSpace(productType))
                && (m.ProductName.Contains(searchText) || string.IsNullOrWhiteSpace(searchText))
                && (bzgg == null || m.ProductBzgg == bzgg)
                && (cd == null || m.ProductCd == cd)
                && (cz == null || m.ProductCz == cz)
                && (dj == null || m.ProductDj == dj)
                && (gg == null || m.ProductGg == gg)
                && (gy == null || m.ProductGy == gy)
                && (hb == null || m.ProductHb == hb)
                && (hd == null || m.ProductHd == hd)
                && (hs == null || m.ProductHs == hs)
                && (mc == null || m.ProductMc == mc)
                && (pp == null || m.ProductPp == pp)
                && (xh == null || m.ProductXh == xh)
                && (ys == null || m.ProductYs == ys)
               )
               .OrderBy(pageWithSortDto.Sort).Skip(skip).Take(pageWithSortDto.PageSize);


            //TODO:领域模型 转 视图模型      
            var dtos = Mapper.Map<List<ProductDto>>(products);
            var productPhotos = await GetProductPhotosByProductNo(products.Select(m => m.ProductNo).ToArray());
            var productSales = await GetProductSalesByProductNo(products.Select(m => m.ProductNo).ToArray());

            dtos.ForEach(p =>
            {
                p.ProductPhoto = productPhotos.FirstOrDefault(m => m.ProductNo == p.ProductNo);
                p.ProductSale = productSales.FirstOrDefault(m => m.ProductNo == p.ProductNo);
            });

            return dtos;
        }

        public async Task<List<BelongTypeDto>> GetBelongTypeDto()
        {
            return await Task.Run(() =>
            {
                var res = ProductRepo.GetQueryble().Select(m => new BelongTypeDto
                {
                    SysNo = m.SysNo,
                    BelongTypeName = m.BelongTypeName,
                }).Distinct().ToList();

                return res;
            });
        }

        //从数据库中取 TypeNo 和 TypeName 字段 并去重
        //如果只需要一个字段，就不需要在Select里new一个对象了
        public async Task<IEnumerable<ProductTypeDto>> GetProductType(string sysNo)
        {
            // var productType = Context.Products.Where(m => !string.IsNullOrWhiteSpace(m.TypeName)
            // && !string.IsNullOrWhiteSpace(m.TypeNo))
            // .Select(m => new ProductTypeDto
            // {
            //     TypeNo = m.TypeNo,
            //     ProductTypeName = m.TypeName
            // }).Distinct().ToList();

            //9-4改造
            var productType = ProductRepo.GetQueryble()
            .Where(m => m.SysNo == sysNo && !string.IsNullOrWhiteSpace(m.TypeName)
                && !string.IsNullOrWhiteSpace(m.TypeNo))
            .Select(m => new ProductTypeDto
            {
                TypeNo = m.TypeNo,
                ProductTypeName = m.TypeName
            }).Distinct().ToList();

            return productType;
        }

        public async Task<Dictionary<string, IEnumerable<string>>> GetProdctProps(string sysNo, string typeNo)
        {
            Dictionary<string, IEnumerable<string>> dicProductType = new Dictionary<string, IEnumerable<string>>();

            var products = await ProductRepo.GetListAsync(
                m => m.SysNo == sysNo && (m.TypeNo == typeNo || string.IsNullOrWhiteSpace(typeNo)));

            dicProductType.Add("ProductBzgg|包装规格", products.Select(m => m.ProductBzgg).Distinct()
                .Where(m => !string.IsNullOrWhiteSpace(m)).ToList());

            dicProductType.Add("ProductCd|产地", products.Select(m => m.ProductCd).Distinct()
                .Where(m => !string.IsNullOrWhiteSpace(m)).ToList());

            dicProductType.Add("ProductCz|材质", products.Select(m => m.ProductCz).Distinct()
                .Where(m => !string.IsNullOrWhiteSpace(m)).ToList());

            dicProductType.Add("ProductDj|等级", products.Select(m => m.ProductDj).Distinct()
                .Where(m => !string.IsNullOrWhiteSpace(m)).ToList());

            dicProductType.Add("ProductGg|规格", products.Select(m => m.ProductGg).Distinct()
                .Where(m => !string.IsNullOrWhiteSpace(m)).ToList());

            dicProductType.Add("ProductGy|工艺", products.Select(m => m.ProductGy).Distinct()
                .Where(m => !string.IsNullOrWhiteSpace(m)).ToList());

            dicProductType.Add("ProductHb|环保", products.Select(m => m.ProductHb).Distinct()
                .Where(m => !string.IsNullOrWhiteSpace(m)).ToList());

            dicProductType.Add("ProductHd|厚度", products.Select(m => m.ProductHd).Distinct()
                .Where(m => !string.IsNullOrWhiteSpace(m)).ToList());

            dicProductType.Add("ProductHs|花色", products.Select(m => m.ProductHs).Distinct()
                .Where(m => !string.IsNullOrWhiteSpace(m)).ToList());

            dicProductType.Add("ProductMc|面材", products.Select(m => m.ProductMc).Distinct()
                .Where(m => !string.IsNullOrWhiteSpace(m)).ToList());

            dicProductType.Add("ProductPp|品牌", products.Select(m => m.ProductPp).Distinct()
                .Where(m => !string.IsNullOrWhiteSpace(m)).ToList());

            dicProductType.Add("ProductXh|型号", products.Select(m => m.ProductXh).Distinct()
                .Where(m => !string.IsNullOrWhiteSpace(m)).ToList());

            dicProductType.Add("ProductYs|颜色", products.Select(m => m.ProductYs).Distinct()
                .Where(m => !string.IsNullOrWhiteSpace(m)).ToList());

            return dicProductType;
        }
        private static object lockObj = new object();
        public async Task<List<ProductDto>> GetProductByProductNosInCache(params string[] strProductNos)
        {
            List<ProductCto> ctos = new List<ProductCto>();

            //脱离 SCAN 的模糊操作方式 ，直接使用foreach遍历
            foreach (var productNos in strProductNos)
            {
                //试着从Redis中获取值
                var res = RedisWorker.GetHashMemory<ProductCto>($"{RedisKeyName.PRODUCT_KEY}:{productNos}").FirstOrDefault();
                //Redis中没有值时从数据库里获取,并在Redis中存一份
                if (res == null)
                {
                    #region 没有锁的方案
                    //从数据库中查找 并把DTO转换为CTO
                    // res = Mapper.Map<ProductDto, ProductCto>((await GetProductByProductNos(productNos)).FirstOrDefault());
                    // RedisWorker.SetHashMemory($"{RedisKeyName.PRODUCT_KEY}:{productNos}", res);
                    #endregion

                    //使用lock解决缓存击穿的问题 使用双if判断减少数据库压力
                    lock (lockObj)
                    {
                        res = RedisWorker.GetHashMemory<ProductCto>($"{RedisKeyName.PRODUCT_KEY}:{productNos}").FirstOrDefault();
                        if (res == null)
                        {
                            res = Mapper.Map<ProductDto, ProductCto>(GetProductByProductNos(productNos).Result.FirstOrDefault());
                            RedisWorker.SetHashMemory($"{RedisKeyName.PRODUCT_KEY}:{productNos}", res);
                        } 
                    }
                }
                ctos.Add(res);
            }
            return Mapper.Map<List<ProductCto>, List<ProductDto>>(ctos);
        }
        public async Task<List<ProductDto>> GetProductByProductNos(params string[] strProductNos)
        {
            var productNos = strProductNos.Distinct();

            var products = await ProductRepo.GetListAsync(m => productNos.Contains(m.ProductNo));
            var productDtos = Mapper.Map<List<Product>, List<ProductDto>>(products);

            var ProductSales = await GetProductSalesByProductNo(productDtos.Select(m => m.ProductNo).ToArray());
            productDtos.ForEach(p =>
            {
                p.ProductSale = ProductSales.FirstOrDefault(m => m.ProductNo == p.ProductNo);
            });

            return productDtos;
        }
    }
}