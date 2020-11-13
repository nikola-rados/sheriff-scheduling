<template>
    <div v-if="isDataMounted">    
        <b-card              
            bg-variant="white"            
            class="mb-2 p-0">
                <b-col>
                    <b-row style="font-size:11px; line-height: 16px;"># {{sheriffInfo.badgeNumber}}</b-row>
                    <b-row style="font-size:9px; line-height: 14px;">{{sheriffInfo.rank}}</b-row>
                    <b-row 
                        style="font-size:12px; line-height: 16px; font-weight: bold; text-transform: Capitalize;" 
                        v-b-tooltip.hover.topleft                                
                        :title="fullName.length>13?fullName:''">
                            {{fullName|truncate(11)}}
                    </b-row>
                </b-col>
                
        </b-card>
        
    </div>
</template>

<script lang="ts">
    import { Component, Vue, Prop } from 'vue-property-decorator';
    import { namespace } from "vuex-class";

    import "@store/modules/CommonInformation";
    const commonState = namespace("CommonInformation");
    import moment from 'moment-timezone';
    import { locationInfoType } from '../../../types/common';
    import { myTeamShiftInfoType } from '../../../types/DutyRoster';


    @Component
    export default class DutyRosterTeamMemberCard extends Vue {        

        @commonState.State
        public location!: locationInfoType;

        @Prop({required: true})
        public sheriffInfo!: myTeamShiftInfoType;

        sheriffId = '';

        isDataMounted = false;
        fullName = '';

        halfUnavailStyle="background-image: linear-gradient(to bottom right, rgb(194, 39, 28),rgb(243, 232, 232), white);"
        halfUnavailHalfSchStyle="background-image: linear-gradient(to bottom right, rgb(194, 39, 28),rgb(243, 232, 232), rgb(12, 120, 170));"
        halfSchStyle="background-image: linear-gradient(to bottom right, rgb(12, 120, 170),rgb(243, 232, 232), white);"
        WeekDay = ['S', 'M', 'T', 'W', 'T', 'F', 'S'];

		selectedStartTime = '';
		selectedEndTime = '';
		showShiftDetails = false;
		isShiftDataMounted = false;
		
		startTimeState = true;
		endTimeState = true;
		selectedDayState = true;

		selectedDays: number[] = [];
        allDaysSelected = false;
        weekDaysSelected = false;

	

		shiftError = false;
		shiftErrorMsg = '';
        shiftErrorMsgDesc = '';
        
        showCancelWarning = false;
        LoanedInDesc = '';
        
        mounted()
        {  
            this.isDataMounted = false;
            this.sheriffId = this.sheriffInfo.sheriffId;          
            this.fullName = this.sheriffInfo.lastName +', '+this.sheriffInfo.firstName;

                 
            this.extractShifts();
        }

        public extractShifts() {
            
           
            
            Vue.nextTick(()=>{this.isDataMounted = true;})                     
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