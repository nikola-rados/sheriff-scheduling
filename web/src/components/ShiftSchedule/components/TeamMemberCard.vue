<template>
    <div v-if="isDataMounted">    
        <b-row 
            style="width:100%; height:100%;" 
            bg-variant="white"            
            class="mx-2 my-1 p-1">
                <b-col cols="9">
                    <b-row style="font-size:11px; line-height: 16px;"># {{sheriffInfo.badgeNumber}}</b-row>
                    <b-row style="font-size:9px; line-height: 14px;">{{sheriffInfo.rank}}</b-row>
                    <b-row 
                        style="font-size:12px; line-height: 16px; font-weight: bold; text-transform: Capitalize;" 
                        v-b-tooltip.hover.topleft                                
                        :title="fullName.length>14?fullName:''">
                            {{fullName|truncate(12)}}
                    </b-row>
                </b-col>
                <b-col cols="3" class="m-0 p-0">
                    <b-button 
                        class="m-0 p-0"                    
                        size="sm" 
                        variant="success" 
                        @click="AddShift()">
                            <b-icon-plus/>
                    </b-button>                    
                </b-col>
        </b-row>

        <b-modal  v-model="showShiftDetails" id="bv-modal-shift-details" header-class="bg-primary text-light">
            <template v-slot:modal-title>
                <span v-if="editMode" class="mb-0 text-light"> 
                    <h3 class="m-0 p-0" >Updating Shift </h3>
                    <h4  class="m-0 pt-2 pb-0 text-warning" style="text-align: center"> {{sheriffInfo.firstName}} {{sheriffInfo.lastName}} </h4> 
                </span>
                <span v-else-if="createMode" class="m-0 p-0" > 
                    <h3 class="m-0 p-0" >Creating Shift for </h3>
                    <h4  class="m-0 pt-2 pb-0 text-warning" style="text-align: center"> {{sheriffInfo.firstName}} {{sheriffInfo.lastName}}</h4>
                </span>
            </template>

            <b-card v-if="isShiftDataMounted" no-body style="font-size: 14px; user-select: none;" >
                
                <b-row v-if="shiftError" id="ShiftError" class="h3 mx-2">
                    <b-badge class="mx-1 mt-2"
                        v-b-tooltip.hover
                        :title="shiftErrorMsgDesc"
                        variant="danger"> {{shiftErrorMsg}}
                        <b-icon class="ml-3"
                            icon = x-square-fill
                            @click="shiftError = false"
                    /></b-badge>                    
                </b-row>              


                <b-row class="mx-1 my-3">
                    <b-form-group class="bg-light">
                        <b-row> 
                            <label class="h6 ml-3 mr-5">Days<span class="text-danger">*</span></label>  
                            <b-form-checkbox
                                size="sm"
                                class="ml-auto mr-4 text-jail"									
                                v-model="weekDaysSelected"
                                :disabled="selectWeekDisabled"
                                @change="toggleWeekDays">
                                    Select Week Days
                            </b-form-checkbox>                       
                            <b-form-checkbox
                                size="sm"
                                class="ml-auto mr-4 text-court"									
                                v-model="allDaysSelected"
                                :disabled="selectAllDisabled"
                                @change="toggleAllDays">
                                    Select All
                            </b-form-checkbox>
                        </b-row>
                        <b-form-checkbox-group
                            size="sm"			
                            v-model="selectedDays"								
                            :state = "selectedDayState?null:false">
                            <b-td
                                v-for="day in dayOptions"
                                :key="day.diff">
                                <b-tr style="float:left">
                                    <b-td><conflicts-icon  :conflictsInfo="day.conflicts.Shift" type="Shift" :index="day.diff" /></b-td>
                                    <b-td><conflicts-icon  :conflictsInfo="day.conflicts.Loaned" type="Loaned" :index="day.diff" /></b-td>
                                    <b-td><conflicts-icon  :conflictsInfo="day.conflicts.Leave" type="Leave" :index="day.diff" /></b-td>                    
                                    <b-td><conflicts-icon  :conflictsInfo="day.conflicts.Training" type="Training" :index="day.diff" /></b-td>                                
                                </b-tr>
                                <b-tr>
                                    <b-td colspan="3" >
                                        <b-form-checkbox style="width:3.32rem;"
                                            @change="weekdaysChanged"                                            					
                                            :disabled="day.fullday"                                            
                                            :value="day.diff">
                                            <span class="h6 font-weight-normal m-0 p-0">{{day.name}}</span>
                                        </b-form-checkbox>
                                    </b-td>
                                </b-tr>
                            </b-td>
                        </b-form-checkbox-group>
                    </b-form-group>
                </b-row>

                <div>Selected: <strong>{{ selectedDays }}</strong></div>

                <b-row class="mx-1 my-0 p-0">
                    <b-form-group class="mr-3" style="width: 7rem">
                        <label class="h6 m-0 p-0">From<span class="text-danger">*</span></label>
                        <b-form-input
                            v-model="selectedStartTime"
                            @click="startTimeState=true"
                            size="sm"
                            type="text"
                            autocomplete="off"
                            @paste.prevent
                            :formatter="timeFormat"
                            placeholder="HH:MM"
                            :state = "startTimeState?null:false"
                        ></b-form-input>
                    </b-form-group>

                    <b-form-group class="mr-5" style="width: 7rem;">
                        <label class="h6 m-0 p-0">To<span class="text-danger">*</span></label>
                        <b-form-input
                            v-model="selectedEndTime"
                            @click="endTimeState=true"
                            size="sm"
                            type="text"
                            autocomplete="off"
                            @paste.prevent
                            :formatter="timeFormat"
                            placeholder="HH:MM"
                            :state = "endTimeState?null:false"
                        ></b-form-input>
                    </b-form-group>                
                </b-row>
            </b-card>

            <template v-slot:modal-footer>
                <b-button
                        variant="secondary"
                        size="sm"
                        @click="closeShiftWindow()"
                ><b-icon-x font-scale="1.5" style="padding:0; vertical-align: middle; margin-right: 0.25rem;"></b-icon-x>Cancel</b-button>
                <b-button
                        variant="success"
                        size="sm"
                        @click="saveShift()"
                ><b-icon-check2 style="padding:0; vertical-align: middle; margin-right: 0.25rem;"></b-icon-check2>Save</b-button>
            </template>
            <template v-slot:modal-header-close>
                <b-button
                    variant="outline-primary"
                    class="text-light closeButton"
                    @click="closeShiftWindow()"
                    >
                    &times;</b-button>
            </template>
		</b-modal>       
    </div>
