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
					<!-- <b-nav-form>
						<b-button style="max-height: 40px;" size="sm" variant="success" @click="AddShift()" class="my-2"><b-icon-plus/>Add Shift</b-button>
					</b-nav-form> -->
				</b-navbar-nav>
			</b-navbar>
		</header>
	</div>
</template>

<script lang="ts">
 
	import { Component, Vue } from 'vue-property-decorator';
	import { namespace } from "vuex-class";
	import moment from 'moment-timezone';
	import "@store/modules/ShiftScheduleInformation";
	const shiftState = namespace("ShiftScheduleInformation");
	import { shiftRangeInfoType } from '../../../types/ShiftSchedule';
	
	@Component
	export default class ScheduleHeader extends Vue {		

		@shiftState.State
		public shiftRangeInfo!: shiftRangeInfoType;

		@shiftState.Action
		public UpdateShiftRangeInfo!: (newShiftRangeInfo: shiftRangeInfoType) => void

		selectedDate = '';

		mounted() {
			console.log('mounted')
			this.selectedDate = moment().format().substring(0,10);			
			this.loadNewDateRange();
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
			// console.log(dateRange)
			this.UpdateShiftRangeInfo(dateRange);
			this.$emit('change'); 
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