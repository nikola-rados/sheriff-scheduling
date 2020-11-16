import { RouteConfig } from 'vue-router'
import VueResource from 'vue-resource'
import Home from '@components/Home.vue'
import DutyRoster from '@components/DutyRoster/DutyRoster.vue'
import SetUp from '@components/DutyRoster/SetUp.vue'
import ManageSchedule from '@components/ShiftSchedule/ManageSchedule.vue'
import DistributeSchedule from '@components/ShiftSchedule/DistributeSchedule.vue'
import MyTeamMembers from '@components/MyTeam/MyTeamMembers.vue'
import DefineRolesAccess from '@components/MyTeam/DefineRolesAccess.vue'
import AssignmentTypes from '@components/ManageTypes/AssignmentTypes.vue'
import LeaveTrainingTypes from '@components/ManageTypes/LeaveTrainingTypes.vue'
import store from "@/store";

function displayFooter(to: any, from: any, next: any) {
  store.commit('CommonInformation/setDisplayFooter',true);
  next();
}

const routes: Array<RouteConfig> = [
  {
    path: '/',
    name: 'Home',
    beforeEnter: displayFooter,
    component: Home
  },
  {
    path: '/duty-roster',
    name: 'DustyRoster',
    component: DutyRoster    
  },
  {
    path: '/duty-roster-setup',
    name: 'SetUp',
    beforeEnter: displayFooter,
    component: SetUp
  },
  {    
    path: '/manage-shift-schedule',
    name: 'ManageSchedule',
    beforeEnter: displayFooter,
    component: ManageSchedule
  },
  {
    path: '/distribute-shift-schedule',
    name: 'DistributeSchedule',
    beforeEnter: displayFooter,
    component: DistributeSchedule  
  },
  {
    path: '/team-members',
    name: 'MyTeamMembers',
    beforeEnter: displayFooter,
    component: MyTeamMembers
  },
  {
    path: '/define-roles-access',
    name: 'DefineRolesAccess',
    beforeEnter: displayFooter,
    component: DefineRolesAccess  
  },
  {    
    path: '/assignment-types',
    name: 'AssignmentTypes',
    beforeEnter: displayFooter,
    component: AssignmentTypes
  },
  {
    path: '/leave-training-types',
    name: 'LeaveTrainingTypes',
    beforeEnter: displayFooter,
    component: LeaveTrainingTypes  
  }
]

export default routes