<template>
    <div> 
        
        <b-card 
            :id="'schCard'+shiftInfo.date"
            :style=" 'border:1px solid; position: relative; left:'+ shiftInfo.startTime +'%; width:' + shiftInfo.timeDuration+'%; height:6rem;'" 
            :bg-variant="blockDrop?'danger':selected?'select':''"
            :border-variant="shiftInfo.type"
            class="m-0 p-0" 
            body-class="m-0 p-0" 
            :header-class=" 'h7 text-center text-capitalize m-0 p-0 text-white bg-'+shiftInfo.type"            
            @mousedown="cardSelected"
            @dragenter="dragEnter"
            @dragleave="dragLeave"  
            @dragover="dragEnter"          
            @dragover.prevent
            @drop.prevent="drop" 
            :header="shiftInfo.type">
                <span style="margin-left:2px;font-size:10px; line-height: 12px; display: block;">{{shiftInfo.timeStamp}}</span>                              
                <!-- <b class="h7 m-0 p-0"> {{shiftInfo.assignee}} </b> --> 
        <!-- <b-card-img src="https://placekitten.com/480/210" alt="Image" bottom></b-card-img> -->
                   
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
        selected = false;
       // cardColor =  

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