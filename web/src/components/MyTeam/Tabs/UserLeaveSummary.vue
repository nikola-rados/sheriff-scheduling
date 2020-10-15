<template> 
    <b-card v-if="displayLeave" no-body class="bg-dark text-white"> 
        <font-awesome-icon icon="suitcase" :id="'leaveIcon'+index" style="font-size: 1.5rem;"></font-awesome-icon>        
            <b-tooltip :target="'leaveIcon'+index" variant="warning" show.sync ="true" triggers="hover">
                <h2 class="text-danger">On Leave:</h2>                
                <b-table  
                    :items="userLeaveInfo"
                    :fields="userLeaveFields"                
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
    import {leaveInfoType} from '../../../types/MyTeam';
    import {leaveJsontype} from '../../../types/MyTeam/jsonTypes';
    import "@store/modules/CommonInformation";  
    const commonState = namespace("CommonInformation");

    @Component
    export default class UserLeaveSummary extends Vue {       

        @Prop({required: true})
        index!: number;

        @Prop({required: true})
        leaveJson!: leaveJsontype[];        

        @commonState.State
        public token!: string;        
        
        userLeaveInfo: leaveInfoType[] = [];
        displayLeave = false;
        userLeaveFields = [
          { key: 'name', label: 'Leave Type', thClass: 'text-primary h3', tdClass: 'font-weight-bold'},
          { key: 'startDate', label: 'Start', thClass: 'text-primary h3'},
          { key: 'endDate', label: 'End', thClass: 'text-primary h3'}
        ];

        mounted()
        {
            console.log("mounted");
            this.leaveJson = [
                 {
      "id": 0,
      "leaveType": {
        "id": 0,
        "type": "CourtRoom",
        "code": "string",
        "subCode": "string",
        "description": "string",
        "effectiveDate": "2020-10-15T16:49:09.090Z",
        "expiryDate": "2020-10-15T16:49:09.090Z",
        "sortOrder": 0,
        "location": {
          "id": 0,
          "agencyId": "string",
          "name": "string",
          "justinCode": "string",
          "parentLocationId": 0,
          "expiryDate": "2020-10-15T16:49:09.090Z",
          "regionId": 0,
          "concurrencyToken": 0
        },
        "locationId": 0,
        "concurrencyToken": 0
      },
      "leaveTypeId": 0,
      "startDate": "2020-10-15T16:49:09.090Z",
      "endDate": "2020-10-15T16:49:09.090Z",
      "expiryDate": "2020-10-15T16:49:09.090Z",
      "isFullDay": true,
      "sheriffId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "concurrencyToken": 0
    }
            ];
            this.extractLeaveInfo();
        }

        public extractLeaveInfo()
        {
            if (this.leaveJson.length > 0 ) {
                this.displayLeave = true;
                this.userLeaveInfo = [];            
                for(const leaveInfoJson of this.leaveJson)
                {
                    const leaveInfo = {} as leaveInfoType;
                    leaveInfo.leaveTypeId = leaveInfoJson.leaveTypeId
                    leaveInfo.name = leaveInfoJson.leaveType.type;                    
                    if (leaveInfoJson.isFullDay) {
                        leaveInfo.startDate = Vue.filter('beautify-date')(leaveInfoJson.startDate);
                        leaveInfo.endDate = Vue.filter('beautify-date')(leaveInfoJson.endDate);
                    } else {
                        leaveInfo.startDate = Vue.filter('beautify-date-time')(leaveInfoJson.startDate);
                        leaveInfo.endDate = Vue.filter('beautify-date-time')(leaveInfoJson.endDate);                    
                    }                   
                    this.userLeaveInfo.push(leaveInfo);
                }
                this.displayLeave = true;       
            } else {
                this.displayLeave = false;
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