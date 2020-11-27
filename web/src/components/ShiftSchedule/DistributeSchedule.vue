<template>
    <b-card bg-variant="white" class="home" no-body>

        <distribute-header v-on:change="loadScheduleInformation" />

        <div id="pdf">

            <b-row class="mt-4 mb-0 mx-4">
                <b-col>
                    <b-row>
                        <b-col cols="3" class="mr-0">
                            <img class="mx-4 mt-3 mb-0 img-fluid d-none d-lg-block" src="../../../public/images/bcss-crest.png"
                                width="88.5"
                                height="22"
                                alt="B.C. Sheriff Logo"
                            /><img>
                        </b-col>
                        <b-col cols="9" class="ml-0">
                                <h2 class="mt-5">B.C. Sheriff Service</h2>
                                <h3 class="text-secondary font-italic">Honour - Integrity - Commitment</h3>
                        </b-col>
                    </b-row>
                </b-col>
                
                <b-col>
                    <b-card class="mt-4 mx-5 border border-dark text-center">
                        <b-card-sub-title class="mb-2 h4">{{location.name}} Schedule</b-card-sub-title>
                        <b-card-title class="h3">{{this.shiftRangeInfo.startDate | beautify-full-date}} - {{this.shiftRangeInfo.endDate | beautify-full-date}}</b-card-title>
                        <b-card-text class="text-secondary h5">Summary as of: <i class="h6">{{today | beautify-date-time-weekday}}</i></b-card-text>
                    </b-card>
                </b-col>            

            </b-row>
            <b-card no-body ><br></b-card>

            <b-overlay opacity="0.6" :show="!isDistributeDataMounted">
                <template #overlay>
                    <loading-spinner :inline="true"/>
                </template>    
                <b-table
                    :key="updateTable"
                    :items="sheriffSchedules" 
                    :fields="fields"
                    small
                    head-row-variant="primary"   
                    bordered
                    fixed>
                        
                        <template v-slot:head() = "data" >
                            <span class="text">{{data.column}}</span> <span> {{data.label}}</span>
                        </template>
                        <template v-slot:head(myteam) = "data" >  
                            <span>{{data.label}}</span>
                        </template>
                        <template v-slot:cell(myteam) = "data" >  
                            <span >{{data.item.myteam.name}}</span>
                            <div style="height:1rem;"                    
                                v-if="data.item.myteam.homeLocation != location.name"> 
                                    <b-icon-box-arrow-in-right style="transform:translate(0,-5px)"/>
                                    <span>Loaned in from {{data.item.myteam.homeLocation}}</span> 
                            </div>
                        </template>
                        <template v-slot:cell() = "data">                        
                            <b-row class="ml-4" v-for="event in data.item[data.field.key]" :key="event.id + event.date + event.location">
                                <span v-if="event.type == 'Shift'">{{event.workSection}} {{event.startTime}} - {{event.endTime}}</span>
                                <span v-else-if="event.type == 'Unavailable' && event.startTime.length>0">Unavailable {{event.startTime}} - {{event.endTime}}</span>
                                <span v-else-if="event.type == 'Unavailable' && event.startTime.length==0">Unavailable</span>
                                <span v-else>
                                    <font-awesome-icon icon="suitcase" v-if="event.type == 'Leave'" style="font-size: 1.5rem;"></font-awesome-icon>
                                    <font-awesome-icon icon="graduation-cap" v-if="event.type == 'Training'" style="font-size: 1.5rem;"></font-awesome-icon>
                                    <span v-if="event.type == 'Loaned'"><b-icon-box-arrow-left/> {{event.location}}</span>
                                    <span> {{event.startTime}} - {{event.endTime}}</span>                                
                                </span>                          
                            </b-row>
                        </template>
                        
                </b-table>
                <div v-if="!isDistributeDataMounted && this.sheriffSchedules.length == 0" style="min-height:115.6px;">
                </div>
            </b-overlay>
            <b-card><br></b-card> 
        </div>       
    </b-card>
</template>

