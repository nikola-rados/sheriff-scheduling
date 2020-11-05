<template>
    <b-card bg-variant="white" class="mx-1 home" no-body>
        <!-- <loading-spinner v-if="!isManageScheduleDataMounted" />     -->
       
                <schedule-header v-on:change="loadScheduleInformation()" />
                <b-table
                    :items="shiftSchedules" 
                    :fields="fields"
                    head-row-variant="primary"   
                    bordered
                    fixed>
                        <template v-slot:table-colgroup>
                            <col style="width:6.5rem">                            
                        </template>
                        <template v-slot:head() = "data" >
                            <span class="text-success">{{data.column}}</span> <span> {{data.label}}</span>
                        </template>
                        <template v-slot:head(myteam) = "data" >                           
                            <span> {{data.label}}</span>
                        </template>
                        <template v-slot:cell()="data"  >                          
                            <schedule-card   :shiftInfo=" data.value"/>
                            <!-- <b-card 
                                style="display:inline-block; background-color:grey; position: relative; left:10%; width:20%; height:6rem;"> 
                            </b-card>
                            <b-card 
                                style="display:inline-block;background-color:blue; position: relative; left:15%; width:10%; height:6rem;"> 
                            </b-card>
                            <b-card 
                                style="display:inline-block;background-color:red; position: relative; left:25%; width:15%; height:6rem;"> 
                            </b-card>
                            <b-card 
                                style="display:inline-block;background-color:green; position: relative; left:25%; width:15%; height:6rem;"> 
                            </b-card> -->
                        </template>

                        <template v-slot:cell(myteam) = "data" >                           
                            <team-member-card  :sheriffInfo=data.item.myteam />
                        </template>
                </b-table>
                <b-card><br></b-card>  
           
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

        @shiftState.State
        public sheriffsAvailabilityInfo!: sheriffAvailabilityInfoType[];

        @shiftState.Action
        public UpdateSheriffsAvailabilityInfo!: (newSheriffsAvailabilityInfo: sheriffAvailabilityInfoType[]) => void

        isManageScheduleDataMounted = false;

        headerDates: string[] = [];
        numberOfheaderDates = 7;

        fields=[
            {key:'myteam',label:'My Team', tdClass:'px-0 mx-0'},
            {key:'Sun', label:'', tdClass:'px-0 mx-0'},
            {key:'Mon', label:'', tdClass:'px-0 mx-0'},
            {key:'Tue', label:'', tdClass:'px-0 mx-0'},
            {key:'Wed', label:'', tdClass:'px-0 mx-0'},
            {key:'Thu', label:'', tdClass:'px-0 mx-0'},
            {key:'Fri', label:'', tdClass:'px-0 mx-0'},
            {key:'Sat', label:'', tdClass:'px-0 mx-0'}
        ]

        shiftSchedules: weekShiftInfoType[] =[];

        @Watch('location.id', { immediate: true })
        locationChange()
        {
            if (this.isManageScheduleDataMounted) {
                this.loadScheduleInformation()                
            }            
        } 

        mounted()
        {                       
            this.loadScheduleInformation();
        }

        public getShifts() {
            //GET shifts using the /api/shift call
            //console.log('getting shifts')
            this.isManageScheduleDataMounted = true;
        }

        public loadScheduleInformation() {

            this.isManageScheduleDataMounted=false;

            this.headerDate();

            const url = 'api/shift/shiftavailability?locationId='+this.location.id+'&start='+this.shiftRangeInfo.startDate+'&end='+this.shiftRangeInfo.endDate;
            this.$http.get(url)
                .then(response => {
                    if(response.data){
                        //console.log(response.data)
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
                sheriffAvailability.conflicts = this.extractConflicts(sheriffAvailabilityJson.conflicts);        
                //sheriffsAvailability.push(sheriffAvailability)
                //console.log(sheriffAvailability)
                
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
            this.getShifts();
        }

        public extractConflicts(conflictsJson){

            const conflicts: conflictsInfoType[] = []

            for(const conflict of conflictsJson){                
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
                                dayOffset: Number(dateIndex), 
                                date:date, 
                                startTime:'', 
                                endTime:'',
                                startInMinutes:0, 
                                timeDuration:0, 
                                type: conflict.conflict, 
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
                                dayOffset: Number(dateIndex), 
                                date:this.headerDates[dateIndex], 
                                startTime:Vue.filter('beautify-time')(conflict.start), 
                                endTime:Vue.filter('beautify-time')(conflict.end), 
                                startInMinutes:moment.duration(start.diff(moment(conflict.start).startOf('day'))).asMinutes(),
                                timeDuration:duration.asMinutes(), 
                                type:conflict.conflict, 
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
                                dayOffset: Number(dateIndex), 
                                date:this.headerDates[dateIndex], 
                                startTime:Vue.filter('beautify-time')(conflict.start), 
                                endTime:Vue.filter('beautify-time')(midnight.format()), 
                                startInMinutes:moment.duration(start.diff(moment(conflict.start).startOf('day'))).asMinutes(),
                                timeDuration:durationStart.asMinutes(), 
                                type:conflict.conflict, 
                                fullday:false
                            })
                            conflicts.push({
                                dayOffset: Number(dateIndex)+1, 
                                date:this.headerDates[dateIndex]+1, 
                                startTime:'00:00', 
                                endTime:Vue.filter('beautify-time')(conflict.end), 
                                startInMinutes:0,
                                timeDuration:durationEnd.asMinutes(), 
                                type:conflict.conflict, 
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

    }
</script>

<style scoped>   

    .card {
        border: white;
    }

</style>