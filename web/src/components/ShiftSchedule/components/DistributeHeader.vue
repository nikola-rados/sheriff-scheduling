<template>
	<div v-on:edit-shifts="EditShifts()">
		<header variant="primary">
			<b-navbar toggleable="lg" class=" m-0 p-0 navbar navbar-expand-lg navbar-dark">
				<b-navbar-nav>
					<h3 style="width:15rem; margin-bottom: 0px;" class="text-white ml-2">Distribute Schedule</h3>
				</b-navbar-nav>

				<b-navbar-nav class="custom-navbar">
					<b-button style="max-height: 40px;" size="sm" variant="secondary" @click="previousDateRange" class="my-2"><b-icon-chevron-left /></b-button>
					<b-form-datepicker
							class="my-2 p-1"
							size="sm"
							style="max-height: 40px;"
							v-model="selectedDate"
							@context="dateChanged"
							@shown = "datePickerOpened = true"
							@hidden = "datePickerOpened = false"
							button-only
							today-button
							close-button
							locale="en-US">
					</b-form-datepicker>
					<b-button style="max-height: 40px;" size="sm" variant="secondary" @click="nextDateRange" class="my-2"><b-icon-chevron-right/></b-button>
				</b-navbar-nav>

				<b-navbar-nav class="mr-2">
					<b-input-group v-if="teamDataReady" class="mr-2 mt-1" style="height: 40px">
						<b-input-group-prepend is-text>
							<b-icon icon="globe"></b-icon>
						</b-input-group-prepend>
						<b-form-select
							style="height: 100%;"
							v-model="selectedTeamMember"
							@change="getSchedule()"                
							>
							<b-form-select-option value="">All</b-form-select-option>
							<b-form-select-option
								v-for="member in teamMemberList"
								:key="member.id"                  
								:value="member">{{member.name}}
							</b-form-select-option>                  
						</b-form-select>
					</b-input-group>
				</b-navbar-nav>

				<b-navbar-nav class="mr-2">
					<b-nav-form>
						<div :class="showWorkSectionChecked?'bg-success':''" :style="(showWorkSectionChecked?'width: 6rem;':'width: 3rem;')">
							<b-form-checkbox class="ml-2 my-2" v-model="showWorkSectionChecked" @change="getSchedule()" size="lg" switch>
								{{viewStatus}}
							</b-form-checkbox>
						</div>
						<div v-b-tooltip.hover
							title="Print Schedule">
							<b-button 
								style="max-height: 40px;" 
								size="sm"
								variant="white"						
								@click="printSchedule()" 
								class="my-2 ml-2">
								<b-icon icon="printer-fill" font-scale="2.0" variant="white"/>
							</b-button>
						</div>
					</b-nav-form>
				</b-navbar-nav>
			</b-navbar>
		</header>		
	</div>
</template>

<script lang="ts">
 
	import { Component, Vue, Watch } from 'vue-property-decorator';
	import { namespace } from "vuex-class";
	import moment from 'moment-timezone';
	import "@store/modules/ShiftScheduleInformation";
	const shiftState = namespace("ShiftScheduleInformation");
	import "@store/modules/CommonInformation";
    const commonState = namespace("CommonInformation");
	import { importConflictMessageType, shiftInfoType, shiftRangeInfoType } from '../../../types/ShiftSchedule';
	import { locationInfoType } from '../../../types/common';
	
	@Component
	export default class DistributeHeader extends Vue {		

		@commonState.State
		public location!: locationInfoType;
				
		@shiftState.State
		public shiftRangeInfo!: shiftRangeInfoType;

		@shiftState.Action
		public UpdateShiftRangeInfo!: (newShiftRangeInfo: shiftRangeInfoType) => void	

		selectedDate = '';
		isShiftDataMounted = false;
		teamDataReady = false;	
		datePickerOpened = false;
		showWorkSectionChecked = false;
		selectedTeamMember = '';

		@Watch('location.id', { immediate: true })
        locationChange()
        {
            if (this.teamDataReady) {
                this.getSheriffs();
            }            
        }

		mounted() {
			console.log('header')
			this.showWorkSectionChecked = false;
			this.getSheriffs();
			
			if(!this.shiftRangeInfo.startDate){
				this.selectedDate = moment().format().substring(0,10);
				this.loadNewDateRange();
			} else {
				this.selectedDate = this.shiftRangeInfo.startDate;
				this.getSchedule();
			}
			console.log(this.selectedDate)
			

			// this.$root.$on('editShifts', () => {
			// 	this.EditShift();
			// });
		}

		public getSheriffs() {            
			this.teamDataReady = true;
			console.log('getting list')
            // const url = 'api/sheriff';
            // this.$http.get(url)
            //     .then(response => {
            //         if(response.data){
            //             this.extractMyTeamFromSheriffs(response.data);                        
            //         }
            //         this.teamDataReady = true;
            //     })
        }
		
		public getSchedule() {
			console.log(this.showWorkSectionChecked);
			this.$emit('change', this.showWorkSectionChecked, this.selectedTeamMember);
		}

		public printSchedule() { 
			console.log('print')
        }
		
		get viewStatus() {
            if(this.showWorkSectionChecked) return 'WS';else return ''
        }		
		
		public dateChanged(event) {
			if(this.datePickerOpened)this.loadNewDateRange();
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
			this.UpdateShiftRangeInfo(dateRange);
			this.getSchedule(); 
		}
	}
</script>

<style scoped>

	.card {
				border: white;
		}

	.custom-navbar {
			float:none;
			margin:0 auto 0 auto;
			display: block;
			text-align: center;
	}

	.custom-navbar > li {
			display: inline-block;
			float:none;
	}

</style>