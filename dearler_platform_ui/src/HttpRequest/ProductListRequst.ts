import { IProductInputDto, IProductPropInputDto, IShoppingCartDto } from "@/Interface/ProductList"
import axios from "axios"

export const GetProduct = async (data: IProductInputDto) => {
    var res = await axios.get("Product/GetProductDto", { params: data })
    return res.data
}
export const GetBelongType = async () => {
    var res = await axios.get("Product/GetBelongType")
    return res.data
}
export const GetType = async (sysNo: string) => {
    var res = await axios.get("Product/GetProductTypeDto?sysNo=" + sysNo)

    //想接着使用params的写法，需要写一个用于传输数据的接口，就像上面的data: IProductInputDto
    // var res = await axios.get("Product/GetProductTypeDto", { params: sysNo })
    return res.data
}
export const GetProp = async (data: IProductPropInputDto) => {
    var res = await axios.get("Product/GetProdctProps", { params: data })
    return res.data
}
export const AddCart = async (data: IShoppingCartDto) => {
    var res = await axios.post("ShoppingCart/SetShoppingCart", data)
    return res.data
}
