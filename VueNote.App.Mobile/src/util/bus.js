import Vue from 'vue'

const bus = new Vue()
Object.defineProperties(bus, {
    on: {
        get() {
            return this.$on.bind(this)
        }
    },
    once: {
        get() {
            return this.$once.bind(this)
        }
    },
    off: {
        get() {
            return this.$off.bind(this)
        }
    },
    emit: {
        get() {
            return this.$emit.bind(this)
        }
    }
})

function install(Vue) {
    Vue.prototype.$bus = bus
}

export default bus
export { install }


