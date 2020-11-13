<template>
    <b-card bg-variant="white" class="home" no-body>

        <b-row  class="m-0 p-0" cols="2" >
            <b-col class="m-0 p-0" cols="11" >
                <duty-roster-header v-on:change="getShifts()"  />
                <loading-spinner v-if="!isDutyRosterDataMounted" />
                <b-table 
                    v-else              
                    :items="dutyRosters" 
                    :fields="fields"
                    small
                    head-row-variant="primary"   
                    bordered                    
                    fixed>
                        <template v-slot:table-colgroup>
                            <col style="width:9rem">                            
                        </template>
                       
                        <template v-slot:cell(assignments)  >                            
                            <!-- <b-row class="ml-3">{{data.value}} </b-row> -->
                            <!-- <b-row>
                                <b-button style="margin-left:auto; margin-right:1rem; height:1rem; width:1rem;" size="sm"  @click="data.toggleDetails();"><b-icon-arrow90deg-down font-scale=".75"  style="transform:translate(-5px,-11px);"/></b-button>
                            </b-row> -->
                        </template>

                        <template v-slot:head(h0) >
                            <div class="grid24">
                                <div v-for="i in 24" :key="i" :style="{gridColumnStart: i,gridColumnEnd:(i+1), gridRow:'1/1'}">{{getBeautifyTime(i-1)}}</div>
                            </div>
                        </template>

                        <template v-slot:cell(h0) >
                            <duty-card/>
                        </template>
                </b-table>                
                <b-card><br></b-card>  
            </b-col>
            <b-col class="p-0 " cols="1"  style="overflow: auto;">
                <b-card 
                    v-if="isDutyRosterDataMounted" 
                    body-class="mx-2 p-0"
                    class="bg-dark m-0 p-0">
                    <b-card-header header-class="m-0 text-white py-2 px-0"> 
                        My Team
                        <b-button
                            @click="toggleDisplayMyteam()"
                            v-b-tooltip.hover                            
                            title="Display Graphical Availability of MyTeam "                            
                            style="font-size:10px; width:1.1rem; margin:0 0 0 .2rem; padding:0; background-color:white; color:#189fd4;" 
                            size="sm">
                                <b-icon-bar-chart-steps /> 
                        </b-button>
                    </b-card-header> 
                        <duty-roster-team-member-card v-for="member in shiftAvailabilityInfo" :key="member.sheriffId" :sheriffInfo="member"/>
                </b-card>
            </b-col>
        </b-row>

        <sheriff-fuel-gauge v-if="isDutyRosterDataMounted && !displayFooter" class="fixed-bottom bg-white"/>
    </b-card>
</template>

