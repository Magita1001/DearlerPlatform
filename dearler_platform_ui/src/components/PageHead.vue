<template>
    <div class="back-headed">
        <router-link :to="backUrl" tag="button"></router-link>
        <h3>{{headText}}</h3>
    </div>
</template>

<script>
    export default {
        props:['headText','backUrl']
    }
</script>

<style lang="scss" scoped>
.back-headed {
    height: 46px;
    width: 100%;
    box-sizing: border-box;
    padding: 0 40px;
    position: relative;
    background-color: #f8f8f8;

    button {
        position: absolute;
        left: 12px;
        top: 12px;
        width: 22px;
        height: 22px;
        background-color: #aaa;
        border-radius: 22px;
        background-image: url("/img/icons-png/back-white.png");
        border: 0 none;
        background-position: center;
        background-repeat: no-repeat;
        outline: none;
    }

    h3 {
        width: 100%;
        text-align: center;
        height: 45px;
        line-height: 45px;
        font-size: 15px;
        color: #333;
    }
}
</style>