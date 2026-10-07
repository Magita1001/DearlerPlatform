<template>
    <div>
        <!-- 搜索面板 -->
        <div class="search-pad">
            <input type="text" name="" id="" v-model="productInfo.searchText" @focus="searchFocus()"
                @blur="searchBlur()" @input="productInfo.search()" />
            <button v-show="pageController.isShowSearchBtn">搜索</button>
            <button v-show="!pageController.isShowSearchBtn" @click="showRight()">筛选</button>
        </div>
        <!-- 物品大类面板 -->
        <div class="system-pad">
            <div v-for="belongType in productInfo.belongTypes" :key="belongType.sysNo" :class="[
                'system-item',
                { 'system-select': productInfo.systemIndex == belongType.sysNo },
            ]" @click="productInfo.selectSystemProduct(belongType.sysNo)">
                <span>{{ belongType.belongTypeName }}</span>
            </div>
        </div>
        <!-- 物品展示列表 -->
        <div class="product-list">
            <ul>
                <li v-for="product in productInfo.products" :key="product.id">
                    <img :src="product.productPhoto.productPhotoUrl" alt="" />
                    <div>
                        <p class="p-name">{{ product.productName }}</p>
                        <p class="p-type">类别：{{ product.typeName }}</p>
                        <p class="p-price">
                            &yen;{{ transPrice(product.productSale.salePrice) }} 张
                        </p>
                        <p class="p-cart" @click="productInfo.onAddCart(product.productNo, 1)">
                            <!-- <img src="/img/icons-png/shoppingCar-white.png" alt=""> -->
                            <!-- <em></em> -->
                            <i>x1</i>
                        </p>
                    </div>
                </li>
            </ul>
            <!-- 左侧物品类型 -->
            <div :class="['left-menu', { 'left-menu-show': pageController.isShowLeft }]">
                <div class="left-switch" @click="showLeft()">
                    <img src="/img/dealerImgs/up.png" alt="" />
                </div>
                <ul>
                    <li v-for="productType in productInfo.productTypes" :key="productType.typeNo" :class="{
                        'left-item-select': productInfo.typeSelected == productType.typeNo,
                    }" @click="productInfo.selectType(productType.typeNo)">
                        {{ productType.productTypeName }}
                    </li>
                </ul>
            </div>
        </div>
        <!-- 右侧物品属性面板 -->
        <div class="right-pad">
            <div class="list-pad">
                <ul class="f-type-list">
                    <template v-for="(values, key) in productInfo.productProps">
                        <li v-if="values.length > 0" :key="key" }>
                            <p>{{ productInfo.getPropKey(key, 1) }}</p>
                            <ul class="f-item-list">
                                <li v-for="value in values" :key="value"
                                    @click="productInfo.selectProp(productInfo.getPropKey(key, 0), value)">
                                    <span :class="{
                                        'prop-select': productInfo.propSelect[productInfo.getPropKey(key, 0)] == value
                                    }">{{ value }}</span>
                                </li>
                                <!-- <li><span class="prop-select">胡桃色</span></li> -->
                            </ul>
                            <div class="clear-tag"></div>
                        </li>
                    </template>
                </ul>
            </div>
            <div class="right-edit">
                <button @click="productInfo.confirmFilter()" style="background-color: rgb(188, 0, 0); color: #fff">
                    确定
                </button>
                <button @click="hideRight()">取消</button>
            </div>

        </div>
        <div class="cover" v-show="pageController.isShowCover" @click="hideRight()"></div>
    </div>
</template>

<script setup lang="ts">
import { AddCart, GetBelongType, GetProduct, GetProp, GetType } from "@/HttpRequest/ProductListRequst";
import { IProduct, IProductInfo } from "@/Interface/ProductList";
import { transPrice } from "@/utility/common";
import { onMounted, reactive, ref } from "vue";
import { LocationQueryValue, useRoute, useRouter } from "vue-router";
import { useStore } from "vuex";

var router = useRouter()
var route = useRoute()

var store = useStore()

const pageController = reactive({
    systemIndex: "1",
    isShowLeft: false,
    isShowCover: false,
    isShowSearchBtn: false,
});

