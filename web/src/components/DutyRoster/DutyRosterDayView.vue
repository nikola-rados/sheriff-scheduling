<template>
    <div>
        <loading-spinner v-if="!isDutyRosterDataMounted" />
        <b-table         
            v-else              
            :items="dutyRosterAssignments" 
            :fields="fields"
            sort-by="assignment"
            style="overflow-x:scroll"
            small   
            borderless
            :sticky-header="displayFooter?'37.7rem':'34rem'"                  
            fixed>
                <template v-slot:table-colgroup>
                    <col style="width:9rem">                         
                </template>
                
                <template v-slot:cell(assignment) ="data"  >
                    <duty-roster-assignment v-on:change="getDutyRosters()" :assignment="data.item" :weekview="false"/>
                </template>

                <template v-slot:head(assignment)="data" >
                    <div style="float: left; margin:0 1rem; padding:0;">
                        <div style="float: left; margin:.15rem .25rem 0  0; font-size:14px">{{data.label}}</div>
                        <b-button
                            v-if="hasPermissionToAddAssignments"
                            variant="success"
                            style="padding:0; height:1rem; width:1rem; margin:auto 0" 
                            @click="addAssignment();"
                            size="sm"> <div style="transform:translate(0,-3px)" >+</div>
                        </b-button>
                    </div>
                </template>

                <template v-slot:head(h0) >
                    <div class="grid24">
                        <div v-for="i in 24" :key="i" :style="{gridColumnStart: i,gridColumnEnd:(i+1), gridRow:'1/1'}">{{getBeautifyTime(i-1)}}</div>
                    </div>
                </template>

                <template v-slot:cell(h0)="data" >
                    <duty-card v-on:change="getDutyRosters()" :dutyRosterInfo="data.item"/>
                </template>
        </b-table>                
        <b-card><br></b-card>
        <sheriff-fuel-gauge v-if="isDutyRosterDataMounted && !displayFooter" class="fixed-bottom bg-white"/>
    </div>
</template>

