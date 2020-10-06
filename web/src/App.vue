<template>    
    <div class="app-outer fill-body" id="app" v-if= "isCommonDataReady">
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
    import {commonInfoType, locationInfoType, userInfoType} from './types/common';
    import {sheriffRankJsonType, locationJsonType} from './types/common/jsonTypes'
    import "@store/modules/CommonInformation";
    import store from "./store";  
    const commonState = namespace("CommonInformation");

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
        public token!: string;

        @commonState.Action
        public UpdateToken!: (newToken: string) => void

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
        sheriffRankList: string[] = []
        currentLocation;
        //  = {name: "abbotsford", id:"-1"};
       
        mounted() {            
            this.loadUserDetails()
        }

        public loadUserDetails() {
            const url = 'api/auth/info'
            const options = {headers:{'Authorization' :'Bearer '+this.token}}
            this.$http.get(url, options)
                .then(response => {
                    if(response.data){
                        const userData = response.data;
                        this.UpdateUser({
                            roles: userData.roles,
                            homeLocationId: 200
                        }) 
                        this.getLocations()                        
                    }                   
                })  
        }

        public loadSheriffRankList()  
        {  
            const url = 'api/managetypes?codeType=SheriffRank'
            const options = {headers:{'Authorization' :'Bearer '+this.token}}
            this.$http.get(url, options)
                .then(response => {
                    if(response.data){
                        this.extractSheriffRankInfo(response.data);
                        if(this.commonInfo.sheriffRankList.length>0 && 
                        this.userDetails.homeLocationId && this.locationList.length>0)
                        {                              
                            this.isCommonDataReady = true;
                        }
                    }                   
                })          
        }        

        public extractSheriffRankInfo(sheriffRankList)
        {
            let sheriffRank: sheriffRankJsonType;

            for(sheriffRank of sheriffRankList)
            {                
                this.sheriffRankList.push(sheriffRank.description)
            }                       
            this.UpdateCommonInfo({
                sheriffRankList: this.sheriffRankList 
            })
        }
        
        public getLocations(): void {

            const url = 'api/location'
            const options = {headers:{'Authorization' :'Bearer '+this.token}}
            this.$http.get(url, options)
                .then(response => {
                    if(response.data){
                        this.extractLocationInfo(response.data);
                        this.loadSheriffRankList();
                    }                   
                }) 
        }
        
        public extractLocationInfo(locationListJson)
        {
            let locationJson: locationJsonType;

            for(locationJson of locationListJson)
            {
                const locationInfo: locationInfoType = {id: locationJson.id, name: locationJson.name}
                this.locationList.push(locationInfo)
            }                       
            this.UpdateLocationList(this.locationList);
        }
        
     }
</script>