<template>
    <div class="grid">
        <div v-for="i in 96" :key="i" :style="{backgroundColor: '#F9F9F9', gridColumnStart: i,gridColumnEnd:(i+1), gridRow:'1/7'}"></div>
        <div
            v-for="block in dutyBlocks" 
            :key="block.id"
            :id="block.id"
            :style="{gridColumnStart: block.startTime, gridColumnEnd:block.endTime, gridRow:block.height,  backgroundColor: block.color, fontSize:'9px', textAlign: 'center', margin:0, padding:0  }"
            v-b-tooltip.hover                            
            :title="block.title" 
            @dragover.prevent
            @drop.prevent="drop" >
                <b style="text-transform: capitalize; margin:  0 padding:0; color:white;">
                    {{block.title|truncate(block.endTime - block.startTime-1)}}
                </b>     
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

        assignDutyError = false;

        isMounted = false;

        assignedArray: number[] = [];
        unassignedArray: number[][] = [];
        dutyDate ='';

        mounted()
        {
            console.log(this.dutyRosterInfo)
            this.isMounted = false;
            this.dutyBlocks = [];
            this.extractDuty();
        }

        public extractDuty(){
            if(this.dutyRosterInfo.attachedDuty){
                const dutyInfo = this.dutyRosterInfo.attachedDuty;

                const dutyStartTime = moment(dutyInfo.startDate).tz(dutyInfo.timezone);
                const startOfDay = moment(dutyStartTime).startOf("day");
                this.dutyDate = startOfDay.format();

                const dutyBin = this.getTimeRangeBins(dutyInfo.startDate, dutyInfo.endDate,startOfDay, dutyInfo.timezone);
                let unassignedArray = this.fillInArray(Array(96).fill(0),1,dutyBin.startBin, dutyBin.endBin); 
                              
                for(const dutySlot of dutyInfo.dutySlots){
                    //console.log(dutySlot)
                    let id = 1000;
                    const assignedDutyBin = this.getTimeRangeBins(dutySlot.startDate, dutySlot.endDate,startOfDay, dutySlot.timezone)
                    unassignedArray = this.fillInArray(unassignedArray,0,assignedDutyBin.startBin, assignedDutyBin.endBin);
                    const sheriff = this.shiftAvailabilityInfo.filter(sheriff=>{if(sheriff.sheriffId==dutySlot.sheriffId)return true})[0];
                    const isOvertime = this.getOverTime(dutySlot.shiftId, dutySlot.isNotAvailable, dutySlot.isNotRequired, dutySlot.isOvertime);                    
                    this.dutyBlocks.push({
                        id: 'dutySlot'+dutySlot.id+'i'+dutyInfo.id+'n'+id++,                    
                        startTime: 1+ assignedDutyBin.startBin,
                        endTime: 1+ assignedDutyBin.endBin,                    
                        color: this.getDutyColor(this.dutyRosterInfo.type.colorCode,dutySlot.isNotAvailable,dutySlot.isNotRequired, isOvertime),
                        height: '2/6',
                        title: this.getTitle(sheriff,dutySlot.isNotAvailable,dutySlot.isNotRequired)
                    })                
                }

                this.extractUnassignedArrays(unassignedArray);

                for(const unassignInx in this.unassignedArray){
                    //console.log(unassignInx)
                    //console.log(this.unassignedArray[unassignInx])
                    const unassignedBin = this.getArrayRangeBins(this.unassignedArray[unassignInx]);
                    this.dutyBlocks.push({
                        id:'dutySlot'+dutyInfo.id+'n'+unassignInx,                    
                        startTime: 1+ unassignedBin.startBin,
                        endTime: 1+ unassignedBin.endBin,                    
                        color: this.dutyRosterInfo.type.colorCode,
                        height: '2/6',
                        title: ''
                    })                
                }

                this.dutyBlocks = _.sortBy(this.dutyBlocks,'startTime')
                //console.log(unassignedArray)

                if(this.dutyBlocks.length>1)
                    for(const dutyBlockInx in this.dutyBlocks){
                        this.dutyBlocks[dutyBlockInx].height = (Number(dutyBlockInx)%2)?'2/4':'4/6'; 
                    }

                
                //console.log(this.dutyBlocks)
            }

            this.isMounted = true; 
        }

        public extractUnassignedArrays(unassignedArray){
            let startBin = 0;
            for(let valueInx=0; valueInx<unassignedArray.length; valueInx++){

                if((unassignedArray[valueInx]>0 && valueInx==0)||(unassignedArray[valueInx]>0 && unassignedArray[valueInx-1]==0)) startBin = valueInx;
                
                if((unassignedArray[valueInx]>0 && valueInx==(unassignedArray.length-1))||(unassignedArray[valueInx]>0 && unassignedArray[valueInx+1]==0)){
                    //console.log(startBin)
                    //console.log(valueInx)
                    const array = this.fillInArray(Array(96).fill(0),1, startBin, valueInx+1)
                    //console.log(array)
                    this.unassignedArray.push(array)
                    //console.log(this.unassignedArray)
                } 
            }
        }

        public getDutyColor(color, isNotAvailable, isNotRequired, isOverTime) {
            if(isOverTime) return '#e85a0e';
            else if(isNotRequired) return  '#28a745';
            else if(isNotAvailable) return '#dc3545';
            else return color;
        }

        public getTitle(sheriff,isNotAvailable,isNotRequired){
            if(isNotAvailable) return 'isNotAvailable';
            else if(isNotRequired) return  'isNotRequired';
            else if(sheriff) return sheriff.lastName+', '+sheriff.firstName;
            else return ' ';
        }

        public getOverTime(shiftId, isNotAvailable, isNotRequired, isOverTime) {
            if(isOverTime) return true;
            else if(isNotRequired || isNotAvailable) return  false;
            else if(!shiftId) return true;
            else return false;
        }
        
        public drop(event: any) 
        {
            console.log(event.target.id)
            if(event.target.id){
                const cardid = event.dataTransfer.getData('text');
                const blockId: string = event.target.id;
                const unassignedBlockId = Number(blockId.substring(blockId.indexOf('n')+1));
                console.log(unassignedBlockId)
                if(unassignedBlockId>=1000) return
                // console.log(cardid)
                const sheriffId = cardid.slice(7)
                // console.log(sheriffId)
                const unassignedArray = this.unassignedArray[unassignedBlockId]

                if(sheriffId=='00000-00000-11111'||sheriffId=='00000-00000-22222'){
                    const timeRangeBins = this.getArrayRangeBins(unassignedArray);
                    const timeRangeDate = this.convertTimeRangeBinsToTime(this.dutyDate, timeRangeBins.startBin, timeRangeBins.endBin);
                    this.assignDuty(sheriffId, timeRangeDate.startTime, timeRangeDate.endTime, null);
                    return
                }

                const sheriff = this.shiftAvailabilityInfo.filter(sheriff=>{if(sheriff.sheriffId==sheriffId)return true})[0];
                const availability = sheriff.availability
                const duties = sheriff.duties
                // console.log(sheriff)
                // console.log(unassignedArray)
                // console.log(availability)
                // console.log(duties)
                

                const unionUnassignAvail = this.unionArrays(unassignedArray, availability)
                console.log(unionUnassignAvail)

                
                if(this.sumOfArrayElements(unionUnassignAvail)>0){
                    console.log('call assign')
                    const timeRangeBins = this.getArrayRangeBins(unionUnassignAvail);
                    const timeRangeDate = this.convertTimeRangeBinsToTime(this.dutyDate, timeRangeBins.startBin, timeRangeBins.endBin);
                    // console.log(timeRangeBins)
                    this.assignDuty(sheriffId, timeRangeDate.startTime, timeRangeDate.endTime, timeRangeBins.binValue)
                    
                }else{
                    const unionUnassignDuties = this.unionArrays(unassignedArray, duties)
                    if(this.sumOfArrayElements(unionUnassignDuties)>0){
                        console.log('overtime conflicts')
                        // add window to display issue 
                        // define variables
                        // this.assignDutyErrorMsg = errMsg.slice(0,60) + (errMsg.length>60?' ...':'');
                        // this.assignDutyErrorMsgDesc = errMsg;
                        // this.assignDutyError = true;
                    }else{
                        console.log('call assign overtime')
                        const timeRangeBins = this.getArrayRangeBins(unassignedArray);
                        const timeRangeDate = this.convertTimeRangeBinsToTime(this.dutyDate, timeRangeBins.startBin, timeRangeBins.endBin);
                        this.assignDuty(sheriffId, timeRangeDate.startTime, timeRangeDate.endTime, null)
                    }
                }
            }
        }

        public unionArrays(arrayA, arrayB){
            return arrayA.map((arr,index) =>{return arr*arrayB[index]});
        }

        // public subtractUnionOfArrays(arrayA, arrayB){
        //     return arrayA.map((arr,index) =>{return arr&&(arrayB[index]>0?0:1)});
        // }

        // public addArrays(arrayA, arrayB){
        //     return arrayA.map((arr,index) =>{return arr+arrayB[index]});
        // }

        public sumOfArrayElements(arrayA){
            return arrayA.reduce((acc, arr) => acc + arr, 0)
        }

        public fillInArray(array, fillInNum, startBin, endBin){
            return array.map((arr,index) =>{if(index>=startBin && index<endBin) return fillInNum; else return arr;});
        }

        public getArrayRangeBins(arrayOriginal){
            const array = _.clone(arrayOriginal);
            const startBin: number = array.findIndex(arr=>{if(arr>0) return true });
            const binValue: number = startBin>=0? array[startBin]: 1;
            //console.log(startBin)
            //console.log(array[startBin])
            return({
                startBin: startBin ,
                endBin: (96-array.reverse().findIndex(arr=>{if(arr>0) return true })),
                binValue: binValue
            })
        }

        public convertTimeRangeBinsToTime(dutyDate, startBin, endBin){            
            const startTime = moment(dutyDate).add(startBin*15, 'minutes').format();
            const endTime = moment(dutyDate).add(endBin*15, 'minutes').format();
            return( {startTime: startTime, endTime:endTime } )
        }

        public getTimeRangeBins(startDate, endDate, startOfDay, timezone){
            const startTime = moment(startDate).tz(timezone);
            const endTime = moment(endDate).tz(timezone);
            const startBin = moment.duration(startTime.diff(startOfDay)).asMinutes()/15;
            const endBin = moment.duration(endTime.diff(startOfDay)).asMinutes()/15;
            return( {startBin: startBin, endBin:endBin } )
        }

        public assignDuty(sheriffId, startDate, endDate, shiftId ) {

            if(this.dutyRosterInfo.attachedDuty){
                const dutyInfo = this.dutyRosterInfo.attachedDuty;
                let isNotRequired = false;
                let isNotAvailable = false;
                if(sheriffId=='00000-00000-11111'){
                    sheriffId = null;
                    isNotRequired = true;
                }else if(sheriffId=='00000-00000-22222'){
                    sheriffId = null;
                    isNotAvailable = true;
                }
                console.log(startDate)
                console.log(endDate)
                console.log(shiftId)
                console.log(dutyInfo)

                const dutySlots = [ { 
                        id: null,                      
                        startDate: startDate,
                        endDate: endDate,
                        dutyId: dutyInfo.id,
                        sheriffId: sheriffId,
                        shiftId: shiftId,
                        timezone: dutyInfo.timezone,
                        isNotRequired: isNotRequired,
                        isNotAvailable: isNotAvailable,
                        isOvertime: false
                    }
                ]

                for(const dutySlot of dutyInfo.dutySlots){
                    dutySlots.push({  
                        id: dutySlot.id,                                              
                        startDate: dutySlot.startDate,
                        endDate: dutySlot.endDate,
                        dutyId: dutySlot.dutyId,
                        sheriffId: dutySlot.sheriffId,
                        shiftId: dutySlot.shiftId,
                        timezone: dutySlot.timezone,
                        isNotRequired: dutySlot.isNotRequired,
                        isNotAvailable: dutySlot.isNotAvailable,
                        isOvertime: dutySlot.isOvertime                           
                    })                    
                }
                
                const body: assignDutyInfoType[] = [
                    {
                        id: dutyInfo.id,
                        startDate: dutyInfo.startDate,
                        endDate: dutyInfo.endDate,
                        locationId: dutyInfo.locationId,
                        assignmentId: dutyInfo.assignmentId,
                        dutySlots: dutySlots,
                        timezone: dutyInfo.timezone
                    }
                ];
                
                console.log(body)
                const url = 'api/dutyroster';
                this.$http.put(url, body )
                    .then(response => {
                        if(response.data){
                                // Update the duty bar with name;
                            this.$emit('change');
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
        
            // console.log(this.unionArrays([0,2,2],[1,0,1]))
            // console.log(this.addArrays([0,0,2],[-1,0,0]))
            // console.log(this.sumOfArrayElements([0,1,2]))

            // console.log(this.fillInArray(Array(6).fill(0), 5 , 2,4))
            // console.log(this.subtractUnionOfArrays([1,1,1,1,0],[0,2,2,0,0]))

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