</template>

<script lang="ts">
    import { Component, Vue, Prop } from 'vue-property-decorator';
    import { namespace } from "vuex-class";
    import "@store/modules/ShiftScheduleInformation";
    const shiftState = namespace("ShiftScheduleInformation");
    import "@store/modules/CommonInformation";
    const commonState = namespace("CommonInformation");
    import ConflictsIcon from './ConflictsIcon.vue'
    import { dayOptionsInfoType, sheriffAvailabilityInfoType,shiftInfoType,shiftRangeInfoType } from '../../../types/ShiftSchedule';
    import moment from 'moment-timezone';
    import { locationInfoType } from '../../../types/common';
    @Component({
        components: {
            ConflictsIcon
        }
    })
    export default class TeamMemberCard extends Vue {

        @shiftState.State
        public shiftRangeInfo!: shiftRangeInfoType;

        @commonState.State
        public location!: locationInfoType;

        @Prop({required: true})
        public sheriffInfo!: sheriffAvailabilityInfoType;

        sheriffId = '';

        isDataMounted = false;
        fullName = '';

        indeterminate = true

        halfUnavailStyle="background-image: linear-gradient(to bottom right, rgb(194, 39, 28),rgb(243, 232, 232), white);"
        halfUnavailHalfSchStyle="background-image: linear-gradient(to bottom right, rgb(194, 39, 28),rgb(243, 232, 232), rgb(12, 120, 170));"
        halfSchStyle="background-image: linear-gradient(to bottom right, rgb(12, 120, 170),rgb(243, 232, 232), white);"
        WeekDay = ['S', 'M', 'T', 'W', 'T', 'F', 'S'];

        selectedDate = '';
		selectedStartTime = '';
		selectedEndTime = '';
		showShiftDetails = false;
		isShiftDataMounted = false;
		createMode = false;
		editMode = false;
		shift = {} as shiftInfoType;
		startTimeState = true;
		endTimeState = true;
		selectedDayState = true;

		selectedDays: number[] = [];
        allDaysSelected = false;
        weekDaysSelected = false;

		dayOptions: dayOptionsInfoType[] = [];		

		shiftError = false;
		shiftErrorMsg = '';
		shiftErrorMsgDesc = '';
        
        mounted()
        {  
            this.isDataMounted = false;
            this.sheriffId = this.sheriffInfo.sheriffId;          
            this.fullName = this.sheriffInfo.lastName +', '+this.sheriffInfo.firstName;

            this.dayOptions = [
                {name:'Sun', diff:0, fullday:false, conflicts:{Training: [], Leave: [], Loaned:[], Shift:[]}},
                {name:'Mon', diff:1, fullday:false, conflicts:{Training: [], Leave: [], Loaned:[], Shift:[]}},
                {name:'Tue', diff:2, fullday:false, conflicts:{Training: [], Leave: [], Loaned:[], Shift:[]}},
                {name:'Wed', diff:3, fullday:false, conflicts:{Training: [], Leave: [], Loaned:[], Shift:[]}},
                {name:'Thu', diff:4, fullday:false, conflicts:{Training: [], Leave: [], Loaned:[], Shift:[]}},
                {name:'Fri', diff:5, fullday:false, conflicts:{Training: [], Leave: [], Loaned:[], Shift:[]}},
                {name:'Sat', diff:6, fullday:false, conflicts:{Training: [], Leave: [], Loaned:[], Shift:[]}}
            ];        
            this.extractConflicts();
            //console.log(this.dayOptions[4].conflicts.Training)
        }

        public extractConflicts() {   
            console.log(this.sheriffInfo.conflicts) 
                      
            for(const conflict of this.sheriffInfo.conflicts){
                this.dayOptions[conflict.dayOffset].conflicts[conflict.type].push(conflict); 
                this.dayOptions[conflict.dayOffset].fullday = this.dayOptions[conflict.dayOffset].fullday || conflict.fullday               
            }

            //console.log(this.dayOptions)
            
            Vue.nextTick(()=>{this.isDataMounted = true;})                     
        }

        get selectAllDisabled(){
            for(const day of this.dayOptions)
                if(day.fullday) return true;
            return false;
        }

        get selectWeekDisabled(){
            for(const day in this.dayOptions)
                if(this.dayOptions[day].fullday && this.dayOptions[day].diff !=0 && this.dayOptions[day].diff !=6) return true;
            return false;
        }

        public weekdaysChanged(){
			Vue.nextTick(()=>{
                this.allDaysSelected = this.selectedDays.length==7? true: false
                this.weekDaysSelected = this.selectedDays.includes(1) && this.selectedDays.includes(2) && this.selectedDays.includes(3) && this.selectedDays.includes(4) && this.selectedDays.includes(5) 
			})
		}

		public toggleAllDays(checked) {
            this.weekDaysSelected = checked ? true: false;
			this.selectedDays = checked ? [0,1,2,3,4,5,6] : [];
        }
        
        public toggleWeekDays(checked) {
            this.allDaysSelected = false;
			this.selectedDays = checked ? [1,2,3,4,5] : [];
		}

		public AddShift() {
			console.log('adding shift')
			// TODO: add when edit functionality is in place
			// const shift = {} as shiftInfoType;
			// this.UpdateShiftToEdit(shift);
			this.createMode = true;
			this.editMode = false;
			this.isShiftDataMounted = true;
            this.showShiftDetails = true;
            this.indeterminate= true
        }
        
        public saveShift() {
            console.log('saving')            
            this.shiftError = false;
			let requiredError = false;
			
			if (!this.selectedStartTime) {
					this.startTimeState = false;
					requiredError = true;
			} else {
                    this.selectedStartTime = this.autoCompleteTime(this.selectedStartTime);
					this.startTimeState = true;
			}
			if (!this.selectedEndTime) {
					this.endTimeState = false;
					requiredError = true;
			} else {
                    this.selectedEndTime = this.autoCompleteTime(this.selectedEndTime);
					this.endTimeState = true;
            }
            if (this.selectedStartTime && this.selectedEndTime && this.selectedStartTime >= this.selectedEndTime ) {
                    this.startTimeState = false;
                    this.endTimeState = false;
					requiredError = true;
            } 
            
			if (this.selectedDays.length < 1) {
					this.selectedDayState = false;
					requiredError = true;
			} else {
					this.selectedDayState = true;
			}

			if (!requiredError) {
                    this.startTimeState = true;
                    this.endTimeState = true;
					if (this.editMode) this.updateShift();
					if (this.createMode) this.createShift();
			} else {
					console.log('Error required')
			}
        }
        
        public autoCompleteTime(time){
            const tail="00:00";
            let result = "";
            
            if(time.length==1) result = '0'+time+ tail.slice(2);
            else if(time.length==4) {
                if(time.slice(3,4)=='2') result = time.slice(0,3)+'15';
                else result = time+(time.slice(-1)=='1' || time.slice(-1)=='4'?'5':'0');
            }
            else if(time.length==5){
                if(time.slice(3,4)=='2') result = time.slice(0,3)+'15';
                else result =time.slice(0,4)+(time.slice(3,4)=='1' || time.slice(3,4)=='4'?'5':'0');
            }
            else  result = time+ tail.slice(time.length);
            return result
        }

		public closeShiftWindow(){
			this.resetShiftWindowState();
			this.showShiftDetails = false;
		}

		public resetShiftWindowState() {
				this.shift = {} as shiftInfoType;
				this.ClearFormState();
		}

		public ClearFormState(){				
				this.selectedDayState = true;
				this.startTimeState = true;
				this.endTimeState = true;
        }
        
        public getListOfDates(days){
            const listOfDates: any[] = [];   
			for(const day of this.dayOptions) {
                if(this.selectedDays.includes(day.diff)){

                    const startTime = this.completeDate(day.diff, this.selectedStartTime);
                    const endTime = this.completeDate(day.diff, this.selectedEndTime);
                    const shifts = this.takeoutConflicts(day.diff, startTime, endTime )
                    console.log(shifts)
                    console.log(shifts.length)
                    if(shifts.length==0){
                        this.shiftErrorMsg = 'Too many shift conflicts on '+day.name +'.';
                        this.shiftErrorMsgDesc = '';
                        this.shiftError = true;
                        return
                    }

                    for(const shift of shifts){
                        if(shift.start>=shift.end)continue;
                        listOfDates.push({
                            id: 0,
                            startDate: moment(shift.start).utc().format(),
                            endDate: moment(shift.end).utc().format(),
                            sheriffId: this.sheriffId,
                            locationId: this.location.id,
                            timezone: this.location.timezone
                        })
                    }
                }                
			}
			console.log(listOfDates)
			return listOfDates;
        }

        public takeoutConflicts(dayOffset, start, end){
            let numberOfConflicts=0
            const shifts: any[]=[];
            const shiftStart = moment.tz(start, this.location.timezone).format();
            const shiftEnd = moment.tz(end, this.location.timezone).format();
            
            for(const conflict of this.sheriffInfo.conflicts){
                if(conflict.dayOffset == dayOffset){
                    const conflictStart = moment(conflict.date).add(conflict.startTime).format()
                    const conflictEnd = moment(conflict.date).add(conflict.endTime).format()                   
                    if(shiftEnd>conflictEnd && shiftStart<conflictStart){
                        shifts.push({start:shiftStart, end: this.roundTime(conflictStart,true)});
                        shifts.push({start:this.roundTime(conflictEnd,false), end:shiftEnd});
                        numberOfConflicts++;
                    }else if(shiftEnd>conflictStart && shiftStart<conflictStart){
                        shifts.push({start:shiftStart, end:this.roundTime(conflictStart,true)});
                        numberOfConflicts++;
                    }else if(shiftEnd>conflictEnd && shiftStart<conflictEnd){
                        shifts.push({start:this.roundTime(conflictEnd,false), end:shiftEnd});
                        numberOfConflicts++;
                    }else if(shiftEnd<=conflictEnd && shiftStart>=conflictStart){                        
                        numberOfConflicts++;
                    }                    
                }
            }
            if(numberOfConflicts == 0){
                shifts.push({start:shiftStart, end:shiftEnd});
            }else if(numberOfConflicts >=2){
                return [];
            } 

            return shifts
        }

        public roundTime(time, floor){
            const minutes = moment(time).minutes()
            let minOffset = 0
            //if(minutes%15 == 0) return time

            if(minutes/15 >= 3) minOffset= floor? 45-minutes: 60-minutes;
            else if(minutes/15 >= 2) minOffset= floor? 30-minutes: 45-minutes;
            else if(minutes/15 >= 1) minOffset= floor? 15-minutes: 30-minutes;
            else minOffset= floor? 0-minutes: 15-minutes;

            //console.log(moment(time).add(minOffset,'minutes').format()) 
            return moment(time).add(minOffset,'minutes').format()           
        }

        public completeDate(offset, time){
            return moment(this.shiftRangeInfo.startDate).add(offset,'days').add(time);
        }

		public createShift() {
            const body = this.getListOfDates(this.selectedDays);
            if(!body || (body && body.length==0)) return
            const url = 'api/shift';
            this.$http.post(url, body )
                .then(response => {
                    if(response.data){
                        this.resetShiftWindowState();
                        this.closeShiftWindow;
                        this.$emit('change');
                    }
                }, err => {
                    const errMsg = err.response.data.error;
                    this.shiftErrorMsg = errMsg.slice(0,60) + (errMsg.length>60?' ...':'');
                    this.shiftErrorMsgDesc = errMsg;
                    this.shiftError = true;
                })
		}

		public updateShift() {
			const shiftDates = this.getListOfDates(this.selectedDays);
			console.log("updating")
			// const body = { }
			// const url = 'api/shift';
			// this.$http.put(url, body )
			//     .then(response => {
			//         if(response.data){
			//             this.resetShiftWindowState();
			//             this.closeShiftWindow;
			//             this.$emit('change');
			//         }
			//     }, err => {
			//         const errMsg = err.response.data.error;
			//         this.shiftErrorMsg = errMsg.slice(0,60) + (errMsg.length>60?' ...':'');
			//         this.shiftErrorMsgDesc = errMsg;
			//         this.shiftError = true;
			//     })

        }
           
        public timeFormat(value , event) {
			if(isNaN(Number(value.slice(-1))) && value.slice(-1) != ':') return value.slice(0,-1)
			if(value.length!=3 && value.slice(-1) == ':') return value.slice(0,-1);

			if(value.length==2 && event.data && value.slice(0,1)>=3 && (value.slice(-1)>=5 || value.slice(-1)==2)) return value.slice(0,-1);
			if(value.length==2 && event.data && value.slice(0,1)>=3 && (value.slice(-1)==0 || value.slice(-1)==3)) return '0'+ value.slice(0,1)+':'+value.slice(1,2)+'0';
			if(value.length==2 && event.data && value.slice(0,1)>=3 && (value.slice(-1)==1 || value.slice(-1)==4)) return '0'+ value.slice(0,1)+':'+value.slice(1,2)+'5';
			if(value.length==2 && event.data && value.slice(0,1)==2 && value.slice(-1)>=5) return value.slice(0,-1);
			if(value.length==2 && event.data && value.slice(0,1)==2 && (value.slice(-1)==4 || value.slice(-1)==4)) return '02:45';
			if(value.length==2 && event.data && value.slice(-1)!=0 && value.slice(-1)!=1 && value.slice(-1)!=3 && value.slice(-1)!=4) return value.slice(0,2)+':';
			if(value.length==2 && event.data) return '0'+value.slice(0,1)+':'+value.slice(1,2);
			if(value.length==3 && value.slice(-1)!=':' ) return value.slice(0,2)+':';
			if(value.length==4 && event.data && (value.slice(0,1)>0||value.slice(1,2)>1) && (value.slice(-1)>=5 || value.slice(-1)==2)) return value.slice(0,-1);
			if(value.length==4 && event.data && value.slice(0,1)>0 && (value.slice(-1)==0 || value.slice(-1)==3)) return value.slice(0,4)+'0';
			if(value.length==4 && event.data && value.slice(0,1)>0 && (value.slice(-1)==1 || value.slice(-1)==4)) return value.slice(0,4)+'5';
			if(value.length==4 && event.data && value.slice(0,1)==0 && value.slice(1,2)<2 && value.slice(-1)>=5 ) return value.slice(1,2)+value.slice(3,4)+':';
			if(value.length==5 && (value.slice(0,2)>=24 || value.slice(3,5)>=60)) return '';
			if(value.length==5 && value.slice(0,2)>=3 && (value.slice(3,4)==0 || value.slice(3,4)==3)) return value.slice(0,4)+'0';
			if(value.length==5 && value.slice(0,2)>=3 && (value.slice(3,4)==1 || value.slice(3,4)==4)) return value.slice(0,4)+'5';
			if(value.length==6 && value.slice(0,1)==0 && (value.slice(4,6)==0||value.slice(4,6)==15||value.slice(4,6)==30||value.slice(4,6)==45) && (value.slice(1,2)+value.slice(3,4))<24) return value.slice(1,2)+value.slice(3,4)+':'+value.slice(4,5)+value.slice(5,6);
		
			if(value.length>5) return value.slice(0,5);
			if(value.length==5 && ( isNaN(value.slice(0,2)) || isNaN(value.slice(3,5)) || value.slice(2,3)!=':') )return '';
			if(value.length==4 && ( isNaN(value.slice(0,2)) || isNaN(value.slice(3,4)) || value.slice(2,3)!=':') )return '';
			return value
		}

    }
</script>

<style scoped>   

    .card {
       border: white;
    }

    .custom-control-input{
        background-color: darkorange;
    }

</style>