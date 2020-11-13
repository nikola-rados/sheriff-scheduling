<template>
    <div class="grid">
        <div v-for="i in 96" :key="i" :style="{backgroundColor: '#EEEEEE', gridColumnStart: i,gridColumnEnd:(i+1), gridRow:'1/6'}"></div>
        <div :style="{gridColumnStart: 3,gridColumnEnd:6, gridRow:'2/2',  backgroundColor: 'red' }"></div>    
        <div :style="{gridColumnStart: 6,gridColumnEnd:9, gridRowStart:3, gridRowEnd:3, backgroundColor: 'blue' }"></div>    
    </div>
    <!-- <div v-if=" isMounted" id="shiftBox" ref="shiftBox" :key="updateBoxes">             
        <div
            v-for="block in dutyBlocks"
            :key="block.key"
            :id="'dutyCard'+block.key"
            :style="{ backgroundColor: block.color, float:'left', position: 'relative', left:block.startTime+'%', width:block.timeDuration+'%', height:block.height}"
            no-body>
                <span v-if="blockSize(block)>30" @mousedown="cardSelected(block)" @dblclick="selectOnlyCard(block)" style="height:100%">                    
                    <span style="text-align: center;font-size:10px; line-height: 12px; display: block;">{{block.timeStamp}}</span>
                </span>
                <span v-else @mousedown="cardSelected(block)" @dblclick="selectOnlyCard(block)" style="height:100%">
                    <span 
                        v-b-tooltip.hover                                
                        :title="block.title + ' ' + block.timeStamp">
                        ...
                    </span>                    
                </span>
        </div>
    </div> -->
</template>

<script lang="ts">
    import { Component, Vue, Prop } from 'vue-property-decorator';
   
    import * as _ from 'underscore';

    @Component
    export default class DutyCard extends Vue {

        // @Prop({required: true})
        // scheduleInfo!: conflictsInfoType;

        // @Prop({required: true})
        // sheriffId!: string;

        // @shiftState.State
        // public selectedShifts!: string[];

        // @shiftState.Action
        // public UpdateSelectedShifts!: (newSelectedShifts: string[]) => void

        dutyBlocks: any[]=[] //dutyBlockInfoType[] = [];

        blockDrop = false;
        updateBoxes =0;

        isMounted = false;

        mounted()
        {
            // console.log(this.scheduleInfo)
            this.isMounted = false;

            //const sortedScheduleInfo: conflictsInfoType[] = _.sortBy(this.scheduleInfo,'startInMinutes')
           
            this.dutyBlocks = []
            let widthOtherElements =0;
            // for(const schedule of sortedScheduleInfo){
                
            //     this.dutyBlocks.push({
            //         id: schedule.id? schedule.id:'',
            //         key: this.sheriffId+schedule.date +'Z' +schedule.startTime,
            //         startTime: schedule.startInMinutes *5 /72 -widthOtherElements,
            //         timeDuration: schedule.timeDuration * 5 /72,
            //         timeStamp: schedule.startTime +'-'+schedule.endTime,
            //         title:schedule.type=='Loaned'? 'Loaned to ' + schedule.location: schedule.type,
            //         color: this.getScheduleColor(schedule.type).body, //unused now
            //         originalColor: this.getScheduleColor(schedule.type).body, //unused now
            //         headerColor: this.getScheduleColor(schedule.type).header, //unused now
            //         selected: false,
            //         type: schedule.type
            //     })
            //     widthOtherElements += (schedule.timeDuration * 5 /72);
            //         //console.log(this.scheduleBlocks)
                            
            // }120*5 /3 -widthOtherElements,


                this.dutyBlocks.push({
                    key: 'k1',
                    startTime: 10,
                    timeDuration: 20,
                    height:'1rem',
                    timeStamp: '8-10',
                    color: '#BEF2F7', //unused now
                    selected: false,
                    type: 'Shift'
                })

                widthOtherElements += (200);

                this.dutyBlocks.push({
                    key: 'k2',
                    startTime: 30,
                    timeDuration: 30,
                    height:'1rem',
                    timeStamp: '10-12',
                    color: '#1EF2F7', //unused now
                    selected: false,
                    type: 'Shift'
                })

                widthOtherElements += (400 * 5 /72);


            this.isMounted = true; 
        }

        

        public getDutyColor(block) {
            let color = '';
            if(block.type=='Unavailable') color = '#CFCFCF'
            else if(block.type=='Shift') color = '#BEF2F7' 
            else color = '#FFEEDD'
            // if (block.id != null && this.selectedShifts.includes(block.id)) color.body ='#F7F54F';
            return color;
        }

        //Wrapper function, the markup wouldn't bind to getScheduleColor. 
        public dutyColor(block){
            return this.getDutyColor(block);
        }

        public blockSize(block){
            //console.log('schCard'+block.key)
            const el = document.getElementById('dutyCard'+block.key)
            //console.log(el)
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

    .grid {        
        display:grid;
        grid-template-columns: repeat(96, 1.041666%);
        grid-template-rows: repeat(4,1rem);
        inline-size: 100%; 
        background-color: #EEEEEE; 
        height: 4rem; 
        margin: 0; 
        padding: 0;
        column-gap: 0;
        row-gap: 0;
           
    }

    .grid > * { 
        padding: 0 0;
        border: 1px dotted rgb(202, 202, 202);
    }

</style>