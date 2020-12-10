<template>
	<header class="app-header">
		<b-navbar toggleable="lg" class="navbar navbar-expand-lg navbar-dark">    
			<b-navbar-brand class="mt-1" href="https://www2.gov.bc.ca">
					<img 
							class="img-fluid d-none d-lg-block"          
							src="../../public/images/bcid-logo-rev-en.svg"
							width="177"
							height="44"
							alt="B.C. Government Logo"
						/>
					<img
						class="img-fluid d-lg-none"
						src="../../public/images/bcid-symbol-rev.svg"
						width="63"
						height="44"
						alt="B.C. Government Logo"
					/>
			</b-navbar-brand>
			<b-navbar-nav class="mt-1 mx-5">
				<b-nav-item to="/duty-roster" ><div style="display: inline-block; white-space: nowrap;">Duty Roster</div></b-nav-item>         
					<b-nav-item-dropdown text="Shift Schedule" dropdown >
						<b-dropdown-item to="/manage-shift-schedule">Manage Schedule</b-dropdown-item>
						<b-dropdown-item to="/distribute-shift-schedule">Distribute Schedule</b-dropdown-item>
					</b-nav-item-dropdown>
					<b-nav-item-dropdown text="My Team" dropdown >
						<b-dropdown-item to="/team-members">My Team Members</b-dropdown-item>
						<b-dropdown-item to="/define-roles-access">Define Roles & Access</b-dropdown-item>
					</b-nav-item-dropdown>
					<b-nav-item-dropdown text="Manage Types" dropdown :disabled="!hasPermissionToEditManageTypes">
						<b-dropdown-item to="/assignment-types">Assignment Types</b-dropdown-item>
						<b-dropdown-item to="/leave-training-types">Leave & Training Types</b-dropdown-item>
					</b-nav-item-dropdown>
			</b-navbar-nav>
			<b-navbar-nav class="ml-5 mt-1 mr-5">
					<b-input-group v-if="locationDataReady" class="mr-2 mt-1" style="height: 40px">
						<b-input-group-prepend is-text>
							<b-icon icon="globe"></b-icon>
						</b-input-group-prepend>
						<b-form-select
							style="height: 100%;"
							v-model="selectedLocation"                
							:disabled="disableLocationChange"
							@change="UpdateLocation(selectedLocation)"                
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
                        <b-dropdown-text>USER</b-dropdown-text>
						<b-dropdown-item-button variant="danger" @click="signout()">Sign out</b-dropdown-item-button>
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
  <!-- <header class="app-header">
    <b-navbar toggleable="lg" class="navbar navbar-expand-lg navbar-dark">    
      <b-navbar-brand class="mt-1" href="https://www2.gov.bc.ca">
          <img 
              class="img-fluid d-none d-lg-block"          
              src="../../public/images/bcid-logo-rev-en.svg"
              width="177"
              height="44"
              alt="B.C. Government Logo"
            />
          <img
            class="img-fluid d-lg-none"
            src="../../public/images/bcid-symbol-rev.svg"
            width="63"
            height="44"
            alt="B.C. Government Logo"
          />
      </b-navbar-brand>
      <b-navbar-nav class="mt-1 mx-5">
        <b-nav-item to="/duty-roster" ><div style="display: inline-block; white-space: nowrap;">Duty Roster</div></b-nav-item>         
          <b-nav-item-dropdown text="Shift Schedule" dropdown :disabled="!hasPermissionToViewSchedulePages">
            <b-dropdown-item v-if="hasPermissionToViewManageSchedule" to="/manage-shift-schedule">Manage Schedule</b-dropdown-item>
            <b-dropdown-item v-if="hasPermissionToViewDistributeSchedule" to="/distribute-shift-schedule">Distribute Schedule</b-dropdown-item>
          </b-nav-item-dropdown>
          <b-nav-item-dropdown text="My Team" dropdown >
            <b-dropdown-item to="/team-members">My Team Members</b-dropdown-item>
            <b-dropdown-item to="/define-roles-access">Define Roles & Access</b-dropdown-item>
          </b-nav-item-dropdown>
          <b-nav-item-dropdown text="Manage Types" dropdown :disabled="!hasPermissionToEditManageTypes">
            <b-dropdown-item to="/assignment-types">Assignment Types</b-dropdown-item>
            <b-dropdown-item to="/leave-training-types">Leave & Training Types</b-dropdown-item>
          </b-nav-item-dropdown>
      </b-navbar-nav>
      <b-navbar-nav class="ml-5 mt-1 mr-5">
          <b-input-group v-if="locationDataReady" class="mr-2 mt-1" style="height: 40px">
            <b-input-group-prepend is-text>
              <b-icon icon="globe"></b-icon>
            </b-input-group-prepend>
            <b-form-select
              style="height: 100%;"
              v-model="selectedLocation"                
              :disabled="disableLocationChange"
              @change="UpdateLocation(selectedLocation)"                
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
  </header> -->
