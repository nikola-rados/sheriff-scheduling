import { shiftRangeInfoType } from '@/types/ShiftSchedule';
import { VuexModule, Module, Mutation, Action } from 'vuex-module-decorators'

@Module({
  namespaced: true
})
class ShiftScheduleInformation extends VuexModule {

  public shiftRangeInfo = {} as shiftRangeInfoType; 

  @Mutation
  public setShiftRangeInfo(shiftRangeInfo): void {   
    this.shiftRangeInfo = shiftRangeInfo
  }

  @Action
  public UpdateShiftRangeInfo(newShiftRangeInfo): void {
    this.context.commit('setShiftRangeInfo', newShiftRangeInfo)
  }
}

export default ShiftScheduleInformation