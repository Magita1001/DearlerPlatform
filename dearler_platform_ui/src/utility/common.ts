/**
 * 价格保留两位小数
 */
export const transPrice = (price: number) => {
    if (price == null) return "0.00";
    else return price.toFixed(2);
}

/**
 * 
 * @param price 
 * @returns 
 */
export const transTime = (time: string | null, reservedTime: boolean = true) => {
    if (reservedTime) {
        return time?.replace("T", " ")
    } else {
        return time?.substring(0, time.indexOf("T"))
    }
}