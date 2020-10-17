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
          { key: 'leaveName', label: 'Leave Type', thClass: 'text-primary h3', tdClass: 'font-weight-bold'},
          { key: 'startDate', label: 'Start', thClass: 'text-primary h3'},
          { key: 'endDate', label: 'End', thClass: 'text-primary h3'}
        ];

        mounted()
        {            
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
                    leaveInfo.leaveName = leaveInfoJson.leaveType.description; 
                    leaveInfo.startDate = Vue.filter('beautify-date-time')(leaveInfoJson.startDate);
                    leaveInfo.endDate = Vue.filter('beautify-date-time')(leaveInfoJson.endDate);
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