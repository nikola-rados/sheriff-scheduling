<template>
    <div 
        v-if="isDataMounted"
        :id="'member-'+sheriffId"
        :draggable="true" 
        v-on:dragstart="DragStart" 
        style="border-radius:5px"          
        :class="bgcolor+' mb-2 p-1'">
            <b-col v-if="!specialMember">
                <b-row style="font-size:11px; line-height: 16px;"># {{sheriffInfo.badgeNumber}}</b-row>
                <b-row style="font-size:9px; line-height: 14px;">{{sheriffInfo.rank}}</b-row>
                <b-row 
                    style="font-size:12px; line-height: 16px; font-weight: bold; text-transform: Capitalize;" 
                    v-b-tooltip.hover.topleft                                
                    :title="fullName.length>13?fullName:''">
                        {{fullName|truncate(11)}}
                </b-row>
                <b-row>
                    <b-badge v-b-tooltip.hover.v-warning.html="allShifts" class="mx-auto mt-1">{{shifts[0]}}<span v-if="shifts.length>1"> ...</span></b-badge>
                </b-row>
            </b-col>
            <b-col class="m-0 p-0" v-else>
                <div class="text-center text-white" style="font-size:13px;">{{fullName}}</div>
            </b-col>
    </div>
</template>

<script lang="ts">
    import { Component, Vue, Prop } from 'vue-property-decorator';
    import { namespace } from "vuex-class";

    import "@store/modules/CommonInformation";
    const commonState = namespace("CommonInformation");
    import moment from 'moment-timezone';
    import * as _ from 'underscore';
    import { locationInfoType } from '../../../types/common';
    import { myTeamShiftInfoType } from '../../../types/DutyRoster';

    @Component
    export default class DutyRosterTeamMemberCard extends Vue {        

        @commonState.State
        public location!: locationInfoType;

        @Prop({required: true})
        public sheriffInfo!: myTeamShiftInfoType;

        sheriffId = '';
        isDataMounted = false;
        fullName = '';
        shifts: string[] = [];
        allShifts = {title:''};

        specialMember = false;
        bgcolor='bg-white';

        mounted()
        {  
            this.isDataMounted = false;
            this.sheriffId = this.sheriffInfo.sheriffId; 
            if(this.sheriffId== '00000-00000-11111'){
                this.fullName = 'Not Required'
                this.bgcolor='bg-success'
                this.specialMember = true
            } else if(this.sheriffId== '00000-00000-22222'){
                this.fullName = 'Not Available'
                this.bgcolor='bg-danger'
                this.specialMember = true
            }else{      
                this.fullName = this.sheriffInfo.lastName +', '+this.sheriffInfo.firstName;
                this.bgcolor='bg-white'
                this.extractShifts();
            }
            this.isDataMounted = true;
        }

        public extractShifts() {
            //console.log(this.sheriffInfo)
            this.shifts = [];
            let tooltipTitle = '<div>';
            const sortedShifts = _.sortBy(this.sheriffInfo.shifts,'startDate');
            for(const shift of sortedShifts){
                this.shifts.push(shift.startDate.substring(11,16) +' - '+shift.endDate.substring(11,16))
                tooltipTitle += shift.startDate.substring(11,16) +' - '+shift.endDate.substring(11,16)+'<br/>'
            }

            tooltipTitle += '</div>'
            this.allShifts.title = tooltipTitle ;
        }

        public DragStart(event: any) 
        { 
            event.dataTransfer.setData('text', event.target.id);
        }

        


    }
</script>

<style scoped>   

    .card {
       border: white;
    }

    .custom-control-input{
        background-color: darkorange;
    }

</style>