<template>
	<div>
		<header variant="primary">
			<b-navbar toggleable="lg" class=" m-0 p-0 navbar navbar-expand-lg navbar-dark">
				<b-navbar-nav class="custom-navbar">
					<b-button style="max-height: 40px;" size="sm" variant="secondary" @click="previousDateRange" class="my-2"><b-icon-chevron-left /></b-button>
						<b-form-datepicker
								class="my-2 p-1"
								size="sm"
								style="max-height: 40px;"
								v-model="selectedDate"
								@context="dateChanged"
								button-only
								locale="en-US">
						</b-form-datepicker>
					<b-button style="max-height: 40px;" size="sm" variant="secondary" @click="nextDateRange" class="my-2"><b-icon-chevron-right/></b-button>
				</b-navbar-nav>

				<b-navbar-nav class="mr-5">
					<b-nav-form>
						<b-button style="max-height: 40px;" size="sm" variant="success" @click="AddShift()" class="my-2"><b-icon-plus/>Add Shift</b-button>
					</b-nav-form>
				</b-navbar-nav>
			</b-navbar>

			<b-modal v-model="showShiftDetails" id="bv-modal-shift-details" header-class="bg-primary text-light">
				<template v-slot:modal-title>
					<h2 v-if="editMode" class="mb-0 text-light"> Updating Shift </h2>
					<h2 v-else-if="createMode" class="mb-0 text-light"> Creating Shift </h2>
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
						<b-form-group class="mr-1" style="width: 12rem">
							<label class="h6 m-0 p-0">Type<span class="text-danger">*</span></label>
							<b-form-select 
								size="sm"
								@change="loadSubTypes"
								v-model="shift.type"
								:state = "selectedTypeState?null:false">
									<b-form-select-option
										v-for="type in shiftTypeOptions"
										:key="type.name"
										:value="type">
												{{type.label}}
									</b-form-select-option>
							</b-form-select>
						</b-form-group>
						<b-form-group class="ml-4" style="width: 14.35rem">
							<label class="h6 my-0 ml-1 p-0">Anticipated Assignment<span class="text-danger">*</span></label>
							<b-form-select 
								size="sm"
								:disabled="!isSubTypeDataReady"
								v-model="shift.subType"
								:state = "selectedSubTypeState?null:false">
									<b-form-select-option
										v-for="subType in shiftSubTypeOptions"
										:key="subType.id"
										:value="subType.code">
												{{subType.code}}
									</b-form-select-option>
							</b-form-select>
						</b-form-group>
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
					
						<b-form-group class="ml-5" style="width: 7rem">
							<label class=" h6 m-0 p-0">Number of FTEs<span class="text-danger">*</span></label>
							<b-form-input
								v-model="numberOfFTEs"
								size="sm"
								type="number"
								:state = "numberOfFTEsState?null:false"
								min="1">
							</b-form-input>
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
		</header>
	</div>
</template>

