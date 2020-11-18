<template>
    <div> 
        <b-row v-if="assignment.FTEnumber>0" 
			:style="{
				borderTop: '0px solid #BBBBBB',
				borderBottom: getBorderBottom,
				height:'2.925rem',
				backgroundColor:'#EEEEEE',
				borderRadius:getBorderRadius,
				margin:'0rem 0.1rem 0 0.1rem'}">
        </b-row>
        <b-row v-else :style="{
				borderTop: '1px solid #BBBBBB',
				borderBottom: getBorderBottom,
				height:'2.785rem',
				backgroundColor:'#EEEEEE',
				borderRadius:getBorderRadius,
				margin:'0.15rem 0.1rem 0 0.1rem'}" > 
            <b-col cols="10" class="m-0 p-0" @click="editAssignment()">
                                   
                <b-row                                  
                    style="text-transform: capitalize;" 
                    class="h6 p-0 mt-2 mb-0 ml-1">
                    <div 
                        v-b-tooltip.hover                            
                        :title="assignment.name.length>10? assignment.name:''"> 
                            {{assignment.name|truncate(10)}} 
                    </div>
                </b-row>
                <b-row v-if="dutyError" >
					<h6 class="ml-3 my-0 p-0"
						><b-badge v-b-tooltip.hover
							:title="dutyErrorMsg"
							variant="danger"> {{dutyErrorMsg|truncate(15)}}
							<b-icon class="ml-2"
								icon = x-square-fill
								@click="dutyError = false"
					/></b-badge></h6>
				</b-row>     
                <b-row v-else class="h7 p-0 m-0 ml-1">
                    <div 
                        v-b-tooltip.hover                            
                        :title="assignment.code.length>8? assignment.code:''"> 
                        <div :style="{float:'left', color:assignment.type.colorCode}"> ({{assignment.type.name}}</div>-{{assignment.code|truncate(8)}}) 
                    </div>
                </b-row>
            </b-col>
            <b-col cols="2" class="m-0 p-0"> 
                <b-button
                    :variant="assignment.type.name"
                    style="padding:0; height:1.2rem; width:1.2rem; margin:.75rem 0" 
                    @click="addDuty();"
                    size="sm"> 
                        <b-icon-plus font-scale="1" style="transform:translate(0,-3px);"/></b-button>
            </b-col>
        </b-row>

		<b-modal v-model="showEditAssignmentDetails" id="bv-modal-edit-assignment-details" header-class="bg-primary text-light">
			<template v-slot:modal-title>
				<h2 class="mb-0 text-light"> Editing Assignment </h2>
			</template>

			<b-card v-if="isAssignmentDataMounted" no-body style="font-size: 14px;user-select: none;" >

				<b-card id="AssignmentError" no-body>
					<h2 v-if="assignmentError" class="mx-1 mt-2"
						><b-badge v-b-tooltip.hover
							:title="assignmentErrorMsgDesc"
							variant="danger"> {{assignmentErrorMsg}}
							<b-icon class="ml-3"
								icon = x-square-fill
								@click="assignmentError = false"
					/></b-badge></h2>
				</b-card>

				<b-row class="mx-1 my-0 p-0">
					<b-form-group class="mr-1" style="width: 12rem">
						<label class="h6 m-0 p-0">Name<span class="text-danger">*</span></label>
						<b-form-input 
						size="sm"
							v-model="assignmentToEdit.name" 
							placeholder="Enter Name" 
							:state = "nameState?null:false">
						</b-form-input>
					</b-form-group>
					<b-form-group class="my-auto ml-auto" style="width: 8.6rem">	
						<b-form-checkbox
							size="sm"									
							v-model="nonReoccuring"
							@change="toggleReoccurring">
								Non Reoccuring
						</b-form-checkbox>						
					</b-form-group>
				</b-row>              

				<b-row class="mx-1 my-0 p-0">
					<b-form-group class="mr-1" style="width: 12rem">
						<label class="h6 m-0 p-0">Assignment Category<span class="text-danger">*</span></label>
						<b-form-select 
							size="sm"
							@change="loadSubTypes"
							v-model="assignmentToEditType"
							:state = "selectedTypeState?null:false">
								<b-form-select-option
									v-for="type in assignmentTypeOptions"
									:key="type.name"
									:value="type">
											{{type.label}}
								</b-form-select-option>
						</b-form-select>
					</b-form-group>
					<b-form-group class="ml-4" style="width: 14.35rem">
						<label class="h6 my-0 ml-1 p-0">Assignment Sub category<span class="text-danger">*</span></label>
						<b-form-select 
							size="sm"
							:disabled="!isSubTypeDataReady"
							v-model="assignmentToEditSubType.id"
							:state = "selectedSubTypeState?null:false">
								<b-form-select-option
									v-for="subType in assignmentSubTypeOptions"
									:key="subType.id"
									:value="subType.id">
											{{subType.code}}
								</b-form-select-option>
						</b-form-select>
					</b-form-group>
				</b-row>

				<b-row v-if="nonReoccuring" class="mx-1 my-1 p-0">
					<b-form-group class="mr-1" style="width: 12rem">					
						<label class="h6 m-0 p-0"> From <span class="text-danger">*</span></label>
						<b-form-datepicker
							tabindex="2"
							class="mb-1"
							size="sm"
							v-model="selectedStartDate"
							placeholder="Start Date*"
							:state = "startDateState?null:false"
							:date-format-options="{ year: 'numeric', month: 'short', day: '2-digit' }"
							locale="en-US">
						</b-form-datepicker>
					</b-form-group>
					<b-form-group class="ml-4" style="width: 14.35rem">
						<label class="h6 m-0 p-0"> To<span class="text-danger">*</span></label>
						<b-form-datepicker
							tabindex="3"
							class="mb-1 mt-0 pt-0"
							size="sm"
							v-model="selectedEndDate"
							placeholder="End Date*"
							:state = "endDateState?null:false"                                    
							:date-format-options="{ year: 'numeric', month: 'short', day: '2-digit' }"
							locale="en-US">
						</b-form-datepicker>
					</b-form-group>
				</b-row>

				<b-row class="mx-1 mt-3 mb-0">
					<b-form-group class="bg-light">							
						<b-row>
							<label class="h6 ml-3 mr-5">Days<span class="text-danger">*</span></label>								
							<b-form-checkbox
								size="sm"
								class="ml-auto mr-4 text-jail"									
								v-model="weekDaysSelected"
								@change="toggleWeekDays">
									Select Week Days
							</b-form-checkbox>    
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

				<b-row class="mx-auto my-0 p-0">
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

					<b-form-group style="width: 7rem;">
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
						size="sm"
						variant="secondary"
						@click="closeEditAssignmentWindow()"
				><b-icon-x style="padding:0; vertical-align: middle; margin-right: 0.25rem;"></b-icon-x>Cancel</b-button>
				<b-button
						size="sm"
						variant="success"
						@click="saveAssignment()"
				><b-icon-check2 style="padding:0; vertical-align: middle; margin-right: 0.25rem;"></b-icon-check2>Save</b-button>
			</template>
			<template v-slot:modal-header-close>
				<b-button
					variant="outline-primary"
					class="text-light closeButton"
					@click="closeEditAssignmentWindow()"
					>
					&times;</b-button>
			</template>
		</b-modal>

		<b-modal v-model="showEditCancelWarning" id="bv-modal-edit-assignment-cancel-warning" header-class="bg-warning text-light">            
			<template v-slot:modal-title>
				<h2 class="mb-0 text-light"> Unsaved Assignment </h2>                                 
			</template>
			<p>Are you sure you want to cancel without saving your changes?</p>
			<template v-slot:modal-footer>
				<b-button variant="secondary" @click="$bvModal.hide('bv-modal-edit-assignment-cancel-warning')"                   
				>No</b-button>
				<b-button variant="success" @click="confirmedCloseEditAssignmentWindow()"
				>Yes</b-button>
			</template>            
			<template v-slot:modal-header-close>                 
				<b-button variant="outline-warning" class="text-light closeButton" @click="$bvModal.hide('bv-modal-edit-assignment-cancel-warning')"
				>&times;</b-button>
			</template>
		</b-modal>
    </div>
