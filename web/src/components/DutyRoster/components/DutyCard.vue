<template>
    <div class="grid">
        <div v-for="i in 96" :key="i" :style="{backgroundColor: '#F9F9F9', gridColumnStart: i,gridColumnEnd:(i+1), gridRow:'1/7'}"></div>
        <div
            v-for="block in dutyBlocks" 
            :key="block.id"
            :style="{gridColumnStart: block.startTime, gridColumnEnd:block.endTime, gridRow:block.height,  backgroundColor: block.color, fontSize:'9px', textAlign: 'center', margin:0, padding:0  }"
            v-b-tooltip.hover                            
            :title="block.title">
                <div style="text-transform: capitalize; margin:  0 padding:0; color:white;">
                    {{block.title|truncateleft(block.endTime - block.startTime-1)}}
                </div>     
        </div>    
        <!-- <div :style="{gridColumnStart: 6,gridColumnEnd:9, gridRow:'4/6', backgroundColor: 'blue' }"></div>     -->
    </div>
</template>

<script lang="ts">
    import { Component, Vue, Prop } from 'vue-property-decorator';
   
    import * as _ from 'underscore';
    import moment from 'moment-timezone';

    @Component
    export default class DutyCard extends Vue {

        @Prop({required: true})
        dutyRosterInfo!: any;

        

        // @shiftState.State
        // public selectedShifts!: string[];

        // @shiftState.Action
        // public UpdateSelectedShifts!: (newSelectedShifts: string[]) => void

        dutyBlocks: any[]=[] 

        blockDrop = false;
        updateBoxes =0;

        isMounted = false;

        mounted()
        {
            console.log(this.dutyRosterInfo)
            this.isMounted = false;
            this.dutyBlocks = []

            if(this.dutyRosterInfo.attachedDuty){

                const dutyStart = moment(this.dutyRosterInfo.attachedDuty.startDate).tz(this.dutyRosterInfo.attachedDuty.timezone);
                const dutyEnd = moment(this.dutyRosterInfo.attachedDuty.endDate).tz(this.dutyRosterInfo.attachedDuty.timezone);
                const startOfDay = moment(dutyStart).startOf("day");
                // console.log(dutyStart)
                this.dutyBlocks.push({
                    id:this.dutyRosterInfo.attachedDuty.id,                    
                    startTime: 1+ moment.duration(dutyStart.diff(startOfDay)).asMinutes()/15,
                    endTime: 1+ moment.duration(dutyEnd.diff(startOfDay)).asMinutes()/15,                    
                    color: this.dutyRosterInfo.type.colorCode,
                    height: '2/6',
                    title: 'Unassigned'
                })
                //console.log(this.dutyBlocks)
            }

            
            
            //const sortedShifts = _.sortBy(this.dutyRosterInfo.shifts,'startDate');

            // for(const shift of sortedShifts){ 
            //     const start = moment(shift.startDate);
            //     const end = moment(shift.endDate);
            //     const startOfDay = moment(shift.startDate).startOf('day');
            //     const startTime = moment.duration(start.diff(startOfDay)).asMinutes()
            //     const endTime = moment.duration(end.diff(startOfDay)).asMinutes()

            //     this.dutyBlocks.push({
            //         id:shift.id,                    
            //         startTime: startTime/15,
            //         endTime: endTime/15,                    
            //         color: this.getDutyColor('free'),
            //         type: 'court',
            //         assignment: 'free'
            //     })
            // }
            this.isMounted = true; 
        }

        public getDutyColor(type) {
            const color = {court:'#189fd4',jail:'#A22BB9',escort:'#ffb007',other:'#c91a5d',free:'#e6e9e2',overtime:'#0cc97e'};
            return color[type];
        }


    }
</script>

<style scoped>   

    .card {
        border: white;
    }

    .grid {        
        display:grid;
        grid-template-columns: repeat(96, 1.041666%);
        grid-template-rows: repeat(6,.5rem);
        inline-size: 100%; 
        background-color: #EEEEEE; 
        height: 2.9rem; 
        margin: 0; 
        padding: 0;
        column-gap: 0;
        row-gap: 0;
           
    }

    .grid > * { 
        padding: 0 0;
        border: 1px dotted rgb(212, 212, 212);
    }

</style>