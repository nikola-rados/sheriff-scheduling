<template>
    <div     
        v-if="shiftInfo.length>0" 
        >             
            <b-card 
                v-for="block in shiftBlocks"
                :key="block.key"
                :id="'schCard'+block.key"
                :style=" 'background-color:grey; display:inline-block; position: relative; left:'+ block.startTime +'%; width:' + block.timeDuration+'%; height:6rem;'" 
                
                >
                    <span v-if="block.timeDuration>20" style="margin-left:2px;font-size:10px; line-height: 12px; display: block;">{{block.timeStamp}}</span>
                    
            </b-card>
    </div>
</template>

<script lang="ts">
    import { Component, Vue, Prop } from 'vue-property-decorator';    
    import {conflictsInfoType} from '../../../types/ShiftSchedule/index'
    import * as _ from 'underscore';

    //  v-b-tooltip.hover                                
    //     :title="timeDuration<=20? shiftInfo.type + ' ' +timeStamp:''"

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
        selected = false;
       // cardColor = 
       
        mounted()
        {
            console.log(this.shiftInfo)

            const sortedShiftInfo: conflictsInfoType[] = _.sortBy(this.shiftInfo,'startInMinutes')
            console.log(sortedShiftInfo)
            this.shiftBlocks = []
            //let width =0;
            for(const shift of sortedShiftInfo){
                console.log(shift)
                if(shift.startTime){

                    this.shiftBlocks.push({
                        key: shift.date +'Z' +shift.startTime,
                        startTime: shift.startInMinutes *5 /72,
                        timeDuration: shift.timeDuration * 5 /72,
                        timeStamp: shift.startTime +'-'+this.shiftInfo.endTime
                    })
                    console.log(this.shiftBlocks)
                }else{

                    this.shiftBlocks.push({
                        key: shift.date +'Z' +shift.startTime,
                        startTime: 20,
                        timeDuration: 60,
                        timeStamp: 'Full Day'
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

        public cardSelected()
        {
            console.log("this.selected")
            this.selected = !this.selected;
        }
    }
</script>

<style scoped>   

    .card {
        border: white;
    }

</style>