</template>

<script lang="ts">
    import { Component, Vue, Prop } from 'vue-property-decorator';

    import moment from 'moment-timezone';

    import { namespace } from "vuex-class";   
    import "@store/modules/CommonInformation";
    const commonState = namespace("CommonInformation");
    import "@store/modules/DutyRosterInformation";   
	const dutyState = namespace("DutyRosterInformation");
	import * as _ from 'underscore';
    import { locationInfoType } from '../../../types/common';
    import { assignmentInfoType, assignmentSubTypeInfoType, dutyRangeInfoType, myTeamShiftInfoType} from '../../../types/DutyRoster';

    @Component
    export default class DutyRosterAssignment extends Vue {

        @Prop({required: true})
        assignment!: any;

        @commonState.State
        public location!: locationInfoType;

        @dutyState.State
        public dutyRangeInfo!: dutyRangeInfoType;

        dutyError = false;
        dutyErrorMsg = '';
        showDutyDetails = false;

		selectedStartTime = '';
		selectedEndTime = '';
		selectedStartDate = '';
		selectedEndDate = '';
		assignmentToEditType = {name:'', label:''};
		assignmentToEditSubType = {} as assignmentSubTypeInfoType;
		selectedDays: number[] = [];
		showEditAssignmentDetails = false;
		showEditCancelWarning = false;
		isAssignmentDataMounted = false;
		initialLoad = false;
		isSubTypeDataReady = false;
		nonReoccuring = false;

		nameState = true;
		selectedTypeState = true;
		selectedSubTypeState = true;
		startDateState = true;
		endDateState = true;	
		startTimeState = true;
		endTimeState = true;
		selectedDayState = true;		
		allDaysSelected = false;
		weekDaysSelected = false;		

		dayOptions = [
			{name:'Sun', diff:0},
			{name:'Mon', diff:1},
			{name:'Tue', diff:2},
			{name:'Wed', diff:3},
			{name:'Thu', diff:4},
			{name:'Fri', diff:5},
			{name:'Sat', diff:6},
		]

		assignmentTypeOptions = [
			{name:'CourtRoom', label:'Court Room'},
			{name:'CourtRole', label:'Court Assignment'},
			{name:'JailRole', label:'Jail Assignment'},
			{name:'EscortRun', label:'Escort Assignment'},
			{name:'OtherAssignment', label:'Other Assignment'}
		]

		assignmentSubTypeOptions = [] as assignmentSubTypeInfoType[];
		assignmentError = false;
		assignmentErrorMsg = '';
		assignmentErrorMsgDesc = '';

		assignmentToEdit = {} as assignmentInfoType;
		originalAssignmentToEdit = {} as assignmentInfoType;

        mounted()
        {
            //this.isDutyDataMounted = false;
			//console.log(this.assignment)
        }

        public editAssignment(){
			console.log('edit')
            console.log('edit')           
			console.log('edit')
			console.log(this.assignment);
			this.isSubTypeDataReady = false;
			this.initialLoad = true; 
			this.loadAssignmentDetails();					           
		}
		
		public loadAssignmentDetails() {

			const assignmentInfo = this.assignment.assignmentDetail;
			this.originalAssignmentToEdit.id = this.assignmentToEdit.id = assignmentInfo.id;
			this.originalAssignmentToEdit.name = this.assignmentToEdit.name = assignmentInfo.name;			
			this.originalAssignmentToEdit.start = this.selectedStartTime = assignmentInfo.start;
			this.originalAssignmentToEdit.end = this.selectedEndTime = assignmentInfo.end;
			this.originalAssignmentToEdit.locationId = this.assignmentToEdit.locationId = assignmentInfo.locationId;
			this.originalAssignmentToEdit.timezone = this.assignmentToEdit.timezone = assignmentInfo.timezone;
			if (assignmentInfo.adhocStartDate) {
				this.nonReoccuring = true;
				this.selectedStartDate = assignmentInfo.adhocStartDate;
				this.selectedEndDate = assignmentInfo.adhocEndDate;
			} else {
				this.nonReoccuring = false;
				this.selectedStartDate = '';
				this.selectedEndDate = '';
			}
			this.originalAssignmentToEdit.reoccuring = !this.nonReoccuring;
			this.originalAssignmentToEdit.adhocStartDate = moment.tz(this.selectedStartDate, this.originalAssignmentToEdit.timezone).utc().format();
			this.originalAssignmentToEdit.adhocEndDate = moment.tz(this.selectedEndDate, this.originalAssignmentToEdit.timezone).utc().format();
			this.originalAssignmentToEdit.sunday = assignmentInfo.sunday;
			this.originalAssignmentToEdit.monday = assignmentInfo.monday;
			this.originalAssignmentToEdit.tuesday = assignmentInfo.tuesday;
			this.originalAssignmentToEdit.wednesday = assignmentInfo.wednesday;
			this.originalAssignmentToEdit.thursday = assignmentInfo.thursday;
			this.originalAssignmentToEdit.friday = assignmentInfo.friday;
			this.originalAssignmentToEdit.saturday = assignmentInfo.saturday;

			if (assignmentInfo.sunday) this.selectedDays.push(0)
			if (assignmentInfo.monday) this.selectedDays.push(1)
			if (assignmentInfo.tuesday) this.selectedDays.push(2)
			if (assignmentInfo.wednesday) this.selectedDays.push(3)
			if (assignmentInfo.thursday) this.selectedDays.push(4)
			if (assignmentInfo.friday) this.selectedDays.push(5)
			if (assignmentInfo.saturday) this.selectedDays.push(6)
			
			const assignmentType = assignmentInfo.lookupCode.type;
			this.assignmentToEditType = this.assignmentTypeOptions.filter(option => {if (option.name == assignmentType) return true})[0];			
			this.assignmentToEdit.type = assignmentType;
			this.originalAssignmentToEdit.type = this.assignmentToEdit.type;
			this.loadSubTypes(this.assignmentToEditType);
		}

		public weekdaysChanged(){
			Vue.nextTick(()=>{
				this.allDaysSelected = this.selectedDays.length==7? true: false
				this.weekDaysSelected = this.selectedDays.includes(1) && this.selectedDays.includes(2) 
										&& this.selectedDays.includes(3) && this.selectedDays.includes(4) 
										&& this.selectedDays.includes(5);
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

		public toggleReoccurring(checked) {
			this.nonReoccuring = true;
		}
		
		public loadSubTypes(type) {
			Vue.nextTick(()=>{
				this.isSubTypeDataReady = false;
				const url = 'api/managetypes?codeType='+ type.name +'&locationId='+this.location.id+'&showExpired=false';
				this.$http.get(url)
					.then(response => {
						if(response.data){
							this.extractSubTypes(response.data);
						}
					})
			});
		}

		public extractSubTypes(subTypesJson) {
			this.assignmentSubTypeOptions = [];
			for(const subTypeJson of subTypesJson) {
				const subType = {} as assignmentSubTypeInfoType;
				subType.id = subTypeJson.id;
				subType.code = subTypeJson.code;
				this.assignmentSubTypeOptions.push(subType)
			}
			this.isSubTypeDataReady = true;
			
			if (this.initialLoad) {
				const assignmentInfo = this.assignment.assignmentDetail;
				this.assignmentToEditSubType = {id: assignmentInfo.lookupCode.id, code: assignmentInfo.lookupCode.code}
				this.originalAssignmentToEdit.lookupCodeId = this.assignmentToEdit.lookupCodeId = assignmentInfo.lookupCode.id;
				this.isAssignmentDataMounted = true;
				this.showEditAssignmentDetails = true;
				this.initialLoad = false;
			}		
			
		}

		public saveAssignment() {
			console.log('saving')
			let requiredError = false;
			if (!this.assignmentToEdit.name) {
				this.nameState = false;
				requiredError = true;
			} else {
				this.nameState = true;
			}
			if (!this.assignmentToEditType) {
				this.selectedTypeState = false;
				requiredError = true;
			} else {
				this.selectedTypeState = true;
			}
			if (!this.assignmentToEditSubType.id) {
				this.selectedSubTypeState = false;
				requiredError = true;
			} else {
				this.selectedSubTypeState = true;
			}
			if (this.nonReoccuring && !this.selectedStartDate) {
				this.startDateState = false;
				requiredError = true;
			} else {
				this.startDateState = true;
			}
			if (this.nonReoccuring && !this.selectedEndDate) {
				this.endDateState = false;
				requiredError = true;
			} else {				
				this.endDateState = true;
			}
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
			if (this.selectedDays.length < 1) {
				this.selectedDayState = false;
				requiredError = true;
			} else {
				this.selectedDayState = true;
			}
			if (!requiredError) {					
					this.saveAssignmentChanges();
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
		
		public isChanged(){
			//TODO: make changes for edit
			this.readEditedAssignment();
			console.log(this.assignmentToEdit)
			console.log(this.originalAssignmentToEdit)

			return !_.isEqual(this.originalAssignmentToEdit, this.assignmentToEdit)		
		
			

			// if( this.assignment.name ||
			// 	this.assignment.type ||
			// 	this.selectedStartTime || this.selectedEndTime ||
            //     this.nonReoccuring || this.selectedDays.length >0) return true;
            // return false;           
		}

		public closeEditAssignmentWindow(){
			if(this.isChanged())
                this.showEditCancelWarning = true;
            else
                this.confirmedCloseEditAssignmentWindow();
		}

		public confirmedCloseEditAssignmentWindow(){
            this.resetAssignmentWindowState();
            this.showEditCancelWarning = false;
			this.showEditAssignmentDetails = false;
        }

		public resetAssignmentWindowState() {
			this.assignmentToEdit = {} as assignmentInfoType;
			this.ClearFormState();
		}

		public ClearFormState(){
			this.allDaysSelected = false;
			this.weekDaysSelected = false;
			this.assignmentToEdit = {} as assignmentInfoType;
            this.selectedStartTime ='';
			this.selectedEndTime = '';
			this.selectedStartDate = '';
			this.selectedEndDate = '';
			this.assignmentToEditType = {name:'', label:''}
			this.assignmentToEditSubType = {} as assignmentSubTypeInfoType;
			this.selectedDays = [] ;
			this.nameState = true;	
			this.selectedTypeState = true;
			this.selectedSubTypeState = true;
			this.selectedDayState = true;
			this.startTimeState = true;
			this.endTimeState = true;
			this.startDateState = true;
			this.endDateState = true;
			this.assignmentError = false;
            this.assignmentErrorMsg = '';
			this.assignmentErrorMsgDesc = '';
			this.nonReoccuring = false;
		}

		public readEditedAssignment() {

			this.assignmentToEdit.sunday = this.selectedDays.includes(0)
			this.assignmentToEdit.monday = this.selectedDays.includes(1)
			this.assignmentToEdit.tuesday = this.selectedDays.includes(2)
			this.assignmentToEdit.wednesday = this.selectedDays.includes(3)
			this.assignmentToEdit.thursday = this.selectedDays.includes(4)
			this.assignmentToEdit.friday = this.selectedDays.includes(5)
			this.assignmentToEdit.saturday = this.selectedDays.includes(6);

			if (this.nonReoccuring) {
				this.assignmentToEdit.reoccuring = false;
				this.assignmentToEdit.adhocStartDate = moment.tz(this.selectedStartDate, this.location.timezone).utc().format();
				this.assignmentToEdit.adhocEndDate = moment.tz(this.selectedEndDate, this.location.timezone).utc().format();
			} else {
				this.assignmentToEdit.reoccuring = true;
				this.assignmentToEdit.adhocStartDate = null;
				this.assignmentToEdit.adhocEndDate = null;
			}

			this.assignmentToEdit.start = this.selectedStartTime;
			this.assignmentToEdit.end = this.selectedEndTime;	
		}


		public saveAssignmentChanges() {

			this.readEditedAssignment();

			// this.assignmentToEdit.sunday = this.selectedDays.includes(0)
			// this.assignmentToEdit.monday = this.selectedDays.includes(1)
			// this.assignmentToEdit.tuesday = this.selectedDays.includes(2)
			// this.assignmentToEdit.wednesday = this.selectedDays.includes(3)
			// this.assignmentToEdit.thursday = this.selectedDays.includes(4)
			// this.assignmentToEdit.friday = this.selectedDays.includes(5)
			// this.assignmentToEdit.saturday = this.selectedDays.includes(6);

			// if (this.nonReoccuring) {
			// 	this.assignmentToEdit.adhocStartDate = moment.tz(this.selectedStartDate, this.location.timezone).utc().format();
			// 	this.assignmentToEdit.adhocEndDate = moment.tz(this.selectedEndDate, this.location.timezone).utc().format();
			// } else {
			// 	this.assignmentToEdit.adhocStartDate = null;
			// 	this.assignmentToEdit.adhocEndDate = null;
			// }

			// this.assignmentToEdit.start = this.selectedStartTime;
			// this.assignmentToEdit.end = this.selectedEndTime;	

			const body = this.assignmentToEdit;

			console.log(body)
			const url = 'api/assignment';
			this.$http.put(url, body )
				.then(response => {
					if(response.data){
							this.confirmedCloseEditAssignmentWindow();
							this.$emit('change');
					}
				}, err => {
					const errMsg = err.response.data.error;
					this.assignmentErrorMsg = errMsg.slice(0,60) + (errMsg.length>60?' ...':'');
					this.assignmentErrorMsgDesc = errMsg;
					this.assignmentError = true;
				})
		}


        public addDuty(){
            console.log('add')
            console.log(this.assignment)
            this.createDuty();
        }

        public createDuty(){
            this.dutyError = false;
            const date = this.dutyRangeInfo.startDate.substring(0,10);
            const startDate = moment.tz(date+'T'+this.assignment.assignmentDetail.start, this.location.timezone).utc().format();
            const endDate = moment.tz(date+'T'+this.assignment.assignmentDetail.end, this.location.timezone).utc().format();
            const body = {
                startDate: startDate, 
                endDate: endDate,
                locationId: this.location.id,
                assignmentId: this.assignment.assignmentDetail.id,
                timezone: this.location.timezone,
                concurrencyToken: 0
            }

			console.log(body)
			const url = 'api/dutyroster';
			this.$http.post(url, body )
				.then(response => {
					if(response.data){
							this.$emit('change');
					}
				}, err => {
					this.dutyErrorMsg = err.response.data.error;
					this.dutyError = true;
				})
		}		
		
		get getBorderRadius(){
			if(this.assignment.totalFTE<2) return '5px'
			else if(this.assignment.FTEnumber == 0) return'5px 5px 0 0'
			else if(this.assignment.FTEnumber == (this.assignment.totalFTE-1))	return'0 0 5px 5px'
			else return '0'
		}

		get getBorderBottom(){
			if(this.assignment.totalFTE<2) return '1px solid #BBBBBB'
			else if(this.assignment.FTEnumber == 0) return'0px solid #BBBBBB'
			else if(this.assignment.FTEnumber == (this.assignment.totalFTE-1))	return'1px solid #BBBBBB'
			else return '0px solid #BBBBBB'
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

        // public getDutyDetails(){
            
        //     this.selectedStartTime = this.assignment.assignmentDetail.start;
        //     this.selectedEndTime = this.assignment.assignmentDetail.end;
        //     this.selectedStartDate = '';
        //     this.selectedEndDate = '';
        // }
    }
</script>

<style scoped>   

    .card {
        border: white;
    }

    .gauge {
        direction:rtl;
        position: sticky;
    }

    .grid24 {        
        display:grid;
        grid-template-columns: repeat(24, 4.1666%);
        grid-template-rows: 2.56rem;
        inline-size: 100%;
        font-size: 10px;         
    }

    .grid24 > * {      
        padding: .75rem 0;
        border: 1px dotted rgb(185, 143, 143);
    }

</style>