<template>
    <div>
        <b-table-simple small borderless>
            <b-tbody>
                <b-tr>
                    <b-td>   
                        <b-tr class="mt-1 bg-white">   
                            <b class="ml-3" v-if="!selectedStartDate || !selectedEndDate" >Full/Partial Day Leave: </b>                          
                            <b class="ml-3" style="background-color: #e8b5b5" v-else-if="isFullDay" >Full Day Leave: </b> 
                            <b class="ml-3" style="background-color: #aed4bc" v-else >Partial Day Leave: </b>
                        </b-tr>
                        <b-tr >
                            <b-form-group style="margin: 0.25rem 0 0 0.5rem;width: 19rem"> 
                                <b-form-select
                                    size = "sm"
                                    v-model="selectedLeave"
                                    :state = "leaveState?null:false">
                                        <b-form-select-option :value="{}">
                                            Select a Leave Type*
                                        </b-form-select-option>
                                        <b-form-select-option
                                            v-for="leave in leaveTypeInfoList" 
                                            :key="leave.id"
                                            :value="leave">
                                                {{leave.description}}
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
                            style="margin: 2rem .5rem 0 0 ; padding:0 .5rem 0 .5rem; "
                            variant="secondary"
                            @click="closeForm()">
                            Cancel
                        </b-button>   
                        <b-button                                    
                            style="margin: 2rem 0 0 0; padding:0 0.7rem 0 0.7rem; "
                            variant="success"                        
                            @click="saveForm()">
                            Save
                        </b-button>  
                    </b-td>
                </b-tr>   
            </b-tbody>
        </b-table-simple>  

        <b-modal v-model="showCancelWarning" id="bv-modal-leave-cancel-warning" header-class="bg-warning text-light">            
            <template v-slot:modal-title>
                <h2 v-if="isCreate" class="mb-0 text-light"> Unsaved New Leave </h2>                
                <h2 v-else class="mb-0 text-light"> Unsaved Leave Changes </h2>                                 
            </template>
            <p>Are you sure you want to cancel without saving your changes?</p>
            <template v-slot:modal-footer>
                <b-button variant="secondary" @click="$bvModal.hide('bv-modal-leave-cancel-warning')"                   
                >No</b-button>
                <b-button variant="success" @click="confirmedCloseForm()"
                >Yes</b-button>
            </template>            
            <template v-slot:modal-header-close>                 
                 <b-button variant="outline-warning" class="text-light closeButton" @click="$bvModal.hide('bv-modal-leave-cancel-warning')"
                 >&times;</b-button>
            </template>
        </b-modal>             
    </div>
</template>

