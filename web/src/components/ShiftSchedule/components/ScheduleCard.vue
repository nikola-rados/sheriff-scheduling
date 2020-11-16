<template>
    <div v-if="scheduleInfo.length>0 && isMounted" id="shiftBox" ref="shiftBox" :key="updateBoxes">             
        <b-card
            v-for="block in scheduleBlocks"
            :key="block.key"
            :id="'schCard'+block.key"
            :style="{ backgroundColor: scheduleColor(block).body, float:'left', position: 'relative', left:block.startTime+'%', width:block.timeDuration+'%', height:'3rem'}"
            no-body>
                <span v-if="blockSize(block)>30" @mousedown="cardSelected(block)" @dblclick="selectOnlyCard(block)" style="height:100%"> 
                    <h6 class="m-0 mb-1 p-0" :style="{ backgroundColor:scheduleColor(block).header, color: 'white', textAlign: 'center', fontSize:'10px', lineHeight: '16px' }"
                        v-b-tooltip.hover                                
                        :title="block.title + ' ' + block.timeStamp">
                        <font-awesome-icon v-if="block.title.includes('Loaned')" style="transform: rotate(180deg); font-size: .55rem;"  icon="sign-out-alt" /> 
                        <font-awesome-icon v-if="block.title=='Leave'" style="font-size: .55rem;"  icon="suitcase"/> 
                        <font-awesome-icon v-if="block.title=='Training'" style="font-size: .5rem;" icon="graduation-cap"/> 
                        <b-icon-person-fill v-if="block.title=='Shift'"/>
                        <b-icon-calendar2-x v-if="block.title=='Unavailable'"/>
                    </h6>
                    <div class="m-0 p-0" style="text-align: center;font-size:10px; line-height: 12px; display: block;">{{block.timeStamp}}</div>
                </span>
                <span v-else @mousedown="cardSelected(block)" @dblclick="selectOnlyCard(block)" style="height:100%">
                    <h6 
                        v-b-tooltip.hover                                
                        :title="block.title + ' ' + block.timeStamp"
                        :style="'background-color:'+scheduleColor(block).header+'; color:white; text-align: center; font-size:6px; line-height: 16px;'">
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

        @Prop({required: true})
        sheriffId!: string;

        @shiftState.State
        public selectedShifts!: string[];

        @shiftState.Action
        public UpdateSelectedShifts!: (newSelectedShifts: string[]) => void

        scheduleBlocks: scheduleBlockInfoType[] = [];

        blockDrop = false;
        updateBoxes =0;

        isMounted = false;

        mounted()
        {
            // console.log(this.scheduleInfo)
            this.isMounted = false;

            const sortedScheduleInfo: conflictsInfoType[] = _.sortBy(this.scheduleInfo,'startInMinutes')
           
            this.scheduleBlocks = []
            let widthOtherElements =0;
            for(const schedule of sortedScheduleInfo){
                if(schedule.startTime){

                    this.scheduleBlocks.push({
                        id: schedule.id? schedule.id:'',
                        key: this.sheriffId+schedule.date +'Z' +schedule.startTime,
                        startTime: schedule.startInMinutes *5 /72 -widthOtherElements,
                        timeDuration: schedule.timeDuration * 5 /72,
                        timeStamp: schedule.startTime +'-'+schedule.endTime,
                        title:schedule.type=='Loaned'? 'Loaned to ' + schedule.location: schedule.type,
                        color: this.getScheduleColor(schedule.type).body, //unused now
                        originalColor: this.getScheduleColor(schedule.type).body, //unused now
                        headerColor: this.getScheduleColor(schedule.type).header, //unused now
                        selected: false,
                        type: schedule.type
                    })
                    widthOtherElements += (schedule.timeDuration * 5 /72);
                    //console.log(this.scheduleBlocks)
                }else{

                    this.scheduleBlocks.push({
                        id: schedule.id? schedule.id:'',
                        key: this.sheriffId+schedule.date +'Z' +schedule.startTime,
                        startTime: 0,
                        timeDuration: 100,
                        timeStamp: 'Full Day',
                        title:schedule.type=='Loaned'? 'Loaned to ' + schedule.location: schedule.type,
                        color: this.getScheduleColor(schedule.type).body, //unused now
                        originalColor: this.getScheduleColor(schedule.type).body, //unused now
                        headerColor: this.getScheduleColor(schedule.type).header, //unused now
                        selected: false, //unused now
                        type: schedule.type
                    })
                }                    
            }
            this.isMounted = true; 
        }

        public selectOnlyCard(block){
            if(block.title=='Shift') {
                this.UpdateSelectedShifts([ block.id ]);
                this.$root.$emit('editShifts');
            }
        }

        public cardSelected(block){
            let selectedCards = this.selectedShifts;
            if(block.title=='Shift'){
                if (!selectedCards.includes(block.id))
                    selectedCards.push(block.id);
                else
                    selectedCards = selectedCards.filter(sc => sc != block.id);
                this.UpdateSelectedShifts(selectedCards);
            }
        }

        public getScheduleColor(block) {
            let color = {body:'#FFEEDD' , header:'#F94567'};
            if(block.type=='Unavailable') color = {body:'#CFCFCF' , header:'#868686'}
            else if(block.type=='Shift') color = {body:'#BEF2F7' , header:'#004567'}
            if (block.id != null && this.selectedShifts.includes(block.id)) color.body ='#F7F54F';
            return color;
        }

        //Wrapper function, the markup wouldn't bind to getScheduleColor. 
        public scheduleColor(block){
            return this.getScheduleColor(block);
        }

        public blockSize(block){
            //console.log('schCard'+block.key)
            const el = document.getElementById('schCard'+block.key)
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

</style>