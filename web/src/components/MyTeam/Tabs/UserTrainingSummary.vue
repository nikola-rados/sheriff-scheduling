<template> 
    <b-card v-if="displayTraining" no-body class="bg-dark text-white"> 
        <font-awesome-icon icon="graduation-cap" :id="'trainingIcon'+index" style="font-size: 1.5rem;"></font-awesome-icon>        
            <b-tooltip :target="'trainingIcon'+index" variant="warning" show.sync ="true" triggers="hover">
                <h2 class="text-danger">On Training:</h2>                
                <b-table  
                    :items="userTrainingInfo"
                    :fields="userTrainingFields"
                    sort-by="startDate"                
                    borderless
                    striped
                    small 
                    responsive="sm"
                    class="my-0 py-0"
                    >
                    <template v-slot:cell(trainingName)="data" >                      
                        <span v-if="data.item.comment && data.item.comment.length">{{data.value}}<br><b class="text-success">({{data.item.comment}})</b></span>
                        <span v-else>{{data.value}}</span> 
                    </template>
                    <template v-slot:cell(startDate)="data" >
                        <span v-if="data.item.isFullDay">{{data.value | beautify-date}}</span>
                        <span v-else>{{data.value | beautify-date-time}}</span> 
                    </template>
                    <template v-slot:cell(endDate)="data" >
                        <span v-if="data.item.isFullDay">{{data.value | beautify-date}}</span>
                        <span v-else>{{data.value | beautify-date-time}}</span> 
                    </template>
                </b-table>                                        
            </b-tooltip>
    </b-card>   
</template>

<script lang="ts">
    import { Component, Vue, Prop } from 'vue-property-decorator';
    import moment from 'moment-timezone';
    import {userTrainingInfoType} from '../../../types/MyTeam';
    import {trainingJsontype} from '../../../types/MyTeam/jsonTypes';

    @Component
    export default class UserTrainingSummary extends Vue {       

        @Prop({required: true})
        index!: number;
        
        @Prop({required: true})
        timezone!: string;

        @Prop({required: true})
        trainingJson!: trainingJsontype[];               
        
        userTrainingInfo: userTrainingInfoType[] = [];
        displayTraining = false;
        userTrainingFields = [
          { key: 'trainingName', label: 'Training Type', thClass: 'text-primary h3', tdClass: 'font-weight-bold'},
          { key: 'startDate', label: 'Start', thClass: 'text-primary h3'},
          { key: 'endDate', label: 'End', thClass: 'text-primary h3'}
        ];

        mounted()
        {            
            this.extractTrainingInfo();
        }

        public extractTrainingInfo()
        {   
            this.displayTraining = false;
            if (this.trainingJson.length > 0 ) {                
                this.userTrainingInfo = [];            
                for(const trainingInfoJson of this.trainingJson)
                {
                    const trainingInfo = {} as userTrainingInfoType;
                    trainingInfo.trainingTypeId = trainingInfoJson.trainingTypeId
                    trainingInfo.trainingName = trainingInfoJson.trainingType.description;
                    if (trainingInfo.trainingName.toUpperCase == ("other").toUpperCase) {
                        trainingInfo.comment = trainingInfoJson.comment?trainingInfoJson.comment:'';
                    }
                    trainingInfo.startDate = moment(trainingInfoJson.startDate).tz(this.timezone).format();
                    trainingInfo.endDate = moment(trainingInfoJson.endDate).tz(this.timezone).format();
                    trainingInfo.isFullDay = Vue.filter('isDateFullday')(trainingInfo.startDate, trainingInfo.endDate);
                    this.userTrainingInfo.push(trainingInfo);
                }
                if(this.userTrainingInfo.length) this.displayTraining = true;       
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