</template>

<script lang="ts">
	import { Component, Vue } from 'vue-property-decorator';
	import { namespace } from "vuex-class";
	import "@store/modules/CommonInformation";  
	import {commonInfoType, locationInfoType, userInfoType} from '../types/common';  
	const commonState = namespace("CommonInformation");
	import axios from "axios";
	import store from "@/store";

	@Component
	export default class NavigationTopbar extends Vue {

		@commonState.State
		public userDetails!: userInfoType;
		
		@commonState.State
		public commonInfo!: commonInfoType;

		@commonState.Action
		public UpdateCommonInfo!: (newCommonInfo: commonInfoType) => void

		@commonState.State
		public locationList!: locationInfoType[];      
		
		@commonState.State
		public location!: locationInfoType;

		@commonState.Action
		public UpdateLocation!: (newLocation: locationInfoType) => void
		
		disableLocationChange = false;
		userIsAdmin = false;
		selectedLocation ={} as locationInfoType;
		locationDataReady = false;
        hasPermissionToEditManageTypes = false;    
        hasPermissionToViewDistributeSchedule = false;
        hasPermissionToViewManageSchedule = false;
        hasPermissionToViewSchedulePages = false;
        // manageSchedulePagePermissions = ["ViewMyShifts", "ViewAllShiftsAtMyLocation", "ViewAllShifts"]
        
        mounted() {
        this.getModulePermissions();
        this.getCurrentLocation();
        this.disableLocationChange = false;
        }

        public getModulePermissions() {
        this.hasPermissionToEditManageTypes = this.userDetails.permissions.includes("EditTypes");
        this.hasPermissionToViewDistributeSchedule = this.userDetails.permissions.includes("ViewDistributeSchedule");
        // this.hasPermissionToViewManageSchedule = this.manageSchedulePagePermissions.some(permission=> this.userDetails.permissions.includes(permission))
        console.log(this.hasPermissionToViewManageSchedule)
        this.hasPermissionToViewSchedulePages = this.hasPermissionToViewDistributeSchedule || this.hasPermissionToViewManageSchedule;
        }

		public getCurrentLocation()
		{
			let currentLocation;
			if (this.userDetails.homeLocationId) {
				const matchingLocation =this.locationList.filter(locationInfo => {
						if (locationInfo.id == this.userDetails.homeLocationId) {
								return true
						}
				});
				currentLocation = matchingLocation[0]

			} else {
				currentLocation = this.locationList[0]
			}      
			
			this.UpdateLocation(currentLocation);
			this.selectedLocation = this.location;
			if (this.selectedLocation.name.length > 0) this.locationDataReady = true;
        }
        
        public signout(){
            // console.log(document.cookie)
            const url = 'api/auth/logout'
            store.commit('CommonInformation/setToken','');
            store.commit('CommonInformation/setTokenExpiry','');
            axios.get(url)
                .then(response => {
                    if(response.data){
                        console.log(response.data)                                      
                    }                   
                })  
        }

	}
</script>

<style scoped>   

</style>