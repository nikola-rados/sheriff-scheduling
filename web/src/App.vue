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

        errorCode = 0;
        errorText = '';
        isCommonDataReady= false;
        sheriffRankList: string[] = []
        currentLocation = {name: "abbotsford", id:"1"};

        mounted() {

            this.loadRequiredInfo();
            
        }

        public loadRequiredInfo() {

            this.errorCode=0;            
            this.$http.get('/api/info')
            .then(Response => Response.json(), err => {this.errorCode= err.status;this.errorText= err.statusText;console.log(err);}        
            ).then(data => {
                if(data){
                    this.extractSheriffRankInfo(data);
                    if(this.commonInfo.sheriffRankList.length>0)
                    {                    
                        this.isCommonDataReady = true;                    
                    }
                }                
            });

        }

        public loadSheriffRankList() {
            this.errorCode=0;            
            this.$http.get('/api/managetypes?codeType=SheriffRank')
            .then(Response => Response.json(), err => {this.errorCode= err.status;this.errorText= err.statusText;console.log(err);}        
            ).then(data => {
                if(data){
                    this.extractSheriffRankInfo(data);
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