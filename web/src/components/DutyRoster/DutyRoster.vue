<template>
    <b-card bg-variant="white" class="home" no-body>
        
        <!-- <div v-else> -->
        <b-row  class="m-0 p-0" cols="2" >
            <b-col class="m-0 p-0" cols="11" >
                <duty-roster-header v-on:change="getShifts()"  />
                <loading-spinner v-if="!isDutyRosterDataMounted" />
                <b-table 
                    v-else               
                    :items="dutyRosters" 
                    :fields="fields"
                    details-td-class="m-0 p-0"
                    small
                    head-row-variant="primary"   
                    bordered                    
                    fixed>
                    <template v-slot:table-colgroup>
                        <col style="width:7.35rem"> 
                    </template>
                        <!-- <template v-slot:head() = "data" >
                            <span class="text-success">{{data.column}}</span> <span> {{data.label}}</span>
                        </template> -->

                        <template v-slot:head(assignments) ="data" >                            
                            <div id="assignmentsheader">{{data.label}} </div>                            
                        </template>
                        <template v-slot:cell(assignments) ="data" >                            
                            <b-row class="ml-3">{{data.value}} </b-row>
                            <b-row>
                                <b-button style="margin-left:auto; margin-right:1rem; height:1rem; width:1rem;" size="sm"  @click="data.toggleDetails();"><b-icon-arrow90deg-down font-scale=".75"  style="transform:translate(-5px,-11px);"/></b-button>
                            </b-row>
                        </template>
                        <template v-slot:cell(h0) >  
                            <b-card style="background-color:#B03456; float:left; position: relative; left:1000%; width:1050%; height:3rem; margin-bottom:.25rem;"/>
                            <!-- <b-card style="background-color:#A034A6; float:left; position: relative; left:100%; width:550%; height:1rem;  margin-bottom:.25rem;"/>
                            <b-card style="background-color:#B00456; float:left; position: relative; left:300%; width:550%; height:1rem;  margin-bottom:.25rem;"/>
                             -->
                            <!-- <schedule-card :shiftInfo="data.item.Sun"/> -->
                            
                        </template> 

                        <!-- <template v-slot:row-details>
                             <b-table
                                :items="[{sheriff:'john'},{sheriff:'joe'},{sheriff:'jill'},{sheriff:'jack'}]" 
                                :fields="[{key:'sheriff', label:'sheriff', thClass:'', tdClass:'h7 px-1 mx-1', thStyle:''},{key:'time', label:'time', thClass:'', tdClass:'px-0 mx-0', thStyle:''}]"
                                small
                                borderless
                                thead-class="d-none"
                                  
                                bordered                    
                                fixed>
                                <template v-slot:table-colgroup>
                                    <col style="width:8.47rem"> 
                                </template>
                                <template v-slot:footer(time) >  
                                    <b-card style="background-color:#BBBBBB; float:left; position: relative; left:22.6%; width:61%; height:.5rem;"/>
                                </template>
                             </b-table>
                        </template>                       -->
                </b-table>
                <div class="bg-light fixed-bottom mb-5" > 
                    <b-row  class="m-0 p-0" cols="2" >
                        <b-col class="m-0 p-0" cols="11" border-vaiant="danger">
                            <b-table
                                :items="myteamAvailability" 
                                :fields="gaugeFields"
                                small
                                class="gauge"
                                thead-class="d-none"
                                sticky-header="600px"                        
                                bordered                    
                                fixed>
                                    <template v-slot:table-colgroup> 
                                        <col>                                      
                                        <col style="width:6.2rem">
                                    </template>                                    
                                    <template v-slot:cell(h0) >  
                                        <b-card style="background-color:#BBBBBB; float:left; position: relative; left:0%; width:50%; height:.5rem;"/>
                                    </template>
                            </b-table> 
                        </b-col>
                    </b-row>
                </div>
                <b-card><br></b-card>  
            </b-col>
            <b-col class="p-0 " cols="1"  style="overflow: auto;">
                <b-card v-if="isDutyRosterDataMounted" style="width:98%;" class="bg-dark mb-5"  header-class="text-white text-center" header="My Team"> 
                    <duty-roster-team-member-card v-for="member in shiftAvailabilityInfo" :key="member.sheriffId" :sheriffInfo="member"/>
                </b-card>
            </b-col>
        </b-row>
    </b-card>
</template>

<script lang="ts">
    import { Component, Vue } from 'vue-property-decorator';
    import DutyRosterHeader from './components/DutyRosterHeader.vue'
    import SheriffAvailability from './components/SheriffAvailability.vue'
    import DutyRosterTeamMemberCard from './components/DutyRosterTeamMemberCard.vue'

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
            SheriffAvailability,
            DutyRosterTeamMemberCard
        }
    })
    export default class DutyRoster extends Vue {

        @commonState.State
        public location!: locationInfoType;

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
            {assignments:'Gates'}
        ]

        fields: any[] = []

        mounted()
        {
            this.isDutyRosterDataMounted = false;
            this.writeFields();
            this.writeGaugeFields(); 
                        
        }

        public writeFields(){
            this.fields = [{key:'assignments', label:'Assignments', thClass:'', tdClass:'px-0 mx-0', thStyle:''}];
            for(let hour=0; hour<24; hour++){
                const time = hour>9? hour+':00': '0'+hour+':00'
                this.fields.push( {key:('h'+hour), label:time, thClass:'align-middle', tdClass:'px-0 mx-0', thStyle:'margin:0; padding:0; font-size:10px'})
            }
        }

        public writeGaugeFields(){

            // thead-class="d-none"
            this.gaugeFields = []
            // for(let hour=0; hour<24; hour++){
            //     const time = hour>9? hour+':00': '0'+hour+':00'
            this.gaugeFields.push( {key:'h0', label:'time', thClass:'align-middle', tdClass:'px-0 mx-0', thStyle:'margin:0; padding:0; font-size:10px'})
            // }
            this.gaugeFields.push({key:'sheriff', label:'sheriff', thClass:'', tdClass:'h7 p-0 m-0', thStyle:'margin:0; padding:0; font-size:10px'})
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

            const url = 'api/shift?locationId='+this.location.id+'&start='+this.dutyRangeInfo.startDate+'&end='+this.dutyRangeInfo.endDate;
            this.$http.get(url)
                .then(response => {
                    if(response.data){
                        console.log(response.data)
                        this.isDutyRosterDataMounted = true;
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

</style>