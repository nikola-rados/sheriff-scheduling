<template>
    <div v-if="shiftInfo.length>0">             
        <b-card
            v-for="block in scheduleBlocks"
            :key="block.key"
            :id="'schCard'+block.key"
            :style=" 'background-color:'+block.color+'; float:left; position: relative; left:'+ block.startTime +'%; width:' + block.timeDuration+'%; height:4rem;'" 
            no-body>
                <span v-if="block.timeDuration>20" @mousedown="cardSelected(block)">
                    <h6 :style="'background-color:'+block.headerColor+'; color:white; text-align: center; font-size:10px; line-height: 16px;'">{{block.title}}</h6>
                    <span style="text-align: center;font-size:10px; line-height: 12px; display: block;">{{block.timeStamp}}</span>
                </span>
                <span v-else>
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

    

    //  :bg-variant="blockDrop?'danger':selected?'select':''"
    //             border-variant="white"
    //             class="m-0 p-0" 
    //             body-class="m-0 p-0 bg-warning" 
    //             header-class="h7 text-center text-capitalize m-0 p-0 bg-warning"            
    //             @mousedown="cardSelected"
    //             @dragenter="dragEnter"
    //             @dragleave="dragLeave"  
    //             @dragover="dragEnter"          
    //             @dragover.prevent
    //             @drop.prevent="drop" 
    //             :header="timeDuration>20? shiftInfo.type:''" 
    //             >

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
       
        mounted()
        {
            //console.log(this.shiftInfo)

            const sortedScheduleInfo: conflictsInfoType[] = _.sortBy(this.scheduleInfo,'startInMinutes')
            //console.log(sortedScheduleInfo)
            this.scheduleBlocks = []
            let widthOtherElements =0;
            for(const schedule of sortedScheduleInfo){
                //console.log(schedule)
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

        public drop(event: any) 
        {
            this.blockDrop = false;
            const cardid = event.dataTransfer.getData('text');
            console.log(cardid)
        }

        public dragEnter(){
            this.blockDrop = true;
        }
        public dragLeave(){
            this.blockDrop = false;
        }

        public cardSelected(block)
        {
            // console.log("this.selected")
            // console.log(block)
            const selectedCards = this.selectedShifts;
            if(block.title=='Shift'){
                block.selected = !block.selected;
                block.color=block.selected?'#F7F54F':block.originalColor;
                selectedCards.push(block.id);
                this.UpdateSelectedShifts(selectedCards);
            }
        }
    }
</script>

<style scoped>   

    .card {
        border: white;
    }

</style>