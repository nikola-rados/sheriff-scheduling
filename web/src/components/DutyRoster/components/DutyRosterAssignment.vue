<template>
    <div> 
        <b-row v-if="assignment.EFTnumber>0" 
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



        <!-- <b-modal v-model="showDutyDetails" id="bv-modal-duty-details" header-class="bg-primary text-light">
			<template v-slot:modal-title>
				<h2 class="mb-0 text-light"> Edit Assignment </h2>
			</template>

			<b-card v-if="isDutyDataMounted" no-body style="font-size: 14px;user-select: none;" >

				<b-card id="DutyError" no-body>
					<h2 v-if="dutyError" class="mx-1 mt-2"
						><b-badge v-b-tooltip.hover
							:title="dutyErrorMsg"
							variant="danger"> {{dutyErrorMsg|truncate(40)}}
							<b-icon class="ml-3"
								icon = x-square-fill
								@click="dutyError = false"
					/></b-badge></h2>
				</b-card>

				<b-row class="mx-1 my-0 p-0">
					<b-form-group class="mr-1" style="width: 12rem">
						<label class="h6 m-0 p-0">Name<span class="text-danger">*</span></label>
						<b-form-input 
						size="sm"
							v-model="assignment.name" 
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
							v-model="assignment.type"
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
						<label class="h6 my-0 ml-1 p-0">Assignment Sub caegory<span class="text-danger">*</span></label>
						<b-form-select 
							size="sm"
							:disabled="!isSubTypeDataReady"
							v-model="assignment.lookupCodeId"
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
							@context="startDatePicked"
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
						@click="closeAssignmentWindow()"
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
					@click="closeAssignmentWindow()"
					>
					&times;</b-button>
			</template>
		</b-modal> -->

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

    import { locationInfoType } from '../../../types/common';
    import { dutyRangeInfoType, myTeamShiftInfoType} from '../../../types/DutyRoster';
    // import { shiftInfoType } from '../../types/ShiftSchedule';

    @Component
    export default class DutyRosterAssignment extends Vue {

        @Prop({required: true})
        assignment!: any;

        @commonState.State
        public location!: locationInfoType;

        @dutyState.State
        public dutyRangeInfo!: dutyRangeInfoType;


        // @dutyState.State
        // public shiftAvailabilityInfo!: myTeamShiftInfoType[];

        // @dutyState.Action
        // public UpdateShiftAvailabilityInfo!: (newShiftAvailabilityInfo: myTeamShiftInfoType[]) => void

        //isDutyDataMounted = false;
        dutyError = false;
        dutyErrorMsg = '';
        showDutyDetails = false;

        selectedStartTime = '';
		selectedEndTime = '';
		selectedStartDate = '';
		selectedEndDate = '';

        mounted()
        {
            //this.isDutyDataMounted = false;
            //console.log(this.assignment)           
        }

        public editAssignment(){
            console.log('edit')           
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
							//this.confirmedCloseAssignmentWindow();
							this.$emit('change');
					}
				}, err => {
					this.dutyErrorMsg = err.response.data.error;
					this.dutyError = true;
				})
		}
		
		// EFTnumber: Number(rosterInx),
        //                     totalEFT: dutyRostersForThisAssignment.length
		get getBorderRadius(){
			if(this.assignment.totalEFT<2) return '5px'
			else if(this.assignment.EFTnumber == 0) return'5px 5px 0 0'
			else if(this.assignment.EFTnumber == (this.assignment.totalEFT-1))	return'0 0 5px 5px'
			else return '0'
		}
		get getBorderBottom(){
			if(this.assignment.totalEFT<2) return '1px solid #BBBBBB'
			else if(this.assignment.EFTnumber == 0) return'0px solid #BBBBBB'
			else if(this.assignment.EFTnumber == (this.assignment.totalEFT-1))	return'1px solid #BBBBBB'
			else return '0px solid #BBBBBB'
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