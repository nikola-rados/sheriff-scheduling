<template> 
    <b-card v-if="displayTraining" no-body class="bg-dark text-white"> 
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
    import moment from 'moment-timezone';
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
                    const trainingInfo = {} as trainingInfoType;
                    trainingInfo.trainingTypeId = trainingInfoJson.trainingTypeId
                    trainingInfo.trainingName = trainingInfoJson.trainingType.description;

                    const startDate = moment(trainingInfoJson.startDate).tz("UTC").format();
                    const endDate = moment(trainingInfoJson.endDate).tz("UTC").format();
                    if(this.isDateFullday(startDate, endDate))                    {
                        trainingInfo.startDate = Vue.filter('beautify-date')(startDate);
                        trainingInfo.endDate = Vue.filter('beautify-date')(endDate);
                    }
                    else{
                        trainingInfo.startDate = Vue.filter('beautify-date-time')(startDate);
                        trainingInfo.endDate = Vue.filter('beautify-date-time')(endDate);
                    }

                    this.userTrainingInfo.push(trainingInfo);
                }
                if(this.userTrainingInfo.length) this.displayTraining = true;       
            } else {
                this.displayTraining = false;
            }
        }

        public isDateFullday(startDate, endDate){
            const start = moment(startDate); 
            const end = moment(endDate);
            const duration = moment.duration(end.diff(start));
            if(duration.asMinutes() < 1440 && duration.asMinutes()> -1440 )  return false;  else return true;
        }
    }
</script>

 <style scoped>

    .tooltip >>> .tooltip-inner{
        max-width: 500px !important;
        width: 400px !important;
    } 

</style>