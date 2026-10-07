import axios from "./AxiosHelper"

export const GetOrderInfo = async (orderNo: string) => {
    var res = await axios.get("OrderInfo/GetSaleOrderDto?orderNo=" + orderNo)
    // var data = await axios.get("OrderInfo/GetSaleOrderDto", { params: { orderNo } })
    return res.data
}
export const BuyAgain = async (orderGuid: string) => {
    var res = await axios.get("OrderInfo/BuyAgain?orderGuid=" + orderGuid)
    // var data = await axios.get("OrderInfo/GetSaleOrderDto", { params: { orderNo } })
    return res.data
}