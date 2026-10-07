import { createRouter, createWebHashHistory, RouteRecordRaw } from 'vue-router'
import HomeView from '../views/HomeView.vue'
import Login from '../views/Login.vue'


const routes: Array<RouteRecordRaw> = [
  {
    path: '/',
    name: 'Login',
    component: Login
  },
  {
    path: '/Home',
    name: 'Home',
    component: HomeView
  },
  {
    path: '/layoutMain',
    name: 'LayoutMain',
    // route level code-splitting
    // this generates a separate chunk (about.[hash].js) for this route
    // which is lazy-loaded when the route is visited.
    component: () => import(/* webpackChunkName: "about" */ '../views/LayoutMain.vue'),

    //默认路由 如果访问 layoutMain 会直接重定向到main里
    redirect: '/main',
    children: [
      {
        path: '/main',
        name: 'Main',
        component: () => import(/* webpackChunkName: "about" */ '../views/Main.vue'),
      },
      {
        path: '/productList',
        name: 'ProductList',
        component: () => import(/* webpackChunkName: "about" */ '../views/ProductList.vue'),
      },
      {
        path: '/shoppingCart',
        name: 'ShoppingCart',
        component: () => import(/* webpackChunkName: "about" */ '../views/ShoppingCart.vue'),
      },
      {
        path: '/orderConfirm',
        name: 'OrderConfirm',
        component: () => import(/* webpackChunkName: "about" */ '../views/OrderConfirm.vue'),
      },
      {
        path: '/orderDetail',
        name: 'OrderDetail',
        component: () => import(/* webpackChunkName: "about" */ '../views/OrderDetail.vue'),
      },
    ]
  },
  {
    path: '/about',
    name: 'about',
    // route level code-splitting
    // this generates a separate chunk (about.[hash].js) for this route
    // which is lazy-loaded when the route is visited.
    component: () => import(/* webpackChunkName: "about" */ '../views/AboutView.vue')
  }
]

const router = createRouter({
  history: createWebHashHistory(),
  routes
})

export default router
