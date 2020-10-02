<template>
  <header class="app-header">
    <b-navbar toggleable="lg" class="navbar navbar-expand-lg navbar-dark">    
      <b-navbar-brand class="mt-1" href="https://www2.gov.bc.ca">
          <img 
              class="img-fluid d-none d-md-block"          
              src="../../public/images/bcid-logo-rev-en.svg"
              width="177"
              height="44"
              alt="B.C. Government Logo"
            />
          <img
            class="img-fluid d-md-none"
            src="../../public/images/bcid-symbol-rev.svg"
            width="63"
            height="44"
            alt="B.C. Government Logo"
          />
      </b-navbar-brand>
      <b-navbar-nav class="mt-1 mx-5">
          <b-nav-item-dropdown text="Duty Roster" dropdown >
            <b-dropdown-item to="/duty-roster">Duty Roster</b-dropdown-item>
            <b-dropdown-item to="/duty-roster-setup">Set-Up</b-dropdown-item>
          </b-nav-item-dropdown>        
          <b-nav-item to="/assignment" style="width: 100%;">Add Assignment</b-nav-item>
          <b-nav-item-dropdown text="Shift Schedule" dropdown >
            <b-dropdown-item to="/manage-shift-schedule">Manage Schedule</b-dropdown-item>
            <b-dropdown-item to="/distribute-shift-schedule">Distribute Schedule</b-dropdown-item>
          </b-nav-item-dropdown>
          <b-nav-item-dropdown text="My Team" dropdown >
            <b-dropdown-item to="/team-members">My Team Members</b-dropdown-item>
            <b-dropdown-item to="/find-manage-users">Find/Manage Users</b-dropdown-item>
            <b-dropdown-item to="/define-roles-access">Define Roles & Access</b-dropdown-item>
          </b-nav-item-dropdown>
          <b-nav-item-dropdown text="Manage Types" dropdown >
            <b-dropdown-item to="/assignment-types">Assignment Types</b-dropdown-item>
            <b-dropdown-item to="/leave-training-types">Leave & Training Types</b-dropdown-item>
          </b-nav-item-dropdown>
      </b-navbar-nav>
      <b-navbar-nav class="ml-5 mt-1 mr-5">
          <b-input-group class="mr-2 mt-1" style="height: 40px">
            <b-input-group-prepend is-text>
              <b-icon icon="globe"></b-icon>
            </b-input-group-prepend>
            <b-form-select
              style="height: 100%;"
              v-model="selectedLocation"                
              :disabled="disableLocationChange"
              @change="updateLocation(selectedLocation)"                
              >
              <b-form-select-option
              v-for="location in locationList"
              :key="location.id"                  
              :value="location">{{location.name}}
              </b-form-select-option>                  
            </b-form-select>
          </b-input-group>
          <b-nav-item-dropdown class="mb-3" dropdown>
            <template v-slot:button-content>
              <b-icon-person-circle></b-icon-person-circle>
            </template>
            <b-dropdown-item-button>PlaceHolder</b-dropdown-item-button>
          </b-nav-item-dropdown>
          <b-nav-item-dropdown class="mb-3 mr-5" dropdown>
            <template v-slot:button-content>
              <b-icon-gear-fill></b-icon-gear-fill>
            </template>
            <b-dropdown-item-button>PlaceHolder</b-dropdown-item-button>
          </b-nav-item-dropdown>
      </b-navbar-nav>
    </b-navbar>    
  </header>
</template>

<script lang="ts">
  import { Component, Vue } from 'vue-property-decorator';
  import { namespace } from "vuex-class";
  import "@store/modules/CommonInformation";  
  import {commonInfoType, locationInfoType} from '../types/common';  
  const commonState = namespace("CommonInformation");


  @Component
  export default class NavigationTopbar extends Vue {

    @commonState.State
    public commonInfo!: commonInfoType;

    @commonState.Action
    public UpdateCommonInfo!: (newCommonInfo: commonInfoType) => void
    
    // locationList: locationInfoType[] = [];

    @commonState.State
    public locationList!: locationInfoType[];
    
    @commonState.Action
    public UpdateLocationList!: (newLocationList: locationInfoType[]) => void
    
    selectedLocation: locationInfoType = {name: "", id:""};
    disableLocationChange = false;

    mounted() {
      //TODO: determine based on user's location
      // this.UpdateLocation({name: "abbotsford", id:"1"});
      this.selectedLocation = this.commonInfo.location;
      //TODO: determine based on user role
      // this.disableLocationChange = true;
      this.getLocations();
    }

    public getLocations(): void {
      //TODO: make call to GET all locations
      this.locationList = [{name: "abbotsford", id:"1"}, {name: "kelowna", id: "2"}]
      
    }

    public updateLocation (): void {
      this.UpdateCommonInfo({
        location: this.selectedLocation,
        sheriffRankList: this.commonInfo.sheriffRankList 
      })
    }
    
    

  }
</script>

<style scoped>   

</style>