<template>
    <div>
        <div class="cart-list">
            <ul>
                <li v-for="type in shoppingCartInfo.types" :key="type.typeNo">
                    <p>
                        <i :class="{ 'cart-select': type.typeSelected }"
                            @click="shoppingCartInfo.onSelectedType(type)">✔</i>
                        <span>{{ shoppingCartInfo.transTypeWhenNull(type.typeName) }}</span>
                    </p>
                    <template v-for="cart in shoppingCartInfo.carts">
                        <div v-if="cart.productDto?.typeName == type.typeName" :key="cart.id">
                            <i :class="{ 'cart-select': cart.cartSelected }"
                                @click="shoppingCartInfo.onSelectCart(cart)">✔</i>
                            <img src="" alt="">
                            <p class="p-name">{{ cart.productDto.productName }}</p>
                            <p class="p-price">&yen;{{ transPrice(
                                shoppingCartInfo.transPriceWhenNull(cart.productDto.productSale.salePrice)) }}</p>
                            <p class="p-num">
                                <span class="sub-num" @click="shoppingCartInfo.onSubNum(cart)">-</span>
                                <input type="text" name="" ref="productNumRef" id="" :value="cart.productNum"
                                    @change="shoppingCartInfo.onChangeNum(cart)">
                                <span class="add0num" @click="shoppingCartInfo.onAddNum(cart)">+</span>
                                <b>块</b>
                            </p>
                        </div>
                    </template>
                </li>
                <!-- <li>
                    <p>
                        <i class="cart-select">✔</i>
                        <span>处理产品</span>
                    </p>
                    <div>
                        <i class="cart-select">✔</i>
                        <img src="" alt="">
                        <p class="p-name">0.9密度板</p>
                        <p class="p-price">&yen;100.00</p>
                        <p class="p-num">
                            <span class="sub-num" @click="shoppingCartInfo.onSubNum()">-</span>
                            <input type="text" name="" id="" :value="buyNum" @change="shoppingCartInfo.onChangeNum()">
                            <span class="add0num" @click="shoppingCartInfo.onAddNum()">+</span>
                            <b>块</b>
                        </p>
                    </div>
                </li>
                <li>
                    <p>
                        <i></i>
                        <span>处理产品</span>
                    </p>
                    <div>
                        <i></i>
                        <img src="" alt="">
                        <p class="p-name">0.9密度板</p>
                        <p class="p-price">&yen;100.00</p>
                        <p class="p-num">
                            <span class="sub-num" @click="shoppingCartInfo.onSubNum()">-</span>
                            <input type="text" name="" id="" :value="buyNum" @change="shoppingCartInfo.onChangeNum()">
                            <span class="add0num" @click="shoppingCartInfo.onAddNum()">+</span>
                            <b>块</b>
                        </p>
                    </div>
                    <div>
                        <i></i>
                        <img src="" alt="">
                        <p class="p-name">0.9密度板</p>
                        <p class="p-price">&yen;100.00</p>
                        <p class="p-num">
                            <span class="sub-num" @click="shoppingCartInfo.onSubNum()">-</span>
                            <input type="text" name="" id="" :value="buyNum" @change="shoppingCartInfo.onChangeNum()">
                            <span class="add0num" @click="shoppingCartInfo.onAddNum()">+</span>
                            <b>块</b>
                        </p>
                    </div>
                </li> -->
            </ul>
        </div>
        <div class="total-pad">
            <i :class="{ 'cart-select': shoppingCartInfo.isAllSelected }">✔</i>
            <span>全选</span>
            <span>
                合计：&yen; <b>{{ shoppingCartInfo.totalPrice }}</b>
            </span>
            <button>确定下单</button>
        </div>
    </div>
</template>

<script setup>
import { GetShoppingCarts, UpdateCartSelected } from '@/HttpRequest/ShoppingCartRequest';
import { ShoppingCartNum } from '@/store';
import { transPrice } from '@/utility/common';
import { reactive, onMounted, ref, watch } from 'vue';
import { useStore } from 'vuex';

const store = useStore()

// mounted() {
//     // this.$store.comm
//     // this.$store.dispatch('setFootMenuIndexAsync', 2);
//     console.log('shoppingCart mounted');
// },
// methods: {
//    
//     }
// },

