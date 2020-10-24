<template>    
    <div class="app-outer fill-body" id="app" v-if= "isCommonDataReady" style="user-select: none;">
        <navigation-topbar />
        <router-view></router-view>
        <navigation-footer id="footer" />
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

        errorCode = 0;
        errorText = '';
        isCommonDataReady= false;
        sheriffRankList: sheriffRankInfoType[] = []
        currentLocation;
       
        mounted() {            
            this.loadUserDetails()
        }

        public loadUserDetails() {
            const url = 'api/auth/info'
            this.$http.get(url)
                .then(response => {
                    if(response.data){
                        const userData = response.data;
                        this.UpdateUser({
                            roles: userData.roles,
                            homeLocationId: userData.homeLocationId
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
        
        public getLocations(): void {

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
            //let locationJson: locationJsonType;
            for(const locationJson of locationListJson){                
                const locationInfo: locationInfoType = {id: locationJson.id, name: locationJson.name, regionId: locationJson.regionId, timezone: locationJson.timezone}
                this.locationList.push(locationInfo)
            }                       
            this.UpdateLocationList(_.sortBy(this.locationList,'name'));
        }
        
     }
</script>