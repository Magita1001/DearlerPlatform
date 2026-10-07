import axios from "axios"

// export const GetShoppingCarts = async (customerNo: string) => {
//     var res = await axios.get("ShoppingCart/GetShoppingCartDtos", { params: { customerNo } })
//     return res.data
// }
export const GetShoppingCarts = async () => {
    var res = await axios.get("ShoppingCart/GetShoppingCartDtos")
    return res.data
}
export const UpdateCartSelected = async (cartGuids: string[], CartSelected: boolean, productNum: number) => {
    var res = await axios.post("ShoppingCart/UpdateCartSelected", { cartGuids, CartSelected, productNum })
    return res.data
}