<template> 
    <b-card no-body class="bg-dark text-white"> 
        <font-awesome-icon icon="graduation-cap" :id="'trainingIcon'+index" style="font-size: 1.5rem;"></font-awesome-icon>        
            <b-tooltip :target="'trainingIcon'+index" variant="warning" show.sync ="true" triggers="hover">
                <h2 class="text-danger">On Training:</h2>                
                <b-table  
                    :items="userTrainingInfo"
                    :fields="userTrainingFields"                
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
    import {trainingInfoType} from '../../../types/MyTeam';
    import {trainingJsontype} from '../../../types/MyTeam/jsonTypes';
    import "@store/modules/CommonInformation";  
    const commonState = namespace("CommonInformation");

    @Component
    export default class UserTrainingSummary extends Vue {       

        @Prop({required: true})
        index!: number;

        @Prop({required: true})
        trainingJson!: trainingJsontype[];        

        @commonState.State
        public token!: string;        
        
        userTrainingInfo: trainingInfoType[] = [];
        displayTraining = false;
        userTrainingFields = [
          { key: 'name', label: 'Training Type', thClass: 'text-primary h3', tdClass: 'font-weight-bold'},
          { key: 'startDate', label: 'Start', thClass: 'text-primary h3'},
          { key: 'endDate', label: 'End', thClass: 'text-primary h3'}
        ];

        mounted()
        {
            console.log("mounted");
            this.trainingJson = [
                {
                    "id": 0,
                    "trainingType": {
                        "id": 0,
                        "type": "CourtRoom",
                        "code": "string",
                        "subCode": "string",
                        "description": "string",
                        "effectiveDate": "2020-10-15T15:38:14.984Z",
                        "expiryDate": "2020-10-15T15:38:14.984Z",
                        "sortOrder": 0,
                        "location": {
                        "id": 0,
                        "agencyId": "string",
                        "name": "string",
                        "justinCode": "string",
                        "parentLocationId": 0,
                        "expiryDate": "2020-10-15T15:38:14.984Z",
                        "regionId": 0,
                        "concurrencyToken": 0
                        },
                        "locationId": 0,
                        "concurrencyToken": 0
                    },
                    "trainingTypeId": 0,
                    "startDate": "2020-10-15T15:38:14.984Z",
                    "endDate": "2020-10-15T15:38:14.984Z",
                    "expiryDate": "2020-10-15T15:38:14.984Z",
                    "isFullDay": true,
                    "sheriffId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
                    "concurrencyToken": 0
                    }
            ];
            this.extractTrainingInfo();
        }

        public extractTrainingInfo()
        {
            if (this.trainingJson.length > 0 ) {
                this.displayTraining = true;
                this.userTrainingInfo = [];            
                for(const trainingInfoJson of this.trainingJson)
                {
                    const trainingInfo = {} as trainingInfoType;
                    trainingInfo.trainingTypeId = trainingInfoJson.trainingTypeId
                    trainingInfo.name = trainingInfoJson.trainingType.type;                    
                    if (trainingInfoJson.isFullDay) {
                        trainingInfo.startDate = Vue.filter('beautify-date')(trainingInfoJson.startDate);
                        trainingInfo.endDate = Vue.filter('beautify-date')(trainingInfoJson.endDate);
                    } else {
                        trainingInfo.startDate = Vue.filter('beautify-date-time')(trainingInfoJson.startDate);
                        trainingInfo.endDate = Vue.filter('beautify-date-time')(trainingInfoJson.endDate);                    
                    }                   
                    this.userTrainingInfo.push(trainingInfo);
                }
                this.displayTraining = true;       
            } else {
                this.displayTraining = false;
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