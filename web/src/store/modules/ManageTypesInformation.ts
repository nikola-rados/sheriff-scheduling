import {teamMemberInfoType} from '../../types/MyTeam';
import { VuexModule, Module, Mutation, Action } from 'vuex-module-decorators'

@Module({
  namespaced: true
})
class ManageTypesInformation extends VuexModule {

  public sortingAssignmentInfo = {} as {prvIndex: number; newIndex: number} 

  @Mutation
  public setSortingAssignmentInfo(sortingAssignmentInfo): void {   
    this.sortingAssignmentInfo = sortingAssignmentInfo
  }

  @Action
  public UpdateSortingAssignmentInfo(newSortingAssignmentInfo): void {
    this.context.commit('setSortingAssignmentInfo', newSortingAssignmentInfo)
  } 

}

export default ManageTypesInformation