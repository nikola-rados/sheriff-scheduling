<template>
    <div>
        <b-table-simple small borderless>
            <b-tbody>
                <b-tr>
                    <b-td>   
                        <b-tr class="mt-1 bg-white">   
                            <b class="ml-3" v-if="!selectedStartDate || !selectedEndDate" >Full/Partial Day Training: </b>                          
                            <b class="ml-3" style="background-color: #e8b5b5" v-else-if="isFullDay" >Full Day Training: </b> 
                            <b class="ml-3" style="background-color: #aed4bc" v-else >Partial Day Training: </b>
                        </b-tr>
                        <b-tr >
                            <b-form-group style="margin: 0.25rem 0 0 0.5rem;width: 19rem"> 
                                <b-form-select
                                    size = "sm"
                                    v-model="selectedTrainingType"
                                    :state = "trainingTypeState?null:false">
                                        <b-form-select-option :value="{}">
                                            Select a Training Type*
                                        </b-form-select-option>
                                        <b-form-select-option
                                            v-for="training in trainingTypeInfoList" 
                                            :key="training.id"
                                            :value="training">
                                                {{training.description}}
                                        </b-form-select-option>     
                                </b-form-select>
                                <b-form-input
                                        v-if="selectedTrainingType.code=='Other'"
                                        v-model="selectedTrainingTypeComment"
                                        placeholder="Comment*"
                                        :state = "trainingTypeCommentState?null:false"
                                        size = "sm">
                                </b-form-input>
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
                        <label class="h6 m-0 p-0"> Expiry Date: </label>
                        <b-form-datepicker
                            class="mb-1 mt-0 pt-0"
                            size="sm"
                            v-model="selectedExpiryDate"
                            placeholder="Expiry Date*"
                            :state = "expiryDateState?null:false"                                    
                            :date-format-options="{ year: 'numeric', month: 'short', day: '2-digit' }"
                            locale="en-US">
                        </b-form-datepicker> 
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

        <b-modal v-model="showCancelWarning" id="bv-modal-training-cancel-warning" header-class="bg-warning text-light">            
            <template v-slot:modal-title>
                <h2 v-if="isCreate" class="mb-0 text-light"> Unsaved New Training </h2>                
                <h2 v-else class="mb-0 text-light"> Unsaved Training Changes </h2>                                 
            </template>
            <p>Are you sure you want to cancel without saving your changes?</p>
            <template v-slot:modal-footer>
                <b-button variant="secondary" @click="$bvModal.hide('bv-modal-training-cancel-warning')"                   
                >No</b-button>
                <b-button variant="success" @click="confirmedCloseForm()"
                >Yes</b-button>
            </template>            
            <template v-slot:modal-header-close>                 
                 <b-button variant="outline-warning" class="text-light closeButton" @click="$bvModal.hide('bv-modal-training-cancel-warning')"
                 >&times;</b-button>
            </template>
        </b-modal>             
    </div>
</template>

