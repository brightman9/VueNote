const store = {
    has(key) {
        return localStorage.getItem(key) !== null
    },
    get(key) {
        const value = JSON.parse(localStorage.getItem(key))
        return value
    },
    set(key, value) {
        localStorage.setItem(key, JSON.stringify(value))
    },
    delete(key) {
        localStorage.removeItem(key)
    }
}

function install(Vue) {
    Vue.prototype.$store = store
}

export default store
export { install }


