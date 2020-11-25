<template>
    <b-card bg-variant="white" class="home" no-body>

        <distribute-header v-on:change="loadScheduleInformation" />

        <b-row>
            <b-card>B.C. Sheriff Service
                <img 
              class="img-fluid d-none d-lg-block"          
              src="../../../public/images/bcss-crest.png"
              width="177"
              height="44"
              alt="B.C. Government Logo"
            />
          <img>
            </b-card>
            <b-card>{{location.name}}</b-card>

        </b-row>
        <b-card><br></b-card>

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
                    </template>
                    
            </b-table>
            <div v-if="!isDistributeDataMounted && this.sheriffSchedules.length == 0" style="min-height:115.6px;">
            </div>
        </b-overlay>
        <b-card><br></b-card>        
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
    import { sheriffAvailabilityInfoType, shiftRangeInfoType, weekShiftInfoType,scheduleInfoType, weekScheduleInfoType, distributeScheduleInfoType } from '../../types/ShiftSchedule/index'
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

        isDistributeDataMounted = false;
        headerDates: string[] = [];
        numberOfheaderDates = 7;
        updateTable=0;

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
                this.loadScheduleInformation(false); 
                //console.log('watch')               
            }            
        }

        public loadScheduleInformation(includeWS: boolean) {
            
            this.isDistributeDataMounted=false;
            this.headerDate();
            const endDate = moment(this.shiftRangeInfo.endDate).endOf('day').format();

            const url = 'api/distributeschedule/location?locationId='
                        +this.location.id+'&start='+this.shiftRangeInfo.startDate+'&end='+endDate + '&includeWorkSection='+includeWS;
            this.$http.get(url)
                .then(response => {
                    if(response.data){
                        console.log(response.data)
                        this.extractTeamScheduleInfo(response.data);                        
                    }                                   
                })            
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