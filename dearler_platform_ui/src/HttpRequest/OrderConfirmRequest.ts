import axios from "./AxiosHelper"

export const GetInvoice = async () => {
    var res = await axios.get("Customer/GetInvoice")
    return res.data
}
export const GetOrderConfirmCarts = async () => {
    var res = await axios.get("OrderConfirm/Get")
    return res.data
}
export const AddOrder = async (data: any) => {
    var res = await axios.post("OrderConfirm/Add", data)
    return res.data
}
