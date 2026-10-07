import router from "@/router";
import axios from "axios";

axios.defaults.baseURL = "http://localhost:5268"

// axios拦截器分为 请求拦截器 和 响应请求拦截器
// 请求拦截器
axios.interceptors.request.use(
    config => {
        // 判断 token 是否存在，如果存在，则在请求前给hearder中加上token
        if (localStorage["token"]) {
            console.log(localStorage["token"])
            config.headers.Authorization = "bearer " + localStorage["token"] // 请求头加上token
        }

        return config
    },
    error => {
        return Promise.reject(error)
    }
)
//响应拦截器
axios.interceptors.response.use(
    response => {
        return response
    },
    error => {
        switch (error.response.status) {
            case 401: {
                router.push("/");
            }

        }
    }
)

export default axios
