<template>    
    <div class="app-outer fill-body" id="app" v-if= "isCommonDataReady">
        <navigation-topbar />
        <b-button @click="go()"> Login </b-button>
        <b-button @click="getToken()"> Token </b-button>
        <router-view></router-view>
        <navigation-footer id="footer" />
    </div>
</template>

<script lang="ts">
    import NavigationTopbar from "@components/NavigationTopbar.vue";
    import NavigationFooter from "@components/NavigationFooter.vue";
    import { Component, Vue } from 'vue-property-decorator';
    import { namespace } from 'vuex-class';
    import {commonInfoType} from './types/common';
    import {sheriffRankJsonType} from './types/common/jsonTypes'
    import "@store/modules/CommonInformation";  
    const commonState = namespace("CommonInformation");
    import axios from "axios";

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

        errorCode = 0;
        errorText = '';
        isCommonDataReady= true;
        sheriffRankList: string[] = []
        currentLocation = {name: "abbotsford", id:"1"};

        public go()
        {
            location.replace('api/auth/login?redirectUri='+this.$route.fullPath);
        }

        getToken()
        {
            const url = 'api/auth/token'
            axios.get(url)
               .then(response => {
                    if(response.data){
                        console.log(response.data.access_token)
                        //localStorage.setItem('token', response.data.access_token);
                        this.UpdateToken(response.data.access_token)
                        this.loadSheriffRankList()
                    }                    
                });
        }
        mounted() { 
            //location.replace('api/auth/login?redirectUri='+this.$route.fullPath);
            this.errorCode=0;
            const url = 'api/auth/token'
            axios.get(url)
               .then(response => {
                    if(response.data){
                        console.log(response.data.access_token)
                        //localStorage.setItem('token', response.data.access_token);
                        this.UpdateToken(response.data.access_token)
                        this.loadSheriffRankList()
                    }                    
                });
        }

        public loadSheriffRankList()  
        {  
            const url = 'api/managetypes?codeType=SheriffRank'
            const options = {headers:{'Authorization' :'Bearer '+this.token}}
            console.log(options)
            axios.get(url, options)
                .then(response => {
                    if(response.data){
                        console.log(response.data)
                        this.extractSheriffRankInfo(response.data);
                        if(this.commonInfo.sheriffRankList.length>0)
                        {                              
                            this.isCommonDataReady = true;
                        }
                    }                   
                });           
        }        

        public extractSheriffRankInfo(sheriffRankList)
        {
            let sheriffRank: sheriffRankJsonType;

            for(sheriffRank of sheriffRankList)
            {                
                this.sheriffRankList.push(sheriffRank.description)
            }
            
            this.UpdateCommonInfo({
                location: this.currentLocation,
                sheriffRankList: this.sheriffRankList 
            })
        }       
        
     }
</script>