<script lang="ts">
    import { Component, Vue, Watch } from 'vue-property-decorator';
    import { namespace } from 'vuex-class'
    import DistributeHeader from './components/DistributeHeader.vue'
    import "@store/modules/ShiftScheduleInformation";   
    const shiftState = namespace("ShiftScheduleInformation");
    import "@store/modules/CommonInformation";
    const commonState = namespace("CommonInformation");    
    import { locationInfoType } from '../../types/common';
    import { sheriffAvailabilityInfoType, shiftRangeInfoType, weekShiftInfoType,scheduleInfoType, weekScheduleInfoType, distributeScheduleInfoType, distributeTeamMemberInfoType } from '../../types/ShiftSchedule/index'
    import { sheriffsAvailabilityJsonType } from '../../types/ShiftSchedule/jsonTypes';
    import moment from 'moment-timezone';
    import * as _ from 'underscore';

    @Component({
        components: {
            DistributeHeader
        }
    })
    export default class DistributeSchedule extends Vue {

        @commonState.State
        public location!: locationInfoType;

        @shiftState.State
        public shiftRangeInfo!: shiftRangeInfoType;

        @shiftState.Action
        public UpdateTeamMemberList!: (newTeamMemberList: distributeTeamMemberInfoType[]) => void

        isDistributeDataMounted = false;
        headerDates: string[] = [];
        today = '';
        numberOfheaderDates = 7;
        updateTable=0;

        teamMembers: distributeTeamMemberInfoType[] = [];

        fields=[
            {key:'myteam', label:'Name', tdClass:'px-0 mx-0', thClass:'text-center'},
            {key:'Sun', label:'', tdClass:'px-0 mx-0', thStyle:'text-align: center;'},
            {key:'Mon', label:'', tdClass:'px-0 mx-0', thStyle:'text-align: center;'},
            {key:'Tue', label:'', tdClass:'px-0 mx-0', thStyle:'text-align: center;'},
            {key:'Wed', label:'', tdClass:'px-0 mx-0', thStyle:'text-align: center;'},
            {key:'Thu', label:'', tdClass:'px-0 mx-0', thStyle:'text-align: center;'},
            {key:'Fri', label:'', tdClass:'px-0 mx-0', thStyle:'text-align: center;'},
            {key:'Sat', label:'', tdClass:'px-0 mx-0', thStyle:'text-align: center;'}
        ]

        sheriffSchedules: weekScheduleInfoType[] =[];

        @Watch('location.id', { immediate: true })
        locationChange()
        {
            if (this.isDistributeDataMounted) {
                this.loadScheduleInformation(false, ''); 
                // console.log('watch')               
            }            
        }

        mounted() {
            this.isDistributeDataMounted=false;			
			this.loadScheduleInformation(false, '');
            this.today = moment().tz(this.location.timezone).format();
        }

        public loadScheduleInformation(includeWS: boolean, sheriffId: string) {
            console.log(includeWS)
            
            this.isDistributeDataMounted=false;
            this.headerDate();
            const endDate = moment(this.shiftRangeInfo.endDate).endOf('day').format();
            
            const url = 'api/distributeschedule/location?locationId='
                        +this.location.id+'&start='+this.shiftRangeInfo.startDate+'&end='+endDate + '&includeWorkSection='+includeWS;
            
            this.$http.get(url)
                .then(response => {
                    if(response.data){
                        this.extractTeamInfo(response.data);

                        let info = [];
                        if (sheriffId.length == 0) {
                            info = response.data;
                        } else {
                            info = response.data.filter(data =>{if(data.sheriffId == sheriffId ) return true})
                        }
                        console.log(info)
                        this.extractTeamScheduleInfo(info);                        
                    }                                   
                })            
        }

        public extractTeamInfo (teamJson) {
            this.teamMembers = [];
            
            for(const sheriffJson of teamJson) {
                const sheriff = {} as distributeTeamMemberInfoType;
                sheriff.sheriffId = sheriffJson.sheriffId;                
                sheriff.name = Vue.filter('capitalize')(sheriffJson.sheriff.lastName) 
                                        + ', ' + Vue.filter('capitalize')(sheriffJson.sheriff.firstName);
                this.teamMembers.push(sheriff);
            }
            this.UpdateTeamMemberList(this.teamMembers);
        }

        public headerDate() {
            this.headerDates = [];
            for(let dayOffset=0; dayOffset<this.numberOfheaderDates; dayOffset++)
            {
                const date= moment(this.shiftRangeInfo.startDate).add(dayOffset,'days').format();
                this.headerDates.push(date);
                this.fields[dayOffset+1].label = ' ' + Vue.filter('beautify-date')(date);
            }
        }

        public extractTeamScheduleInfo(sheriffsScheduleJson: sheriffsAvailabilityJsonType[]) {

            const sheriffsSchedule: distributeScheduleInfoType[] = [];
            this.sheriffSchedules = [];
            
            for(const sheriffScheduleJson of sheriffsScheduleJson) {
                const sheriffSchedule = {} as distributeScheduleInfoType;
                sheriffSchedule.sheriffId = sheriffScheduleJson.sheriffId;                
                sheriffSchedule.name = Vue.filter('capitalize')(sheriffScheduleJson.sheriff.lastName) 
                                        + ', ' + Vue.filter('capitalize')(sheriffScheduleJson.sheriff.firstName);
                sheriffSchedule.homeLocation = sheriffScheduleJson.sheriff.homeLocation.name;                                        
                const isInLoanLocation = (sheriffScheduleJson.sheriff.homeLocation.id !=this.location.id)
                sheriffSchedule.conflicts =isInLoanLocation? this.extractInLoanLocationConflicts(sheriffScheduleJson.conflicts) :this.extractSchedules(sheriffScheduleJson.conflicts, false);        
                
                
                this.sheriffSchedules.push({
                    myteam: sheriffSchedule,
                    Sun: sheriffSchedule.conflicts.filter(conflict=>{if(conflict.dayOffset ==0) return true}),
                    Mon: sheriffSchedule.conflicts.filter(conflict=>{if(conflict.dayOffset ==1) return true}),
                    Tue: sheriffSchedule.conflicts.filter(conflict=>{if(conflict.dayOffset ==2) return true}),
                    Wed: sheriffSchedule.conflicts.filter(conflict=>{if(conflict.dayOffset ==3) return true}),
                    Thu: sheriffSchedule.conflicts.filter(conflict=>{if(conflict.dayOffset ==4) return true}),
                    Fri: sheriffSchedule.conflicts.filter(conflict=>{if(conflict.dayOffset ==5) return true}),
                    Sat: sheriffSchedule.conflicts.filter(conflict=>{if(conflict.dayOffset ==6) return true})
                })
            }          
           
            this.isDistributeDataMounted = true;
            this.updateTable++;
        }

        public extractInLoanLocationConflicts(conflictsJson){
            let conflictsJsonAwayLocation: any[] = []
            const conflicts: scheduleInfoType[] = []
            for(const conflict of conflictsJson){  
                conflict.start = moment(conflict.start).tz(this.location.timezone).format();
                conflict.end = moment(conflict.end).tz(this.location.timezone).format();              
                if(conflict.conflict !='AwayLocation' || conflict.locationId != this.location.id) continue;
                conflict['startDay']=conflict.start.substring(0,10);
                conflict['endDay']=conflict.end.substring(0,10);
                conflictsJsonAwayLocation.push(conflict);
            }
            conflictsJsonAwayLocation = _.sortBy(conflictsJsonAwayLocation,'start')

            for(const dateIndex in this.headerDates){
                const date = this.headerDates[dateIndex]
                const day = date.substring(0,10);
                let numberOfConflictsPerDay = 0;
                let previousConflictEndDate = moment(date).startOf('day').format();
                for(const conflict of conflictsJsonAwayLocation){

                    if(day>=conflict.startDay && day<=conflict.endDay)
                    { 
                        numberOfConflictsPerDay++;
                        if(Vue.filter('isDateFullday')(conflict.start,conflict.end)){                            
                            break;
                        } else {                            
                            numberOfConflictsPerDay++;
                            //console.log( numberOfConflictsPerDay)
                            const start = moment(previousConflictEndDate)
                            const end = moment(conflict.start)
                            const duration = moment.duration(end.diff(start)).asMinutes();
                            if(duration>5)
                                conflicts.push({
                                    id:'0',
                                    location:conflict.conflict=='AwayLocation'?conflict.location.name:'',
                                    dayOffset: Number(dateIndex), 
                                    date:date, 
                                    startTime:Vue.filter('beautify-time')(previousConflictEndDate), 
                                    endTime:Vue.filter('beautify-time')(conflict.start),
                                    type:'Unavailable',
                                    workSection: ''
                                })
                            previousConflictEndDate = conflict.end;  
                        }
                    }   
                }

                if( numberOfConflictsPerDay == 0){
                    conflicts.push({
                        id:'0',
                        location:'',
                        dayOffset: Number(dateIndex), 
                        date:date, 
                        startTime:'', 
                        endTime:'',
                        type:'Unavailable', 
                        workSection: ''
                    })
                } else if( numberOfConflictsPerDay > 1){
                    const start = moment(previousConflictEndDate)
                    const end = moment(date).endOf('day')
                    const duration = moment.duration(end.diff(start)).asMinutes();
                    if(duration>5)
                        conflicts.push({
                            id:'0',
                            location: '',
                            dayOffset: Number(dateIndex), 
                            date:date, 
                            startTime:Vue.filter('beautify-time')(previousConflictEndDate), 
                            endTime:Vue.filter('beautify-time')(end.format()),
                            type:'Unavailable', 
                            workSection:''
                        })
                }
            }
            const shifts = this.extractSchedules(conflictsJson, true);
            for (const shift of shifts) {
                conflicts.push(shift);
            }
            return conflicts
        }

        public getConflictsType(conflict){
            if(conflict.conflict =='AwayLocation') return 'Loaned'
            else if(conflict.conflict =='Scheduled') return 'Shift'
            else return conflict.conflict
        }

        public getWorkSection(conflict){
            if(conflict.conflict =='Scheduled') {
                const WS = conflict.workSection?conflict.workSection: '';
                return WS;
            } 
            else return '';
        }
        
        public extractSchedules(conflictsJson, onlyShedules){

            const schedules: scheduleInfoType[] = []

            for(const conflict of conflictsJson){
                if (conflict.conflict=="Scheduled" && conflict.locationId != this.location.id) continue;
                if (conflict.conflict!="Scheduled" && onlyShedules) continue;
                conflict.start = moment(conflict.start).tz(this.location.timezone).format();
                conflict.end = moment(conflict.end).tz(this.location.timezone).format();               
                if(Vue.filter('isDateFullday')(conflict.start,conflict.end))
                {               
                    for(const dateIndex in this.headerDates){
                        const date = this.headerDates[dateIndex]
                        if(date>=conflict.start && date<=conflict.end)
                        {
                            schedules.push({
                                id:conflict.shiftId? conflict.shiftId:0,
                                location:conflict.conflict=='AwayLocation'?conflict.location.name:'',
                                dayOffset: Number(dateIndex), 
                                date:date, 
                                startTime:'', 
                                endTime:'',
                                type:this.getConflictsType(conflict), 
                                workSection: this.getWorkSection(conflict)
                            })        
                        }                       
                    }
                }
                else{
                    
                    for(const dateIndex in this.headerDates){
                        const date = this.headerDates[dateIndex].substring(0,10);
                        const nextDate = moment(this.headerDates[dateIndex]).add(1,'days').format().substring(0,10);
                        if(date == conflict.start.substring(0,10) && date == conflict.end.substring(0,10))
                        {  
                            const start = moment(conflict.start)
                            const end = moment(conflict.end)
                            const duration = moment.duration(end.diff(start));//duration.asMinutes()                            
                            schedules.push({
                                id:conflict.shiftId? conflict.shiftId:0,
                                location:conflict.conflict=='AwayLocation'?conflict.location.name:'',
                                dayOffset: Number(dateIndex), 
                                date:this.headerDates[dateIndex], 
                                startTime:Vue.filter('beautify-time')(conflict.start), 
                                endTime:Vue.filter('beautify-time')(conflict.end), 
                                type:this.getConflictsType(conflict), 
                                workSection:this.getWorkSection(conflict)
                            })        
                        } else if(date == conflict.start.substring(0,10) && nextDate == conflict.end.substring(0,10))
                        {  
                            const start = moment(conflict.start)
                            const midnight = moment(conflict.start).endOf('day')
                            const end = moment(conflict.end)
                            const durationStart = moment.duration(midnight.diff(start));
                            const durationEnd = moment.duration(end.diff(midnight));
                            schedules.push({
                                id:conflict.shiftId? conflict.shiftId:0,
                                location:conflict.conflict=='AwayLocation'?conflict.location.name:'',
                                dayOffset: Number(dateIndex), 
                                date:this.headerDates[dateIndex], 
                                startTime:Vue.filter('beautify-time')(conflict.start), 
                                endTime:Vue.filter('beautify-time')(midnight.format()),
                                type:this.getConflictsType(conflict), 
                                workSection:this.getWorkSection(conflict)
                            })
                            schedules.push({
                                id:conflict.shiftId? conflict.shiftId:0,
                                location:conflict.conflict=='AwayLocation'?conflict.location.name:'',
                                dayOffset: Number(dateIndex)+1, 
                                date:moment(this.headerDates[dateIndex]).add(1,'day').format(), 
                                startTime:'00:00', 
                                endTime:Vue.filter('beautify-time')(conflict.end),
                                type:this.getConflictsType(conflict), 
                                workSection:this.getWorkSection(conflict)
                            })        
                        }                       
                    }
                } 
            }

            return schedules
        } 

    }
</script>

<style scoped>    
    .card {
        border: white;
    }   

</style>