const productInfo: IProductInfo = reactive({
    systemIndex: "1",
    searchText: "",
    products: [],
    belongTypes: [],
    productTypes: [],
    productProps: {},
    propSelect: {},
    typeSelected: "",
    timer: 0,
    pageIndex: 1,

    //获取物品
    getProducts: async (sysNo: string,
        productType: string,
        searchText: string,
        propductProp: string | null) => {
        // productInfo.products.push(await GetProduct({
        var products = (await GetProduct({
            searchText: searchText,
            sysNo: sysNo,
            productType: productType,
            productProps: propductProp,
            sort: "ProductName",
            pageIndex: productInfo.pageIndex,
        }));
        products.forEach((p: IProduct) => {
            productInfo.products.push(p)
        });
        //因为push不能接收数组变量，所有也可以写成这样 '...'展开运算符会把数组展开成以逗号分隔的多个参数
        // productInfo.products.push(...products)

        console.log(productInfo.products);
    },
    getBelongTypes: async () => {
        productInfo.belongTypes = await GetBelongType();
    },
    /**
     * 点击大类时我们不需要考虑搜索的内容，因为每次点击大类，都应该清空搜索框
     * 但是点击所搜物品时，应该考虑大类
     */
    selectSystemProduct: async (sysNo: string) => {
        productInfo.propSelect = []
        productInfo.typeSelected = ""
        productInfo.searchText = ""
        router.push(`/productList?belongType=${sysNo}`)

        //#region 过期方案
        // productInfo.typeSelected = null
        // productInfo.systemIndex = sysNo;

        // await productInfo.getProducts(sysNo);
        // await productInfo.getType(sysNo);
        // await productInfo.getProps(sysNo, null)
        //#endregion
    },
    /**
     * 从后端获取物品类型
     */
    getType: async (sysNo: string) => {
        // productInfo.propSelect = []
        productInfo.searchText = ""
        productInfo.productTypes = await GetType(sysNo)
    },
    /**
     * 选择物品类型(左侧) 选择物品类型时可以清空搜索栏
     */
    selectType: async (typeNo: string) => {
        //#region 废弃方案
        // await productInfo.getProducts(productInfo.systemIndex, productInfo.typeSelected)
        // 方案1
        // if (productInfo.typeSelected == typeNo) {
        //     productInfo.typeSelected = ""
        // } else {
        //     productInfo.typeSelected = typeNo
        // }
        // var url = `/productList?belongType=${productInfo.systemIndex}`
        // if (productInfo.typeSelected?.trim() != "") {
        //     url += `&typeSelected=${productInfo.typeSelected}`
        // }        
        // router.push(url)

        // 方案2 然后在解析字符串时再对 productInfo.typeSelected 真正赋值
        // var url = `/productList?belongType=${productInfo.systemIndex}`
        // if (productInfo.typeSelected == typeNo) {
        //     url
        // } else {
        //     url += `&typeSelected=${typeNo}`
        // }
        // router.push(url)
        //#endregion
        productInfo.propSelect = []
        if (productInfo.typeSelected == typeNo) {
            productInfo.typeSelected = ""
        } else {
            productInfo.typeSelected = typeNo
        }
        setRouter()
    },
    /**
     * 从后端获取物品属性
     */
    getProps: async (sysNo: string, typeNo: string) => {
        //如果参数名和方法需要的参数名一致，可以写成下面这样，而不是 belongTypeName:belongTypeName
        var res = await GetProp({ sysNo, typeNo })
        productInfo.productProps = res
        console.log(productInfo.productProps)
    },
    // /**
    //  * 价格保留两位小数
    //  */
    // transPrice: (price: number) => {
    //     if (price == null) return "0.00";
    //     else return price.toFixed(2);
    // },

    /**
     * 分割物品种类的名称
     */
    getPropKey: (key: string | number, index: number) => {
        return (key as string).split("|")[index]
    },
    /**
     * 搜索物品
     */
    search: () => {
        clearTimeout(productInfo.timer)
        productInfo.timer = setTimeout(async () => {
            setRouter()

            //#region 旧方案 11-6更改
            // var url = `/productList?belongType=${productInfo.systemIndex}`
            // if (productInfo.searchText.trim() != "") {
            //     url += `&keyWords=${productInfo.searchText}`
            // }
            // if (productInfo.typeSelected?.trim() != "") {
            //     url += `&type=${productInfo.typeSelected}`
            // }

            // router.push(url)

            //history是浏览器提供的用于在浏览器历史记录中跳转的对象，go()或go(0)作用是刷新界面
            // history.go()

            // productInfo.products = await GetProduct({
            //     searchText: productInfo.searchText,
            //     sysNo: productInfo.systemIndex,
            //     productType: productInfo.typeSelected,
            //     sort: "ProductName",
            //     pageIndex: 1,
            // });
            //#endregion
        }, 1000);
    },
    /**
     * 选择属性
     */
    selectProp: (propKey: string, propValue: string) => {
        if (productInfo.propSelect[propKey] == propValue) {
            productInfo.propSelect[propKey] = ""
        } else {
            productInfo.propSelect[propKey] = propValue
        }
    },
    /**
     * 确认/拼接 筛选字符串
     */
    confirmFilter: () => {
        setRouter()
    },
    /**
     * 物品添加到购物车
     */
    onAddCart: async (productNo: string, productNum: number) => {
        console.log("进入addcart")
        const customerNo = localStorage["cno"]
        var res = await AddCart({ customerNo, productNo, productNum })

        var currentCartNum = store.getters["shoppingCart/getCartNum"]
        store.dispatch("shoppingCart/setCartNum", currentCartNum + 1)
    }
});
/**
 * 将选中的物品属性转化为字符串
 */
