<template>
    <div> 
        
        <b-card 
            :id="'schCard'+shiftInfo.date"
            :style=" 'position: relative; left:'+ shiftInfo.startTime +'%; width:' + shiftInfo.timeDuration+'%; height:6rem;'" 
            :bg-variant="!blockDrop?shiftInfo.color:'danger'" 
            class="m-0 p-0" 
            body-class="m-0 p-0" 
            header-class=" h6 m-0 p-0"
            @dragenter="dragEnter"
            @dragleave="dragLeave"  
            @dragover="dragEnter"          
            @dragover.prevent
            @drop.prevent="drop" 
            :header="shiftInfo.type">
                <span style="margin-left:2px;font-size:10px; line-height: 12px; display: block;">{{shiftInfo.timeStamp}}</span>                              
                <!-- <b class="h7 m-0 p-0"> {{shiftInfo.assignee}} </b> --> 
        
                   
       </b-card>
    </div>
</template>

<script lang="ts">
    import { Component, Vue, Prop } from 'vue-property-decorator';

    import {shiftInfoType} from '../../../types/ShiftSchedule/index'

    @Component
    export default class ScheduleCard extends Vue {

        @Prop({required: true})
        shiftInfo!: shiftInfoType;

        blockDrop = false;

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
    }
</script>

<style scoped>   

    .card {
        border: white;
    }

</style>