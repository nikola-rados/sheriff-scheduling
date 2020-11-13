<template>
    <div 
        v-if="isDataMounted"
        :id="'member-'+sheriffId"
        :draggable="true" 
        v-on:dragstart="DragStart" 
        style="border-radius:5px"          
        class="mb-2 p-1 bg-white">
            <b-col>
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
        allShifts = {title:''}

        mounted()
        {  
            this.isDataMounted = false;
            this.sheriffId = this.sheriffInfo.sheriffId;          
            this.fullName = this.sheriffInfo.lastName +', '+this.sheriffInfo.firstName;
            this.extractShifts();
        }

        public extractShifts() {
            console.log(this.sheriffInfo)
            this.shifts = [];
            let tooltipTitle = '<div>';
            const sortedShifts = _.sortBy(this.sheriffInfo.shifts,'startDate');
            for(const shift of sortedShifts){
                this.shifts.push(shift.startDate.substring(11,16) +' - '+shift.endDate.substring(11,16))
                tooltipTitle += shift.startDate.substring(11,16) +' - '+shift.endDate.substring(11,16)+'<br/>'
            }

            tooltipTitle += '</div>'
            this.allShifts.title = tooltipTitle ;

            this.isDataMounted = true;                     
        }

        // (){
        //     return '<div>'+this.shifts[0]+'<br/>'+this.shifts[0] +'</div>'
        // }

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