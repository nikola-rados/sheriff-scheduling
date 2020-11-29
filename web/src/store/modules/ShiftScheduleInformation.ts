import { shiftRangeInfoType, sheriffAvailabilityInfoType, shiftInfoType, distributeTeamMemberInfoType } from '@/types/ShiftSchedule';
import { VuexModule, Module, Mutation, Action } from 'vuex-module-decorators'

@Module({
  namespaced: true
})
class ShiftScheduleInformation extends VuexModule {

  public shiftRangeInfo = {} as shiftRangeInfoType;
  public sheriffsAvailabilityInfo = [] as sheriffAvailabilityInfoType[];
  public selectedShifts = [] as string[];
  public teamMemberList = [] as distributeTeamMemberInfoType[];

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

  @Mutation
  public setSelectedShifts(selectedShifts): void {   
    this.selectedShifts = selectedShifts
  }

  @Action
  public UpdateSelectedShifts(newSelectedShifts): void {
    this.context.commit('setSelectedShifts', newSelectedShifts)
  }

  @Mutation
  public setTeamMemberList(teamMemberList): void {   
    this.teamMemberList = teamMemberList
  }

  @Action
  public UpdateTeamMemberList(newTeamMemberList): void {
    this.context.commit('setTeamMemberList', newTeamMemberList)
  }
}

export default ShiftScheduleInformation