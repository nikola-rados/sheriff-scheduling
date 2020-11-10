<template>
    <b-card bg-variant="white" class="mx-1 home" no-body>
        
        <schedule-header v-on:change="loadScheduleInformation()" />
        <loading-spinner v-if="!isManageScheduleDataMounted" />
        <div v-else>
            <b-table
                :key="updateTable"
                :items="shiftSchedules" 
                :fields="fields"
                head-row-variant="primary"   
                bordered
                fixed>
                    <template v-slot:table-colgroup>
                        <col style="width:8.5rem">                            
                    </template>
                    <template v-slot:head() = "data" >
                        <span class="text-danger">{{data.column}}</span> <span> {{data.label}}</span>
                    </template>
                    <template v-slot:head(myteam) = "data" >                           
                        <span> {{data.label}}</span>
                    </template>
                    <template v-slot:cell()="data" >
                        <schedule-card :sheriffId="data.item.myteam.sheriffId" :scheduleInfo=" data.value"/>
                    </template>
                    <template v-slot:cell(myteam) = "data" > 
                        <team-member-card v-on:change="loadScheduleInformation()" :sheriffInfo=data.item.myteam />
                    </template>
            </b-table>
            <b-card><br></b-card>
        </div>  
           
    </b-card>
</template>

<script lang="ts">
    import { Component, Vue, Watch } from 'vue-property-decorator';
    import { namespace } from "vuex-class";
    import ScheduleCard from './components/ScheduleCard.vue'
    import TeamMemberCard from './components/TeamMemberCard.vue'
    import ScheduleHeader from './components/ScheduleHeader.vue'
    import "@store/modules/ShiftScheduleInformation";   
    const shiftState = namespace("ShiftScheduleInformation");
    import "@store/modules/CommonInformation";
    const commonState = namespace("CommonInformation");
    import store from '../../store';
    import { locationInfoType } from '../../types/common';
    import { sheriffAvailabilityInfoType, shiftRangeInfoType, weekShiftInfoType,conflictsInfoType } from '../../types/ShiftSchedule/index'
    import { sheriffsAvailabilityJsonType } from '../../types/ShiftSchedule/jsonTypes';
    import moment from 'moment-timezone';
    import * as _ from 'underscore';

    @Component({
        components: {
            ScheduleCard,
            TeamMemberCard,
            ScheduleHeader
        }
    })
    export default class ManageSchedule extends Vue {

        @commonState.State
        public location!: locationInfoType;

        @shiftState.State
        public shiftRangeInfo!: shiftRangeInfoType;

        @shiftState.Action
        public UpdateSelectedShifts!: (newSelectedShifts: string[]) => void

        @shiftState.State
        public sheriffsAvailabilityInfo!: sheriffAvailabilityInfoType[];

        @shiftState.Action
        public UpdateSheriffsAvailabilityInfo!: (newSheriffsAvailabilityInfo: sheriffAvailabilityInfoType[]) => void

        isManageScheduleDataMounted = false;

        headerDates: string[] = [];
        numberOfheaderDates = 7;
        updateTable=0;

        fields=[
            {key:'myteam', label:'My Team', tdClass:'px-0 mx-0'},
            {key:'Sun', label:'', tdClass:'px-0 mx-0', thStyle:'text-align: center;'},
            {key:'Mon', label:'', tdClass:'px-0 mx-0', thStyle:'text-align: center;'},
            {key:'Tue', label:'', tdClass:'px-0 mx-0', thStyle:'text-align: center;'},
            {key:'Wed', label:'', tdClass:'px-0 mx-0', thStyle:'text-align: center;'},
            {key:'Thu', label:'', tdClass:'px-0 mx-0', thStyle:'text-align: center;'},
            {key:'Fri', label:'', tdClass:'px-0 mx-0', thStyle:'text-align: center;'},
            {key:'Sat', label:'', tdClass:'px-0 mx-0', thStyle:'text-align: center;'}
        ]

        shiftSchedules: weekShiftInfoType[] =[];

        @Watch('location.id', { immediate: true })
        locationChange()
        {
            if (this.isManageScheduleDataMounted) {
                this.loadScheduleInformation() 
                //console.log('watch')               
            }            
        } 

        // mounted()
        // {                       
        //     //this.loadScheduleInformation();
        //     console.log('mount manage')
        // }

        public loadScheduleInformation() {

            this.UpdateSelectedShifts([]);
            this.isManageScheduleDataMounted=false;

            this.headerDate();

            const endDate = moment(this.shiftRangeInfo.endDate).endOf('day').format();

            const url = 'api/shift/shiftavailability?locationId='+this.location.id+'&start='+this.shiftRangeInfo.startDate+'&end='+endDate;
            this.$http.get(url)
                .then(response => {
                    if(response.data){
                        console.log(response.data)
                        this.extractTeamAvailabilityInfo(response.data);                        
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
            //console.log(this.headerDates)
        }

        public extractTeamAvailabilityInfo(sheriffsAvailabilityJson: sheriffsAvailabilityJsonType[]) {

           // const sheriffsAvailability: sheriffAvailabilityInfoType[] = [];
            this.shiftSchedules = [];
            
            for(const sheriffAvailabilityJson of sheriffsAvailabilityJson) {
                const sheriffAvailability = {} as sheriffAvailabilityInfoType;
                sheriffAvailability.sheriffId = sheriffAvailabilityJson.sheriffId;
                sheriffAvailability.firstName = sheriffAvailabilityJson.sheriff.firstName;
                sheriffAvailability.lastName = sheriffAvailabilityJson.sheriff.lastName;
                sheriffAvailability.badgeNumber = sheriffAvailabilityJson.sheriff.badgeNumber;
                sheriffAvailability.rank = sheriffAvailabilityJson.sheriff.rank;
                sheriffAvailability.homeLocation= {id: sheriffAvailabilityJson.sheriff.homeLocation.id, name: sheriffAvailabilityJson.sheriff.homeLocation.name}
                const isInLoanLocation = (sheriffAvailabilityJson.sheriff.homeLocation.id !=this.location.id)
                sheriffAvailability.conflicts =isInLoanLocation? this.extractInLoanLocationConflicts(sheriffAvailabilityJson.conflicts) :this.extractConflicts(sheriffAvailabilityJson.conflicts, false);        
                //sheriffsAvailability.push(sheriffAvailability)
               // console.log(isInLoanLocation)
                
                this.shiftSchedules.push({
                    myteam: sheriffAvailability,
                    Sun: sheriffAvailability.conflicts.filter(conflict=>{if(conflict.dayOffset ==0) return true}),
                    Mon: sheriffAvailability.conflicts.filter(conflict=>{if(conflict.dayOffset ==1) return true}),
                    Tue: sheriffAvailability.conflicts.filter(conflict=>{if(conflict.dayOffset ==2) return true}),
                    Wed: sheriffAvailability.conflicts.filter(conflict=>{if(conflict.dayOffset ==3) return true}),
                    Thu: sheriffAvailability.conflicts.filter(conflict=>{if(conflict.dayOffset ==4) return true}),
                    Fri: sheriffAvailability.conflicts.filter(conflict=>{if(conflict.dayOffset ==5) return true}),
                    Sat: sheriffAvailability.conflicts.filter(conflict=>{if(conflict.dayOffset ==6) return true})
                })
            }
            //this.UpdateSheriffsAvailabilityInfo(sheriffsAvailability);
            this.isManageScheduleDataMounted = true;
            this.updateTable++;
        }

        public extractConflicts(conflictsJson, onlyShedules){

            const conflicts: conflictsInfoType[] = []

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
                            // console.error('start_end_now')
                            // console.log(conflict.conflict)
                            // console.log(conflict.start)
                            // console.log(conflict.end)
                            // console.log(date)
                            conflicts.push({
                                id:conflict.shiftId? conflict.shiftId:0,
                                location:conflict.conflict=='AwayLocation'?conflict.location.name:'',
                                dayOffset: Number(dateIndex), 
                                date:date, 
                                startTime:'', 
                                endTime:'',
                                startInMinutes:0, 
                                timeDuration:0, 
                                type:this.getConflictsType(conflict), 
                                fullday: true
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
                            // console.warn('start_end_now')
                            // console.log(conflict.conflict)
                            // console.log(conflict.start)
                            // console.log(conflict.end)
                            // console.log(date)
                            // console.log(nextDate)
                            // console.log(moment.duration(start.diff(moment(conflict.start).startOf('day'))).asMinutes())
                            // console.log(duration.asMinutes())
                            conflicts.push({
                                id:conflict.shiftId? conflict.shiftId:0,
                                location:conflict.conflict=='AwayLocation'?conflict.location.name:'',
                                dayOffset: Number(dateIndex), 
                                date:this.headerDates[dateIndex], 
                                startTime:Vue.filter('beautify-time')(conflict.start), 
                                endTime:Vue.filter('beautify-time')(conflict.end), 
                                startInMinutes:moment.duration(start.diff(moment(conflict.start).startOf('day'))).asMinutes(),
                                timeDuration:duration.asMinutes(), 
                                type:this.getConflictsType(conflict), 
                                fullday:false
                            })        
                        }else if(date == conflict.start.substring(0,10) && nextDate == conflict.end.substring(0,10))
                        {  
                            const start = moment(conflict.start)
                            const midnight = moment(conflict.start).endOf('day')
                            const end = moment(conflict.end)
                            const durationStart = moment.duration(midnight.diff(start));
                            const durationEnd = moment.duration(end.diff(midnight));
                            // console.warn('Next______Day')
                            // console.log(conflict.conflict)
                            // console.log(conflict.start)
                            // console.log(conflict.end)
                            // console.log(date)
                            // console.log(nextDate)
                            // console.log(moment.duration(start.diff(moment(conflict.start).startOf('day'))).asMinutes())
                            // console.log(durationStart.asMinutes())
                            // console.log(durationEnd.asMinutes())
                            conflicts.push({
                                id:conflict.shiftId? conflict.shiftId:0,
                                location:conflict.conflict=='AwayLocation'?conflict.location.name:'',
                                dayOffset: Number(dateIndex), 
                                date:this.headerDates[dateIndex], 
                                startTime:Vue.filter('beautify-time')(conflict.start), 
                                endTime:Vue.filter('beautify-time')(midnight.format()), 
                                startInMinutes:moment.duration(start.diff(moment(conflict.start).startOf('day'))).asMinutes(),
                                timeDuration:durationStart.asMinutes(), 
                                type:this.getConflictsType(conflict), 
                                fullday:false
                            })
                            conflicts.push({
                                id:conflict.shiftId? conflict.shiftId:0,
                                location:conflict.conflict=='AwayLocation'?conflict.location.name:'',
                                dayOffset: Number(dateIndex)+1, 
                                date:moment(this.headerDates[dateIndex]).add(1,'day').format(), 
                                startTime:'00:00', 
                                endTime:Vue.filter('beautify-time')(conflict.end), 
                                startInMinutes:0,
                                timeDuration:durationEnd.asMinutes(), 
                                type:this.getConflictsType(conflict), 
                                fullday:false
                            })        
                        }                       
                    }
                } 
            }
            //console.log(conflicts)
            //console.log(moment())

            return conflicts
        } 

        public extractInLoanLocationConflicts(conflictsJson){
            let conflictsJsonAwayLocation: any[] = []
            const conflicts: conflictsInfoType[] = []
            for(const conflict of conflictsJson){  
                conflict.start = moment(conflict.start).tz(this.location.timezone).format();
                conflict.end = moment(conflict.end).tz(this.location.timezone).format();              
                if(conflict.conflict !='AwayLocation' || conflict.locationId != this.location.id) continue;
                //console.log(conflict)
                conflict['startDay']=conflict.start.substring(0,10);
                conflict['endDay']=conflict.end.substring(0,10);
                conflictsJsonAwayLocation.push(conflict);
            }
            conflictsJsonAwayLocation = _.sortBy(conflictsJsonAwayLocation,'start')
            //console.log(conflictsJsonAwayLocation)

            for(const dateIndex in this.headerDates){
                const date = this.headerDates[dateIndex]
                const day = date.substring(0,10);
                let numberOfConflictsPerDay = 0;
                let previousConflictEndDate = moment(date).startOf('day').format();
                for(const conflict of conflictsJsonAwayLocation){

                    if(day>=conflict.startDay && day<=conflict.endDay)
                    {  
                        // console.error('start_end_now')                    
                        // console.log(conflict.start)
                        // console.log(conflict.end)
                        // console.log(date)
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
                                    startInMinutes:moment.duration(start.diff(moment(conflict.start).startOf('day'))).asMinutes(),
                                    timeDuration:duration, 
                                    type:'Unavailable', 
                                    fullday:false
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
                        startInMinutes:0, 
                        timeDuration:0, 
                        type:'Unavailable', 
                        fullday: true
                    })
                } else if( numberOfConflictsPerDay > 1){
                    // console.warn('start_end_now')                    
                    // console.log(previousConflictEndDate)
                    // console.log(date)
                    const start = moment(previousConflictEndDate)
                    const end = moment(date).endOf('day')
                    const duration = moment.duration(end.diff(start)).asMinutes();
                    // console.log(start)
                    // console.log(end)
                    // console.log(duration)
                    if(duration>5)
                        conflicts.push({
                            id:'0',
                            location: '',
                            dayOffset: Number(dateIndex), 
                            date:date, 
                            startTime:Vue.filter('beautify-time')(previousConflictEndDate), 
                            endTime:Vue.filter('beautify-time')(end.format()), 
                            startInMinutes:moment.duration(start.diff(moment(previousConflictEndDate).startOf('day'))).asMinutes(),
                            timeDuration:duration, 
                            type:'Unavailable', 
                            fullday:false
                        })
                }
            }
            const shifts = this.extractConflicts(conflictsJson, true);

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

    }
</script>

<style scoped>   

    .card {
        border: white;
    }

</style>