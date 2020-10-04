import {commonInfoType, locationInfoType} from '../../types/common';
import { VuexModule, Module, Mutation, Action } from 'vuex-module-decorators'


@Module({
  namespaced: true
})
class CommonInformation extends VuexModule {

  public commonInfo: commonInfoType = {location: {name: '', id: ''}, sheriffRankList: []};

  public token = '';

  @Mutation
  public setCommonInfo(commonInfo): void {   
    this.commonInfo = commonInfo
  }

  @Action
  public UpdateCommonInfo(newCommonInfo): void {
    this.context.commit('setCommonInfo', newCommonInfo)
  }

  @Mutation
  public setToken(token): void {   
    this.token = token
  }

  @Action
  public UpdateToken(newToken): void {
     this.context.commit('setToken', newToken)
  }
  
  

}

export default CommonInformation 