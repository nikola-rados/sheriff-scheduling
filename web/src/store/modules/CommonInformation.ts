import {commonInfoType, locationInfoType} from '../../types/common';
import { VuexModule, Module, Mutation, Action } from 'vuex-module-decorators'

@Module({
  namespaced: true
})
class CommonInformation extends VuexModule {

  public commonInfo: commonInfoType = {location: {name: '', id: ''}, sheriffRankList: []}

  @Mutation
  public setCommonInfo(commonInfo): void {   
    this.commonInfo = commonInfo
  }

  @Action
  public UpdateCommonInfo(newCommonInfo): void {
    this.context.commit('setCommonInfo', newCommonInfo)
  }

  public locationList: locationInfoType[] = [];

  @Mutation
  public setLocationList(locationList): void {   
    this.locationList = locationList
  }

  @Action
  public UpdateLocationList(newLocationList): void {
    this.context.commit('setLocationList', newLocationList)
  }
  

}

export default CommonInformation 