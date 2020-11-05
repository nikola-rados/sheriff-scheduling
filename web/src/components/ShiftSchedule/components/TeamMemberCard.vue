<template>
    <div v-if="isDataMounted">    
        <b-row 
            style="width:100%; height:100%;" 
            bg-variant="white"            
            class="mx-2 my-1 p-1" 
           >
            <b-col cols="8">
                <b-row style="font-size:11px; line-height: 16px;"># {{sheriffInfo.badgeNumber}}</b-row>
                <b-row style="font-size:9px; line-height: 14px;">{{sheriffInfo.rank}}</b-row>
                <b-row 
                    style="font-size:12px; line-height: 16px; font-weight: bold; text-transform: Capitalize;" 
                    v-b-tooltip.hover.topleft                                
                    :title="fullName.length>14?fullName:''">
                        {{fullName|truncate(12)}}
                </b-row>
            </b-col>
            <b-col cols="4">
                <b-button 
                    style="max-height: 40px;" 
                    size="sm" 
                    variant="success" 
                    @click="AddShift()"><b-icon-plus/>
                </b-button>                    
            </b-col>
        </b-row>

        <b-modal v-model="showShiftDetails" id="bv-modal-shift-details" header-class="bg-primary text-light">
            <template v-slot:modal-title>
                <h2 v-if="editMode" class="mb-0 text-light"> Updating Shift - {{sheriffInfo.firstName}} {{sheriffInfo.lastName}} </h2>
                <h2 v-else-if="createMode" class="mb-0 text-light"> Creating Shift - {{sheriffInfo.firstName}} {{sheriffInfo.lastName}}</h2>
            </template>

            <b-card v-if="isShiftDataMounted" no-body style="font-size: 14px;user-select: none;" >

                <b-card id="ShiftError" no-body>
                    <h2 v-if="shiftError" class="mx-1 mt-2"
                        ><b-badge v-b-tooltip.hover
                            :title="shiftErrorMsgDesc"
                            variant="danger"> {{shiftErrorMsg}}
                            <b-icon class="ml-3"
                                icon = x-square-fill
                                @click="shiftError = false"
                    /></b-badge></h2>
                </b-card>              

                <b-row class="mx-1 my-0 p-0">
                    
                </b-row>

                <b-row class="mx-1 my-3">
                    <b-form-group class="bg-light">							
                        <b-row>
                            <label class="h6 ml-3 mr-5">Days<span class="text-danger">*</span></label>								
                            <b-form-checkbox
                                size="sm"
                                class="ml-auto mr-4 text-court"									
                                v-model="allDaysSelected"
                                @change="toggleAllDays">
                                    Select All
                            </b-form-checkbox>
                        </b-row>

                        <b-row>
                            <b-col
                            v-for="day in dayOptions"
                                    :key="day.diff"
                                    :value="day.diff"
                            
                            >
                                <!-- <user-location-summary v-if="day.conflict.away.length>0" class="mx-3" :homeLocation="teamMember.homeLocationNm" :awayJson="teamMember.awayLocation" :index="teamMember.badgeNumber"/> -->
                                <training-conflicts class="mx-2" v-if="day.conflicts.Training.length>0" :trainingConflicts="day.conflicts.Training" :index="sheriffId" :timezone="location.timezone?location.timezone:'UTC'"/>
                                <!-- <user-leave-summary class="mx-2" v-if="teamMember.leave.length>0" :leaveJson="teamMember.leave" :index="teamMember.badgeNumber" :timezone="teamMember.homeLocation?teamMember.homeLocation.timezone:'UTC'"/> -->
                            
                            </b-col>
                            
                        </b-row>


                        <b-form-checkbox-group
                            size="sm"			
                            v-model="selectedDays"								
                            :state = "selectedDayState?null:false">
                                <b-form-checkbox
                                    @change="weekdaysChanged"									
                                    :class="day.diff? 'ml-2 pl-3' :'ml-1 pl-4' +'align-middle'"
                                    v-for="day in dayOptions"
                                    :key="day.diff"
                                    :value="day.diff">
                                        {{day.name}}
                                </b-form-checkbox>
                        </b-form-checkbox-group>
                    </b-form-group>
                </b-row>

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
                        @click="closeShiftWindow()"
                ><b-icon-x font-scale="1.5" style="padding:0; vertical-align: middle; margin-right: 0.25rem;"></b-icon-x>Cancel</b-button>
                <b-button
                        variant="success"
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
    import TrainingConflicts from './TrainingConflicts.vue'
    import { dayOptionsInfoType, sheriffAvailabilityInfoType,shiftInfoType,shiftRangeInfoType } from '../../../types/ShiftSchedule';
    import moment from 'moment-timezone';
    import { locationInfoType } from '../../../types/common';

    @Component({
        components: {
            TrainingConflicts
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
                {name:'Sun', diff:0, conflicts:{Training: [], Leave: [], AwayLocation:[]}},
                {name:'Mon', diff:1, conflicts:{Training: [], Leave: [], AwayLocation:[]}},
                {name:'Tue', diff:2, conflicts:{Training: [], Leave: [], AwayLocation:[]}},
                {name:'Wed', diff:3, conflicts:{Training: [], Leave: [], AwayLocation:[]}},
                {name:'Thu', diff:4, conflicts:{Training: [], Leave: [], AwayLocation:[]}},
                {name:'Fri', diff:5, conflicts:{Training: [], Leave: [], AwayLocation:[]}},
                {name:'Sat', diff:6, conflicts:{Training: [], Leave: [], AwayLocation:[]}}
            ];        
            this.extractConflicts();
            console.log(this.dayOptions[4].conflicts.Training)
        }

        public extractConflicts() {    
                      
            for(const conflict of this.sheriffInfo.conflicts)
            {
                this.dayOptions[conflict.dayOffset].conflicts[conflict.type].push(conflict);                
            }
            
            Vue.nextTick(()=>{this.isDataMounted = true;})
                     
        }



        public weekdaysChanged(){
			Vue.nextTick(()=>{
				this.allDaysSelected = this.selectedDays.length==7? true: false
			})
		}

		public toggleAllDays(checked) {
			this.selectedDays = checked ? [0,1,2,3,4,5,6] : [];
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
        }
        
        public saveShift() {
			console.log('saving')

			let requiredError = false;
			
			if (!this.selectedStartTime) {
					this.startTimeState = false;
					requiredError = true;
			} else {
					this.startTimeState = true;
			}
			if (!this.selectedEndTime) {
					this.endTimeState = false;
					requiredError = true;
			} else {
					this.endTimeState = true;
			}
			if (this.selectedDays.length < 1) {
					this.selectedDayState = false;
					requiredError = true;
			} else {
					this.selectedDayState = true;
			}

			if (!requiredError) {
					if (this.editMode) this.updateShift();
					if (this.createMode) this.createShift();
			} else {
					console.log('Error required')
			}
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

		public createShift() {
			const shiftDates = this.getListOfDates(this.selectedDays);

			const body = { }
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
        
        public getListOfDates(days){
            const listOfDates: string[] = [];
			const firstDayOfWeek = this.shiftRangeInfo.startDate;      
			for(const day of days) {
				const date = moment(firstDayOfWeek).add(day, 'days').format().substring(0,10)         
				listOfDates.push(date)                
			}
			// console.log(listOfDates)
			return listOfDates;
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
        border:2px solid rgb(124, 136, 136);
    }

    .badge {
        width: 0.75rem;
        height: 0.9rem;
        margin:0.5rem .04rem 0 .04rem;
        padding: 0.15rem 0 0 0;
        border: solid 1px rgb(53, 56, 53); 
        border-radius: 4px;
        /* background-image: linear-gradient(to bottom right, rgb(12, 120, 170),rgb(243, 232, 232), white);*/        
    }

</style>