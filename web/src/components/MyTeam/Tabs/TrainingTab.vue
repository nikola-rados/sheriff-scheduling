<template>
    <div>
        <b-card  style="height:400px;overflow: auto;" no-body>                                        
            <h2 v-if="trainingError" class="mx-1 mt-0"><b-badge v-b-tooltip.hover :title="trainingErrorMsgDesc"  variant="danger"> {{trainingErrorMsg}} <b-icon class="ml-3" icon = x-square-fill @click="trainingError = false" /></b-badge></h2>
            <b-card v-if="!addNewTraining">
                <b-button  style="transform:translate(0px,-5px);" size="sm" variant="success" @click="addNewTraining = true"> <b-icon icon="plus" /> Add </b-button>
            </b-card> 

            <b-card v-if="addNewTraining" class="my-3" border-variant="light" no-body>
                <b-table-simple small borderless >
                    <b-tbody>
                        <b-tr>
                            <b-td>   
                                <b-tr class="mt-1">   
                                    <b class="ml-3" v-if="!selectedStartDate || !selectedEndDate" >Full/Partial Day Training: </b>                          
                                    <b class="ml-3" style="background-color: #e8b5b5" v-else-if="isFullDay" >Full Day Training: </b> 
                                    <b class="ml-3" style="background-color: #aed4bc" v-else >Partial Day Training: </b>
                                </b-tr>
                                <b-tr >
                                    <b-form-group style="margin: 0 0 0 0.75rem;width: 15rem"> 
                                        <b-form-select
                                            class="mb-1"
                                            size = "sm"
                                            v-model="selectedTrainingType"
                                            :state = "trainingTypeState?null:false">
                                                <b-form-select-option :value="{}">
                                                    Select a Training Type*
                                                </b-form-select-option>
                                                <b-form-select-option
                                                    v-for="training in trainingTypes" 
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
                                    style="margin: 0 .5rem 0 0 ; padding:0 .5rem 0 .5rem; "
                                    variant="secondary"
                                    @click="clearTrainingSelection()">
                                    Cancel
                                </b-button>   
                                <b-button                                    
                                    style="width:70px;margin: 0 0 0 0; padding:0 1rem 0 1rem; "
                                    variant="success"                        
                                    @click="saveTraining()">
                                    Save
                                </b-button>   
                            </b-td>
                        </b-tr>   
                    </b-tbody>
                </b-table-simple>              
            </b-card>

            <div>
                <b-card no-body border-variant="white" bg-variant="white" v-if="!assignedTrainings.length">
                        <span class="text-muted ml-4 my-5">No trainings have been assigned.</span>
                </b-card>

                <b-card v-else  no-body border-variant="light" bg-variant="white">
                    <b-table
                        :items="assignedTrainings"
                        :fields="fields"
                        head-row-variant="primary"
                        striped
                        borderless
                        small
                        sort-by="startDate"
                        responsive="sm"
                        >  
                            <template v-slot:cell(isFullDay)="data" >
                                <span v-if="data.value">Full</span> 
                                <span v-else>Partial</span> 
                            </template>
                            <template v-slot:cell(trainingType)="data" >
                                <span 
                                    class="text-primary"
                                    v-b-tooltip.hover.right                                
                                    :title="data.item.comment?data.item.comment:data.value.code"> 
                                        {{data.value.description}}
                                </span>
                            </template>
                            <template v-slot:cell(startDate)="data" >
                                <span>{{data.value | beautify-date}}</span> 
                            </template>
                            <template v-slot:cell(endDate)="data" >
                                <span>{{data.value | beautify-date}}</span> 
                            </template>
                            <template v-slot:cell(startTime)="data" >
                                <span v-if="!data.item.isFullDay">{{data.item.startDate | beautify-time}}</span> 
                            </template>
                            <template v-slot:cell(endTime)="data" >
                                <span v-if="!data.item.isFullDay">{{data.item.endDate | beautify-time }}</span> 
                            </template>
                            <template v-slot:cell(expiryDate)="data" >
                                <span>{{data.value | beautify-date}}</span> 
                            </template>
                            <template v-slot:cell(edit)="data" >                                       
                                <b-button class="my-0 py-0" size="sm" variant="transparent" @click="deleteTraining(data.item)"><b-icon icon="trash-fill" font-scale="1.25" variant="danger"/></b-button>
                                <b-button class="my-0 py-0" size="sm" variant="transparent" @click="editTraining(data.item)"><b-icon icon="pencil-square" font-scale="1.25" variant="primary"/></b-button>
                            </template>
                            
                    </b-table> 
                </b-card> 
            </div>                                     
        </b-card>
        
    </div>
