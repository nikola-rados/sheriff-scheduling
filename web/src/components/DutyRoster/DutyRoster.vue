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
                        <col style="width:8.5rem"> 
                    </template>
                        <!-- <template v-slot:head() = "data" >
                            <span class="text-success">{{data.column}}</span> <span> {{data.label}}</span>
                        </template> -->
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

                        <template v-slot:row-details>
                            <!-- <div style="margin:0; padding:0; background-color:red; width:100%; height:2rem;"></div> -->
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
                                <template v-slot:cell(time) >  
                                    <b-card style="background-color:#BBBBBB; float:left; position: relative; left:22.6%; width:70%; height:.5rem;"/>
                                </template>
                             </b-table>
                        </template>                      
                </b-table>
                <b-card><br></b-card>  
            </b-col>
            <b-col class="p-0 " cols="1"  style="overflow: auto;">
                <b-card v-if="isDutyRosterDataMounted" style="width:98%;" class="bg-dark mb-5" no-body header-class="text-white text-center" header="My Team"> 
                    <!-- <team-member-card v-for="member in sheriffsAvailabilityInfo" :key="member.sheriffId" :sheriffId="member.sheriffId"/> -->
                </b-card>
            </b-col>
        </b-row>
    </b-card>
</template>

<script lang="ts">
    import { Component, Vue } from 'vue-property-decorator';
    import DutyRosterHeader from './components/DutyRosterHeader.vue'
    import SheriffAvailability from './components/SheriffAvailability.vue'

    import { namespace } from "vuex-class";   
    import "@store/modules/CommonInformation";
    const commonState = namespace("CommonInformation");

    import "@store/modules/DutyRosterInformation";   
    const dutyState = namespace("DutyRosterInformation");

    import { locationInfoType } from '../../types/common';
    import { dutyRangeInfoType, myTeamShiftInfoType} from '@/types/DutyRoster';

    @Component({
        components: {
            DutyRosterHeader,
            SheriffAvailability
        }
    })
    export default class DutyRoster extends Vue {

        @commonState.State
        public location!: locationInfoType;

        @dutyState.State
        public dutyRangeInfo!: dutyRangeInfoType;

        isDutyRosterDataMounted = false;

        myTeamShifts: myTeamShiftInfoType[] =[]; 


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
        }

        public writeFields(){
            this.fields = [{key:'assignments', label:'Assignments', thClass:'', tdClass:'px-0 mx-0', thStyle:''}];
            for(let hour=0; hour<24; hour++){
                const time = hour>9? hour+':00': '0'+hour+':00'
                this.fields.push( {key:('h'+hour), label:time, thClass:'align-middle', tdClass:'px-0 mx-0', thStyle:'margin:0; padding:0; font-size:10px'})
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
            this.myTeamShifts = [];
            for(const shift of shiftsJson)
            {
                console.log(shift)
            }

        }

    }
</script>

<style scoped>   

    .card {
        border: white;
    }

</style>