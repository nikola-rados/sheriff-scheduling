import 'core-js/stable'
import 'regenerator-runtime/runtime'
import 'intersection-observer'
import Vue from 'vue'
import VueRouter from 'vue-router'
import { BootstrapVue, BootstrapVueIcons } from 'bootstrap-vue'
import { FontAwesomeIcon } from '@fortawesome/vue-fontawesome'
import { library } from '@fortawesome/fontawesome-svg-core'
import { faGraduationCap, faSuitcase } from '@fortawesome/free-solid-svg-icons'
import "@styles/index.scss";
import App from './App.vue';
import routes from './router/index'
import store from './store/index'
import http from "./plugins/http";
import "./filters"
import LoadingSpinner from "@components/LoadingSpinner.vue";

library.add(faGraduationCap);
library.add(faSuitcase);

Vue.use(VueRouter);
Vue.use(BootstrapVue);
Vue.use(BootstrapVueIcons);
Vue.use(http);
Vue.config.productionTip = true;
Vue.component('loading-spinner', LoadingSpinner);
Vue.component('font-awesome-icon', FontAwesomeIcon)

const router = new VueRouter({
	mode: 'history',
	routes: routes
});

new Vue({
	router,
	store,
	render: h => h(App)	
}).$mount('#app');