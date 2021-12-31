<template>
	<div>
		<header variant="primary">
			<b-navbar toggleable="lg" class=" m-0 p-0 navbar navbar-expand-lg navbar-dark">
				<b-navbar-nav>
					<h3 style="width:15rem; margin-bottom: 0px;" class="text-white ml-2 font-weight-normal">Distribute Schedule</h3>
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
					<b-input-group class="mr-2 my-0" style="height: 40px">
						<b-input-group-prepend is-text>
							<b-icon icon="person-fill"></b-icon>
						</b-input-group-prepend>
						<b-form-select
							style="height: 100%;"
							v-model="selectedTeamMember"
							@change="getSchedule()"                
							>
							<b-form-select-option :value="{sheriffId: '', name: 'All'}">All</b-form-select-option>
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
						<div v-if="hasPermissionToViewDutyRoster" :class="showWorkSectionChecked?'bg-success':''" :style="'border:1px solid green;border-radius:5px;'+(showWorkSectionChecked?'width: 8rem;':'width: 8rem;')">
							<b-form-checkbox class="ml-2 my-1" v-model="showWorkSectionChecked" @change="getSchedule()" size="lg" switch>
								<div class="text-white">{{viewStatus}}</div>
							</b-form-checkbox>
						</div>
						<div v-b-tooltip.hover.noninteractive
							title="Print Schedule">
							<b-button 
								style="max-height: 40px;" 
								size="sm"
								variant="white"						
								@click="printSchedule()" 
								class="my-0 ml-2">
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
	import { distributeTeamMemberInfoType, shiftRangeInfoType } from '../../../types/ShiftSchedule';
	import { locationInfoType, userInfoType } from '../../../types/common';

	import { Printd } from 'printd'

// 	import VueHtmlToPaper from 'vue-html-to-paper';
// 	const options = {
// 		name: '_blank',
// 		specs: [
// 			'fullscreen=yes',
// 			'titlebar=yes',
// 			'scrollbars=yes'
// 		],
// 		styles: [
// 			"https://maxcdn.bootstrapcdn.com/bootstrap/4.0.0/css/bootstrap.min.css",
// 				`@media print {
// 					@page {
// 						size: landscape
// 					}
// 				}`
// 		]
// 	}
// 	Vue.use(VueHtmlToPaper, options);
	//import z from 'node_modules/'
	@Component
	export default class DistributeHeader extends Vue {	

		@commonState.State
		public location!: locationInfoType;

		@commonState.State
        public userDetails!: userInfoType;

		@shiftState.State
		public teamMemberList!: distributeTeamMemberInfoType[];
				
		@shiftState.State
		public shiftRangeInfo!: shiftRangeInfoType;

		@shiftState.Action
		public UpdateShiftRangeInfo!: (newShiftRangeInfo: shiftRangeInfoType) => void	

		selectedDate = '';
		datePickerOpened = false;
		showWorkSectionChecked = false;		
		hasPermissionToViewDutyRoster = false;

		selectedTeamMember = {sheriffId: '', name: 'All'} as distributeTeamMemberInfoType;

		@Watch('location.id', { immediate: true })
        locationChange()
        {
			this.selectedTeamMember = {sheriffId: '', name: 'All'};
			this.showWorkSectionChecked = false;            
        }

		mounted() {

			this.showWorkSectionChecked = false;
			this.hasPermissionToViewDutyRoster = this.userDetails.permissions.includes("ViewDutyRoster");
			
			
			if(!this.shiftRangeInfo.startDate){
				this.selectedDate = moment().format().substring(0,10);
				this.loadNewDateRange();
			} else {
				this.selectedDate = this.shiftRangeInfo.startDate;
				this.getSchedule();
			}			
		}		
		
		public getSchedule() {
			// console.log(this.showWorkSectionChecked);
			// console.log(this.selectedTeamMember);
			Vue.nextTick(()=>this.$emit('change', this.showWorkSectionChecked, this.selectedTeamMember.sheriffId))
		}

		public printSchedule() { 
			//console.log('print')
			//this.$htmlToPaper('pdf');
			const pdfPage: Printd = new Printd()
			const styles = [				
				"https://unpkg.com/bootstrap/dist/css/bootstrap.min.css",
				`@media print {
					@page {
						size:16.5in 11.7in;
					}
				}`,
				`.card {border: white;}`,
				`.table{border: 3px solid;}`,
				`tr {border: 3px solid;}`,
				`th {border: 3px solid black;}`,
				`td {height: 2.5rem;border: 3px solid;}`,
				`.bg-spl-leave {background-color: #ffee07;}`,
				`.bg-a-l-leave {background-color: #007bff;}`,
				`.bg-med-dental-leave {background-color: #fd7e14;}`,
				`.bg-stiip-leave {background-color: #dc3545;}`,
				`.bg-cto-leave {background-color: #33b652;}`,
				`.bg-lwop-leave {background-color: #6e42c1dc;}`,
				`.bg-bereavement-leave {background-color: #6c757d;}`,
				`.bg-training-leave {background-color: #b46d47;}`,
				`.bg-overtime-leave {background-color: #ced4da;}`,
				`.bg-primary {background-color: #1b4f86;}`

			]
			const pageToPrint = document.getElementById("pdf")
			if(pageToPrint) pdfPage.print(pageToPrint, styles)
        }
		
		get viewStatus() {
            if(this.showWorkSectionChecked) return 'WS';else return 'No WS'
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