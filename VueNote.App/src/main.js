// The Vue build version to load with the `import` command
// (runtime-only or standalone) has been set in webpack.base.conf with an alias.
import Vue from 'vue'
import App from './App'
import router from './router'
import { install as installStore } from './util/store'
import { install as installHttp } from './util/http'
import { install as installAuth } from './util/auth'
import { install as installBus } from './util/bus'
import ElementUI from 'element-ui'
import vuescroll from 'vuescroll'
import VueQuillEditor from 'vue-quill-editor'
import 'quill/dist/quill.core.css'
import 'quill/dist/quill.snow.css'
import 'quill/dist/quill.bubble.css'
  
Vue.use(installStore)
Vue.use(installHttp)
Vue.use(installAuth)
Vue.use(installBus)
Vue.use(ElementUI)
Vue.use(vuescroll)
Vue.use(VueQuillEditor)

Vue.config.productionTip = false

/* eslint-disable no-new */
new Vue({
  el: '#app',
  router,
  components: { App },
  template: '<App/>'
})
