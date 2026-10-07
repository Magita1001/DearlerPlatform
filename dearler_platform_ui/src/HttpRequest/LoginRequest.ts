import axios from "./AxiosHelper"

axios.defaults.baseURL = "http://localhost:5268"

export const Login = async (data: any) => {
    var res = await axios.post("Login/CheckLogin", data)
    return res.data
}
