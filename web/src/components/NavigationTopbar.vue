<template>
	<header class="app-header">
		<b-navbar toggleable="lg" class="navbar navbar-expand-lg navbar-dark">    
			<b-navbar-brand v-if="displayHeader" class="my-0">
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
            <b-navbar-nav v-else style="height:3rem"/>
            
			<b-navbar-nav v-if="displayHeader" class="ml-3 my-0 mr-3">
                <b-nav-item-dropdown right class="my-0" menu-class="bg-info" dropdown>
                    <template v-slot:button-content>
                        <b-icon icon="globe"></b-icon>
                    </template>
                    <b-dropdown-item 
                        style="width:auto;"
                        v-for="location in locationList"
                        :key="location.id"                  
                        :value="location"
                        :disabled="disableLocationChange"
                        @click="UpdateLocation(location)">
                        {{ location.name }}
                    </b-dropdown-item>
                </b-nav-item-dropdown>
            </b-navbar-nav>

            <b-navbar-nav v-if="displayHeader" class="ml-3 my-0 mr-3">
                <b-nav-item-dropdown right class="my-0" menu-class="bg-info" dropdown>
                    <template v-slot:button-content>
                        <b-icon-list></b-icon-list>
                    </template>
                    <b-dropdown-item to="/manage-duty-roster" :disabled="!hasPermissionToViewDutyRosterPage">Manage Duties</b-dropdown-item>
                    <b-dropdown-item to="/view-duty-roster" :disabled="!hasPermissionToViewDutyRosterPage">View Duties</b-dropdown-item>
                    <b-dropdown-divider></b-dropdown-divider>

                    <b-dropdown-item v-if="hasPermissionToViewManageSchedule" to="/manage-shift-schedule" :disabled="!hasPermissionToViewSchedulePages">Manage Shift Schedule</b-dropdown-item>
                    <b-dropdown-item v-if="hasPermissionToViewDistributeSchedule" to="/distribute-shift-schedule" :disabled="!hasPermissionToViewSchedulePages">Distribute Shift Schedule</b-dropdown-item>
                    <b-dropdown-divider></b-dropdown-divider>

                    <b-dropdown-item v-if="hasPermissionToViewProfilePage" to="/team-members" :disabled="!hasPermissionToViewTeamPages">My Team Members</b-dropdown-item>
                    <b-dropdown-item v-if="hasPermissionToViewRolesPage" to="/define-roles-access" :disabled="!hasPermissionToViewTeamPages">Define Roles & Access</b-dropdown-item>
                    <b-dropdown-divider></b-dropdown-divider>

                    <b-dropdown-item to="/assignment-types" :disabled="!hasPermissionToEditManageTypes">Assignment Types</b-dropdown-item>
                    <b-dropdown-item to="/leave-training-types" :disabled="!hasPermissionToEditManageTypes">Leave & Training Types</b-dropdown-item>
                    <b-dropdown-divider></b-dropdown-divider>

                    <b-dropdown-text class="text-primary"><b-icon-person/> {{userDetails.firstName}} {{userDetails.lastName}}</b-dropdown-text>
                    <b-dropdown-item-button variant="danger" @click="signout()"><b-icon-box-arrow-right/> Sign out</b-dropdown-item-button>
                </b-nav-item-dropdown>
			</b-navbar-nav>
		</b-navbar>    
	</header>
</template>

<script lang="ts">
	import { Component, Vue} from 'vue-property-decorator';
	import { namespace } from "vuex-class";
	import "@store/modules/CommonInformation";  
	import {commonInfoType, locationInfoType, userInfoType} from '../types/common';  
	const commonState = namespace("CommonInformation");
    import store from "@/store";

	@Component
	export default class NavigationTopbar extends Vue {

        @commonState.State
        public displayHeader!: boolean;

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
        hasPermissionToViewDutyRosterPage = false;
        hasPermissionToViewProfilePage = false;
        hasPermissionToViewRolesPage = false;
        hasPermissionToViewTeamPages = false;
        
        mounted() {
            this.getModulePermissions();
            this.getCurrentLocation();
            this.disableLocationChange = false;
        }

        public getModulePermissions() {
            this.hasPermissionToEditManageTypes = this.userDetails.permissions.includes("EditTypes");
            this.hasPermissionToViewDistributeSchedule = this.userDetails.permissions.includes("ViewDistributeSchedule");
            this.hasPermissionToViewManageSchedule = this.userDetails.permissions.includes("ViewShifts");
            this.hasPermissionToViewSchedulePages = this.hasPermissionToViewDistributeSchedule || this.hasPermissionToViewManageSchedule;
            this.hasPermissionToViewDutyRosterPage = this.userDetails.permissions.includes("ViewDutyRoster");
            this.hasPermissionToViewRolesPage = this.userDetails.permissions.includes("ViewRoles");
            const hasPermissionToViewProvinceProfiles = this.userDetails.permissions.includes("ViewProvince");
            const hasPermnissionToViewProfilesInOwnLocation = this.userDetails.permissions.includes("ViewHomeLocation") || this.userDetails.permissions.includes("ViewAssignedLocation");
            const hasPermnissionToViewRegionProfiles = this.userDetails.permissions.includes("ViewRegion");
            this.hasPermissionToViewProfilePage = hasPermissionToViewProvinceProfiles || hasPermnissionToViewProfilesInOwnLocation || hasPermnissionToViewRegionProfiles;
            this.hasPermissionToViewTeamPages = this.hasPermissionToViewProfilePage || this.hasPermissionToViewRolesPage;
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
			if (this.selectedLocation && this.selectedLocation.name.length > 0) this.locationDataReady = true;
        }
        
        public signout(){            
            store.commit('CommonInformation/setToken','');
            store.commit('CommonInformation/setTokenExpiry','');
            Vue.$cookies.set("logout","1",)
            window.location.replace(`${process.env.BASE_URL}api/auth/logout`);            
        }

	}
</script>

<style scoped>   

    ul >>> .dropdown-menu {
        width: 250px;
    }
</style>