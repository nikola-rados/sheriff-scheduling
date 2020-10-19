import Vue from 'vue'
import Vuex from 'vuex'
import CommonInformation from '@/store/modules/CommonInformation'
import TeamMemberInformation from '@/store/modules/TeamMemberInformation'
Vue.use(Vuex)

const store = new Vuex.Store({
  modules: {
    CommonInformation,
    TeamMemberInformation
  }
})

export default store
