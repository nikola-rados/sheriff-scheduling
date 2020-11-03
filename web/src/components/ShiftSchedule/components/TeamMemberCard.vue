<template>
    <div >    
        <b-card 
            v-if="isDataMounted"
            :id="'member'+sheriffId"
            style=" width:100%; height:4.5rem;" 
            bg-variant="white"            
            class="mx-0 my-1 p-0" 
            body-class="m-0 p-0"
            :draggable="true" 
            v-on:dragstart="DragStart"            
            header-class=" h6 m-0 p-0">
                <b-row style="margin-left:2px;font-size:11px; line-height: 16px;"># {{sheriffInfo.badgeNumber}}</b-row>
                <b-row style="margin-left:2px;font-size:9px; line-height: 14px;">{{sheriffInfo.rank}}</b-row>
                <b-row 
                    style="margin-left:2px;font-size:12px; line-height: 16px; font-weight: bold; text-transform: Capitalize;" 
                    v-b-tooltip.hover.topleft                                
                    :title="sheriffInfo.fullName.length>14?sheriffInfo.fullName:''">
                        {{sheriffInfo.fullName|truncate(12)}}
                </b-row>
                <b-row style="margin-left:0px;font-size:10px;">
                    <b-badge 
                        v-for="sch in sheriffSchedules" 
                        :key="sch.weekday"
                        :id="'sch'+sheriffId+'-'+sch.weekday"
                        :variant="sch.variant" 
                        :style="sch.style" >
                            {{sch.text}}
                            <b-tooltip v-if="sch.tooltip.header"  :target="'sch'+sheriffId+'-'+sch.weekday" variant="warning" show.sync ="true" triggers="hover" placement="topleft">
                                <b-card 
                                    class="m-0 p-0" 
                                    body-class="m-0 p-0" 
                                    header-class="h6 my-0 py-0 bg-dark text-white" 
                                    :header="sch.tooltip.header" >
                                        <div>{{sch.tooltip.desc}}</div>
                                        <div>{{sch.tooltip.time}}</div>
                                </b-card>
                            </b-tooltip>  
                    </b-badge>                                 
                </b-row>
        </b-card>
       
    </div>
</template>

<script lang="ts">
    import { Component, Vue, Prop } from 'vue-property-decorator';
    import { namespace } from "vuex-class";
    import "@store/modules/ShiftScheduleInformation";
    const shiftState = namespace("ShiftScheduleInformation");
    import { sheriffAvailabilityInfoType } from '../../../types/ShiftSchedule';

    @Component
    export default class TeamMemberCard extends Vue {

        @Prop({required: true})
        sheriffId!: string;

        @shiftState.State
        public sheriffsAvailabilityInfo!: sheriffAvailabilityInfoType[];

        isDataMounted = false;

        sheriffInfo = {} as sheriffAvailabilityInfoType;

        halfUnavailStyle="background-image: linear-gradient(to bottom right, rgb(194, 39, 28),rgb(243, 232, 232), white);"
        halfUnavailHalfSchStyle="background-image: linear-gradient(to bottom right, rgb(194, 39, 28),rgb(243, 232, 232), rgb(12, 120, 170));"
        halfSchStyle="background-image: linear-gradient(to bottom right, rgb(12, 120, 170),rgb(243, 232, 232), white);"
        WeekDay = ['S', 'M', 'T', 'W', 'T', 'F', 'S'];
        sheriffSchedules=[
            {weekday: 0, text:this.WeekDay[0], variant:'white',  style:'', tooltip:{}},
            {weekday: 1, text:this.WeekDay[1], variant:'white',  style:'', tooltip:{}},
            {weekday: 2, text:this.WeekDay[2], variant:'white',  style:'', tooltip:{}},
            {weekday: 3, text:this.WeekDay[3], variant:'white',  style:'', tooltip:{}},
            {weekday: 4, text:this.WeekDay[4], variant:'white',  style:'', tooltip:{}},
            {weekday: 5, text:this.WeekDay[5], variant:'white',  style:'', tooltip:{}},
            {weekday: 6, text:this.WeekDay[6], variant:'white',  style:'', tooltip:{}}           
        ]
        
        mounted()
        {
            this.sheriffInfo = this.sheriffsAvailabilityInfo.filter(sheriff =>{if(sheriff.sheriffId == this.sheriffId) return true})[0]
            this.sheriffInfo['fullName'] = this.sheriffInfo.lastName +', '+this.sheriffInfo.firstName;
            this.extractConflicts();
            this.isDataMounted = true;
            console.log(this.sheriffInfo)
        }

        public extractConflicts(){
            for(const conflict of this.sheriffInfo.conflicts){
                console.log(conflict)
            }
        }         

        public DragStart(event: any) 
        { 
            event.dataTransfer.setData('text', event.target.id);
        }

        public drag(event: any)
        {
            console.log(event)
        }

        // style:this.halfUnavailStyle, tooltip:{header:'Partial Leave', desc:'illness', time:'08:00-09:00'}
        //   {weekday: 0, text:this.WeekDay[0], variant:'white',  style:'', tooltip:{}},
        //     {weekday: 1, text:this.WeekDay[1], variant:'info',   style:'', tooltip:{}},
        //     {weekday: 2, text:this.WeekDay[2], variant:'white', },
        //     {weekday: 3, text:this.WeekDay[3], variant:'danger', style:'', tooltip:{header:'FullDay Training', desc:'computer', time:'08:00-09:00'}},
        //     {weekday: 4, text:this.WeekDay[4], variant:'white',  style:'', tooltip:{}},
        //     {weekday: 5, text:this.WeekDay[5], variant:'white',  style:this.halfUnavailHalfSchStyle, tooltip:{header:'Partial Loaned To', desc:'Abbostford Court', time:'09:00-11:00'}},
        //     {weekday: 6, text:this.WeekDay[6], variant:'white',  style:this.halfSchStyle, tooltip:{}}           

    }
</script>

<style scoped>   

    .card {
        border:2px solid rgb(124, 136, 136);
    }

    .badge {
        width: 0.75rem;
        height: 0.9rem;
        margin:0.5rem .04rem 0 .04rem;
        padding: 0.15rem 0 0 0;
        border: solid 1px rgb(53, 56, 53); 
        border-radius: 4px;
        /* background-image: linear-gradient(to bottom right, rgb(12, 120, 170),rgb(243, 232, 232), white);*/        
    }

</style>