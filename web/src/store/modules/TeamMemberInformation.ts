import {teamMemberInfoType} from '../../types/MyTeam';
import { VuexModule, Module, Mutation, Action } from 'vuex-module-decorators'

@Module({
  namespaced: true
})
class TeamMemberInformation extends VuexModule {

  public userToEdit = {} as teamMemberInfoType

  @Mutation
  public setUserToEdit(userToEdit): void {   
    this.userToEdit = userToEdit
  }

  @Action
  public UpdateUserToEdit(newUserToEdit): void {
    this.context.commit('setUserToEdit', newUserToEdit)
  } 

}

export default TeamMemberInformation