const productPropToString = () => {
    productProps = ""
    for (const key in productInfo.propSelect) {
        const value = productInfo.propSelect[key]
        if (productInfo.propSelect[key] != '')
            productProps += `${key}_${value}^`
    }
    productProps = productProps.substring(0, productProps.length - 1)
}
/**
 * 设置路由 Url
 */
const setRouter = () => {
    var url = `/productList?belongType=${productInfo.systemIndex}`
    if (productInfo.searchText.trim() != "") {
        url += `&keyWords=${productInfo.searchText}`
    }
    if (productInfo.typeSelected?.trim() != "") {
        url += `&typeSelected=${productInfo.typeSelected}`
    }
    productPropToString()
    if (productProps != "") {
        url += `&prop=${productProps}`
    }

    router.push(url)
}
const showLeft = () => {
    pageController.isShowLeft = !pageController.isShowLeft;
};
const searchFocus = () => {
    pageController.isShowSearchBtn = true;
};
const searchBlur = () => {
    pageController.isShowSearchBtn = false;
};
// const confirmFilter = () => { };
const showRight = () => {
    pageController.isShowCover = true;
    var dom = document.querySelector(".right-pad") as HTMLElement
    dom.style.right = "0";
};
const hideRight = () => {
    pageController.isShowCover = false;
    var dom = document.querySelector(".right-pad") as HTMLElement
    dom.style.right = "-85%";
};

let searchText = ""
let systemIndex = ""
let productType = ""
let productProps = ""
// 解析Url字符串
const resolutionAddress = () => {
    searchText = route.query.keyWords as string ?? ""
    productInfo.systemIndex = systemIndex = route.query.belongType as string ?? "1"
    productInfo.typeSelected = productType = route.query.typeSelected as string ?? ""

    productProps = route.query.prop as string ?? ""
    //格式大约为：&prop=xxx_xxx^yyy_yyy
    if (productProps != "") {
        var arrProductProps = productProps.split("^")
        for (let i = 0; i < arrProductProps.length; i++) {
            const element = arrProductProps[i]
            productInfo.propSelect[element.split("_")[0]] = element.split("_")[1]
        }
    }
}
/**
 * 监听页面滚动事件
 */
const handleScroll = () => {
    var htmlDom = document.querySelector("html") as HTMLElement
    // 1.获取当前页面长度
    var htmlHeight = htmlDom.offsetHeight
    // console.log(htmlHeight)
    // 2.获取滚动条距离顶部的距离 scrollTop指的是 当前界面顶部 距离 整个界面最顶部 的距离
    var scrollTop = htmlDom.scrollTop
    // 3.获得当前可视区域的高度
    var screenHeight = document.documentElement.clientHeight
    // 4.获取可视区域底部到整个界面底部的距离
    var diffHeight = htmlHeight - scrollTop - screenHeight
    // console.log(diffHeight)

    if (diffHeight <= 2 && scrollTop > 0) {
        onPageChange()
        console.log("分页了_" + productInfo.pageIndex)
    }
}
const onPageChange = async () => {
    productInfo.pageIndex++
    await productInfo.getProducts(systemIndex, productType, searchText, productProps);
}

onMounted(async () => {
    window.addEventListener("scroll", handleScroll)
    resolutionAddress()
    await productInfo.getProducts(systemIndex, productType, searchText, productProps);
    await productInfo.getType(systemIndex);
    await productInfo.getBelongTypes();
    await productInfo.getProps(systemIndex, productType);
});
</script>

<style lang="scss" scoped>
.i-search:after {
    background-color: #b70101 !important;
}

.search-pad {
    z-index: 10;
    position: fixed;
    width: 100%;
    padding: 6px 20px;
    background-color: #f0f0f0;
    display: flex;

    input {
        height: 28px;
        box-sizing: border-box;
        border: 1px solid #ddd;
        border-radius: 3px;
        flex: 1;
        outline: none;
    }

    button {
        background-color: transparent;
        width: 56px;
        border: 0 none;
        font-size: 14px;
        font-weight: bold;
        color: #333;
        outline: none;
    }
}

