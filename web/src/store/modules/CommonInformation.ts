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
  

}

export default CommonInformation 