<script lang="ts">
    import { Component, Vue, Watch} from 'vue-property-decorator';

    import DutyCard from './components/DutyCard.vue'
    import SheriffFuelGauge from './components/SheriffFuelGauge.vue'
    import DutyRosterAssignment from './components/DutyRosterAssignment.vue'

    import moment from 'moment-timezone';

    import { namespace } from "vuex-class";   
    import "@store/modules/CommonInformation";
    const commonState = namespace("CommonInformation");

    import "@store/modules/DutyRosterInformation";   
    const dutyState = namespace("DutyRosterInformation");

    import {localTimeInfoType, locationInfoType, userInfoType } from '../../types/common';
    import { assignmentCardInfoType, attachedDutyInfoType, dutyRangeInfoType, myTeamShiftInfoType, dutiesDetailInfoType} from '../../types/DutyRoster';
    import { shiftInfoType } from '../../types/ShiftSchedule';

    @Component({
        components: {
            DutyCard,
            SheriffFuelGauge,
            DutyRosterAssignment
        }
    })
    export default class DutyRosterDayView extends Vue {

        @commonState.State
        public location!: locationInfoType;

        @commonState.State
        public displayFooter!: boolean;

        @commonState.State
        public userDetails!: userInfoType;

        @dutyState.State
        public dutyRangeInfo!: dutyRangeInfoType;

        @dutyState.State
        public shiftAvailabilityInfo!: myTeamShiftInfoType[];

        @dutyState.Action
        public UpdateShiftAvailabilityInfo!: (newShiftAvailabilityInfo: myTeamShiftInfoType[]) => void

        isDutyRosterDataMounted = false;
        hasPermissionToAddAssignments = false;

        dutyRosterAssignments: assignmentCardInfoType[] = [];

        dutyRostersJson: attachedDutyInfoType[] = [];
        dutyRosterAssignmentsJson;

        fields =[
            {key:'assignment', stickyColumn: true, label:'Assignments', thClass:'text-white m-0 p-0', tdClass:'p-0 m-0', thStyle:'background-color: #556077;'},
            {key:'h0', label:'', thClass:'', tdClass:'p-0 m-0', thStyle:'margin:0; padding:0;'}
        ]

        dutyColors = [
            {name:'courtroom',  colorCode:'#189fd4'},
            {name:'court',      colorCode:'#189fd4'},
            {name:'jail' ,      colorCode:'#A22BB9'},
            {name:'escort',     colorCode:'#ffb007'},
            {name:'other',      colorCode:'#7a4528'}, 
            {name:'overtime',   colorCode:'#e85a0e'},
            {name:'free',       colorCode:'#e6d9e2'}                        
        ]

        @Watch('location.id', { immediate: true })
        locationChange()
        {
            if (this.isDutyRosterDataMounted) {
                this.getDutyRosters();                                
            }            
        } 

        mounted()
        {
            this.isDutyRosterDataMounted = false;
            console.log('dayview dutyroster mounted')
            this.getDutyRosters()
        }

        public getBeautifyTime(hour: number){
            return( hour>9? hour+':00': '0'+hour+':00')
        }

        public getDutyRosters(){
            this.hasPermissionToAddAssignments = this.userDetails.permissions.includes("CreateAssignments");			
            const url = 'api/dutyroster?locationId='+this.location.id+'&start='+this.dutyRangeInfo.startDate+'&end='+this.dutyRangeInfo.endDate;
            this.$http.get(url)
                .then(response => {
                    if(response.data){
                        this.dutyRostersJson = response.data; 
                        console.log(response.data);
                        this.getAssignments();                                                                   
                    }                                   
                })      
        }

        public getAssignments(){
            const url = 'api/assignment?locationId='+this.location.id+'&start='+this.dutyRangeInfo.startDate+'&end='+this.dutyRangeInfo.endDate;
            this.$http.get(url)
                .then(response => {
                    if(response.data){
                        console.log(response.data)
                        this.dutyRosterAssignmentsJson = response.data; 
                        this.getShifts();                             
                    }                                   
                })      
        }

        public getShifts(){
            this.isDutyRosterDataMounted = false;
            const url = 'api/shift?locationId='+this.location.id+'&start='+this.dutyRangeInfo.startDate+'&end='+this.dutyRangeInfo.endDate +'&includeDuties=true';
            this.$http.get(url)
                .then(response => {
                    if(response.data){
                        console.log(response.data)                        
                        this.extractTeamShiftInfo(response.data);                        
                        this.extractAssignmentsInfo(this.dutyRosterAssignmentsJson);                                               
                    }                                   
                })      
        }        

        public extractTeamShiftInfo(shiftsJson){
            this.UpdateShiftAvailabilityInfo([]);
            const allDutySlots: any[] = []
            for(const dutyRoster of this.dutyRostersJson)            
                allDutySlots.push(...dutyRoster.dutySlots)              

            // console.log(allDutySlots)
            for(const shiftJson of shiftsJson)
            {
                //console.log(shiftJson)
                const availabilityInfo = {} as myTeamShiftInfoType;
                const shiftInfo = {} as shiftInfoType;
                shiftInfo.id = shiftJson.id;
                shiftInfo.startDate =  moment(shiftJson.startDate).tz(this.location.timezone).format();
                shiftInfo.endDate = moment(shiftJson.endDate).tz(this.location.timezone).format();
                shiftInfo.timezone = shiftJson.timezone;
                shiftInfo.sheriffId = shiftJson.sheriffId;
                shiftInfo.locationId = shiftJson.locationId;
                const rangeBin = this.getTimeRangeBins(shiftInfo.startDate, shiftInfo.endDate, this.location.timezone);

                const dutySlots = allDutySlots.filter(dutyslot=>{if(dutyslot.sheriffId==shiftInfo.sheriffId)return true})
                let duties = Array(96).fill(0)
                const dutiesDetail: dutiesDetailInfoType[] = [];
                for(const duty of dutySlots){
                    console.log(duty)
                    const color = this.getType(duty.assignmentLookupCode.type)
                    const dutyRangeBin = this.getTimeRangeBins(duty.startDate, duty.endDate, this.location.timezone);
                    dutiesDetail.push({
                        id:duty.id , 
                        startBin:dutyRangeBin.startBin, 
                        endBin: dutyRangeBin.endBin,
                        name: color.name,
                        colorCode: color.colorCode,
                        color: duty.shiftId? color.colorCode: this.dutyColors[5].colorCode,
                        type: duty.assignmentLookupCode.type,
                        code: duty.assignmentLookupCode.code
                    })
                    //console.log(dutiesDetail)
                    duties = this.fillInArray(duties, duty.id , dutyRangeBin.startBin,dutyRangeBin.endBin)
                }

                const index = this.shiftAvailabilityInfo.findIndex(shift => shift.sheriffId == shiftInfo.sheriffId)
                
                if (index != -1) {
                    let availability = this.fillInArray(this.shiftAvailabilityInfo[index].availability, shiftJson.id , rangeBin.startBin,rangeBin.endBin)
                    const newavailability = this.subtractUnionOfArrays(availability, duties);
                    availability =this.unionArrays(availability, newavailability);
                    this.shiftAvailabilityInfo[index].duties = duties;
                    this.shiftAvailabilityInfo[index].availability = availability;
                    this.shiftAvailabilityInfo[index].shifts.push(shiftInfo);
                    this.shiftAvailabilityInfo[index].dutiesDetail.push(...dutiesDetail);
                } else {
                    let availability = this.fillInArray(Array(96).fill(0), shiftJson.id , rangeBin.startBin,rangeBin.endBin)
                    const newavailability = this.subtractUnionOfArrays(availability, duties);
                    availability =this.unionArrays(availability, newavailability);
                    availabilityInfo.shifts = [shiftInfo];
                    availabilityInfo.sheriffId = shiftJson.sheriff.id;
                    availabilityInfo.badgeNumber = shiftJson.sheriff.badgeNumber;
                    availabilityInfo.firstName = shiftJson.sheriff.firstName;
                    availabilityInfo.lastName = shiftJson.sheriff.lastName;
                    availabilityInfo.rank = shiftJson.sheriff.rank;
                    availabilityInfo.availability = availability;
                    availabilityInfo.duties = duties;
                    availabilityInfo.dutiesDetail = dutiesDetail;
                    this.shiftAvailabilityInfo.push(availabilityInfo);
                }
            }
            this.UpdateShiftAvailabilityInfo(this.shiftAvailabilityInfo);            
        }

        public extractAssignmentsInfo(assignments){

            this.dutyRosterAssignments =[]
            let sortOrder = 0;
            for(const assignment of assignments){
                sortOrder++;
                const dutyRostersForThisAssignment: attachedDutyInfoType[] = this.dutyRostersJson.filter(dutyroster=>{if(dutyroster.assignmentId == assignment.id)return true}) 
                //console.log(dutyRostersForThisAssignment)
               
               if(dutyRostersForThisAssignment.length>0){
                    for(const rosterInx in dutyRostersForThisAssignment){
                        this.dutyRosterAssignments.push({
                            assignment:('00' + sortOrder).slice(-3)+'FTE'+('0'+ rosterInx).slice(-2) ,
                            assignmentDetail: assignment,
                            name:assignment.name,
                            code:assignment.lookupCode.code,
                            type: this.getType(assignment.lookupCode.type),
                            attachedDuty: dutyRostersForThisAssignment[rosterInx],
                            FTEnumber: Number(rosterInx),
                            totalFTE: dutyRostersForThisAssignment.length
                        })
                    }
                }else{                
                    this.dutyRosterAssignments.push({
                        assignment:('00' + sortOrder).slice(-3)+'FTE00' ,
                        assignmentDetail: assignment,
                        name:assignment.name,
                        code:assignment.lookupCode.code,
                        type: this.getType(assignment.lookupCode.type),
                        attachedDuty: null,
                        FTEnumber: 0,
                        totalFTE: 0
                    })
                }
            }

            this.isDutyRosterDataMounted = true;
            this.$emit('dataready')
            Vue.nextTick(()=>{
                const el = document.getElementsByClassName('b-table-sticky-header')                
                const scrollSize = window.innerWidth*0.9173-185
                if(el[0]) el[0].addEventListener("scroll",()=>{
                    if(el[1]) el[1].scrollLeft = el[0].scrollLeft
                })
                if(el[0]) el[0].scrollLeft = (scrollSize*0.5425)
                if(el[1]) el[1].scrollLeft = (scrollSize*0.5425)
            })
        }
        public getType(type: string){
            for(const color of this.dutyColors){
                if(type.toLowerCase().includes(color.name))return color
            }
            return this.dutyColors[3]
        }

        public fillInArray(array, fillInNum, startBin, endBin){
            return array.map((arr,index) =>{if(index>=startBin && index<endBin) return fillInNum; else return arr;});
        }

        // public addArrays(arrayA, arrayB){
        //     return arrayA.map((arr,index) =>{return arr+arrayB[index]});
        // }

        public unionArrays(arrayA, arrayB){
            return arrayA.map((arr,index) =>{return arr*arrayB[index]});
        }

        public subtractUnionOfArrays(arrayA, arrayB){
            return arrayA.map((arr,index) =>{return arr&&(arrayB[index]>0?0:1)});
        }

        public getTimeRangeBins(startDate, endDate, timezone){
            const startTime = moment(startDate).tz(timezone);
            const endTime = moment(endDate).tz(timezone);
            const startOfDay = moment(startTime).startOf("day");
            const startBin = moment.duration(startTime.diff(startOfDay)).asMinutes()/15;
            const endBin = moment.duration(endTime.diff(startOfDay)).asMinutes()/15;
            return( {startBin: startBin, endBin:endBin } )
        }

        public addAssignment(){ 
            this.$emit('addAssignmentClicked');            
        }
        
    }
</script>

<style scoped>   

    .card {
        border: white;
    }

    .gauge {
        direction:rtl;
        position: sticky;
    }

    .grid24 {        
        display:grid;
        grid-template-columns: repeat(24, 8.333%);
        grid-template-rows: 1.65rem;
        inline-size: 100%;
        font-size: 10px;
        height: 1.58rem;         
    }

    .grid24 > * {      
        padding: 0.3rem 0;
        border: 1px dotted rgb(185, 143, 143);
        color: white;
        background-color:   #003366;
        font-size: 12px;
    }

</style>