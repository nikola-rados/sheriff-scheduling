import { dutyRangeInfoType} from '@/types/DutyRoster';
import { VuexModule, Module, Mutation, Action } from 'vuex-module-decorators'

@Module({
  namespaced: true
})
class DutyRosterInformation extends VuexModule {

  public dutyRangeInfo = {} as dutyRangeInfoType;
  
  @Mutation
  public setDutyRangeInfo(dutyRangeInfo): void {   
    this.dutyRangeInfo = dutyRangeInfo
  }

  @Action
  public UpdateDutyRangeInfo(newDutyRangeInfo): void {
    this.context.commit('setDutyRangeInfo', newDutyRangeInfo)
  }

  
}

export default DutyRosterInformation