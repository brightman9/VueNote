import Vue from 'vue'
import Router from 'vue-router'
import Home from '../pages/Home'
import NoteSearch from '../components/note/NoteSearch'
import NoteDetail from '../components/note/NoteDetail'
import Login from '../pages/Login'
import NotFound from '../pages/NotFound'
import auth from '../util/auth'

Vue.use(Router)

const router = new Router({
  routes: [
    {
      path: '/',
      redirect: { path: '/notes', query: { type: 'all' } },
      component: Home,
      children: [
        {
          path: '/notes',
          component: NoteSearch,
          meta: { permission: 'Note' }
        },
        {
          path: '/note',
          component: NoteDetail,
          meta: { permission: 'Note' }
        }]
    },
    {
      path: '/login',
      component: Login
    },
    {
      path: '/notFound',
      component: NotFound
    }
  ]
})

router.beforeEach((to, from, next) => {
  if (to.path === '/login') {
    next()
  }
  else if (!auth.hasLogin()) {
    next('/login')
  }
  else if (to.meta && to.meta.permission && !auth.hasPermission(to.meta.permission)) {
    next('/notFound')
  }
  else {
    next()
  }
})

export default router
