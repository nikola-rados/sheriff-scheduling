import {commonInfoType, locationInfoType} from '../../types/common';
import { VuexModule, Module, Mutation, Action } from 'vuex-module-decorators'
import axios from "axios";

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
  public UpdateToken(redirectPath): void {

    axios.get('api/auth/token').then(tokenRefreshResponse => {
      this.context.commit('setToken', tokenRefreshResponse.data.access_token);
        
    }).catch((error) => {
        location.replace(redirectPath);
    });
  } 

}

export default CommonInformation 