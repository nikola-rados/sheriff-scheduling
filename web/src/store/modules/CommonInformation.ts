import {locationInfoType} from '../../types/common';
import { VuexModule, Module, Mutation, Action } from 'vuex-module-decorators'

@Module({
  namespaced: true
})
class CommonInformation extends VuexModule {
  
  public location: locationInfoType = {name: '', id: ''}

  @Mutation
  public setLocation(location): void {   
    this.location = location
  }

  @Action
  public UpdateLocation(newLocation): void {
    this.context.commit('setLocation', newLocation)
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