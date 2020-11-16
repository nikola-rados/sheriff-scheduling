<template>
    <div v-if="isSheriffFuelGauge">
        <b-row  class="m-0 p-0" cols="2" >
            <b-col class="m-0 p-0" cols="11" >
                <b-table
                    :items="myTeamMembers" 
                    :fields="gaugeFields"
                    small
                    sort-by="availability"
                    :sort-desc="true"
                    class="gauge"                   
                    sticky-header="200px"                        
                    bordered                    
                    fixed>
                        <template v-slot:table-colgroup> 
                            <col>                                      
                            <col style="width:7.7rem">
                        </template>

                        <template v-slot:head(availability) >
                            <div class="gridfuel24">
                                <div v-for="i in 24" :key="i" :style="{gridColumnStart: i,gridColumnEnd:(i+1), gridRow:'1/1'}">{{getBeautifyTime(24-i)}}</div>
                            </div>
                        </template>

                        <template v-slot:cell(availability)="data" >
                            <sheriff-availability-card class="m-0 p-0" :sheriffInfo="data.item.sheriff" />
                        </template>

                        <template v-slot:head(name) >                           
                            <b-button
                                @click="closeDisplayMyteam()"
                                v-b-tooltip.hover.right                            
                                title="Close Footer Display"
                                style="font-size:10px; width:1.15rem; margin:0 0 0 .75rem; padding:0; background-color:white;color:#189fd4;" 
                                size="sm">
                                    <b-icon-bar-chart-steps /> 
                            </b-button>
                             My Team
                        </template>

                         <template v-slot:cell(name)="data" >
                            <div
                                style="height:1rem; font-size:9px; line-height: 16px; text-transform: capitalize; margin:0; padding:0"
                                v-b-tooltip.hover.right                             
                                :title="data.item.fullName">
                                    {{data.value}}
                            </div>
                        </template>
                </b-table> 
            </b-col>
            <b-col class="m-0 p-0" cols="1" >
                <b-card 
                class="bg-light"
                    header="Colors" 
                    header-class=" m-0 p-0 bg-primary text-white text-center" 
                    no-body>
                    <b-row style="margin:0 0 .25rem .25rem; width:7.6rem;">
                        <div
                            style="width:3.8rem;"
                            class="m-0 p-0"
                            v-for="color in dutyColors" 
                            :key="color.colorCode"> 
                            <div :style="{backgroundColor:color.colorCode, width:'.65rem', height:'.65rem', borderRadius:'15px', margin:'.2rem .2rem 0 0', float:'left'}"/>
                            <div style="font-size:9px; text-transform: capitalize; margin:0 0 0 0; padding:0">{{color.name}}</div>
                        </div>
                    </b-row>
                </b-card>
            </b-col>
        </b-row>
    </div>
</template>

<script lang="ts">
    import { Component, Vue } from 'vue-property-decorator';
    import SheriffAvailabilityCard from './SheriffAvailabilityCard.vue'
    import { myTeamShiftInfoType} from '../../../types/DutyRoster';

    import moment from 'moment-timezone';

    import { namespace } from "vuex-class";   
    import "@store/modules/CommonInformation";
    const commonState = namespace("CommonInformation");

    import "@store/modules/DutyRosterInformation";   
    const dutyState = namespace("DutyRosterInformation");

    @Component({
        components: {
            SheriffAvailabilityCard
        }
    })
    export default class SheriffFuelGauge extends Vue {
       
        @dutyState.State
        public shiftAvailabilityInfo!: myTeamShiftInfoType[];

        @commonState.Action
        public UpdateDisplayFooter!: (newDisplayFooter: boolean) => void
       
        isSheriffFuelGauge = false;

        myTeamMembers: any[] = []

        gaugeFields = [
            {key:'availability', label:'', thClass:'text-white bg-primary', tdClass:'p-0 m-0', thStyle:'margin:0; padding:0;'},
            {key:'name', label:'My Team', thClass:'text-center text-white bg-primary', tdClass:' py-0 my-0', thStyle:'margin:0; padding:0;'}            
        ]

        mounted()
        {
            this.isSheriffFuelGauge = false;
            //console.log(this.shiftAvailabilityInfo)
            this.extractSheriffAvailability()                                    
        }

        dutyColors = [
            {name:'court' , colorCode:'#189fd4'},
            {name:'jail' ,  colorCode:'#A22BB9'},
            {name:'escort', colorCode:'#ffb007'},
            {name:'other',  colorCode:'#c91a5d'},
            {name:'overtime',colorCode:'#0cc97e'},
            {name:'free',   colorCode:'#e6d9e2'}            
        ]
            
        public extractSheriffAvailability(){
            this.myTeamMembers = [];
            for(const sheriff of this.shiftAvailabilityInfo){
                console.log(sheriff)
                this.myTeamMembers.push({                     
                    name: Vue.filter('truncate')(sheriff.lastName,10) + ', '+ sheriff.firstName.charAt(0).toUpperCase(),
                    fullName: sheriff.firstName + ' ' + sheriff.lastName,
                    availability: this.getSheriffFreeTime(sheriff),
                    sheriff: sheriff
                })
            }
            this.isSheriffFuelGauge = true;
        }

        public getBeautifyTime(hour: number){
            return( hour>9? hour+':00': '0'+hour+':00')
        }

        public closeDisplayMyteam(){
            this.UpdateDisplayFooter(true)
        }

        public getSheriffFreeTime(sheriff){
            let freeTime = 0;
            for(const shift of sheriff.shifts){ 
                const start = moment(shift.startDate);
                const end = moment(shift.endDate);
                freeTime += moment.duration(end.diff(start)).asMinutes()
            }
            console.log(freeTime)
            return freeTime;
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
        overflow-y: scroll;
    }

     .gridfuel24 {        
        display:grid;
        grid-template-columns: repeat(24, 4.1666%);
        grid-template-rows: 1.57rem;
        inline-size: 100%;
        font-size: 9px;         
    }

    .gridfuel24 > * {      
        padding: .25rem 0;
        border: 1px dotted rgb(185, 143, 143);
    }
</style>