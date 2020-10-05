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
    import {sheriffRankJsonType} from './types/common/jsonTypes'
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

        errorCode = 0;
        errorText = '';
        isCommonDataReady= false;
        sheriffRankList: string[] = []
        currentLocation = {name: "abbotsford", id:"-1"};
       
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
                            homeLocationId: this.currentLocation.id
                        })                        
                        this.UpdateLocation(this.currentLocation) 
                        this.loadSheriffRankList()                        
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
                        console.log(this.commonInfo.sheriffRankList.length + this.location.id + this.userDetails.homeLocationId)

                        if(this.commonInfo.sheriffRankList.length>0 && this.location.id && this.userDetails.homeLocationId)
                        {                              
                            this.isCommonDataReady = true;
                            console.log("data ready")
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
        
     }
</script>