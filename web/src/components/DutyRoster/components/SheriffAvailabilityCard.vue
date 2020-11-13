<template>
    <div v-if="isMounted" class="gridsheriff">
        <div v-for="i in 96" :key="i" :style="{backgroundColor: '#F1FEF1', gridColumnStart: i,gridColumnEnd:(i+1), gridRow:'1/2'}"></div>
        <div 
            v-for="block in dutyShiftBlocks"
            :key="block.id"
            :style="{gridColumnStart: (97-block.startTime),gridColumnEnd:(97-block.endTime), gridRow:'1/1',  backgroundColor: block.color, fontSize:'9px', textAlign: 'center', margin:0, padding:0 }"
            v-b-tooltip.hover.right                             
            :title="block.assignment">
                <div style="text-transform: capitalize; margin:0 padding: 0;">
                    {{block.assignment|truncateleft(block.endTime - block.startTime-1)}}
                </div>                
        </div>
    </div>
</template>

<script lang="ts">
    import { Component, Vue, Prop } from 'vue-property-decorator';
    import { myTeamShiftInfoType} from '../../../types/DutyRoster';
   
    import * as _ from 'underscore';
    import moment from 'moment-timezone';

    @Component
    export default class SheriffAvailabilityCard extends Vue {

        @Prop({required: true})
        sheriffInfo!: myTeamShiftInfoType;

        dutyShiftBlocks: any[]=[] 

        blockDrop = false;
        updateBoxes =0;

        isMounted = false;

        mounted()
        {
            // console.log(this.scheduleInfo)
            this.isMounted = false;

            const sortedShifts = _.sortBy(this.sheriffInfo.shifts,'startDate');           
            this.dutyShiftBlocks = []

            for(const shift of sortedShifts){ 
                const start = moment(shift.startDate);
                const end = moment(shift.endDate);
                const startOfDay = moment(shift.startDate).startOf('day');
                const startTime = moment.duration(start.diff(startOfDay)).asMinutes()
                const endTime = moment.duration(end.diff(startOfDay)).asMinutes()

                this.dutyShiftBlocks.push({
                    id:shift.id,                    
                    startTime: startTime/15,
                    endTime: endTime/15,                    
                    color: this.getDutyColor('free'),
                    type: 'court',
                    assignment: 'free'
                })
            }
            this.isMounted = true; 
        }

        

        public getDutyColor(type) {
            const color = {court:'#189fd4',jail:'#A22BB9',escort:'#ffb007',other:'#c91a5d',free:'#e6e9e2',overtime:'#0cc97e'};
            return color[type];
        }

        //Wrapper function, the markup wouldn't bind to getScheduleColor. 
        public dutyColor(block){
            return this.getDutyColor(block);
        }

    }
</script>

<style scoped>

    .gridsheriff {        
        display:grid;
        grid-template-columns: repeat(96, 1.041666%);
        grid-template-rows: repeat(1,.9rem);
        inline-size: 100%; 
        background-color: #fcf5f5; 
        height: 1rem; 
        margin: 0; 
        padding: 0;
        column-gap: 0;
        row-gap: 0;
           
    }

    .gridsheriff > * { 
        padding: 0px 0;
        border: 1px dotted rgb(202, 202, 202);
    }

</style>