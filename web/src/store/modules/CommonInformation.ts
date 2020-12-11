import {localTimeInfoType, commonInfoType, locationInfoType, userInfoType} from '../../types/common';
import { VuexModule, Module, Mutation, Action } from 'vuex-module-decorators'


@Module({
  namespaced: true
})
class CommonInformation extends VuexModule {

  public commonInfo: commonInfoType = {sheriffRankList: []};

  public location: locationInfoType = {name: '', id: 0, regionId:0, timezone:''};

  public locationList: locationInfoType[] = [];

  public userDetails: userInfoType = {roles: [], homeLocationId: 0, permissions: []}

  public displayFooter = true;

  public displayHeader = true;

  public token = '';
  public tokenExpiry: Date = new Date();

  public localTime = {} as localTimeInfoType;


  @Mutation
  public setLocalTime(localTime: localTimeInfoType): void {   
    this.localTime = localTime
  }

  @Action
  public UpdateLocalTime(newLocalTime: localTimeInfoType): void {
    this.context.commit('setLocalTime', newLocalTime)
  } 


  @Mutation
  public setDisplayHeader(displayHeader: boolean): void {   
    this.displayHeader = displayHeader
  }

  @Action
  public UpdateDisplayHeader(newDisplayHeader: boolean): void {
    this.context.commit('setDisplayHeader', newDisplayHeader)
  }
  
  @Mutation
  public setDisplayFooter(displayFooter: boolean): void {   
    this.displayFooter = displayFooter
  }

  @Action
  public UpdateDisplayFooter(newDisplayFooter: boolean): void {
    this.context.commit('setDisplayFooter', newDisplayFooter)
  } 

  @Mutation
  public setCommonInfo(commonInfo): void {   
    this.commonInfo = commonInfo
  }

  @Action
  public UpdateCommonInfo(newCommonInfo): void {
    this.context.commit('setCommonInfo', newCommonInfo)
  } 

  @Mutation
  public setLocationList(locationList): void {   
    this.locationList = locationList
  }

  @Action
  public UpdateLocationList(newLocationList): void {
    this.context.commit('setLocationList', newLocationList)
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

  @Mutation
  public setTokenExpiry(tokenExpiry): void {
    this.tokenExpiry = tokenExpiry
  }

  @Action
  public UpdateToken(newToken): void {
     this.context.commit('setToken', newToken)
  }
  
  

}

export default CommonInformation 