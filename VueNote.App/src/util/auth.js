import store from './store'

const auth = {
    getCurrentUser(){
        const currentUser = store.get('currentUser')
        return currentUser
    },
    hasLogin() {
        return store.has('token')
    },
    hasPermission(name) {
        const permissions = store.get('permissions')
        return permissions && permissions.includes(name)
    },
    deleteToken() {
        store.delete('token')
    }
}

function install(Vue) {
    Vue.prototype.$auth = auth
}

export default auth
export { install }


