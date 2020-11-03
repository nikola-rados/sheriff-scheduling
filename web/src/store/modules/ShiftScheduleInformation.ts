import { shiftRangeInfoType, sheriffAvailabilityInfoType } from '@/types/ShiftSchedule';
import { VuexModule, Module, Mutation, Action } from 'vuex-module-decorators'

@Module({
  namespaced: true
})
class ShiftScheduleInformation extends VuexModule {

  public shiftRangeInfo = {} as shiftRangeInfoType;
  public sheriffsAvailabilityInfo = [] as sheriffAvailabilityInfoType[];  

  @Mutation
  public setShiftRangeInfo(shiftRangeInfo): void {   
    this.shiftRangeInfo = shiftRangeInfo
  }

  @Action
  public UpdateShiftRangeInfo(newShiftRangeInfo): void {
    this.context.commit('setShiftRangeInfo', newShiftRangeInfo)
  }

  @Mutation
  public setSheriffsAvailabilityInfo(sheriffsAvailabilityInfo): void {   
    this.sheriffsAvailabilityInfo = sheriffsAvailabilityInfo
  }

  @Action
  public UpdateSheriffsAvailabilityInfo(newSheriffsAvailabilityInfo): void {
    this.context.commit('setSheriffsAvailabilityInfo', newSheriffsAvailabilityInfo)
  }
}

export default ShiftScheduleInformation