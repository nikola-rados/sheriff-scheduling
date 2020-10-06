import {commonInfoType, locationInfoType, userInfoType} from '../../types/common';
import { VuexModule, Module, Mutation, Action } from 'vuex-module-decorators'


@Module({
  namespaced: true
})
class CommonInformation extends VuexModule {

  public commonInfo: commonInfoType = {sheriffRankList: []};

  public location: locationInfoType = {name: '', id: ''};

  public userDetails: userInfoType = {roles: [], homeLocationId: ''}

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
  public setLocation(location): void {   
    this.location = location
  }

  @Action
  public UpdateLocation(newLocation): void {
    this.context.commit('setLocation', newLocation)
  }

  @Mutation
  public setUser(user): void {   
    this.userDetails = user
  }

  @Action
  public UpdateUser(newUser): void {
    this.context.commit('setUser', newUser)
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