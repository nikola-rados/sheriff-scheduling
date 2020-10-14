<template> 
    <b-card no-body class="bg-dark text-white">       
    
        <b-icon-box-arrow-left v-if="displayLoaned" v-b-tooltip.hover.v-warning v-b-tooltip.hover.right.html="awayLocationInfoHtml" font-scale="1.5"></b-icon-box-arrow-left>        
       
           
    </b-card>   
</template>

<script lang="ts">
    import { Component, Vue, Prop } from 'vue-property-decorator';
    import { namespace } from 'vuex-class';
    import {awayLocationInfoType} from '../../../types/MyTeam';
    import {awayLocationsJsontype} from '../../../types/MyTeam/jsonTypes';
    import "@store/modules/CommonInformation";  
    const commonState = namespace("CommonInformation");

    @Component
    export default class UserLocationSummary extends Vue {       

        @Prop({required: true})
        awayLocationJson!: awayLocationsJsontype[];        

        @commonState.State
        public token!: string;        
        
        userAwayLocationInfo: awayLocationInfoType[] = [];
        awayLocationInfoHtml = '';
        displayLoaned = false;

        mounted()
        {
            console.log("mounted");
            this.awayLocationJson = [
                {
                "id": 0,
                "location": {
                    "id": 0,
                    "agencyId": "string",
                    "name": "Victoria Law Courts",
                    "justinCode": "string",
                    "parentLocationId": 0,
                    "expiryDate": "2020-10-13T22:26:36.212Z",
                    "regionId": 0,
                    "concurrencyToken": 0
                },
                "locationId": 297,
                "startDate": "2020-10-15T22:26:36.212Z",
                "endDate": "2020-10-16T22:26:36.212Z",
                "expiryDate": "2020-10-19T22:26:36.212Z",
                "isFullDay": true,
                "sheriffId": "4e2ff3c0-2671-4328-b2c9-1f0ec5e70aba",
                "concurrencyToken": 807
                }
            ];
            this.extractAwayLocationsInfo();
        }

        public extractAwayLocationsInfo()
        {
            if (this.awayLocationJson.length > 0 ) {
                this.displayLoaned = true;
                this.userAwayLocationInfo = [];
                console.log(this.awayLocationJson)            
                for(const awayInfoJson of this.awayLocationJson)
                {
                    const awayInfo = {} as awayLocationInfoType;
                    awayInfo.locationId = awayInfoJson.locationId;
                    awayInfo.name = awayInfoJson.location.name;
                    awayInfo.isFullDay = awayInfoJson.isFullDay;
                    awayInfo.startDate = awayInfoJson.startDate;
                    awayInfo.endDate = awayInfoJson.endDate;
                    this.userAwayLocationInfo.push(awayInfo);
                }
                console.log(this.userAwayLocationInfo)
                this.createTooltipHtml();         
            } else {
                this.displayLoaned = false;
            }
        }

        public createTooltipHtml() {
            this.awayLocationInfoHtml = 'On loan to ';

            for(const awayLocationInfo of this.userAwayLocationInfo)
            {
                this.awayLocationInfoHtml += awayLocationInfo.name + '<br>'
                if(awayLocationInfo.isFullDay) {
                    this.awayLocationInfoHtml += 'Date: ' + Vue.filter('beautify-date-time')(awayLocationInfo.startDate)
                     + ' to ' + Vue.filter('beautify-date-time')(awayLocationInfo.endDate);
                } else {
                    this.awayLocationInfoHtml += 'From ' + Vue.filter('beautify-date-time')(awayLocationInfo.startDate)
                     + ' to ' + Vue.filter('beautify-date-time')(awayLocationInfo.endDate);
                }
            }

            this.displayLoaned = true;            
        }


    }
</script>

 <style scoped>   

    /* .card {
        border: white;
    }

    .btnfile {
        position: relative;
        overflow: hidden;
        border: 1px solid;
        background: lightgrey;
    }
    .btnfile:hover {
        background-color: #103c6b;
        color: white;
    } */
    
   


</style>