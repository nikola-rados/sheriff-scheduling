<template>
    <div>
        <div 
            v-if="isDataMounted"
            :id="'member-'+sheriffId"
            :draggable="hasPermissionToAddAssignDuty" 
            v-on:dragstart="DragStart" 
            style="border-radius:5px"          
            :class="bgcolor+' p-1'">
                <b-col v-if="!specialMember" class="b-0 p-0">
                    <div style="font-size:11px; line-height: 16px;"># {{sheriffInfo.badgeNumber}}</div>
                    <div style="font-size:9px; line-height: 14px;">{{sheriffInfo.rank}}</div>
                    <div 
                        style="font-size:12px; line-height: 16px; font-weight: bold; text-transform: Capitalize;" 
                        v-b-tooltip.hover.noninteractive                               
                        :title="fullName.length>13?fullName:''">
                            {{fullName|truncate(11)}}
                    </div>

                    <b-row v-if="!weekView">
                        <b-badge v-b-tooltip.hover.noninteractive.v-warning.html="allShifts" class="mx-auto mt-1">{{shifts[0]}}<span v-if="shifts.length>1"> ...</span></b-badge>
                    </b-row>

                    <b-row v-else style="margin:0; padding:0; font-size:10px;">
                        <b-badge 
                            class="week-view-badge"
                            v-for="sch in sheriffSchedules"                            
                            :key="sch.weekday"
                            :id="'sch'+sheriffId+'-'+sch.weekday"
                            :variant="sch.variant" 
                            :style="sch.style" >
                                {{sch.text}}                            
                        

                            <b-tooltip v-if="sch.shifts.length>0" noninteractive :target="'sch'+sheriffId+'-'+sch.weekday" variant="warning" show.sync ="true" triggers="hover" placement="topright">
                                <h2 class="text-danger  mb-1 mx-0 p-0">{{sch.name}}</h2>
                                <b-card bg-variant="dark" header-class="text-warning m-0 p-0" body-class="m-0 p-0" header="Sheriff Shifts:">             
                                    <b-table  
                                        :items="sch.shifts"
                                        :fields="shiftFields"
                                        sort-by="startTime"                
                                        borderless
                                        striped
                                        small 
                                        responsive="sm"
                                        class="my-0 py-0 text-white"
                                        >
                                        <template v-slot:cell(startDate)="data" >
                                            <span  >{{data.value | beautify-time}}</span> 
                                        </template>
                                        <template v-slot:cell(endDate)="data" >
                                            <span  >{{data.value | beautify-time}}</span> 
                                        </template>
                                    </b-table>
                                </b-card> 
                                
                                <b-card v-if="sch.duties.length>0" class="mt-1" bg-variant="dark" header-class="text-warning m-0 p-0" body-class="m-0 p-0" header="Assigned Duties:">
                               
                                    <b-table  
                                        :items="sch.duties"
                                        :fields="dutyFields"
                                        sort-by="startTime"                
                                        borderless
                                        striped
                                        small 
                                        responsive="sm"
                                        class="my-0 py-0"
                                        >
                                        <template v-slot:cell(startTime)="data" >
                                            <span >{{data.value | beautify-time}}</span> 
                                        </template>
                                        <template v-slot:cell(endTime)="data" >
                                            <span>{{data.value | beautify-time}}</span> 
                                        </template>
                                    </b-table>     
                                </b-card>                                   
                            </b-tooltip>
                        </b-badge>                                 
                    </b-row>

                </b-col>
                <b-col class="m-0 p-0" v-else>
                    <div class="text-center text-white" style="font-size:13px;">{{fullName}}</div>
                </b-col>
        </div>
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
    import { locationInfoType, userInfoType } from '../../../types/common';
    import { dutyRangeInfoType, myTeamShiftInfoType } from '../../../types/DutyRoster';

    @Component
    export default class DutyRosterTeamMemberCard extends Vue {        

        @commonState.State
        public location!: locationInfoType;

        @commonState.State
        public userDetails!: userInfoType;

        @dutyState.State
        public dutyRangeInfo!: dutyRangeInfoType;

        @Prop({required: true})
        public sheriffInfo!: myTeamShiftInfoType;

        @Prop({required: true})
        public weekView!: boolean;

        sheriffId = '';
        isDataMounted = false;
        hasPermissionToAddAssignDuty = false;
        fullName = '';
        shifts: string[] = [];
        allShifts = {title:''};

        specialMember = false;
        bgcolor='bg-white mb-2';

        shiftFields = [
            {key:'startDate', label:'Start', thClass:'text-info m-0 p-0', tdClass:'text-white p-0 m-0', thStyle:''},
            {key:'endDate', label:'End', thClass:'text-info m-0 p-0', tdClass:'text-white p-0 m-0', thStyle:''}
        ]

        dutyFields = [
            {key:'name', label:'Type', thClass:'text-info m-0 p-0', tdClass:'text-white p-0 m-0', thStyle:''},
            {key:'code', label:'Name', thClass:'text-info m-0 p-0', tdClass:'text-white p-0 m-0', thStyle:''},
            {key:'startTime', label:'Start', thClass:'text-info m-0 p-0', tdClass:'text-white p-0 m-0', thStyle:''},
            {key:'endTime', label:'End', thClass:'text-info m-0 p-0', tdClass:'text-white p-0 m-0', thStyle:''}
        ]

        halfUnavailStyle="background-image: linear-gradient(to bottom right, rgb(194, 39, 28),rgb(243, 232, 232), white);"
        halfUnavailHalfSchStyle="background-image: linear-gradient(to bottom right, rgb(194, 39, 28),rgb(243, 232, 232), rgb(12, 120, 170));"
        halfSchStyle="background-image: linear-gradient(to bottom right, rgb(12, 120, 170),rgb(243, 232, 232), white);"
        WeekDay = ['S', 'M', 'T', 'W', 'T', 'F', 'S'];
        weekDayNames = ['Sunday','Monday','Tuesday','Wednesday','Thursday','Friday','Saturday'];

        sheriffSchedules: any[] = [];

        mounted()
        {
            //console.error(this.sheriffInfo)  
            this.isDataMounted = false;
            this.hasPermissionToAddAssignDuty = this.userDetails.permissions.includes("CreateAndAssignDuties");
            this.sheriffId = this.sheriffInfo.sheriffId; 
            if(this.sheriffId== '00000-00000-11111'){
                this.fullName = 'Not Required'
                this.bgcolor='bg-success mb-2'
                this.specialMember = true
            } else if(this.sheriffId== '00000-00000-22222'){
                this.fullName = 'Not Available'
                this.bgcolor='bg-danger'
                this.specialMember = true
            }else{      
                this.fullName = this.sheriffInfo.lastName +', '+this.sheriffInfo.firstName;
                this.bgcolor='bg-white mb-2'
                for(let dayOffset=0; dayOffset<7; dayOffset++){
                    const date= moment(this.dutyRangeInfo.startDate).add(dayOffset,'days').format()
                    this.sheriffSchedules.push({
                        date: date, 
                        weekday: dayOffset, 
                        text:this.WeekDay[dayOffset],
                        name:this. weekDayNames[dayOffset], 
                        variant:'white',  
                        style:'', 
                        shifts:[],
                        duties:[]
                    })
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
            // if(this.sheriffInfo.firstName=='Alex'){
                //console.warn(this.sheriffInfo)
                //console.log(this.sheriffSchedules)
            // }
            for(const scheduleIndex in this.sheriffSchedules) {

                const schedule = this.sheriffSchedules[scheduleIndex];            
                
                const filteredShifts = this.sheriffInfo.shifts.filter(shift=>{if(shift.startDate.substring(0,10)==schedule.date.substring(0,10))return true;});
                const filteredDuties = this.sheriffInfo.dutiesDetail.filter(duty=>{if(duty.startTime && duty.startTime.substring(0,10)==schedule.date.substring(0,10))return true;});
                if (filteredShifts.length == 0) {
                    this.sheriffSchedules[scheduleIndex].variant = 'danger';
                } else {
                    this.sheriffSchedules[scheduleIndex].shifts = filteredShifts 
                    this.sheriffSchedules[scheduleIndex].duties = filteredDuties
                    if(filteredDuties.length>0) {
                         
                        let availability = this.getAvailability(filteredShifts, schedule.date)
                        const duties = this.getSheriffDuties(filteredDuties, schedule.date) 
                        availability = this.subtractUnionOfArrays(availability,duties) 
                        //console.log(availability)
                        this.sheriffSchedules[scheduleIndex].variant = 'info';
                        if(this.sumOfArrayElements(availability)==0)
                            this.sheriffSchedules[scheduleIndex].style =''
                        else this.sheriffSchedules[scheduleIndex].style = this.halfSchStyle;
                    }
                }
                
            }
            // console.log(this.sheriffSchedules)
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

        public getAvailability(shifts, startOfDay){
            
            //console.log(shifts)
            let availability = Array(96).fill(0)
            for(const shift of shifts){
                const rangeBin = this.getTimeRangeBins(shift.startDate, shift.endDate, startOfDay, this.location.timezone);
                //console.log(rangeBin)
                availability = this.fillInArray(availability, shift.id , rangeBin.startBin,rangeBin.endBin)
            }
            return availability            
        }

        public getSheriffDuties(dutiesDetail,startOfDay){
    
                //console.log(dutiesDetail)
                let duties = Array(96).fill(0)
                for(const duty of dutiesDetail){                    
                    //console.log(duty)
                    duties = this.fillInArray(duties, duty.id , duty.startBin, duty.endBin)
                }
                return duties           
        }

        public fillInArray(array, fillInNum, startBin, endBin){
            return array.map((arr,index) =>{if(index>=startBin && index<endBin) return fillInNum; else return arr;});
        }

        public getTimeRangeBins(startDate, endDate, startOfDay, timezone){
            const startTime = moment(startDate).tz(timezone);
            const endTime = moment(endDate).tz(timezone);
            const startBin = moment.duration(startTime.diff(startOfDay)).asMinutes()/15;
            const endBin = moment.duration(endTime.diff(startOfDay)).asMinutes()/15;
            return( {startBin: startBin, endBin:endBin } )
        }

        public subtractUnionOfArrays(arrayA, arrayB){
            return arrayA.map((arr,index) =>{return arr*(arrayB[index]>0?0:1)});
        }

        public sumOfArrayElements(arrayA){
            return arrayA.reduce((acc, arr) => acc + (arr>0?1:0), 0)
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

    .tooltip >>> .tooltip-inner{
        max-width: 500px !important;
        width: 400px !important;
    } 

    .week-view-badge {
        width: 0.75rem;
        height: 0.9rem;
        margin:0.5rem .04rem 0 .04rem;
        padding: 0.15rem 0 0 0;
        border: solid 1px rgb(53, 56, 53); 
        border-radius: 5px;
        /* background-image: linear-gradient(to bottom right, rgb(12, 120, 170),rgb(243, 232, 232), white);*/        
    }
    .week-view-badge:hover{
        border: 2px solid rgb(255, 208, 0);
       
    }

</style>