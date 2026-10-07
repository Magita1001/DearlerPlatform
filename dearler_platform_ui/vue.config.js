const { defineConfig } = require('@vue/cli-service')

module.exports = defineConfig({
  transpileDependencies: true,
  css: {
    loaderOptions: {
      css: {
        // 兼容旧版 css-loader 的 API 格式
        url: {
          filter: (url) => {
            // 如果 url 路径是以 /img/ 开头的绝对路径，返回 false（不进行解析打包）
            if (url.startsWith('/img/')) {
              return false;
            }
            return true;
          }
        }
      }
    }
  }
})