<script lang="ts">
 
	import { Component, Vue } from 'vue-property-decorator';
	import { namespace } from "vuex-class";
	import moment from 'moment-timezone';
	import "@store/modules/ShiftScheduleInformation";
	const shiftState = namespace("ShiftScheduleInformation");
	import "@store/modules/CommonInformation";
	const commonState = namespace("CommonInformation");
	import { shiftInfoType, shiftRangeInfoType, shiftSubTypeInfoType } from '../../../types/ShiftSchedule';
	import { locationInfoType } from '../../../types/common';
 
	@Component
	export default class ScheduleHeader extends Vue {

		@commonState.State
		public location!: locationInfoType;

		@shiftState.State
		public shiftRangeInfo!: shiftRangeInfoType;

		@shiftState.Action
		public UpdateShiftRangeInfo!: (newShiftRangeInfo: shiftRangeInfoType) => void

		selectedDate = '';
		selectedStartTime = '';
		selectedEndTime = '';
		showShiftDetails = false;
		isShiftDataMounted = false;
		isSubTypeDataReady = false;
		createMode = false;
		editMode = false;
		shift = {} as shiftInfoType;
		numberOfFTEs = 1;

		selectedTypeState = true;
		selectedSubTypeState = true;
		numberOfFTEsState = true;
		startTimeState = true;
		endTimeState = true;
		selectedDayState = true;

		selectedDays: number[] = [];
		allDaysSelected = false;

		dayOptions = [
			{name:'Sun', diff:0},
			{name:'Mon', diff:1},
			{name:'Tue', diff:2},
			{name:'Wed', diff:3},
			{name:'Thu', diff:4},
			{name:'Fri', diff:5},
			{name:'Sat', diff:6},
		]

		shiftTypeOptions = [
			{name:'CourtRole', label:'Court Assignment'},
			{name:'JailRole', label:'Jail Assignment'},
			{name:'EscortRun', label:'Escort Assignment'},
			{name:'OtherAssignment', label:'Other Assignment'}
		]

		shiftSubTypeOptions = [] as shiftSubTypeInfoType[];

		shiftError = false;
		shiftErrorMsg = '';
		shiftErrorMsgDesc = '';

		mounted() {
			console.log('mounted')
			this.selectedDate = moment().format().substring(0,10);			
			this.loadNewDateRange();
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
			this.isSubTypeDataReady = false;
			this.isShiftDataMounted = true;
			this.showShiftDetails = true;
		}

		public loadSubTypes(type) {
			console.log(type);

			Vue.nextTick(()=>{
				this.isSubTypeDataReady = false;
				const url = 'api/managetypes?codeType='+ type.name +'&locationId='+this.location.id+'&showExpired=false';
				//console.log(url)
				this.$http.get(url)
					.then(response => {
							if(response.data){
									console.log(response.data)
									this.extractSubTypes(response.data);
							}
					})
			});
		}

		public extractSubTypes(subTypesJson) {
			this.shiftSubTypeOptions = [];
			for(const subTypeJson of subTypesJson) {
				const subType = {} as shiftSubTypeInfoType;
				subType.id = subTypeJson.id;
				subType.code = subTypeJson.code;
				this.shiftSubTypeOptions.push(subType)
			}
			this.isSubTypeDataReady = true;
		}

		public saveShift() {
			console.log('saving')

			let requiredError = false;

			if (!this.shift.type) {
					this.selectedTypeState = false;
					requiredError = true;
			} else {
					this.selectedTypeState = true;
			}
			if (!this.shift.subType) {
					this.selectedSubTypeState = false;
					requiredError = true;
			} else {
					this.selectedSubTypeState = true;
			}
			if (!this.numberOfFTEs) {
					this.numberOfFTEsState = false;
					requiredError = true;
			} else {
					this.numberOfFTEsState = true;
			}
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
				this.selectedTypeState = true;
				this.selectedSubTypeState = true;
				this.selectedDayState = true;
				this.numberOfFTEsState = true;
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
			const firstDayOfWeek = moment(this.selectedDate).startOf('week').format();      
			for(const day of days) {
				const date = moment(firstDayOfWeek).add(day, 'days').format().substring(0,10)         
				listOfDates.push(date)                
			}
			console.log(listOfDates)
			return listOfDates;
		}

		public dateChanged() {
			console.log('date changed')
			this.loadNewDateRange();
		}

		public nextDateRange() {			
			this.selectedDate = moment(this.selectedDate).add(7, 'days').format().substring(0,10);
			this.loadNewDateRange(); 
		}

		public previousDateRange() {
			this.selectedDate = moment(this.selectedDate).subtract(7, 'days').format().substring(0,10);
			this.loadNewDateRange();
		}

		public loadNewDateRange() {
			const firstDayOfWeek = moment(this.selectedDate).startOf('week').format()
			const lastDayOfWeek = moment(this.selectedDate).endOf('week').format();
			const dateRange = {startDate: firstDayOfWeek.substring(0,10), endDate: lastDayOfWeek.substring(0,10)}
			console.log(dateRange)
			this.UpdateShiftRangeInfo(dateRange);
			this.$emit('change'); 
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

	.custom-navbar {
			float:none;
			margin:0 auto;
			display: block;
			text-align: center;
	}

	.custom-navbar > li {
			display: inline-block;
			float:none;
	}

</style>