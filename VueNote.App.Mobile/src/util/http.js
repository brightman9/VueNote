import Axios from 'axios'
import store from './store'
import { Message } from 'element-ui'
import router from '../router'

const axios = Axios.create({
    timeout: 100000
})
// 请求拦截器
axios.interceptors.request.use(function (config) {
    // 让请求自动附加身份token
    if (store.has('token')) {
        config.headers.Authorization = store.get('token')
    }
    return config;
}, // 处理请求异常
    function (error) {
        Message.error('请求失败')
        return Promise.reject(error);
    });

// 响应拦截器
axios.interceptors.response.use(
    function (response) {
        return response.data;
    },
    // 处理响应异常
    function (error) {
        // 无响应结果的异常，一般是app自身代码异常、跨域失败等原因造成的
        if (!error.response) {
            Message.error('请求失败')
            return Promise.reject(error);
        }
        // 请求超时
        if (error.code == 'ECONNABORTED' && error.message.indexOf('timeout') != -1) {
            Message.error('请求超时')
            return Promise.reject(error);
        }

        // 处理响应异常
        switch (error.response.status) {
            case 400: {
                Message.error('请求失败')
                break
            }
            case 401: {
                Message.info('身份过期，请重新登录')
                router.push('/login')
                break
            }
            case 403: // 对无权访问的地址，按不存在的情况处理，以防止恶意用户揣测服务端目录结构
            case 404: {
                router.push('/notFound')
                break
            }
            case 408:
            case 500:
            case 502:
            case 503:
            case 504:
            default: {
                Message.error('服务器异常')
                break
            }
        }
        return Promise.reject(error);
    });

const http =
{
    get(url, params) {
        return axios.get(url, { params })
    },
    post(url, data) {
        return axios.post(url, data)
    },
    postParams(url, params) {
        return axios({
            method: 'post',
            url: url,
            params: params
        });
    }
}

function install(Vue) {
    Vue.prototype.$http = http
}

export default http
export { install }