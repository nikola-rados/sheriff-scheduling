<template>
    <div>    
        <b-card 
            :style=" 'position: relative; left:'+ shiftInfo.startTime +'%; width:' + shiftInfo.timeDuration+'%; height:6rem;'" 
            :bg-variant="shiftInfo.color" 
            class="m-0 p-0" 
            body-class="m-0 p-0" 
            header-class=" h6 m-0 p-0" 
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

    }
</script>

<style scoped>   

    .card {
        border: white;
    }

</style>