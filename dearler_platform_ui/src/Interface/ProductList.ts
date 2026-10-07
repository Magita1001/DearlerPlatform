export interface IProductInputDto {
    searchText: string | null,
    sysNo: string,
    productType: string | null,
    productProps: string | null,
    sort: string,
    pageIndex: number,
}

export interface IProductPropInputDto {
    sysNo: string,
    typeNo: string
}

export interface IProductInfo {
    systemIndex: string,
    searchText: string,
    products: IProduct[],
    belongTypes: IBelongType[],
    productTypes: IProductType[],
    productProps: any,
    propSelect: any,
    typeSelected: string | null,
    timer: number
    pageIndex: number

    getProducts(sysNo: string, productType?: string | null, searchText?: string | null, propductProp?: string | null): void,

    //正式的 在接口中 声明函数的两种语法
    //属性语法：不支持重载，更严格的类型检查
    // transPrice: (price: number) => string;
    //方法语法：允许重载，宽松的类型检查
    // transPrice(price: number): string,

    getBelongTypes(): void,
    getType(sysNo: string): void,
    selectSystemProduct(index: string): void,
    getProps(sysNo: string, typeNo: string | null): void,

    selectType(typeNo: string): void,
    getPropKey(key: string | number, index: number | null): string,

    search(): void
    selectProp(propKey: string, propValue: string): void
    confirmFilter(): void,
    onAddCart(productNo: string, productNum: number): void
}

export interface IProduct {
    id: number;
    sysNo: string;
    productNo: string;
    productName: string;
    typeNo: string;
    typeName: string;
    productPp: string;
    productXh: string;
    productCz: string;
    productHb: string;
    productHd: string;
    productGy: string;
    productHs: string;
    productMc: string;
    productDj: string;
    productCd: string;
    productGg: string;
    productYs: string;
    unitNo: string;
    unitName: string;
    productNote: string;
    productBzgg: string;
    belongTypeNo: string;
    belongTypeName: string;
    productPhoto: IProductPhoto;
    productSale: IProductSale;
}
export interface IProductPhoto {
    id: number;
    sysNo: string;
    productNo: string;
    productPhotoUrl: string;
}
export interface IProductSale {
    id: number;
    sysNo: string;
    productNo: string;
    stockNo: string;
    salePrice: number;
}
export interface IBelongType {
    sysNo: string;
    belongTypeName: string;
}
export interface IProductType {
    typeNo: string;
    productTypeName: string;
}
export interface IShoppingCartDto {
    customerNo: string,
    productNo: string,
    productNum: number
}
