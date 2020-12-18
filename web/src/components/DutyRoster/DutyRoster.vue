<template>
    <b-card bg-variant="white" class="home" no-body>

        <b-row  class="mx-0 mt-0 mb-5 p-0" cols="2" >
            <b-col class="m-0 p-0" cols="11" >
                <duty-roster-header v-on:change="reloadDutyRosters" :runMethod="headerAddAssignment" />
                <duty-roster-week-view v-if="weekView" :key="updateDutyRoster" v-on:addAssignmentClicked="addAssignment" v-on:dataready="reloadMyTeam()" />
                <duty-roster-day-view v-if="!weekView&&headerReady" :key="updateDutyRoster" v-on:addAssignmentClicked="addAssignment" v-on:dataready="reloadMyTeam()" />
                
            </b-col>
            <b-col class="p-0 " cols="1"  style="overflow: auto;">
                <b-card 
                    v-if="isDutyRosterDataMounted"
                    :key="updateMyTeam" 
                    body-class="mx-2 p-0"
                    style="overflow-x: hidden;"
                    class="bg-dark m-0 p-0 no-top-rounding">
                        <b-card-header header-class="m-0 text-white py-2 px-0"> 
                            My Team
                            <b-button
                                v-if="!weekView"
                                @click="toggleDisplayMyteam()"
                                v-b-tooltip.hover                            
                                title="Display Graphical Availability of MyTeam "                            
                                style="font-size:10px; width:1.1rem; margin:0 0 0 .2rem; padding:0; background-color:white; color:#189fd4;" 
                                size="sm">
                                    <b-icon-bar-chart-steps /> 
                            </b-button>
                        </b-card-header>
                        <duty-roster-team-member-card :sheriffInfo="memberNotRequired" :weekView="weekView"/>
                        <duty-roster-team-member-card :sheriffInfo="memberNotAvailable" :weekView="weekView"/> 
                        <div :style="{overflowX: 'hidden', overflowY: 'auto', height: getheight}">
                            <duty-roster-team-member-card v-for="member in shiftAvailabilityInfo" :key="member.sheriffId" :sheriffInfo="member" :weekView="weekView"/>
                        </div>
                </b-card>
            </b-col>
        </b-row>
    </b-card>
</template>

<script lang="ts">
    import { Component, Vue, Watch, Emit } from 'vue-property-decorator';
    import DutyRosterHeader from './components/DutyRosterHeader.vue'
    import DutyRosterTeamMemberCard from './components/DutyRosterTeamMemberCard.vue'

    import DutyRosterDayView from './DutyRosterDayView.vue';
    import DutyRosterWeekView from './DutyRosterWeekView.vue'

    import moment from 'moment-timezone';

    import { namespace } from "vuex-class";   
    import "@store/modules/CommonInformation";
    const commonState = namespace("CommonInformation");

    import "@store/modules/DutyRosterInformation";   
    const dutyState = namespace("DutyRosterInformation");

    import { localTimeInfoType } from '../../types/common';
    import { assignmentCardInfoType, attachedDutyInfoType, dutyRangeInfoType, myTeamShiftInfoType, dutiesDetailInfoType} from '../../types/DutyRoster';
    import { shiftInfoType } from '../../types/ShiftSchedule';

    @Component({
        components: {
            DutyRosterHeader,
            DutyRosterTeamMemberCard,
            DutyRosterDayView,
            DutyRosterWeekView
        }
    })
    export default class DutyRoster extends Vue {

        @commonState.State
        public localTime!: localTimeInfoType;

        @commonState.Action
        public UpdateLocalTime!: (newLocalTime: localTimeInfoType) => void

        @dutyState.State
        public dutyRangeInfo!: dutyRangeInfoType;

        @commonState.State
        public displayFooter!: boolean;

        @commonState.Action
        public UpdateDisplayFooter!: (newDisplayFooter: boolean) => void

        @dutyState.State
        public shiftAvailabilityInfo!: myTeamShiftInfoType[];

        memberNotRequired = {} as myTeamShiftInfoType;
        memberNotAvailable = {} as myTeamShiftInfoType;
        isDutyRosterDataMounted = false;

        updateDutyRoster = 0;
        updateMyTeam = 0;

        weekView = false;
        headerReady = false;

        headerAddAssignment = new Vue();      

        mounted()
        {
            this.isDutyRosterDataMounted = false;
            this.memberNotRequired.sheriffId ='00000-00000-11111';
            this.memberNotAvailable.sheriffId ='00000-00000-22222';
            

            window.setTimeout(this.updateCurrentTimeCallBack, 1000);
        }

        public reloadDutyRosters(type){
            this.isDutyRosterDataMounted = false;
            console.log(type)
            console.log('reload dutyroster')                
            this.updateCurrentTime();
            if(type=='Day'){
                this.weekView = false
                this.UpdateDisplayFooter(false)
            } else{
                this.weekView = true
                this.UpdateDisplayFooter(true)
            }

            this.headerReady = true;
            this.updateDutyRoster++;

        }

        public reloadMyTeam(){
            this.isDutyRosterDataMounted=true
            this.updateMyTeam++;
        }

        public toggleDisplayMyteam(){
            if(this.displayFooter){
                this.UpdateDisplayFooter(false)
                const el = document.getElementsByClassName('b-table-sticky-header') 
                Vue.nextTick(()=>{            
                    if(el[1]) el[1].scrollLeft = el[0].scrollLeft
                })
            }
            else this.UpdateDisplayFooter(true)
        }        

        public addAssignment(){ 
            this.headerAddAssignment.$emit('addassign');
        }

        public updateCurrentTime() {
            const currentTime = moment();
            const startOfDay = moment(currentTime).startOf('day')
            const timeBin = (moment.duration(currentTime.diff(startOfDay)).asMinutes()/15 +0.5)            
            const currentTimeObject = { 
                timeString: currentTime.format(), 
                timeSlot: Math.floor(timeBin),                
                dayOfWeek: currentTime.weekday(),
                isTodayInView: currentTime.isBetween(this.dutyRangeInfo.startDate, this.dutyRangeInfo.endDate),
            }
            //console.log(currentTimeObject)
            this.UpdateLocalTime(currentTimeObject);
        }

        public updateCurrentTimeCallBack() {
            this.updateCurrentTime();
            window.setTimeout(this.updateCurrentTime, 60000);
        }

        get getheight(){
            const height = 0.02811625*(window.innerWidth)-17
            return (this.displayFooter? (height+3)+'rem' : height+'rem')
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
        grid-template-columns: repeat(24, 4.1666%);
        grid-template-rows: 1.65rem;
        inline-size: 100%;
        font-size: 10px;
        height: 1.58rem;         
    }

    .grid24 > * {      
        padding: 0.3rem 0;
        border: 1px dotted rgb(185, 143, 143);
    }

</style>