</template>

<script lang="ts">
    import { Component, Vue, Prop } from 'vue-property-decorator';
    import moment from 'moment-timezone';
    import {teamMemberInfoType, trainingInfoType,trainingTypeInfoType} from '../../../types/MyTeam';
    
    import { namespace } from 'vuex-class';
    import "@store/modules/CommonInformation"; 
    const commonState = namespace("CommonInformation");

    import "@store/modules/TeamMemberInformation"; 
    const TeamMemberState = namespace("TeamMemberInformation");

    @Component
    export default class TrainingTab extends Vue {

        @commonState.State
        public token!: string;

        @TeamMemberState.State
        public userToEdit!: teamMemberInfoType;

        @TeamMemberState.Action
        public UpdateUserToEdit!: (userToEdit: teamMemberInfoType) => void

        selectedTrainingType = {} as trainingTypeInfoType | undefined;
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

        addNewTraining = false;
        trainingError = false;
        trainingErrorMsg = '';
        trainingErrorMsgDesc = '';
        updateTable=0;
        
        assignedTrainings: trainingInfoType[] = [];
        trainingTypes: trainingTypeInfoType[] =[];

        fields =  
        [     
            {key:'isFullDay', label:'Day',sortable:false, tdClass: 'border-top', }, 
            {key:'trainingType', label:'Type',sortable:false, tdClass: 'border-top', }, 
            {key:'startDate', label:'Start Date',  sortable:false, tdClass: 'border-top', thClass:'',},
            {key:'startTime', label:'Start Time',  sortable:false, tdClass: 'border-top', thClass:'h6 align-middle',},
            {key:'endDate',   label:'End Date',  sortable:false, tdClass: 'border-top', thClass:'',},
            {key:'endTime',   label:'End Time',  sortable:false, tdClass: 'border-top', thClass:'h6 align-middle',},
            {key:'expiryDate',label:'Expiry Date',  sortable:false, tdClass: 'border-top', thClass:'',},    
            {key:'edit',      label:'',  sortable:false, tdClass: 'border-top', thClass:'',},       
        ];

        mounted()
        {             
            console.log('TrainingTab')
            this.GetTrainings();
            
        }
   
        public GetTrainings(){
            const url = 'api/managetypes?codeType=TrainingType'
            const options = {headers:{'Authorization' :'Bearer '+this.token}}
            this.$http.get(url, options)
                .then(response => {
                    console.log(response)
                    if(response.data){
                        this.trainingTypes = response.data   
                        this.extractTrainings();                                        
                    }                                   
                })
        }

        public extractTrainings ()
        {
            console.log(this.userToEdit)
            if(this.userToEdit.training)
                for(const training of this.userToEdit.training)
                {
                    const assignedTraining = {} as trainingInfoType;

                    assignedTraining.id = training.id;
                    assignedTraining.trainingType = training.trainingType;
                    assignedTraining.trainingTypeId = training.trainingTypeId;
                    assignedTraining.startDate = moment(training.startDate).tz("UTC").format();
                    assignedTraining.endDate = moment(training.endDate).tz("UTC").format();
                    assignedTraining.expiryDate = training.trainingCertificationExpiry;
                    assignedTraining.comment = training.comment 

                    if(this.isDateFullday(assignedTraining.startDate,assignedTraining.endDate)){ 
                        assignedTraining.isFullDay = true;
                        assignedTraining['_cellVariants'] = {isFullDay:'danger'}                 
                    }else{
                        assignedTraining.isFullDay = false;
                        assignedTraining['_cellVariants'] = {isFullDay:'success'}                    
                    }
                    
                    this.assignedTrainings.push(assignedTraining)
                }
        }

        public isDateFullday(startDate, endDate){
            const start = moment(startDate); 
            const end = moment(endDate);
            const duration = moment.duration(end.diff(start));
            if(duration.asMinutes() < 1440 && duration.asMinutes()> -1440 )  return false;  else return true;
        }

        public updateUser(){
            const user = this.userToEdit            
            this.UpdateUserToEdit(user);
            console.log(user)
            this.$emit('change') 
        }

        public deleteTraining(training){
            console.log('delete training')            
            console.log(training)

            this.trainingError = false;
            const url = 'api/sheriff/training?id='+training.id;
            const options = {headers:{'Authorization' :'Bearer '+this.token}}
            this.$http.delete(url, options)
                .then(response => {
                    console.log(response)
                    console.log('delete success')
                    const index = this.assignedTrainings.findIndex(assignedtraining=>{if(assignedtraining.id == training.id) return true;})
                    if(index>=0) this.assignedTrainings.splice(index,1);
                    this.$emit('change');
                }, err=>{
                    const errMsg = err.response.data.error;
                    this.trainingErrorMsg = errMsg.slice(0,60) + (errMsg.length>60?' ...':'');
                    this.trainingErrorMsgDesc = errMsg;
                    this.trainingError = true;
                });
        }

        public editTraining(training){
            console.log('edit training')
        }


        public saveTraining(){
                this.trainingError   = false; 
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
                        sheriffId: this.userToEdit.id,
                        comment: this.selectedTrainingTypeComment
                    }

                    console.log(body)
                    const url = 'api/sheriff/training'  
                    const options = {headers:{'Authorization' :'Bearer '+this.token}}
                    this.$http.post(url, body, options)
                        .then(response => {
                            console.log(response)
                            console.log('assign success')
                            this.addToAssignedTrainingList(response.data);
                            this.clearTrainingSelection();
                        }, err=>{   
                            //console.log(err.response.data);
                            const errMsg = err.response.data.error;
                            this.trainingErrorMsg = errMsg.slice(0,60) + (errMsg.length>60?' ...':'');
                            this.trainingErrorMsgDesc = errMsg;
                            this.trainingError = true;
                        });
                }
        }

        public addToAssignedTrainingList(addedTrainingInfo)
        {
            const assignedTraining: trainingInfoType =
            {
                id: addedTrainingInfo.id,
                trainingType: addedTrainingInfo.trainingType,
                trainingTypeId: addedTrainingInfo.trainingTypeId,
                sheriffId : addedTrainingInfo.sheriffId,
                startDate: addedTrainingInfo.startDate,
                endDate: addedTrainingInfo.endDate,
                expiryDate: addedTrainingInfo.trainingCertificationExpiry,
                comment: addedTrainingInfo.comment              
            }
            
            if(this.isDateFullday(assignedTraining.startDate,assignedTraining.endDate)){ 
                assignedTraining['isFullDay'] = true;
                assignedTraining['_cellVariants'] = {isFullDay:'danger'}                 
            }else{
                assignedTraining['isFullDay'] = false;
                assignedTraining['_cellVariants'] = {isFullDay:'success'}                    
            }

            this.assignedTrainings.push(assignedTraining); 
            this.$emit('change');                     
        }

        public clearTrainingSelection(){
            this.selectedTrainingType = {} as trainingTypeInfoType | undefined;
            this.selectedTrainingTypeComment = '';
            this.selectedEndDate = '';
            this.selectedStartDate = '';
            this.selectedStartTime = '';
            this.selectedEndTime = '';
            this.addNewTraining= false;

            this.trainingTypeState  = true;
            this.trainingTypeCommentState = true;
            this.endDateState   = true;
            this.startDateState = true;
            this.startTimeState = true;
            this.endTimeState   = true;
            this.expiryDateState = true;
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
    .card {
        border: white;
    }
    td {
        margin: 0rem 1rem 0.1rem 0rem;
        padding: 0rem 1rem 0.1rem 0rem;
        
        background-color: whitesmoke ;
    }
</style>