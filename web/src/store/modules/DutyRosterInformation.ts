import { dutyRangeInfoType, myTeamShiftInfoType} from '@/types/DutyRoster';
import { VuexModule, Module, Mutation, Action } from 'vuex-module-decorators'

@Module({
  namespaced: true
})
class DutyRosterInformation extends VuexModule {

  public dutyRangeInfo = {} as dutyRangeInfoType;
  public shiftAvailabilityInfo: myTeamShiftInfoType[] = [];
  
  @Mutation
  public setDutyRangeInfo(dutyRangeInfo): void {   
    this.dutyRangeInfo = dutyRangeInfo
  }

  @Action
  public UpdateDutyRangeInfo(newDutyRangeInfo): void {
    this.context.commit('setDutyRangeInfo', newDutyRangeInfo)
  }

  @Mutation
  public setShiftAvailabilityInfo(shiftAvailabilityInfo): void {   
    this.shiftAvailabilityInfo = shiftAvailabilityInfo;
  }

  @Action
  public UpdateShiftAvailabilityInfo(newShiftAvailabilityInfo): void {
    this.context.commit('setShiftAvailabilityInfo', newShiftAvailabilityInfo)
  }
  
}

export default DutyRosterInformation