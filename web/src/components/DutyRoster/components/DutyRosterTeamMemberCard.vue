<template>
    <div>
        <div 
            v-if="!weekView && isDataMounted"
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
                        v-b-tooltip.hover                                
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
    
        <b-card 
            v-if="weekView && isDataMounted"
            :id="'member-'+sheriffId"
            :style="specialMember?'border-radius:5px':'width:100%; height:4.5rem; border:1px solid rgb(124, 136, 136);'"                       
            :class="bgcolor+' mx-0 my-1 p-0'" 
            body-class="m-0 p-0"
            :draggable="true" 
            v-on:dragstart="DragStart"            
            header-class=" h6 m-0 p-0">
            <b-col class="m-0 p-0" v-if="!specialMember">
                <b-row style="margin-left:2px;font-size:11px; line-height: 16px;"># {{sheriffInfo.badgeNumber}}</b-row>
                <b-row style="margin-left:2px;font-size:9px; line-height: 14px;">{{sheriffInfo.rank}}</b-row>
                <b-row 
                    style="margin-left:2px;font-size:12px; line-height: 16px; font-weight: bold; text-transform: Capitalize;" 
                    v-b-tooltip.hover.topleft                                
                    :title="fullName.length>14?fullName:''">
                        {{fullName|truncate(12)}}
                </b-row>
                <b-row style="margin-left:0px;font-size:10px;">
                    <b-badge 
                        class="week-view-badge"
                        v-for="sch in sheriffSchedules"
                        v-b-tooltip.hover.v-warning.html="sch.tooltip?sch.tooltip:''" 
                        :key="sch.weekday"
                        :id="'sch'+sheriffId+'-'+sch.weekday"
                        :variant="sch.variant" 
                        :style="sch.style" >
                            {{sch.text}}                            
                    </b-badge>                                 
                </b-row>
            </b-col>
            <b-col class="m-0 p-0" v-else>
                <div class="text-center text-white" style="font-size:13px;">{{fullName}}</div>
            </b-col>
        </b-card>
    </div>
</template>

<script lang="ts">
    import { Component, Vue, Prop } from 'vue-property-decorator';
    import { namespace } from "vuex-class";

    import "@store/modules/CommonInformation";
    const commonState = namespace("CommonInformation");

    import "@store/modules/DutyRosterInformation";   
    const dutyState = namespace("DutyRosterInformation");
    
    import moment from 'moment-timezone';
    import * as _ from 'underscore';
    import { locationInfoType } from '../../../types/common';
    import { dutyRangeInfoType, myTeamShiftInfoType } from '../../../types/DutyRoster';

    @Component
    export default class DutyRosterTeamMemberCard extends Vue {        

        @commonState.State
        public location!: locationInfoType;

        @dutyState.State
        public dutyRangeInfo!: dutyRangeInfoType;

        @Prop({required: true})
        public sheriffInfo!: myTeamShiftInfoType;

        @Prop({required: true})
        public weekView!: boolean;

        sheriffId = '';
        isDataMounted = false;
        fullName = '';
        shifts: string[] = [];
        allShifts = {title:''};

        specialMember = false;
        bgcolor='bg-white';

        halfUnavailStyle="background-image: linear-gradient(to bottom right, rgb(194, 39, 28),rgb(243, 232, 232), white);"
        halfUnavailHalfSchStyle="background-image: linear-gradient(to bottom right, rgb(194, 39, 28),rgb(243, 232, 232), rgb(12, 120, 170));"
        halfSchStyle="background-image: linear-gradient(to bottom right, rgb(12, 120, 170),rgb(243, 232, 232), white);"
        WeekDay = ['S', 'M', 'T', 'W', 'T', 'F', 'S'];

        sheriffSchedules: any[] = [];

        mounted()
        {
            // console.log(this.sheriffInfo)  
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
                for(let dayOffset=0; dayOffset<7; dayOffset++){
                    const date= moment(this.dutyRangeInfo.startDate).add(dayOffset,'days').format()
                    this.sheriffSchedules.push({date: date, weekday: dayOffset, text:this.WeekDay[dayOffset], variant:'white',  style:'', tooltip:{}})
                }
                if (this.weekView) {
                    this.extractSchedules();
                } else {
                    this.extractShifts();                    
                }                
            }
            this.isDataMounted = true;
        }

        public extractSchedules() {
            console.log(this.sheriffInfo)
            console.log(this.sheriffSchedules)

            for(const scheduleIndex in this.sheriffSchedules) {
                // console.log(this.sheriffSchedules[scheduleIndex])
                const schedule = this.sheriffSchedules[scheduleIndex];
            
                let tooltipTitle = '<div>';
                const filteredShifts = this.sheriffInfo.shifts.filter(shift=>{if(shift.startDate.substring(1,10)==schedule.date.substring(1,10))return true;});
                if (filteredShifts.length == 0) {
                    this.sheriffSchedules[scheduleIndex].variant = 'danger';
                } else {
                    
                    for(const shift of filteredShifts){ 
                        console.log(shift)
                        tooltipTitle += shift.startDate.substring(11,16) +' - '+shift.endDate.substring(11,16)+'<br/>'
                    }

                    tooltipTitle += '</div>'
                    this.sheriffSchedules[scheduleIndex].tooltip = tooltipTitle;
                }
                
            }
            // console.log(this.sheriffSchedules)
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

    .week-view-badge {
        width: 0.75rem;
        height: 0.9rem;
        margin:0.5rem .04rem 0 .04rem;
        padding: 0.15rem 0 0 0;
        border: solid 1px rgb(53, 56, 53); 
        border-radius: 4px;
        /* background-image: linear-gradient(to bottom right, rgb(12, 120, 170),rgb(243, 232, 232), white);*/        
    }

</style>