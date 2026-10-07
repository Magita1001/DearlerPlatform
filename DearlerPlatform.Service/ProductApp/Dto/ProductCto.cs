using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DearlerPlatform.Domain;

namespace DearlerPlatform.Service.ProductApp.Dto
{
    /// <summary>
    /// Cto(Catch) 代表存到Redis中的数据
    /// </summary>
    public class ProductCto
    {
        public int Id { get; set; }
        public string SysNo { get; set; }
        public string ProductNo { get; set; }
        public string ProductName { get; set; }
        public string TypeNo { get; set; }
        public string TypeName { get; set; }
        public string ProductPp { get; set; }
        public string ProductXh { get; set; }
        public string ProductCz { get; set; }
        public string ProductHb { get; set; }
        public string ProductHd { get; set; }
        public string ProductGy { get; set; }
        public string ProductHs { get; set; }
        public string ProductMc { get; set; }
        public string ProductDj { get; set; }
        public string ProductCd { get; set; }
        public string ProductGg { get; set; }
        public string ProductYs { get; set; }
        public string UnitNo { get; set; }
        public string UnitName { get; set; }
        public string ProductNote { get; set; }
        public string ProductBzgg { get; set; }
        public string BelongTypeNo { get; set; }
        public string BelongTypeName { get; set; }

        // public ProductPhoto ProductPhoto { get; set; }
        // public ProductSale ProductSale { get; set; }
        //把这两个复杂类型序列化再存到Redis中
        public string ProductPhoto { get; set; }
        public string ProductSale { get; set; }
    }
}