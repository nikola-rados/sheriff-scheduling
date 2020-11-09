<template>
    <div v-if="scheduleInfo.length>0" id="shiftBox" ref="shiftBox" :key="updateBoxes">             
        <b-card
            v-for="block in scheduleBlocks"
            :key="block.key"
            :id="'schCard'+block.key"
            :style=" 'background-color:'+block.color+'; float:left; position: relative; left:'+ block.startTime +'%; width:' + block.timeDuration+'%; height:4rem;'"             
            no-body>
                <span v-if="blockSize(block)>30" @mousedown="cardSelected(block)"> 
                    <h6 :style="'background-color:'+block.headerColor+'; color:white; text-align: center; font-size:10px; line-height: 16px;'">
                        <font-awesome-icon v-if="block.title=='Loaned'" style="transform: rotate(180deg); font-size: .55rem;"  icon="sign-out-alt" /> 
                        <font-awesome-icon v-if="block.title=='Leave'" style="font-size: .55rem;"  icon="suitcase"/> 
                        <font-awesome-icon v-if="block.title=='Training'" style="font-size: .5rem;" icon="graduation-cap"/> 
                        <b-icon-person-fill v-if="block.title=='Shift'"/>
                        <b-icon-calendar2-x v-if="block.title=='Unavailable'"/>
                    </h6>
                    <span style="text-align: center;font-size:10px; line-height: 12px; display: block;">{{block.timeStamp}}</span>
                </span>
                <span v-else @mousedown="cardSelected(block)">
                    <h6 
                        v-b-tooltip.hover                                
                        :title="block.title + ' ' + block.timeStamp"
                        :style="'background-color:'+block.headerColor+'; color:white; text-align: center; font-size:6px; line-height: 16px;'">
                        ...
                    </h6>                    
                </span>
        </b-card>
    </div>
</template>

<script lang="ts">
    import { Component, Vue, Prop } from 'vue-property-decorator';
    import { namespace } from "vuex-class";    
    import * as _ from 'underscore';
    import "@store/modules/ShiftScheduleInformation";   
    const shiftState = namespace("ShiftScheduleInformation");
    import {conflictsInfoType, scheduleBlockInfoType} from '../../../types/ShiftSchedule/index'

    @Component
    export default class ScheduleCard extends Vue {

        @Prop({required: true})
        scheduleInfo!: conflictsInfoType;

        @shiftState.State
        public selectedShifts!: string[];

        @shiftState.Action
        public UpdateSelectedShifts!: (newSelectedShifts: string[]) => void

        scheduleBlocks: scheduleBlockInfoType[] = [];

        blockDrop = false;
        updateBoxes =0;

        mounted()
        {
            // console.log(this.scheduleInfo)

            const sortedScheduleInfo: conflictsInfoType[] = _.sortBy(this.scheduleInfo,'startInMinutes')
           
            this.scheduleBlocks = []
            let widthOtherElements =0;
            for(const schedule of sortedScheduleInfo){
                if(schedule.startTime){

                    this.scheduleBlocks.push({
                        id: schedule.id? schedule.id:'',
                        key: schedule.date +'Z' +schedule.startTime,
                        startTime: schedule.startInMinutes *5 /72 -widthOtherElements,
                        timeDuration: schedule.timeDuration * 5 /72,
                        timeStamp: schedule.startTime +'-'+schedule.endTime,
                        title:schedule.type,
                        color: schedule.type=='Shift'?'#BEF2F7':'#FFEEDD',
                        originalColor: schedule.type=='Shift'?'#BEF2F7':'#FFEEDD',
                        headerColor: schedule.type=='Shift'?'#004567':'#F94567',
                        selected: false
                    })
                    widthOtherElements += (schedule.timeDuration * 5 /72);
                    //console.log(this.scheduleBlocks)
                }else{

                    this.scheduleBlocks.push({
                        key: schedule.date +'Z' +schedule.startTime,
                        startTime: 0,
                        timeDuration: 100,
                        timeStamp: 'Full Day',
                        title:schedule.type,
                        color: schedule.type=='Shift'?'#BEF2F7':'#FFEEDD',
                        originalColor: schedule.type=='Shift'?'#BEF2F7':'#FFEEDD',
                        headerColor: schedule.type=='Shift'?'#004567':'#F94567',
                        selected: false
                    })
                }                    
            } 
        }

        // public getScheduleColor(type){
        //     if(type=='Unavailable')
        // }
       
        public cardSelected(block){
            const selectedCards = this.selectedShifts;
            console.log(block)
            if(block.title=='Shift'){
                block.selected = !block.selected;
                block.color=block.selected?'#F7F54F':block.originalColor;
                selectedCards.push(block.id);
                this.UpdateSelectedShifts(selectedCards);
            }
        }

        public blockSize(block){
            const el = document.getElementById('schCard'+block.key)
            if(el){
                return el.clientWidth
            }else{
                return 0
            }            
        }

        created() {
            window.addEventListener("resize", this.onResize);
        }

        destroyed() {
            window.removeEventListener("resize", this.onResize);
        }

        public onResize() {          
            this.updateBoxes++;
        }

    }
</script>

<style scoped>   

    .card {
        border: white;
    }

</style>