<script lang="ts">
    import { Component, Vue, Prop } from 'vue-property-decorator';
    import moment from 'moment-timezone';
    import {teamMemberInfoType ,userLeaveInfoType} from '../../../../types/MyTeam';
    import {leaveInfoType} from '../../../../types/common';
    import { leaveTypeJson } from '../../../../types/common/jsonTypes';
    import { namespace } from 'vuex-class';
    import "@store/modules/TeamMemberInformation"; 
    const TeamMemberState = namespace("TeamMemberInformation");

    @Component
    export default class AddLeaveForm extends Vue {        

        @TeamMemberState.State
        public userToEdit!: teamMemberInfoType;

        @Prop({required: true})
        formData!: userLeaveInfoType;

        @Prop({required: true})
        isCreate!: boolean;

        @Prop({required: true})
        leaveTypeInfoList!: leaveInfoType[];       

        selectedLeave = {} as leaveInfoType | undefined;
        leaveState = true;      

        selectedStartDate = '';
        startDateState = true; 

        selectedEndDate = '';
        endDateState = true;

        selectedStartTime = '';
        startTimeState = true;

        selectedEndTime = '';
        endTimeState = true; 

        originalLeave = {} as leaveInfoType | undefined;
        originalStartDate = '';
        originalEndDate = '';
        originalStartTime = '';
        originalEndTime = '';

        formDataId = 0;
        showCancelWarning = false;
        
        mounted()
        { 
            this.clearSelections();
            if(this.formData.id) {
                this.extractFormInfo();
            }               
        }        

        public extractFormInfo(){
            this.formDataId = this.formData.id? this.formData.id:0;
            
            const index = this.leaveTypeInfoList.findIndex(leave=>{if(leave.id == this.formData.leaveTypeId)return true})
            this.originalLeave = this.selectedLeave = (index>=0)? this.leaveTypeInfoList[index]: {} as leaveInfoType;            
            this.originalStartDate = this.selectedStartDate = this.formData.startDate.substring(0,10)            
            this.originalEndDate = this.selectedEndDate =  this.formData.endDate.substring(0,10)
            
            const startDate = this.selectedStartDate+"T"+(this.selectedStartTime?this.selectedStartTime:'00:00:00')+".000Z";
            const endDate =   this.selectedEndDate+"T"+(this.selectedEndTime?this.selectedEndTime:'00:00:00')+".000Z";
            const displayTime = this.isDateFullday(startDate,endDate)
           
            this.originalStartTime = this.selectedStartTime = displayTime? '' :this.formData.startDate.substring(11,19)            
            this.originalEndTime = this.selectedEndTime = displayTime? '' :this.formData.endDate.substring(11,19)
        }

        public saveForm(){
                this.leaveState  = true;
                this.endDateState   = true;
                this.startDateState = true;
                this.startTimeState = true;
                this.endTimeState   = true;
                const isFullDay = this.isFullDay

                if(this.selectedLeave && !this.selectedLeave.id ){
                    this.leaveState  = false;
                }else if(this.selectedStartDate == ""){
                    this.leaveState  = true;
                    this.startDateState = false;
                }else if(this.selectedEndDate == ""){
                    this.leaveState  = true;
                    this.startDateState = true;
                    this.endDateState   = false;
                }else if(this.selectedEndTime == "" && this.selectedStartTime != ""){
                    this.leaveState  = true;
                    this.startDateState = true;
                    this.endDateState   = true;
                    this.startTimeState = true;
                    this.endTimeState   = false;
                }else if(this.selectedStartTime == "" && this.selectedEndTime != ""){
                    this.leaveState  = true;
                    this.startDateState = true;
                    this.endDateState   = true;
                    this.endTimeState   = true;
                    this.startTimeState = false;
                }else{
                    this.leaveState  = true;
                    this.endDateState   = true;
                    this.startDateState = true;
                    this.startTimeState = true;
                    this.endTimeState   = true;

                    const startDate = this.selectedStartDate+"T"+(this.selectedStartTime?this.selectedStartTime:'00:00:00')+".000Z";
                    const endDate =   this.selectedEndDate+"T"+(this.selectedEndTime?this.selectedEndTime:'00:00:00')+".000Z";

                    const body = {
                        leaveTypeId: this.selectedLeave?this.selectedLeave.id:0,
                        startDate: startDate,
                        endDate: endDate,                      
                        isFullDay: isFullDay,
                        id: this.formDataId
                    } 
                    this.$emit('submit', body, this.isCreate);                  
                }
        }

        public closeForm(){
            if(this.isChanged())
                this.showCancelWarning = true;
            else
                this.confirmedCloseForm();
        }

        public isChanged(){
            if(this.isCreate){
                if((this.selectedLeave && this.selectedLeave.id) ||
                    this.selectedStartDate || this.selectedEndDate ||
                    this.selectedStartTime || this.selectedEndTime) return true;
                return false;
            }else{
                if((this.originalLeave && this.selectedLeave && (this.originalLeave.id != this.selectedLeave.id)) ||
                    (this.originalStartDate != this.selectedStartDate)|| 
                    (this.originalEndDate != this.selectedEndDate) ||
                    (this.originalStartTime != this.selectedStartTime) || 
                    (this.originalEndTime != this.selectedEndTime)) return true;
                return false;
            }
        }

        public confirmedCloseForm(){           
            this.clearSelections();
            this.$emit('cancel');
        }

        public clearSelections(){
            this.selectedLeave = {} as leaveInfoType;
            this.selectedEndDate = '';
            this.selectedStartDate = '';
            this.selectedStartTime = '';
            this.selectedEndTime = '';
            this.leaveState  = true;
            this.endDateState   = true;
            this.startDateState = true;
            this.startTimeState = true;
            this.endTimeState   = true;            
        }

        public isDateFullday(startDate, endDate){
            const start = moment(startDate); 
            const end = moment(endDate);
            const duration = moment.duration(end.diff(start));
            if(duration.asMinutes() < 1440 && duration.asMinutes()> -1440 )  return false;  else return true;
        }

        get isFullDay(){    
            if(this.selectedStartTime == '' && this.selectedEndTime == '')
                return true
            else if(this.selectedStartDate && this.selectedEndDate){
                const startDate = this.selectedStartDate+"T"+(this.selectedStartTime?this.selectedStartTime:'00:00:00')+".000Z";
                const endDate =   this.selectedEndDate+"T"+(this.selectedEndTime?this.selectedEndTime:'00:00:00')+".000Z";
                return this.isDateFullday(startDate,endDate)
            }else
                return false           
        }
    }
</script>

<style scoped>
    td {
        margin: 0rem 0.5rem 0.1rem 0rem;
        padding: 0rem 0.5rem 0.1rem 0rem;
        
        background-color: white ;
    }
</style>