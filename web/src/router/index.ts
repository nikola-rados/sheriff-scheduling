import { RouteConfig } from 'vue-router'
import VueResource from 'vue-resource'
import Home from '@components/Home.vue'
import DutyRoster from '@components/DutyRoster/DutyRoster.vue'
import SetUp from '@components/DutyRoster/SetUp.vue'
import AddAssignment from '@components/Assignment/AddAssignment.vue'
import ManageSchedule from '@components/ShiftSchedule/ManageSchedule.vue'
import DistributeSchedule from '@components/ShiftSchedule/DistributeSchedule.vue'
import MyTeamMembers from '@components/MyTeam/MyTeamMembers.vue'
import FindManageUsers from '@components/MyTeam/FindManageUsers.vue'
import DefineRolesAccess from '@components/MyTeam/DefineRolesAccess.vue'
import AssignmentTypes from '@components/ManageTypes/AssignmentTypes.vue'
import LeaveTrainingTypes from '@components/ManageTypes/LeaveTrainingTypes.vue'

// function authGuard(to: any, from: any, next: any) { 

//   get('/user-info/')
//   .then((response) => {
//     const userId = response.data.user_id;
//     const loginUrl = response.data.login_uri;
    
//     if (userId) {
//       const userName = response.data.first_name + " " + response.data.last_name;
//       store.dispatch("application/setUserName", userName);
//       store.dispatch("common/setUserId", userId);
//       next();
//     } else {
//       location.replace(loginUrl);
//     }
//   }).catch((error) => {
//     //TODO: determine workflow
//     console.log(error)   
    
//   });
// }

const routes: Array<RouteConfig> = [
  {
    path: '/',
    name: 'Home',
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
    component: SetUp
  },
  {
    path: '/assignment',
    name: 'AddAssignment',
    component: AddAssignment
  },
  {    
      path: '/manage-shift-schedule',
      name: 'ManageSchedule',
      component: ManageSchedule
  },
  {
    path: '/distribute-shift-schedule',
    name: 'DistributeSchedule',
    component: DistributeSchedule  
  },
  {
    path: '/team-members',
    name: 'MyTeamMembers',
    component: MyTeamMembers
  },
  {    
      path: '/find-manage-users',
      name: 'FindManageUsers',
      component: FindManageUsers
  },
  {
    path: '/define-roles-access',
    name: 'DefineRolesAccess',
    component: DefineRolesAccess  
  },
  {    
      path: '/assignment-types',
      name: 'AssignmentTypes',
      component: AssignmentTypes
  },
  {
    path: '/leave-training-types',
    name: 'LeaveTrainingTypes',
    component: LeaveTrainingTypes  
  }
]

export default routes