<template>    
    <div class="app-outer fill-body" id="app" v-if= "isCommonDataReady" style="user-select: none;">
        <navigation-topbar />
        <router-view></router-view>
        <navigation-footer id="footer" v-if="displayFooter"/>
    </div>
</template>

<script lang="ts">
    import NavigationTopbar from "@components/NavigationTopbar.vue";
    import NavigationFooter from "@components/NavigationFooter.vue";
    import { Component, Vue } from 'vue-property-decorator';
    import { namespace } from 'vuex-class';
    import {commonInfoType, locationInfoType, sheriffRankInfoType, userInfoType} from './types/common';
    import {sheriffRankJsonType, locationJsonType} from './types/common/jsonTypes'
    import "@store/modules/CommonInformation";
    import store from "./store";  
    const commonState = namespace("CommonInformation");
    import * as _ from 'underscore';

    @Component({
        components: {
            NavigationTopbar,
            NavigationFooter
        }
    })    
    export default class App extends Vue {

        @commonState.State
        public commonInfo!: commonInfoType;

        @commonState.Action
        public UpdateCommonInfo!: (newCommonInfo: commonInfoType) => void

        @commonState.State
        public location!: locationInfoType;

        @commonState.Action
        public UpdateLocation!: (newLocation: locationInfoType) => void

        @commonState.State
        public userDetails!: userInfoType;

        @commonState.Action
        public UpdateUser!: (newUser: userInfoType) => void

        @commonState.State
        public locationList!: locationInfoType[];
        
        @commonState.Action
        public UpdateLocationList!: (newLocationList: locationInfoType[]) => void

        @commonState.State
        public displayFooter!: boolean;

        errorCode = 0;
        errorText = '';
        isCommonDataReady= false;
        sheriffRankList: sheriffRankInfoType[] = []
        currentLocation;
       
        mounted() {
            this.isCommonDataReady = false;            
            this.loadUserDetails();
        }

        public loadUserDetails() {
            const url = 'api/auth/info'
            this.$http.get(url)
                .then(response => {
                    if(response.data){
                        const userData = response.data;
                        console.log(response.data)
                        const permissions =    [
                            "Login",
                            "ViewOwnProfile",
                            "ViewProfilesInOwnLocation",
                            "ViewProfilesInAllLocation",
                            "CreateUsers",
                            "ExpireUsers",
                            "EditUsers",
                            "ViewRoles",
                            "CreateAndAssignRoles",
                            "ExpireRoles",
                            "EditRoles",
                            "ViewMyShifts",
                            "ViewAllShiftsAtMyLocation",
                            "ViewAllShifts",
                            "CreateAndAssignShifts",
                            "ExpireShifts",
                            "EditShifts",
                            "ViewDistributeSchedule",
                            "ViewAssignedLocation",
                            "ViewRegion",
                            "ViewProvince",
                            "ExpireLocation",
                            "ViewHomeLocation",
                            "ViewAssignments",
                            "CreateAssignments",
                            "EditAssignments",
                            "ExpireAssignments",
                            "ViewDuties",
                            "CreateAndAssignDuties",
                            "EditDuties",
                            "ExpireDuties",
                            "ImportShifts",
                            "ExpireTypes",
                            "CreateTypes",
                            "EditTypes"
                        ]
                        this.UpdateUser({
                            roles: userData.roles,
                            homeLocationId: userData.homeLocationId,
                            permissions: permissions//userData.permissions
                        }) 
                        this.getLocations()                        
                    }                   
                })  
        }

        public loadSheriffRankList(){  
            const url = 'api/managetypes?codeType=SheriffRank'
            this.$http.get(url)
                .then(response => {
                    if(response.data){
                        this.extractSheriffRankInfo(response.data);
                        if(this.commonInfo.sheriffRankList.length>0 && 
                        this.userDetails.roles.length>0 && this.locationList.length>0)
                        {                              
                            this.isCommonDataReady = true;
                        }
                    }                   
                })          
        }        

        public extractSheriffRankInfo(sheriffRankList){

            let sheriffRank: sheriffRankJsonType;

            for(sheriffRank of sheriffRankList){                
                this.sheriffRankList.push({id: Number(sheriffRank.id), name: sheriffRank.description})
            }                       
            this.UpdateCommonInfo({
                sheriffRankList: this.sheriffRankList 
            })
        }
        
        public getLocations() {
            const url = 'api/location'
            this.$http.get(url)
                .then(response => {
                    if(response.data){
                        this.extractLocationInfo(response.data);
                        this.loadSheriffRankList();
                    }                   
                }) 
        }
        
        public extractLocationInfo(locationListJson){            
            const locations: locationInfoType[] = [];
            for(const locationJson of locationListJson){                
                const locationInfo: locationInfoType = {id: locationJson.id, name: locationJson.name, regionId: locationJson.regionId, timezone: locationJson.timezone}
                locations.push(locationInfo)
            }                       
            this.UpdateLocationList(_.sortBy(locations,'name'));
        }
        
     }
</script>