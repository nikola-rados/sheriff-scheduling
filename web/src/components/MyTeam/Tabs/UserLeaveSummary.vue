<template> 
    <b-card v-if="displayLeave" no-body class="bg-dark text-white"> 
        <font-awesome-icon icon="suitcase" :id="'leaveIcon'+index" style="font-size: 1.5rem;"></font-awesome-icon>        
            <b-tooltip :target="'leaveIcon'+index" variant="warning" show.sync ="true" triggers="hover">
                <h2 class="text-danger">On Leave:</h2>                
                <b-table  
                    :items="userLeaveInfo"
                    :fields="userLeaveFields"
                    sort-by="startDate"                
                    borderless
                    striped
                    small 
                    responsive="sm"
                    class="my-0 py-0"
                    >
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
    import {userLeaveInfoType} from '../../../types/MyTeam';
    import {leaveJsontype} from '../../../types/MyTeam/jsonTypes';

    @Component
    export default class UserLeaveSummary extends Vue {       

        @Prop({required: true})
        index!: number;

        @Prop({required: true})
        timezone!: string;

        @Prop({required: true})
        leaveJson!: leaveJsontype[];  
        
        userLeaveInfo: userLeaveInfoType[] = [];
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
            this.displayLeave = false;
            if (this.leaveJson.length > 0 ) {                
                this.userLeaveInfo = [];            
                for(const leaveInfoJson of this.leaveJson)
                {
                    const leaveInfo = {} as userLeaveInfoType;
                    leaveInfo.id = leaveInfoJson.id;
                    leaveInfo.leaveTypeId = leaveInfoJson.leaveTypeId;
                    leaveInfo.leaveName = leaveInfoJson.leaveType.description;

                    leaveInfo.startDate = moment(leaveInfoJson.startDate).tz(this.timezone).format();
                    leaveInfo.endDate = moment(leaveInfoJson.endDate).tz(this.timezone).format();
                    leaveInfo.isFullDay = Vue.filter('isDateFullday')(leaveInfo.startDate, leaveInfo.endDate);
                    this.userLeaveInfo.push(leaveInfo);
                }
                if(this.userLeaveInfo.length) this.displayLeave = true;       
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