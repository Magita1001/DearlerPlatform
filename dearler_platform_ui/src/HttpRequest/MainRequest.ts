import axios from "axios"

// export const GetCartNum = async (customerNo: string) => {
//     var res = await axios.get("ShoppingCart/GetShoppingCartNum", { params: { customerNo } })
//     return res.data
// }
export const GetCartNum = async () => {
    var res = await axios.get("ShoppingCart/GetShoppingCartNum")
    return res.data
}
