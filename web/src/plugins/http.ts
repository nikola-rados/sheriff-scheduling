import axios from "axios";
import Vue from 'vue';
import store from "@/store";

import createAuthRefreshInterceptor from 'axios-auth-refresh';
import {AxiosAuthRefreshOptions} from 'axios-auth-refresh';

const refreshAuthLogic = failedRequest => axios.get('api/auth/token').then(tokenRefreshResponse => {
    if (tokenRefreshResponse.data.access_token == null) {
        location.replace('/api/auth/login?redirectUri=/');
        return Promise.resolve();
    }
    store.commit('CommonInformation/setToken',tokenRefreshResponse.data.access_token);
    store.commit('CommonInformation/setTokenExpiry',tokenRefreshResponse.data.expires_at);
    if (failedRequest != null)
        failedRequest.response.config.headers['Authorization'] = 'Bearer ' + tokenRefreshResponse.data.access_token;
    return Promise.resolve();
}).catch((error) => {
    location.replace('/api/auth/login?redirectUri=/');
});

const options: AxiosAuthRefreshOptions = {
    statusCodes: [ 401 ]
}

function configureInstance(){
    createAuthRefreshInterceptor(axios, refreshAuthLogic, options);    
    
    axios.interceptors.request.use(async config => {
        if (config.url != 'api/auth/token' && new Date() > new Date(store.state.CommonInformation.tokenExpiry))
        {
            console.log("Refreshing token, without 401.");
            await refreshAuthLogic(null);
        }
        const token = store.state.CommonInformation.token;
        config.headers['Authorization'] = 'Bearer ' +  token;    
        return config;
    });
    return axios;
}

const httpInstance = configureInstance();
export default {
    install () {
        Vue.prototype.$http = httpInstance
    }
};