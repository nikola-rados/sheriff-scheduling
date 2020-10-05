import axios from "axios";
import Vue from 'vue';
import store from "@/store";

import createAuthRefreshInterceptor from 'axios-auth-refresh';

const refreshAuthLogic = failedRequest => axios.get('api/auth/token').then(tokenRefreshResponse => {
    
    store.commit('CommonInformation/setToken',tokenRefreshResponse.data.access_token);
    failedRequest.response.config.headers['Authorization'] = 'Bearer ' + tokenRefreshResponse.data.access_token;
    return Promise.resolve();
}).catch((error) => {
    location.replace('/api/auth/login?redirectUri=/');
});

function configureInstance(){
    createAuthRefreshInterceptor(axios, refreshAuthLogic);    
    return axios
}

const httpInstance = configureInstance();
export default {
    install () {
        Vue.prototype.$http = httpInstance
    }
};