.system-pad {
    z-index: 10;
    background-color: #fff;
    display: flex;
    position: fixed;
    width: 100%;
    top: 40px;

    .system-item {
        flex: 1;
        text-align: center;
        border-bottom: 1px #ddd solid;
        border-right: 1px transparent solid;
        border-left: 1px transparent solid;

        span {
            border: 0 none !important;
            background-color: #f0f2f5;
            margin: 6px 5px;
            font-size: 12px;
            font-weight: normal;
            text-align: center;
            border-radius: 4px;
            padding: 6px 0;
            display: block;
            height: 22px;
            line-height: 12px;
        }
    }

    .system-select {
        border-bottom: 1px transparent solid;
        border-right: 1px #ddd solid;
        border-left: 1px #ddd solid;

        span {
            background-color: transparent;
        }
    }
}

.product-list {
    padding-top: 75px;

    ul {
        background-color: #fff;

        li {
            list-style: none;
            height: 88px;
            padding-left: 108px;
            position: relative;

            img {
                height: 66px;
                width: 66px;
                background-color: #ccc;
                position: absolute;
                left: 28px;
                top: 11px;
            }

            div {
                overflow: hidden; //列表被撑开的原因
                padding: 10px 0;
                border-bottom: 1px solid #f0f0f0;
                padding-bottom: 6px;
                text-align: left;

                .p-name {
                    font-size: 13px;
                }

                .p-type {
                    font-size: 12px;
                    color: #666;
                    margin-top: 8px;
                }

                .p-price {
                    font-size: 13px;
                    color: #f23030;
                    margin-top: 8px;
                }

                .p-cart {
                    position: relative;
                    float: right;
                    background-color: #b70101;
                    height: 20px;
                    width: 40px;
                    background-image: url("/img/icons-png/shoppingCar-white.png");
                    background-repeat: no-repeat;
                    background-position: center;
                    background-position-x: 45%;
                    background-size: 16px;
                    border-radius: 30px;
                    margin-right: 26px;
                    // top: -20px;

                    i {
                        position: absolute;
                        right: -18px;
                        font-size: 12px;
                        top: 3px;
                    }
                }
            }
        }
    }

    .left-menu {
        position: fixed;
        height: calc(100% - 116px);
        left: -106px;
        width: 125px;
        background-color: #fff;
        top: 76px;
        border-radius: 0 18px 0 0;
        border: 1px solid #d7d7d7;
        overflow: hidden;
        transition: 0.5s;
        margin-bottom: 120px;

        .left-switch {
            width: 20px;
            background-color: #fff;
            position: absolute;
            right: 0;
            height: 100%;

            img {
                position: absolute;
                top: 42%;
                width: 20px;
                left: 2px;
                transform: rotate(90deg);
                transition: 0.5s;
            }
        }

        ul {
            position: absolute;
            height: 100%;
            width: 106px;
            background-color: #f0f0f0;
            overflow: auto;

            li {
                width: 106px;
                height: 50px;
                text-align: center;
                line-height: 50px;
                border-bottom: 1px solid #d7d7d7;
                padding: 0;
                font-size: 12px;
                color: #333;
            }

            li.left-item-select {
                background-color: #fff;
            }
        }
    }

    .left-menu-show {
        left: 0;

        .left-switch {
            img {
                transform: rotate(-90deg);
            }
        }
    }
}

.right-pad {
    position: fixed;
    /* right: -85%; */
    right: -85%;
    top: 0;
    width: 85%;
    height: 100%;
    background-color: #f7f7f7;
    z-index: 103;
    transition: 580ms;
    z-index: 101;

    ul {
        list-style: none;
        overflow: hidden;
    }

    .list-pad {
        overflow: auto;
        height: 100%;
        padding-bottom: 40px;

        .f-type-list {
            overflow: hidden;

            >li {
                padding: 10px;
                background-color: #fff;
                margin-bottom: 10px;

                .f-item-list {
                    overflow: hidden;
                    display: flex;
                    flex-wrap: wrap;

                    li {
                        flex-basis: 33.3%;

                        span {
                            display: block;
                            margin-top: 10px;
                            margin-right: 10px;
                            background: #eee;
                            border: 1px solid #eee;
                            padding: 5px 0;
                            text-align: center;
                            border-radius: 6px;
                            font-size: 13px;
                            overflow: hidden;
                            height: 29px;
                            line-height: 22px;
                        }

                        .prop-select {
                            border: 1px solid red;
                            background: #fff;
                            color: red;
                        }
                    }
                }

                p {
                    font-size: 14px;
                }
            }
        }
    }

    .right-edit {
        position: absolute;
        bottom: 0;
        right: 0;
        width: 100%;

        button {
            float: left;
            height: 40px;
            width: 50%;
            line-height: 40px;
            text-align: center;
            border: 0px none;
        }
    }
}

.cover {
    z-index: 11;
    position: fixed;
    height: 100%;
    width: 100%;
    left: 0;
    top: 0;
    background-color: rgba(51, 51, 51, 0.36);
}
</style>
