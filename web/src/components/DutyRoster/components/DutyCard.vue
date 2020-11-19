<template>
    <div class="grid">
        <div v-for="i in 96" :key="i" :style="{backgroundColor: '#F9F9F9', gridColumnStart: i,gridColumnEnd:(i+1), gridRow:'1/7'}"></div>
        <div
            v-for="block in dutyBlocks" 
            :key="block.id"
            :style="{gridColumnStart: block.startTime, gridColumnEnd:block.endTime, gridRow:block.height,  backgroundColor: block.color, fontSize:'9px', textAlign: 'center', margin:0, padding:0  }"
            v-b-tooltip.hover                            
            :title="block.title" 
            @dragover.prevent
            @drop.prevent="drop" >
                <div style="text-transform: capitalize; margin:  0 padding:0; color:white;">
                    {{block.title|truncate(block.endTime - block.startTime-1)}}
                </div>     
        </div>    
        <!-- <div :style="{gridColumnStart: 6,gridColumnEnd:9, gridRow:'4/6', backgroundColor: 'blue' }"></div>     -->
    
        <b-modal v-model="assignDutyError" size="lg" id="bv-modal-assign-duty-error" header-class="bg-primary text-light">
			<!-- <b-table
            :items="assignDutyErrorMsg"
			:fields="importConflictFields"        
            thead-class="d-none"
            responsive="sm"
            borderless 
            small              
            striped
            >
            </b-table> -->
            <template v-slot:modal-title>
                <h2 class="mb-0 text-light">Assign Duty Issues</h2>                   
            </template>
            
            <template v-slot:modal-footer>
                <b-button variant="primary" @click="$bvModal.hide('bv-modal-assign-duty-error')">Ok</b-button>
            </template>            
            <template v-slot:modal-header-close>                 
                <b-button variant="outline-warning" class="text-light closeButton" @click="$bvModal.hide('bv-modal-assign-duty-error')"
                >&times;</b-button>
            </template>
        </b-modal>


    </div>
</template>

