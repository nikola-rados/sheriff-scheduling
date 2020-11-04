<template>
    <b-card bg-variant="white" class="home" no-body>
        <!-- <loading-spinner v-if="!isManageScheduleDataMounted" />     -->
        <b-row  class="m-0 p-0" cols="2" >
                      
            <!-- <b-col class="pl-1 " cols="1" style="overflow: auto;">
                <b-card style="width:113%;" class="bg-dark mb-5" no-body header-class="text-white " header="My Team"> 
                    
                </b-card>
            </b-col> -->
            <b-col class="m-0 p-0" cols="11" style="overflow: auto;">
                <schedule-header v-on:change="loadScheduleInformation()" />
                <b-table
                    :items="shiftSchedules" 
                    :fields="fields"
                    head-row-variant="primary"   
                    bordered
                    fixed>
                        <template v-slot:head() = "data" >
                            <span class="text-success">{{data.column}}</span> <span> {{data.label}}</span>
                        </template>
                        <template v-slot:head(myteam) = "data" >
                           
                            <span> {{data.label}}</span>
                        </template>
                        <!-- <template v-slot:cell(Sun) = "data" >  
                            <schedule-card :shiftInfo="data.item.Sun"/>
                        </template> -->

                        <template v-slot:cell(myteam) = "data" > {{data}}
                            <team-member-card  :sheriffInfo=data.item.myteam />
                        </template>
                </b-table>
                <b-card><br></b-card>  
            </b-col>
        </b-row>
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
    import { sheriffAvailabilityInfoType, shiftRangeInfoType, weekShiftInfoType } from '../../types/ShiftSchedule/index'
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

        teamMembers = [
            {badgeNumber:1231},
            {badgeNumber:4561},
            {badgeNumber:7891},
            {badgeNumber:9991},
        ]

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

        shiftSchedules: weekShiftInfoType[] =[]
        
        //         Sun:{date:'2020-1-1', startTime:60 ,timeDuration: 20, type: 'court', subType:'', color: 'info', timeStamp: '08:00 - 16:00', assignee:''},
        

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
            console.log('getting shifts')
            this.isManageScheduleDataMounted = true;
        }

        public loadScheduleInformation() {

            this.isManageScheduleDataMounted=false;

            this.headerDate();

            const url = 'api/shift/shiftavailability?locationId='+this.location.id+'&start='+this.shiftRangeInfo.startDate+'&end='+this.shiftRangeInfo.endDate;
            this.$http.get(url)
                .then(response => {
                    if(response.data){
                        console.log(response.data)
                        this.extractTeamAvailabilityInfo(response.data);                        
                    }                                   
                })            
        }

        public headerDate() {
            
            for(let dayOffset=0; dayOffset<7; dayOffset++)
            {
                const date= moment(this.shiftRangeInfo.startDate).add(dayOffset,'days').format()
                this.fields[dayOffset+1].label = ' ' + Vue.filter('beautify-date')(date);
            }
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
                sheriffAvailability.conflicts = sheriffAvailabilityJson.conflicts;        
                //sheriffsAvailability.push(sheriffAvailability)
                console.log(sheriffAvailability)
                this.shiftSchedules.push({myteam:sheriffAvailability,Sun:{},Mon:{},Tue:{},Wed:{},Thu:{},Fri:{},Sat:{}})
            }
            
            
            //this.UpdateSheriffsAvailabilityInfo(sheriffsAvailability);
            this.getShifts();
        }

    }
</script>

<style scoped>   

    .card {
        border: white;
    }

</style>