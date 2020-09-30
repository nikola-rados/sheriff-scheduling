import Vue from 'vue'
import Vuex from 'vuex'
import CommonInformation from '@/store/modules/CommonInformation'

Vue.use(Vuex)

const store = new Vuex.Store({
  modules: {
    CommonInformation,
  }
})

export default store
