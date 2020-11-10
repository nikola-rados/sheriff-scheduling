<template>
	<div>
		<header variant="primary">
			<b-navbar toggleable="lg" class=" m-0 p-0 navbar navbar-expand-lg navbar-dark">
                <b-navbar-nav class="ml-1 mr-auto">
					<b-button style="height: 2.2rem;" size="sm" variant="success" @click="addAssignment" class="my-2"> Add Assignment</b-button>
                </b-navbar-nav>
				<b-navbar-nav class="custom-navbar">
                    <b-col>
                        <b-row style="margin:.25rem auto .25rem auto; width:73%;">
                            <b-button style="height: 2rem;" size="sm" variant="secondary" @click="previousDateRange" class="my-0"><b-icon-chevron-left /></b-button>
                            <b-form-datepicker
                                    class="my-0 py-0 mx-1"
                                    size = "sm"
                                    style="height: 2rem;"
                                    v-model = "selectedDate"
                                    @context = "dateChanged"
                                    @shown = "datePickerOpened = true"
                                    @hidden = "datePickerOpened = false"
                                    button-only
                                    locale="en-US">
                            </b-form-datepicker>
                            <b-button style="height: 2rem;" size="sm" variant="secondary" @click="nextDateRange" class="my-0"><b-icon-chevron-right/></b-button>
                        </b-row>
                        <b-row style="margin:0 0 .25rem auto;">
                            <div style=" border-radius:5px; background-color:#f2d2f7; color:#fc0366; width:10rem;">{{selectedDate|beautify-date-weekday}}</div>
                        </b-row>
                    </b-col>
                </b-navbar-nav>
			</b-navbar>
            
		</header>
	</div>
</template>

<script lang="ts">
 
	import { Component, Vue } from 'vue-property-decorator';
	import moment from 'moment-timezone';
	
	import { namespace } from "vuex-class";   
    import "@store/modules/CommonInformation";
    const commonState = namespace("CommonInformation");

    import "@store/modules/DutyRosterInformation";   
    const dutyState = namespace("DutyRosterInformation");

    import { locationInfoType } from '@/types/common';
    import { dutyRangeInfoType} from '@/types/DutyRoster';
	
	@Component
	export default class DutyRosterHeader extends Vue {

        @commonState.State
        public location!: locationInfoType;

        @dutyState.Action
        public UpdateDutyRangeInfo!: (newDutyRangeInfo: dutyRangeInfoType) => void

        selectedDate = ''
        datePickerOpened = false

        mounted() {
			this.selectedDate = moment().format().substring(0,10);			
			this.loadNewDateRange();
		}

        public addAssignment(){
            console.log('add assignment')
        }
        
        public dateChanged(event) {
			//console.log(event)
			//console.log(this.datePickerOpened)
			if(this.datePickerOpened)this.loadNewDateRange();
		}

		public nextDateRange() {			
			this.selectedDate = moment(this.selectedDate).add(1, 'days').format().substring(0,10);
			this.loadNewDateRange(); 
		}

		public previousDateRange() {
			this.selectedDate = moment(this.selectedDate).subtract(1, 'days').format().substring(0,10);
			this.loadNewDateRange();
		}

		public loadNewDateRange() {
			const beginTime = moment(this.selectedDate).startOf('day').format()
			const endTime = moment(this.selectedDate).endOf('day').format();
			const dateRange = {startDate: beginTime, endDate: endTime}
			this.UpdateDutyRangeInfo(dateRange);
			this.$emit('change'); 
		}

    }
</script>

<style scoped>

	.card {
				border: white;
		}

	.custom-navbar {
			float: none;
			margin:0 auto 0 0;
			display: block;
			text-align: center;
	}

	.custom-navbar > li {
			display: inline-block;
			float:none;
	}

</style>