<template>
    <b-card bg-variant="white" class="home" no-body>
        <b-row  class="mx-0 mt-0 mb-5 p-0" cols="2" >
            <b-col class="m-0 p-0" cols="11" >
                <duty-roster-header v-on:change="reloadDutyRosters" :runMethod="headerAddAssignment" />
                <duty-roster-week-view :runMethod="dutyRosterWeekViewMethods" v-if="weekView" :key="updateDutyRoster" v-on:addAssignmentClicked="addAssignment" v-on:dataready="reloadMyTeam()" />
                <duty-roster-day-view :runMethod="dutyRosterDayViewMethods" v-if="!weekView&&headerReady" :key="updateDutyRoster" v-on:addAssignmentClicked="addAssignment" v-on:dataready="reloadMyTeam()"/>
                
            </b-col>
            <b-col class="p-0 " cols="1"  style="overflow: auto;">
                <b-card
                    v-if="isDutyRosterDataMounted"
                    :key="updateMyTeam"                     
                    body-class="mx-2 p-0"
                    style="overflow-x: hidden;"
                    class="bg-dark m-0 p-0 no-top-rounding">
                    <div id="myTeamHeader" class="mb-2">
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
                        <duty-roster-team-member-card :sheriffInfo="memberIsClosed" :weekView="weekView"/>  
                    </div>                   
                    <div id="dutyrosterteammember" :style="{overflowX: 'hidden', overflowY: 'auto', height: getHeight}">
                        <duty-roster-team-member-card v-on:change="updateDutyRosterPage()" v-for="member in shiftAvailabilityInfo" :key="member.sheriffId" :sheriffInfo="member" :weekView="weekView"/>
                    </div>
                </b-card>
            </b-col>
        </b-row>
    </b-card>
</template>

<script lang="ts">
    import { Component, Vue, Watch } from 'vue-property-decorator';
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
    import { dutyRangeInfoType, myTeamShiftInfoType} from '../../types/DutyRoster';
    
    @Component({
        components: {
            DutyRosterHeader,
            DutyRosterTeamMemberCard,
            DutyRosterDayView,
            DutyRosterWeekView
        }
    })
    export default class ManageDutyRoster extends Vue {

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

        memberNotRequired = { sheriffId: '00000-00000-11111' } as myTeamShiftInfoType;
        memberNotAvailable = { sheriffId: '00000-00000-22222' } as myTeamShiftInfoType;
        memberIsClosed = { sheriffId: '00000-00000-33333' } as myTeamShiftInfoType;
        
        isDutyRosterDataMounted = false;
        updateDutyRoster = 0;
        updateMyTeam = 0;

        weekView = false;
        headerReady = false;
        windowHeight = 0;
        bottomHeight = 0;
        gageHeight = 0;
        tableHeight = 0;

        headerAddAssignment = new Vue();         
        dutyRosterDayViewMethods = new Vue();
        dutyRosterWeekViewMethods = new Vue();

        @Watch('displayFooter')
        footerChange() 
        {
            Vue.nextTick(() => 
            {
                this.calculateTableHeight()
            })
        }

        mounted()
        {
            this.isDutyRosterDataMounted = false;
            window.setTimeout(this.updateCurrentTimeCallBack, 1000);
            window.addEventListener('resize', this.getWindowHeight);
            this.getWindowHeight()
        }

        beforeDestroy() {
            window.removeEventListener('resize', this.getWindowHeight);
        }
        
        public reloadDutyRosters(type){
            this.isDutyRosterDataMounted = false;
            // console.log(type)
            // console.log('reload dutyroster')                
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
            Vue.nextTick(()=>this.calculateTableHeight())
            this.updateMyTeam++;
        }

        public getWindowHeight() {
            this.windowHeight = document.documentElement.clientHeight;   
            this.calculateTableHeight();     
        }

        get getHeight() {
            return this.windowHeight - this.tableHeight + 'px';
        }

        public calculateTableHeight() {
            const topHeaderHeight = (document.getElementsByClassName("app-header")[0] as HTMLElement)?.offsetHeight || 0;
            const myTeamHeader =  document.getElementById("myTeamHeader")?.offsetHeight || 0;
            const footerHeight = document.getElementById("footer")?.offsetHeight || 0;
            this.gageHeight = (document.getElementsByClassName("fixed-bottom")[0] as HTMLElement)?.offsetHeight || 0;
            this.bottomHeight = this.displayFooter ? footerHeight : this.gageHeight;
            // console.log('My Team - Window: ' + this.windowHeight)
            // console.log('My Team - Top: ' + topHeaderHeight)
            // console.log('My Team - TeamHeader: ' + myTeamHeader)
            // console.log('My Team - BottomHeight: ' + this.bottomHeight)
            // console.log('My Team - New height: ' + (this.windowHeight - topHeaderHeight - myTeamHeader - this.bottomHeight))
            this.tableHeight = (topHeaderHeight + myTeamHeader + this.bottomHeight+8)
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

        public updateDutyRosterPage() {
            if (!this.weekView) {
                this.dutyRosterDayViewMethods.$emit('getData');
            } else {
                this.dutyRosterWeekViewMethods.$emit('getData');
            }
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