// const productNumRef = ref() //数据发生改变时，这个方式不能及时触发变化后的数据
const shoppingCartInfo = reactive({
    carts: [],
    types: [],
    buyNum: 1,
    totalPrice: 0,
    isAllSelected: false,

    onAddNum(cart) {
        cart.productNum++
        UpdateCartSelected([cart.cartGuid], cart.cartSelected, cart.productNum)
    },
    onSubNum: async (cart) => {
        if (cart.productNum > 0) {
            cart.productNum--
            var res = await UpdateCartSelected([cart.cartGuid], cart.cartSelected, cart.productNum)
            if (res.isSuccess == "Remove") {

                var index = shoppingCartInfo.carts.findIndex(m => m.cartGuid == cart.cartGuid)
                shoppingCartInfo.carts.splice(index, 1)
            }
        }
    },
    onChangeNum(cart) {
        //#region 方案一
        //event.target.value 代表触发当前事件的Dom元素，通过它拿到当前控件上的值
        var currNum = event.target.value;
        //判断这个值是一个合法的数字并且大于0 
        if (!isNaN(currNum) && currNum > 0) {
            // 把值赋给 cart.productNum
            cart.productNum = currNum;
        } else {
            // 否则使用cart.productNum 的值还原 target.value
            event.target.value = cart.productNum;
        }
        //#endregion


        //#region 方案二： 在 input 标签中使用 ref 来对 Dom 组件进行响应式获取 
        // 教程中给出的这个方案，错误原因不是因为不能及时拿到变化后的值，而是productNumRef拿到的是一整组的input

        // var currNum = productNumRef.value.value; //这里的第一个value代表组件，第二个代表值
        // //判断这个值是一个合法的数字并且大于0 
        // if (!isNaN(currNum) && currNum > 0) {
        //     // 把值赋给 cart.productNum
        //     cart.productNum = currNum;
        // } else {            
        //     // 否则使用cart.productNum 的值还原 target.value
        //     productNumRef.value.value = cart.productNum;
        // }
        //#endregion
    },
    /**
     * 得到购物车中所有数据
     */
    onGetShoppingCart: async () => {
        var customerNo = localStorage["cno"]
        var res = await GetShoppingCarts(customerNo)
        shoppingCartInfo.carts = res.carts
        shoppingCartInfo.types = res.types
        // console.log(shoppingCartInfo.carts)
        // console.log(shoppingCartInfo.types)
    },
    onSelectCart: async (cart) => {
        cart.cartSelected = !cart.cartSelected
        var data = await UpdateCartSelected([cart.cartGuid], cart.cartSelected, cart.productNum)
        console.log(data.data.cartSelected)
        //找到和传进来的 cart 类型相同的 type 对象
        // var type = shoppingCartInfo.types.filter(m => m.typeNo == cart.productDto.typeNo)[0]
        //根据这个 type 找出所有 shoppingCartInfo.carts 中的同类物品
        // var cartsOfType = shoppingCartInfo.carts.filter(m => m.productDto.typeNo == type.typeNo)

        // //判断所有同类物品是否满足全部选中的条件
        // if (cartsOfType.every(m => m.cartSelected)) {
        //     type.typeSelected = true
        // } else {
        //     type.typeSelected = false
        // }

        // shoppingCartInfo.checkTypeSelected()
    },
    /**
     * 选择类型时触发 对同类型物品执行全选操作
     */
    onSelectedType: (type) => {
        var cartGuids = []

        type.typeSelected = !type.typeSelected
        shoppingCartInfo.carts.filter(m => m.productDto.typeNo == type.typeNo).forEach(m => {
            cartGuids.push(m.cartGuid)

            m.cartSelected = type.typeSelected
        })
        UpdateCartSelected(cartGuids, type.typeSelected)
        // shoppingCartInfo.checkIsAllSelected()
    },
    /**
     * 检测是否选中某一类下的所有物品
     */
    checkTypeSelected: () => {
        //筛选所有的type
        shoppingCartInfo.types.forEach(type => {
            //筛选出所有和 type 类型相同的 cart
            var cartsOfType = shoppingCartInfo.carts.filter(m => m.productDto.typeNo == type.typeNo)

            //判断所有同类物品是否满足全部选中的条件，是的话将此类型物品设为全选
            if (cartsOfType.every(m => m.cartSelected)) {
                type.typeSelected = true
            } else {
                type.typeSelected = false
            }
        })
        shoppingCartInfo.checkIsAllSelected()
    },
    /**
     * 检测是否选中所有物品
     */
    checkIsAllSelected: () => {
        if (shoppingCartInfo.carts.every(m => m.cartSelected)) {
            shoppingCartInfo.isAllSelected = true
        } else {
            shoppingCartInfo.isAllSelected = false
        }
    },
    /**
     * 计算总价
     */
    calcTotalPrice: () => {
        shoppingCartInfo.totalPrice = 0
        let currentCartNum = 0
        shoppingCartInfo.carts.filter(c => c.cartSelected == true).forEach(c => {
            // var singleProce = c.productDto.productSale?.salePrice ?? 0
            shoppingCartInfo.totalPrice += c.productDto.productSale?.salePrice * c.productNum
            currentCartNum += c.productNum
        })
        // ShoppingCartNum.value = currentCartNum
        //vuex响应式改造，替换ShoppingCartNum这种全局响应式变量
        store.dispatch("shoppingCart/setCartNum", currentCartNum)

    },

    transTypeWhenNull: (typeName) => {
        // console.log(type)
        var name = typeName || "未分类产品"
        return name
    },
    transPriceWhenNull: (price) => {
        return price ?? 0
    }

})
/**
 * 使用vue的 watch 对carts进行深度监听 数据发生变化时就会触发
 */
