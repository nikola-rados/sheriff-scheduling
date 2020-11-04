import { VuexModule, Module, Mutation, Action } from 'vuex-module-decorators'

@Module({
  namespaced: true
})
class ManageTypesInformation extends VuexModule {

  public sortingAssignmentInfo = {} as {prvIndex: number; newIndex: number}
  public sortingLeaveTrainingInfo = {} as {prvIndex: number; newIndex: number}  

  @Mutation
  public setSortingAssignmentInfo(sortingAssignmentInfo): void {   
    this.sortingAssignmentInfo = sortingAssignmentInfo
  }

  @Action
  public UpdateSortingAssignmentInfo(newSortingAssignmentInfo): void {
    this.context.commit('setSortingAssignmentInfo', newSortingAssignmentInfo)
  } 

  @Mutation
  public setSortingLeaveTrainingInfo(sortingLeaveTrainingInfo): void {   
    this.sortingLeaveTrainingInfo = sortingLeaveTrainingInfo
  }

  @Action
  public UpdateSortingLeaveTrainingInfo(newSortingLeaveTrainingInfo): void {
    this.context.commit('setSortingLeaveTrainingInfo', newSortingLeaveTrainingInfo)
  } 

}

export default ManageTypesInformation