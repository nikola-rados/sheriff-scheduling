import axios from "axios";
import Vue from 'vue';
import createAuthRefreshInterceptor from 'axios-auth-refresh';





const refreshAuthLogic = failedRequest => axios.get('/api/token').then(tokenRefreshResponse => {
    localStorage.setItem('token', tokenRefreshResponse.data.token);
    failedRequest.response.config.headers['Authorization'] = 'Bearer ' + tokenRefreshResponse.data.token;
    return Promise.resolve();
}).catch((error) => {
    axios.get('/login').then(response => {console.log(response)})
}

);

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