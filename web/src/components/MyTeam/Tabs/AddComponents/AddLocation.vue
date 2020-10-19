<template>
    <div>
        <b-table-simple small borderless >
            <b-tbody>
                <b-tr>
                    <b-td>   
                        <b-tr class="mt-1">   
                            <b class="ml-3" v-if="!selectedStartDate || !selectedEndDate" >Full/Partial Day: </b>                          
                            <b class="ml-3" style="background-color: #e8b5b5" v-else-if="isFullDay" >Full Day: </b> 
                            <b class="ml-3" style="background-color: #aed4bc" v-else >Partial Day: </b>
                        </b-tr>
                        <b-tr >
                            <b-form-group style="margin: 0.25rem 0 0 0.75rem;width: 20rem"> 
                                <b-form-select
                                    size = "sm"
                                    v-model="selectedLocation"
                                    :state = "locationState?null:false">
                                        <b-form-select-option :value="{}">
                                            Select a location*
                                        </b-form-select-option>
                                        <b-form-select-option
                                            v-for="location in noHomeLocationList" 
                                            :key="location.id"
                                            :value="location">
                                                {{location.name}}
                                        </b-form-select-option>     
                                </b-form-select>
                            </b-form-group>
                        </b-tr>                                
                    </b-td>
                    <b-td>
                        <label class="h6 m-0 p-0"> From: </label>
                        <b-form-datepicker
                            class="mb-1"
                            size="sm"
                            v-model="selectedStartDate"
                            placeholder="Start Date*"
                            :state = "startDateState?null:false"
                            :date-format-options="{ year: 'numeric', month: 'short', day: '2-digit' }"
                            locale="en-US">
                        </b-form-datepicker>
                        <b-form-timepicker
                            size="sm"
                            v-model="selectedStartTime"
                            placeholder="Start Time"
                            reset-button
                            :state = "startTimeState?null:false" 
                            locale="en">                                   
                        </b-form-timepicker>
                    </b-td>
                    <b-td>
                        <label class="h6 m-0 p-0"> To: </label>
                        <b-form-datepicker
                            class="mb-1 mt-0 pt-0"
                            size="sm"
                            v-model="selectedEndDate"
                            placeholder="End Date*"
                            :state = "endDateState?null:false"                                    
                            :date-format-options="{ year: 'numeric', month: 'short', day: '2-digit' }"
                            locale="en-US">
                        </b-form-datepicker> 
                        <b-form-timepicker
                            size="sm" 
                            v-model="selectedEndTime"
                            placeholder="End Time" 
                            reset-button
                            :state = "endTimeState?null:false"
                            locale="en">
                        </b-form-timepicker>
                    </b-td>
                    <b-td >
                        <b-button                                    
                            style="margin: 1.75rem 0 0 0.75rem; "
                            variant="success"                        
                            @click="saveAwayLocation()">
                            Save
                        </b-button>   
                    </b-td>
                </b-tr>   
            </b-tbody>
        </b-table-simple>              
    </div>
</template>

<script lang="ts">
    import { Component, Vue, Prop } from 'vue-property-decorator';
    import moment from 'moment-timezone';
    import {teamMemberInfoType} from '../../../../types/MyTeam';
    import {locationInfoType} from '../../../../types/common';
    import { namespace } from 'vuex-class';
    import "@store/modules/CommonInformation";
    const commonState = namespace("CommonInformation");
    import "@store/modules/TeamMemberInformation"; 
    const TeamMemberState = namespace("TeamMemberInformation");

    @Component
    export default class LocationTab extends Vue {

        @commonState.State
        public locationList!: locationInfoType[];

        @TeamMemberState.State
        public userToEdit!: teamMemberInfoType;

        selectedLocation = {} as locationInfoType | undefined;
        locationState = true;

        selectedEndDate = ''
        endDateState = true

        selectedStartDate = ''
        startDateState = true

        selectedStartTime = ''
        startTimeState = true

        selectedEndTime = ''
        endTimeState = true
        

        mounted()
        {             
                   
        }

        public isDateFullday(startDate, endDate){
            const start = moment(startDate); 
            const end = moment(endDate);
            const duration = moment.duration(end.diff(start));
            if(duration.asMinutes() < 1440 && duration.asMinutes()> -1440 )  return false;  else return true;
        }

    

        public saveAwayLocation(){
                this.locationState  = true;
                this.endDateState   = true;
                this.startDateState = true;
                this.startTimeState = true;
                this.endTimeState   = true;
                const isFullDay = this.isFullDay

                if(this.selectedLocation && !this.selectedLocation.id ){
                    this.locationState  = false;
                }else if(this.selectedStartDate == ""){
                    this.locationState  = true;
                    this.startDateState = false;
                }else if(this.selectedEndDate == ""){
                    this.locationState  = true;
                    this.startDateState = true;
                    this.endDateState   = false;
                }else if(this.selectedEndTime == "" && this.selectedStartTime != ""){
                    this.locationState  = true;
                    this.startDateState = true;
                    this.endDateState   = true;
                    this.startTimeState = true;
                    this.endTimeState   = false;
                }else if(this.selectedStartTime == "" && this.selectedEndTime != ""){
                    this.locationState  = true;
                    this.startDateState = true;
                    this.endDateState   = true;
                    this.endTimeState   = true;
                    this.startTimeState = false;
                }else{
                    this.locationState  = true;
                    this.endDateState   = true;
                    this.startDateState = true;
                    this.startTimeState = true;
                    this.endTimeState   = true;

                    const startDate = this.selectedStartDate+"T"+(this.selectedStartTime?this.selectedStartTime:'00:00:00')+".000Z";
                    const endDate =   this.selectedEndDate+"T"+(this.selectedEndTime?this.selectedEndTime:'00:00:00')+".000Z";

                    const body = {
                        locationId: this.selectedLocation?this.selectedLocation.id:0,
                        startDate: startDate,
                        endDate: endDate,                      
                        isFullDay: isFullDay
                    }
                   
                }
        }

        public clearLocationSelection(){
            this.selectedLocation = {} as locationInfoType | undefined;
            this.selectedEndDate = '';
            this.selectedStartDate = '';
            this.selectedStartTime = '';
            this.selectedEndTime = '';
            this.locationState  = true;
            this.endDateState   = true;
            this.startDateState = true;
            this.startTimeState = true;
            this.endTimeState   = true;
        }

        get noHomeLocationList(){
            return this.locationList.filter(location =>{ if(this.userToEdit.homeLocationId == location.id) return false;else return true})
        }

        get isFullDay(){    

            if(this.selectedStartTime == '' && this.selectedEndTime == '')
                return true
            else if(this.selectedStartDate && this.selectedEndDate)
            {
                const startDate = this.selectedStartDate+"T"+(this.selectedStartTime?this.selectedStartTime:'00:00:00')+".000Z";
                const endDate =   this.selectedEndDate+"T"+(this.selectedEndTime?this.selectedEndTime:'00:00:00')+".000Z";
                return this.isDateFullday(startDate,endDate)
            }
            else
                return false
        }
    }
</script>

<style scoped>
    td {
        margin: 0rem 1rem 0.1rem 0rem;
        padding: 0rem 1rem 0.1rem 0rem;
        
        background-color: whitesmoke ;
    }
</style>