<template> 
    <b-card v-if="displayLoaned" no-body class="bg-dark text-white"> 
        <b-icon-box-arrow-left :id="'awayLocationIcon'+index" font-scale="1.5"></b-icon-box-arrow-left>
            <b-tooltip :target="'awayLocationIcon'+index" variant="warning" show.sync ="true" triggers="hover">
                <h2 class="text-danger">On loan to:</h2>                
                <b-table  
                    :items="userAwayLocationInfo"
                    :fields="userAwayLocationFields"                
                    borderless
                    striped
                    small 
                    responsive="sm"
                    class="my-0 py-0"
                    >
                </b-table>                                        
            </b-tooltip>
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
        index!: number;

        @Prop({required: true})
        awayLocationJson!: awayLocationsJsontype[];        

        @commonState.State
        public token!: string;        
        
        userAwayLocationInfo: awayLocationInfoType[] = [];
        awayLocationInfoHtml = '';
        displayLoaned = false;
        userAwayLocationFields = [
          { key: 'name', label: 'Location', thClass: 'text-primary h3', tdClass: 'font-weight-bold'},
          { key: 'startDate', label: 'Start', thClass: 'text-primary h3'},
          { key: 'endDate', label: 'End', thClass: 'text-primary h3'}
        ];

        mounted()
        {            
            // this.awayLocationJson = [
            //     {
            //     "id": 0,
            //     "location": {
            //         "id": 0,
            //         "agencyId": "string",
            //         "name": "Victoria Law Courts",
            //         "justinCode": "string",
            //         "parentLocationId": 0,
            //         "expiryDate": "2020-10-13T22:26:36.212Z",
            //         "regionId": 0,
            //         "concurrencyToken": 0
            //     },
            //     "locationId": 297,
            //     "startDate": "2020-10-15T22:26:36.212Z",
            //     "endDate": "2020-10-16T22:26:36.212Z",
            //     "expiryDate": "2020-10-19T22:26:36.212Z",
            //     "isFullDay": true,
            //     "sheriffId": "4e2ff3c0-2671-4328-b2c9-1f0ec5e70aba",
            //     "concurrencyToken": 807
            //     },
            //     {
            //     "id": 0,
            //     "location": {
            //         "id": 0,
            //         "agencyId": "string",
            //         "name": "Victoria Law Courts",
            //         "justinCode": "string",
            //         "parentLocationId": 0,
            //         "expiryDate": "2020-10-13T22:26:36.212Z",
            //         "regionId": 0,
            //         "concurrencyToken": 0
            //     },
            //     "locationId": 297,
            //     "startDate": "2020-10-15T22:26:36.212Z",
            //     "endDate": "2020-10-16T22:26:36.212Z",
            //     "expiryDate": "2020-10-19T22:26:36.212Z",
            //     "isFullDay": false,
            //     "sheriffId": "4e2ff3c0-2671-4328-b2c9-1f0ec5e70aba",
            //     "concurrencyToken": 807
            //     }
            // ];
            this.extractAwayLocationsInfo();
        }

        public extractAwayLocationsInfo()
        {
            if (this.awayLocationJson.length > 0 ) {
                this.displayLoaned = true;
                this.userAwayLocationInfo = [];          
                for(const awayInfoJson of this.awayLocationJson)
                {
                    const awayInfo = {} as awayLocationInfoType;
                    awayInfo.locationId = awayInfoJson.locationId;
                    awayInfo.name = awayInfoJson.location.name;
                    awayInfo.isFullDay = awayInfoJson.isFullDay;
                    if (awayInfoJson.isFullDay) {
                        awayInfo.startDate = Vue.filter('beautify-date')(awayInfoJson.startDate);
                        awayInfo.endDate = Vue.filter('beautify-date')(awayInfoJson.endDate);
                    } else {
                        awayInfo.startDate = Vue.filter('beautify-date-time')(awayInfoJson.startDate);
                        awayInfo.endDate = Vue.filter('beautify-date-time')(awayInfoJson.endDate);
                    }
                    this.userAwayLocationInfo.push(awayInfo);
                }
                this.displayLoaned = true;
                 
            } else {
                this.displayLoaned = false;
            }
        }

        


    }
</script>

 <style scoped>

    .tooltip >>> .tooltip-inner{
        max-width: 500px !important;
        width: 400px !important;
    }

</style>