<script lang="ts">
    import { Component, Vue, Prop } from 'vue-property-decorator';
   
    import * as _ from 'underscore';
    import moment from 'moment-timezone';
    import { assignDutyInfoType, assignmentCardInfoType, myTeamShiftInfoType } from '../../../types/DutyRoster';

    import { namespace } from "vuex-class"; 
    import "@store/modules/DutyRosterInformation";   
    const dutyState = namespace("DutyRosterInformation");

    @Component
    export default class DutyCard extends Vue {

        @Prop({required: true})
        dutyRosterInfo!: assignmentCardInfoType;

        @dutyState.State
        public shiftAvailabilityInfo!: myTeamShiftInfoType[];

        dutyBlocks: any[]=[] 

        blockDrop = false;
        updateBoxes =0;

        assignDutyError = false;

        isMounted = false;

        assignedArray: number[] = [];
        unassignedArray: number[] = [];
        dutyDate ='';

        mounted()
        {
            console.log(this.dutyRosterInfo)
            this.isMounted = false;
            this.dutyBlocks = []

            if(this.dutyRosterInfo.attachedDuty){

                const dutyStartTime = moment(this.dutyRosterInfo.attachedDuty.startDate).tz(this.dutyRosterInfo.attachedDuty.timezone);
                const dutyEndTime = moment(this.dutyRosterInfo.attachedDuty.endDate).tz(this.dutyRosterInfo.attachedDuty.timezone);
                const startOfDay = moment(dutyStartTime).startOf("day");
                const dutyStartBin = moment.duration(dutyStartTime.diff(startOfDay)).asMinutes()/15;
                const dutyEndBin = moment.duration(dutyEndTime.diff(startOfDay)).asMinutes()/15;

                this.dutyDate = startOfDay.format();

                this.unassignedArray = this.fillInArray(Array(96).fill(0),1,dutyStartBin, dutyEndBin); 
                // console.log(dutyStart)
                this.dutyBlocks.push({
                    id:this.dutyRosterInfo.attachedDuty.id,                    
                    startTime: 1+ dutyStartBin,
                    endTime: 1+ dutyEndBin,                    
                    color: this.dutyRosterInfo.type.colorCode,
                    height: '2/6',
                    title: ''
                })
                //console.log(this.dutyBlocks)
            }

            this.isMounted = true; 
        }

        public getDutyColor(type) {
            const color = {court:'#189fd4',jail:'#A22BB9',escort:'#ffb007',other:'#0cc97e',free:'#e6e9e2',overtime:'#0cc97e'};
            return color[type];
        }

         public drop(event: any) 
        {
            this.blockDrop = false;
            const cardid = event.dataTransfer.getData('text');
            console.log(cardid)
            const sheriffId = cardid.slice(7)
            console.log(sheriffId)
            // console.log(this.unionArrays([0,2,2],[1,0,1]))
            // console.log(this.addArrays([0,0,2],[-1,0,0]))
            // console.log(this.sumOfArrayElements([0,1,2]))

            // console.log(this.fillInArray(Array(6).fill(0), 5 , 2,4))
            // console.log(this.subtractUnionOfArrays([1,1,1,1,0],[0,2,2,0,0]))

            const sheriff = this.shiftAvailabilityInfo.filter(sheriff=>{if(sheriff.sheriffId==sheriffId)return true})[0];

            const availability = sheriff.availability
            const duties = sheriff.duties
            console.log(sheriff)
            // console.log(Array(3).fill(0))

            console.log(this.unassignedArray)
            console.log(availability)
            const unionUnassignAvail = this.unionArrays(this.unassignedArray, availability)
            console.log(unionUnassignAvail)

            
            if(this.sumOfArrayElements(unionUnassignAvail)>0){
                console.log('call assign')
                const timeRangeBins = this.getArrayRangeBins(unionUnassignAvail);
                const timeRangeDate = this.convertTimeRangeBinsToTime(this.dutyDate, timeRangeBins.startBin, timeRangeBins.endBin);
                this.assignDuty(sheriffId, timeRangeDate.startTime, timeRangeDate.endTime)
                
            }else{
                const unionUnassignDuties = this.unionArrays(this.unassignedArray, duties)
                if(this.sumOfArrayElements(unionUnassignDuties)>0){
                    console.log('overtime conflicts')
                    // add window to display issue 
                    // define variables
					// this.assignDutyErrorMsg = errMsg.slice(0,60) + (errMsg.length>60?' ...':'');
					// this.assignDutyErrorMsgDesc = errMsg;
					// this.assignDutyError = true;
                }else{
                    console.log('call assign overtime')
                    const timeRangeBins = this.getArrayRangeBins(this.unassignedArray);
                    const timeRangeDate = this.convertTimeRangeBinsToTime(this.dutyDate, timeRangeBins.startBin, timeRangeBins.endBin);
                    this.assignDuty(sheriffId, timeRangeDate.startTime, timeRangeDate.endTime)
                }
            }
        }

        public unionArrays(arrayA, arrayB){
            return arrayA.map((arr,index) =>{return arr*arrayB[index]});
        }

        public subtractUnionOfArrays(arrayA, arrayB){
            return arrayA.map((arr,index) =>{return arr&&(arrayB[index]>0?0:1)});
        }

        public addArrays(arrayA, arrayB){
            return arrayA.map((arr,index) =>{return arr+arrayB[index]});
        }

        public sumOfArrayElements(arrayA){
            return arrayA.reduce((acc, arr) => acc + arr, 0)
        }

        public fillInArray(array, fillInNum, startBin, endBin){
            return array.map((arr,index) =>{if(index>=startBin && index<endBin) return fillInNum; else return arr;});
        }

        public getArrayRangeBins(arrayOriginal){
            const array = _.clone(arrayOriginal);
            return({
                startBin: array.findIndex(arr=>{if(arr>0) return true }),
                endBin: (96-array.reverse().findIndex(arr=>{if(arr>0) return true }))
            })
        }

        public convertTimeRangeBinsToTime(dutyDate, startBin, endBin){
            
            const startTime = moment(dutyDate).add(startBin*15, 'minutes').format();
            const endTime = moment(dutyDate).add(endBin*15, 'minutes').format();

            return( {startTime: startTime, endTime:endTime } )
        }

        public assignDuty(sheriffId, startDate, endDate ) {

            if(this.dutyRosterInfo.attachedDuty){
                const dutyInfo = this.dutyRosterInfo.attachedDuty;

                console.log(startDate)
                console.log(endDate)
                console.log(dutyInfo)
                
                const body: assignDutyInfoType[] = [
                    {
                        id: dutyInfo.id,
                        startDate: dutyInfo.startDate,
                        endDate: dutyInfo.endDate,
                        locationId: dutyInfo.locationId,
                        assignmentId: dutyInfo.assignmentId,
                        dutySlots: [ {                        
                                startDate: startDate,
                                endDate: endDate,
                                dutyId: dutyInfo.id,
                                sheriffId: sheriffId,
                                shiftId: null,
                                timezone: dutyInfo.timezone,
                                isNotRequired: false,
                                isNotAvailable: false
                            }
                        ],
                        timezone: dutyInfo.timezone
                    }
                ];
                
                console.log(body)
                const url = 'api/dutyroster';
                this.$http.put(url, body )
                    .then(response => {
                        if(response.data){
                                // Update the duty bar with name;
                                // this.$emit('change');
                        }
                    }, err => {
                        const errMsg = err.response.data.error;
                        // add window to display issue 
                        // define variables
                        // this.assignDutyErrorMsg = errMsg.slice(0,60) + (errMsg.length>60?' ...':'');
                        // this.assignDutyErrorMsgDesc = errMsg;
                        // this.assignDutyError = true;
                    })
            }
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