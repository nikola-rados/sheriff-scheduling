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
    import {commonInfoType} from './types/common';
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

        errorCode = 0;
        errorText = '';
        isCommonDataReady= true;
        sheriffRankList: string[] = []
        currentLocation = {name: "abbotsford", id:"-1"};
       
        mounted() {            
                    this.loadSheriffRankList()
        }

        public loadSheriffRankList()  
        {  
            const url = 'api/managetypes?codeType=SheriffRank'
            const options = {headers:{'Authorization' :'Bearer '+this.token}}
            // console.log(options)
            this.$http.get(url, options)
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