<script lang="ts">
    import { Component, Vue } from 'vue-property-decorator';
    import DutyRosterHeader from './components/DutyRosterHeader.vue'
    import DutyRosterTeamMemberCard from './components/DutyRosterTeamMemberCard.vue'
    import DutyCard from './components/DutyCard.vue'

    import SheriffFuelGauge from './components/SheriffFuelGauge.vue'

    import { namespace } from "vuex-class";   
    import "@store/modules/CommonInformation";
    const commonState = namespace("CommonInformation");

    import "@store/modules/DutyRosterInformation";   
    const dutyState = namespace("DutyRosterInformation");

    import { locationInfoType } from '../../types/common';
    import { dutyRangeInfoType, myTeamShiftInfoType} from '../../types/DutyRoster';
    import { shiftInfoType } from '../../types/ShiftSchedule';

    @Component({
        components: {
            DutyRosterHeader,
            DutyRosterTeamMemberCard,
            DutyCard,
            SheriffFuelGauge
        }
    })
    export default class DutyRoster extends Vue {

        @commonState.State
        public location!: locationInfoType;

        @commonState.State
        public displayFooter!: boolean;

        @commonState.Action
        public UpdateDisplayFooter!: (newDisplayFooter: boolean) => void

        @dutyState.State
        public dutyRangeInfo!: dutyRangeInfoType;

        @dutyState.State
        public shiftAvailabilityInfo!: myTeamShiftInfoType[];

        @dutyState.Action
        public UpdateShiftAvailabilityInfo!: (newShiftAvailabilityInfo: myTeamShiftInfoType[]) => void

        isDutyRosterDataMounted = false;

        // myTeamShifts: myTeamShiftInfoType[] =[]; 

        myteamAvailability=[{sheriff:'jill'},{sheriff:'jack'},{sheriff:'jill'},{sheriff:'jack'}]
        gaugeFields: any[] = []

        dutyRosters =[
            {assignments:'101'},
            {assignments:'102'},
            {assignments:'103'},
            {assignments:'Test'},
            {assignments:'CEW'},
            {assignments:'Gates'},
            {assignments:'101'},
            {assignments:'102'},
            {assignments:'103'},
            {assignments:'Test'},
            {assignments:'CEW'}
        ]

        fields =[
            {key:'assignments', label:'Assignments', thClass:'m-0 p-2', tdClass:' p-0 m-0', thStyle:''},
            {key:'h0', label:'', thClass:'', tdClass:'bg-danger p-0 m-0', thStyle:'margin:0; padding:0;'}
        ]

        mounted()
        {
            this.isDutyRosterDataMounted = false;
        }

       

        public getBeautifyTime(hour: number){
            return( hour>9? hour+':00': '0'+hour+':00')
        }

        public blockSize(){
            //console.log('schCard'+block.key)
            const el = document.getElementById('assignmentsheader')
            console.log(el)
            if(el){
                return el.clientWidth
            }else{
                return 0
            }            
        }

        public getShifts(){

            this.isDutyRosterDataMounted = false;
            const url = 'api/shift?locationId='+this.location.id+'&start='+this.dutyRangeInfo.startDate+'&end='+this.dutyRangeInfo.endDate;
            this.$http.get(url)
                .then(response => {
                    if(response.data){
                        console.log(response.data)                        
                       this.extractTeamShiftInfo(response.data);                                               
                    }                                   
                })      
        }

        public extractTeamShiftInfo(shiftsJson){
            this.UpdateShiftAvailabilityInfo([]);
            for(const shiftJson of shiftsJson)
            {
                console.log(shiftJson)
                const availabilityInfo = {} as myTeamShiftInfoType;
                const shiftInfo = {} as shiftInfoType;
                shiftInfo.id = shiftJson.id;
                shiftInfo.startDate = shiftJson.startDate;
                shiftInfo.endDate = shiftJson.endDate;
                shiftInfo.timezone = shiftJson.timezone;
                shiftInfo.sheriffId = shiftJson.sheriffId;
                shiftInfo.locationId = shiftJson.locationId;
                const index = this.shiftAvailabilityInfo.findIndex(shift => shift.sheriffId == shiftInfo.sheriffId)
                
                if (index != -1) {
                    this.shiftAvailabilityInfo[index].shifts.push(shiftInfo);
                } else {
                    availabilityInfo.shifts = [shiftInfo];
                    availabilityInfo.sheriffId = shiftJson.sheriff.id;
                    availabilityInfo.badgeNumber = shiftJson.sheriff.badgeNumber;
                    availabilityInfo.firstName = shiftJson.sheriff.firstName;
                    availabilityInfo.lastName = shiftJson.sheriff.lastName;
                    availabilityInfo.rank = shiftJson.sheriff.rank;
                    this.shiftAvailabilityInfo.push(availabilityInfo);
                }
            }
            this.UpdateShiftAvailabilityInfo(this.shiftAvailabilityInfo);
            this.isDutyRosterDataMounted = true;
        }

        public toggleDisplayMyteam(){
            console.log('display')
            if(this.displayFooter) this.UpdateDisplayFooter(false)
            else this.UpdateDisplayFooter(true)
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
        grid-template-rows: 2.56rem;
        inline-size: 100%;
        font-size: 10px;         
    }

    .grid24 > * {      
        padding: .75rem 0;
        border: 1px dotted rgb(185, 143, 143);
    }

</style>