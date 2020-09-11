import { RouteConfig } from 'vue-router'
import Home from '@components/Home.vue'

const routes: Array<RouteConfig> = [
  {
    path: '/',
    name: 'Home',
    component: Home
  }
]

export default routes