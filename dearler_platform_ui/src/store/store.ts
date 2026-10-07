import { createStore } from "vuex";
export default createStore({
    // //存放数据(data)
    // state: {
    //     cartNum: 20,
    // },
    // //获取数据
    // getters: {
    //     getCartNum(state) {
    //         return state.cartNum
    //     }
    // },
    // //同步数据修改器 负责修改state中数据的增删改
    // // store.commit("方法名",参数)
    // mutations: {
    //     setCartNum(state, num) {
    //         state.cartNum = num
    //     },
    // },
    // //异步数据修改器 最终数据还是交给mutations处理
    // // store.dispatch("方法名",参数)
    // actions: {
    //     setCartNum(context, num) {
    //         context.commit("setCartNum", num)
    //     },
    // },
    //子模块拆分，防止单个store过于臃肿
    modules: {
        shoppingCart: {
            namespaced: true,
            state: {
                cartNum: 10
            },
            //获取数据
            getters: {
                getCartNum(state) {
                    return state.cartNum
                }
            },
            //同步数据修改器 负责修改state中数据的增删改
            // store.commit("方法名",参数)
            mutations: {
                setCartNum(state, num) {
                    state.cartNum = num
                },
            },
            //异步数据修改器 最终数据还是交给mutations处理
            // store.dispatch("方法名",参数)
            actions: {
                setCartNum(context, num) {
                    context.commit("setCartNum", num)
                },
            },
        }
    },
})