<template>
    <div v-if="shiftInfo.length>0">             
        <b-card
            v-for="block in shiftBlocks"
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
    import {conflictsInfoType} from '../../../types/ShiftSchedule/index'
    import * as _ from 'underscore';

    

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
        shiftInfo!: conflictsInfoType;

        shiftBlocks: any[] = []

        blockDrop = false;
        
       // cardColor = 
       
        mounted()
        {
            //console.log(this.shiftInfo)

            const sortedShiftInfo: conflictsInfoType[] = _.sortBy(this.shiftInfo,'startInMinutes')
            //console.log(sortedShiftInfo)
            this.shiftBlocks = []
            let widthOtherElements =0;
            for(const shift of sortedShiftInfo){
                //console.log(shift)
                if(shift.startTime){

                    this.shiftBlocks.push({
                        key: shift.date +'Z' +shift.startTime,
                        startTime: shift.startInMinutes *5 /72 -widthOtherElements,
                        timeDuration: shift.timeDuration * 5 /72,
                        timeStamp: shift.startTime +'-'+shift.endTime,
                        title:shift.type,
                        color: shift.type=='Shift'?'#BEF2F7':'#FFEEDD',
                        originalColor: shift.type=='Shift'?'#BEF2F7':'#FFEEDD',
                        headerColor: shift.type=='Shift'?'#004567':'#F94567',
                        selected: false
                    })
                    widthOtherElements += (shift.timeDuration * 5 /72);
                    //console.log(this.shiftBlocks)
                }else{

                    this.shiftBlocks.push({
                        key: shift.date +'Z' +shift.startTime,
                        startTime: 0,
                        timeDuration: 100,
                        timeStamp: 'Full Day',
                        title:shift.type,
                        color: shift.type=='Shift'?'#BEF2F7':'#FFEEDD',
                        originalColor: shift.type=='Shift'?'#BEF2F7':'#FFEEDD',
                        headerColor: shift.type=='Shift'?'#004567':'#F94567',
                        selected: false
                    })
                }
                    
            }
            //#ebb734
            
            
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
            console.log("this.selected")
            console.log(block)
            if(block.title=='Shift'){
                block.selected = !block.selected;
                block.color=block.selected?'#F7F54F':block.originalColor;
            }
        }
    }
</script>

<style scoped>   

    .card {
        border: white;
    }

</style>