<script lang="ts">
    import { Component, Vue, Prop } from 'vue-property-decorator';
    import moment from 'moment-timezone';
    import {teamMemberInfoType ,userTrainingInfoType} from '../../../../types/MyTeam';
    import {trainingInfoType} from '../../../../types/common';
    import { trainingTypeJson } from '../../../../types/common/jsonTypes';
    import { namespace } from 'vuex-class';
    import "@store/modules/TeamMemberInformation"; 
    const TeamMemberState = namespace("TeamMemberInformation");

    @Component
    export default class AddTrainingForm extends Vue {        

        @TeamMemberState.State
        public userToEdit!: teamMemberInfoType;

        @Prop({required: true})
        formData!: userTrainingInfoType;

        @Prop({required: true})
        isCreate!: boolean;

        @Prop({required: true})
        trainingTypeInfoList!: trainingInfoType[];       

        selectedTrainingType = {} as trainingInfoType | undefined;
        trainingTypeState = true;

        selectedTrainingTypeComment =''
        trainingTypeCommentState = true

        selectedEndDate = ''
        endDateState = true

        selectedStartDate = ''
        startDateState = true

        selectedStartTime = ''
        startTimeState = true

        selectedEndTime = ''
        endTimeState = true

        selectedExpiryDate = ''
        expiryDateState = true; 

        originalTrainingType = {} as trainingInfoType | undefined;
        originalTrainingTypeComment =''
        originalEndDate = ''
        originalStartDate = ''
        originalStartTime = ''
        originalEndTime = ''
        originalExpiryDate = ''

        formDataId = 0;
        showCancelWarning = false;
        
        mounted()
        { 
            this.clearTrainingSelections();
            if(this.formData.id) {
                this.extractFormInfo();
            }               
        }        

        public extractFormInfo(){
            this.formDataId = this.formData.id? this.formData.id:0;
            
            const index = this.trainingTypeInfoList.findIndex(training=>{if(training.id == this.formData.trainingTypeId)return true})
            this.originalTrainingType = this.selectedTrainingType = (index>=0)? this.trainingTypeInfoList[index]: {} as trainingInfoType; 
            this.originalExpiryDate = this.selectedExpiryDate = this.formData.expiryDate.substring(0,10);
            this.originalTrainingTypeComment = this.selectedTrainingTypeComment = this.formData.comment? this.formData.comment:'';          
            this.originalStartDate = this.selectedStartDate = this.formData.startDate.substring(0,10)            
            this.originalEndDate = this.selectedEndDate =  this.formData.endDate.substring(0,10)
            
            const startDate = this.selectedStartDate+"T"+(this.selectedStartTime?this.selectedStartTime:'00:00:00')+".000Z";
            const endDate =   this.selectedEndDate+"T"+(this.selectedEndTime?this.selectedEndTime:'00:00:00')+".000Z";
            const displayTime = this.isDateFullday(startDate,endDate)
           
            this.originalStartTime = this.selectedStartTime = displayTime? '' :this.formData.startDate.substring(11,19)            
            this.originalEndTime = this.selectedEndTime = displayTime? '' :this.formData.endDate.substring(11,19)
        }

        public saveForm(){
                this.trainingTypeState  = true;
                this.endDateState    = true;
                this.startDateState  = true;
                this.startTimeState  = true;
                this.endTimeState    = true;
                this.expiryDateState = true;
                const isFullDay = this.isFullDay

                if(this.selectedTrainingType && !this.selectedTrainingType.id ){
                    this.trainingTypeState  = false;
                }else if(this.selectedTrainingType && this.selectedTrainingType.code=='Other' && this.selectedTrainingTypeComment == ""){
                    this.trainingTypeState  = true;
                    this.trainingTypeCommentState = false;
                }else if(this.selectedStartDate == ""){
                    this.trainingTypeState  = true;
                    this.trainingTypeCommentState = true;
                    this.startDateState = false;
                }else if(this.selectedEndDate == ""){
                    this.trainingTypeState  = true;
                    this.trainingTypeCommentState = true;
                    this.startDateState = true;
                    this.endDateState   = false;
                }else if(this.selectedEndTime == "" && this.selectedStartTime != ""){
                    this.trainingTypeState  = true;
                    this.trainingTypeCommentState = true;
                    this.startDateState = true;
                    this.endDateState   = true;
                    this.startTimeState = true;
                    this.endTimeState   = false;
                }else if(this.selectedStartTime == "" && this.selectedEndTime != ""){
                    this.trainingTypeState  = true;
                    this.trainingTypeCommentState = true;
                    this.startDateState = true;
                    this.endDateState   = true;
                    this.endTimeState   = true;
                    this.startTimeState = false;
                }else if(this.selectedExpiryDate == ""){
                    this.trainingTypeState  = true;
                    this.trainingTypeCommentState = true;
                    this.startDateState = true;
                    this.endDateState   = true;
                    this.endTimeState   = true;
                    this.startTimeState = true;
                    this.expiryDateState = false;
                }else{
                    this.trainingTypeState  = true;
                    this.trainingTypeCommentState = true;
                    this.endDateState   = true;
                    this.startDateState = true;
                    this.startTimeState = true;
                    this.endTimeState   = true;
                    this.expiryDateState = true;

                    const startDate = this.selectedStartDate+"T"+(this.selectedStartTime?this.selectedStartTime:'00:00:00')+".000Z";
                    const endDate =   this.selectedEndDate+"T"+(this.selectedEndTime?this.selectedEndTime:'00:00:00')+".000Z";

                    const body = {
                        trainingTypeId: this.selectedTrainingType?this.selectedTrainingType.id:0,
                        startDate: startDate,
                        endDate: endDate,                      
                        trainingCertificationExpiry: this.selectedExpiryDate,
                        comment: this.selectedTrainingTypeComment,
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
                if((this.selectedTrainingType && this.selectedTrainingType.id) ||
                    this.selectedStartDate || this.selectedEndDate ||
                    this.selectedExpiryDate || this.selectedTrainingTypeComment ||
                    this.selectedStartTime || this.selectedEndTime) return true;
                return false;
            }else{
                if((this.originalTrainingType && this.selectedTrainingType && (this.originalTrainingType.id != this.selectedTrainingType.id)) ||
                    (this.originalStartDate != this.selectedStartDate)|| 
                    (this.originalEndDate != this.selectedEndDate) ||
                    (this.originalExpiryDate != this.selectedExpiryDate) ||
                    (this.originalTrainingTypeComment != this.selectedTrainingTypeComment) ||
                    (this.originalStartTime != this.selectedStartTime) || 
                    (this.originalEndTime != this.selectedEndTime)) return true;
                return false;
            }
        }

        public confirmedCloseForm(){  
            this.clearTrainingSelections();
            this.$emit('cancel');
        }

        public clearTrainingSelections(){
            this.selectedTrainingType = {} as trainingInfoType | undefined;
            this.selectedTrainingTypeComment = '';
            this.selectedEndDate = '';
            this.selectedStartDate = '';
            this.selectedStartTime = '';
            this.selectedEndTime = '';

            this.trainingTypeState  = true;
            this.trainingTypeCommentState = true;
            this.endDateState   = true;
            this.startDateState = true;
            this.startTimeState = true;
            this.endTimeState   = true;
            this.expiryDateState = true;            
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