watch(() => shoppingCartInfo.carts, (newValue, oldValue) => {
    shoppingCartInfo.checkTypeSelected()
    shoppingCartInfo.calcTotalPrice()
},
    { deep: true } //深度监听，一般鉴定对象内的数据都需要深度监听
)

onMounted(async () => {
    await shoppingCartInfo.onGetShoppingCart()
    shoppingCartInfo.checkTypeSelected()
})

</script>

<style lang="scss" scoped>
.cart-list {
    text-align: left;

    ul {
        margin-bottom: 108px;

        li {
            background-color: #fff;
            margin-bottom: 12px;

            >p {
                padding-left: 46px;
                position: relative;
                height: 46px;
                border-bottom: 1px solid #ddd;

                i {
                    border: 1px solid #a9a9a9;
                    width: 18px;
                    height: 18px;
                    line-height: 18px;
                    border-radius: 18px;
                    position: absolute;
                    left: 13px;
                    top: 13px;
                    text-align: center;
                    font-size: 12px;
                    color: #fff;
                    font-style: normal;
                }

                i.cart-select {
                    background-color: crimson;
                    border: 1px solid crimson;
                }

                span {
                    display: inline-block;
                    border-left: 3px solid crimson;
                    height: 28px;
                    margin: 9px 0;
                    padding-left: 8px;
                    line-height: 30px;
                }
            }

            div {
                padding-left: 46px;
                position: relative;
                height: 98px;
                padding: 8px 14px 8px 148px;

                i {
                    border: 1px solid #a9a9a9;
                    width: 18px;
                    height: 18px;
                    line-height: 18px;
                    border-radius: 18px;
                    position: absolute;
                    left: 13px;
                    top: 28px;
                    text-align: center;
                    font-size: 12px;
                    color: #fff;
                    font-style: normal;
                }

                i.cart-select {
                    background-color: crimson;
                    border: 1px solid crimson;
                }

                img {
                    width: 68px;
                    height: 68px;
                    background-color: #ccc;
                    position: absolute;
                    left: 58px;
                    ;
                    top: 20px;
                }

                p.p-name {
                    font-size: 13px;
                    margin-top: 10px;
                    height: 30px;
                    ;
                }

                p.p-price {
                    font-size: 13px;
                    height: 20px;
                    color: crimson;
                }

                p.p-num {
                    text-align: right;
                    padding-right: 20px;

                    span {
                        display: inline-block;
                        width: 18px;
                        height: 18px;
                        border: 1px solid crimson;
                        color: crimson;
                        border-radius: 9px;
                        text-align: center;
                        line-height: 18px;
                    }

                    input {
                        width: 28px;
                        border: none 0px;
                        outline: none;
                        text-align: center;
                    }

                    b {
                        font-weight: normal;
                        margin-left: 10px;
                        font-size: 13px;
                    }

                }
            }
        }
    }
}

.total-pad {
    height: 58px;
    width: 100%;
    background-color: #383838;
    position: fixed;
    left: 0;
    bottom: 40px;

    i {

        display: inline-block;
        border: 1px solid #a9a9a9;
        width: 18px;
        height: 18px;
        line-height: 18px;
        border-radius: 18px;
        background-color: #fff;
        margin-left: 13px;
        margin-top: 20px;
        vertical-align: bottom;
        height: 18px;
        text-align: center;
        font-size: 12px;
        color: #fff;
        font-style: normal;
    }

    i.cart-select {
        background-color: crimson;
        border: 1px solid crimson;
    }

    span {
        color: #fff;
        margin-left: 6px;
        font-size: 13px;

        b {
            font-size: 15px;
        }
    }

    button {
        float: right;
        height: 58px;
        width: 120px;
        border: 0 none;
        background-color: #ddd;
        color: #aaa;
        font-size: 15px;
        font-weight: bold;
    }
}
</style>
