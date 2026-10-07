<template>
    <div>
        <div class="login-pad">
            <h2>
                <img src="/img/icons/favicon.ico" alt="">
                经销商平台
            </h2>
            <p>
                <input type="text" placeholder="用户名" v-model="userNo">
            </p>
            <p>
                <!-- 为回车添加登陆事件 -->
                <input type="password" placeholder="密码" v-model="password" @keyup.enter="login()" />
            </p>
            <!-- 原登录按钮方案 -->
            <!-- <router-link to="/Home" v-slot="{ navigate }">
                <button @click="navigate">
                    →
                </button>
            </router-link> -->
            <button @click="login()">
                →
            </button>

        </div>
        <div class="login-bottom">
            &copy;全栈ACE
        </div>
    </div>
</template>

<script lang="ts">
import { reactive, toRefs } from 'vue';
import { IloginInfo } from '@/Interface/Login';
import { Login } from '@/HttpRequest/LoginRequest';
import { useRouter } from 'vue-router';

export default {
    setup() {
        var router = useRouter();

        //reactive意为响应式对象
        const loginInfo: IloginInfo = reactive({
            userNo: "",
            password: "",
            login: async () => {
                var res = await Login({
                    customerNo: loginInfo.userNo,
                    password: loginInfo.password
                })
                
                if (res != null) {
                    localStorage["cno"] = loginInfo.userNo;
                    localStorage["token"] = res;
                    router.push("/layoutMain");
                }
            }
        })
        return {
            ...toRefs(loginInfo)
        }
    }
}
</script>

<style lang="scss" scoped>
.login-pad {
    text-align: center;
    width: 60%;
    margin: auto;
    margin-top: 26%;

    h2 {
        font-weight: normal;
        margin-bottom: 30px;

        img {
            display: inline-block;
            width: 36px;
            height: 36px;
            background: transparent;
            border-radius: 18px;
            vertical-align: -9px;
        }
    }

    p {
        width: 100%;
        margin-top: 20px;

        input {
            width: 100%;
            box-sizing: border-box;
            height: 36px;
            border-radius: 18px;
            border: 0 none;
            background-color: #f0f0f0;
            text-align: center;
        }
    }

    button {
        margin-top: 36px;
        width: 60px;
        height: 60px;
        border-radius: 30px;
        border: 0 none;
        background-color: rgb(79, 137, 245);
        color: #fff;
        font-size: 26px;
        font-weight: bold;
    }
}

.login-bottom {
    position: absolute;
    bottom: 10px;
    text-align: center;
    width: 100%;
    font-size: 14px;
}
</style>
