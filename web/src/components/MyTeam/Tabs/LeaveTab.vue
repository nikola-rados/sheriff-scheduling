<template>
    <div>
        <b-card v-if="leaveTabDataReady" style="height:400px;overflow: auto;" no-body>                                        
            <b-card id="LeaveError" no-body>
                <h2 v-if="leaveError" class="mx-1 mt-2"><b-badge v-b-tooltip.hover :title="leaveErrorMsgDesc"  variant="danger"> {{leaveErrorMsg}} <b-icon class="ml-3" icon = x-square-fill @click="leaveError = false" /></b-badge></h2>
            </b-card>
            <b-card  v-if="!addNewLeave">                
                <b-button size="sm" variant="success" @click="addNewLeave = true"> <b-icon icon="plus" /> Add </b-button>
            </b-card> 

            <b-card v-if="addNewLeave" class="my-3" border-variant="light" no-body>
                <b-table-simple small borderless >
                    <b-tbody>
                        <b-tr>
                            <b-td>   
                                <b-tr class="mt-1">   
                                    <b class="ml-3" v-if="!selectedStartDate || !selectedEndDate" >Full/Partial Day Leave: </b>                          
                                    <b class="ml-3" style="background-color: #e8b5b5" v-else-if="isFullDay" >Full Day Leave: </b> 
                                    <b class="ml-3" style="background-color: #aed4bc" v-else >Partial Day Leave: </b>
                                </b-tr>
                                <b-tr >
                                    <b-form-group style="margin: 0.25rem 0 0 0.75rem;width: 15rem"> 
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
                                    @click="clearLeaveSelection()">
                                    Cancel
                                </b-button>   
                                <b-button                                    
                                    style="width:70px;margin: 2rem 0 0 0; padding:0 1rem 0 1rem; "
                                    variant="success"                        
                                    @click="saveLeave()">
                                    Save
                                </b-button> 
                            </b-td>
                        </b-tr>   
                    </b-tbody>
                </b-table-simple>              
            </b-card>

            <div>
                <b-card no-body border-variant="white" bg-variant="white" v-if="!assignedLeaves.length">
                        <span class="text-muted ml-4 my-5">No leaves have been assigned.</span>
                </b-card>

                <b-card v-else  no-body border-variant="light" bg-variant="white">
                    <b-table
                        :items="assignedLeaves"
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
                            <template v-slot:cell(leaveName)="data" >
                                <span
                                    v-b-tooltip.hover.right                                
                                    :title="data.item.comment?data.item.comment:(data.item.leaveType?data.item.leaveType.code:'')"
                                    class="text-primary">
                                    {{data.item.leaveName}}</span>
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
                            <template v-slot:cell(editLeave)="data" >                                       
                                <b-button class="my-0 py-0" size="sm" variant="transparent" @click="confirmDeleteLeave(data.item)"><b-icon icon="trash-fill" font-scale="1.25" variant="danger"/></b-button>
                                <b-button class="my-0 py-0" size="sm" variant="transparent" @click="editLeave(data.item)"><b-icon icon="pencil-square" font-scale="1.25" variant="primary"/></b-button>
                            </template>
                    </b-table> 
                </b-card> 
            </div>                                     
        </b-card>
        <b-modal v-model="confirmDelete" id="bv-modal-confirm-delete" header-class="bg-warning text-light">
            <template v-slot:modal-title>
                    <h2 class="mb-0 text-light">Confirm Delete Leave</h2>                    
            </template>
            <p>Are you sure you want to delete the "{{leaveToDelete.leaveType?leaveToDelete.leaveType.description:''}}" leave?</p>
            <template v-slot:modal-footer>
                <b-button variant="danger" @click="deleteLeave()">Delete</b-button>
                <b-button variant="primary" @click="$bvModal.hide('bv-modal-confirm-delete')">Cancel</b-button>
            </template>            
            <template v-slot:modal-header-close>                 
                <b-button variant="outline-warning" class="text-light closeButton" @click="$bvModal.hide('bv-modal-confirm-delete')"
                >&times;</b-button>
            </template>
        </b-modal>        
    </div>
</template>

<script lang="ts">
    import { Component, Vue } from 'vue-property-decorator';
    import moment from 'moment-timezone';    
    import { namespace } from 'vuex-class';
    import "@store/modules/CommonInformation";
    const commonState = namespace("CommonInformation"); 
    import "@store/modules/TeamMemberInformation";
    const TeamMemberState = namespace("TeamMemberInformation");
    import {teamMemberInfoType, userLeaveInfoType} from '../../../types/MyTeam';
    import {leaveInfoType} from '../../../types/common';
    import { leaveTypeJson } from '../../../types/common/jsonTypes';

    @Component
    export default class LeaveTab extends Vue {

        @commonState.State
        public token!: string;

        @TeamMemberState.State
        public userToEdit!: teamMemberInfoType;                

        leaveTypeInfoList: leaveInfoType[] = [];
        leaveTabDataReady = false;
        selectedLeave = {} as leaveInfoType | undefined;
        leaveState = true;

        selectedEndDate = ''
        endDateState = true

        selectedStartDate = ''
        startDateState = true

        selectedStartTime = ''
        startTimeState = true

        selectedEndTime = ''
        endTimeState = true

        addNewLeave = false;
        leaveError = false;
        leaveErrorMsg = '';
        leaveErrorMsgDesc = '';
        updateTable=0;

        confirmDelete = false;
        leaveToDelete = {} as userLeaveInfoType;
        
        assignedLeaves: userLeaveInfoType[] = [];

        fields =  
        [     
            {key:'isFullDay', label:'Type',sortable:false, tdClass: 'border-top' },       
            {key:'leaveName', label:'Leave',sortable:false, tdClass: 'border-top'  }, 
            {key:'startDate', label:'Start Date',  sortable:false, tdClass: 'border-top', thClass:'',},
            {key:'startTime', label:'Start Time',  sortable:false, tdClass: 'border-top', thClass:'h6 align-middle',},
            {key:'endDate',   label:'End Date',  sortable:false, tdClass: 'border-top', thClass:'',},
            {key:'endTime',   label:'End Time',  sortable:false, tdClass: 'border-top', thClass:'h6 align-middle',},  
            {key:'editLeave', label:'',  sortable:false, tdClass: 'border-top', thClass:'',},       
        ];

        mounted()
        {
            this.leaveTabDataReady = false;
            this.extractLeaves();          
        }

        public extractLeaves ()
        {
            const assignedLeavesJson = this.userToEdit.leave? this.userToEdit.leave: [];
            for(const leaveJson of assignedLeavesJson){                
                const leave = {} as userLeaveInfoType;
                leave.id = leaveJson.id;
                leave.leaveType = leaveJson.leaveType;
                leave.leaveTypeId = leaveJson.leaveTypeId;
                leave.leaveName = leaveJson.leaveType?leaveJson.leaveType.description: '';
                leave.comment = leaveJson.comment?leaveJson.comment:'';               

                if(this.isDateFullday(leaveJson.startDate,leaveJson.endDate)){ 
                    leave.isFullDay = true;
                    leave['_cellVariants'] = {isFullDay:'danger'}                 
                } else{
                    leave.isFullDay = false;
                    leave['_cellVariants'] = {isFullDay:'success'}                    
                }
                
                leave.startDate = moment(leaveJson.startDate).tz("UTC").format();
                leave.endDate = moment(leaveJson.endDate).tz("UTC").format();
                this.assignedLeaves.push(leave);               
            }
            this.loadLeaveTypes();
        }

        public loadLeaveTypes() {
            const url = 'api/managetypes?codeType=LeaveType'
            const options = {headers:{'Authorization' :'Bearer '+this.token}}
            this.$http.get(url, options)
                .then(response => {
                    if(response.data){
                        this.extractLeaveTypeInfo(response.data);                        
                    }                   
                })
        }

        public extractLeaveTypeInfo(leaveTypeListJson){
            let leaveType: leaveTypeJson;
            for(leaveType of leaveTypeListJson){
                const leaveTypeInfo = {} as leaveInfoType;
                leaveTypeInfo.id = leaveType.id;
                leaveTypeInfo.code = leaveType.code;
                leaveTypeInfo.description = leaveType.description;
                this.leaveTypeInfoList.push(leaveTypeInfo)
            } 
            
            this.leaveTabDataReady = true; 
        }

        public isDateFullday(startDate, endDate){
            const start = moment(startDate); 
            const end = moment(endDate);
            const duration = moment.duration(end.diff(start));
            if(duration.asMinutes() < 1440 && duration.asMinutes()> -1440 )  return false;  else return true;
        }

        public saveLeave() {
                this.leaveError  = false; 
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
                    this.leaveState     = true;
                    this.startDateState = true;
                    this.endDateState   = true;
                    this.endTimeState   = true;
                    this.startTimeState = false;
                }else{
                    this.leaveState     = true;
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
                        sheriffId: this.userToEdit.id,
                    }
                    const url = 'api/sheriff/leave'  
                    const options = {headers:{'Authorization' :'Bearer '+this.token}}
                    this.$http.post(url, body, options)
                        .then(response => {
                            console.log(response)
                            this.addToLeaveList(response.data);
                            this.clearLeaveSelection();
                        }, err=>{   
                            //console.log(err.response.data);
                            const errMsg = err.response.data.error;
                            this.leaveErrorMsg = errMsg.slice(0,60) + (errMsg.length>60?' ...':'');
                            this.leaveErrorMsgDesc = errMsg;
                            this.leaveError = true;
                        });
                }
        }

        public addToLeaveList(addedLeaveInfo){
            const leave = {} as userLeaveInfoType;
            leave.id = addedLeaveInfo.id;
            leave.leaveType = addedLeaveInfo.leaveType;
            leave.leaveTypeId = addedLeaveInfo.leaveTypeId; 
            leave.leaveName = addedLeaveInfo.leaveType.description;
            leave.startDate = addedLeaveInfo.startDate;
            leave.endDate = addedLeaveInfo.endDate;
            leave.comment = addedLeaveInfo.comment? addedLeaveInfo.comment:'';
            
            if(this.isDateFullday(leave.startDate,leave.endDate)){ 
                leave.isFullDay = true;
                leave['_cellVariants'] = {isFullDay:'danger'}                 
            }else{
                leave.isFullDay = false;
                leave['_cellVariants'] = {isFullDay:'success'}                    
            }

            this.assignedLeaves.push(leave); 
            this.$emit('change');                     
        }

        public clearLeaveSelection() {
            this.selectedLeave = {} as leaveInfoType | undefined;
            this.selectedEndDate = '';
            this.selectedStartDate = '';
            this.selectedStartTime = '';
            this.selectedEndTime = '';
            this.addNewLeave= false;

            this.leaveState     = true;
            this.endDateState   = true;
            this.startDateState = true;
            this.startTimeState = true;
            this.endTimeState   = true;
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

        public editLeave() {
            console.log("editing")
        }

        public confirmDeleteLeave(leave) {
            this.leaveToDelete = leave;           
            this.confirmDelete=true; 
        }

        public deleteLeave() {
            console.log('delete leave')
            this.confirmDelete = false;    

            this.leaveError = false; 
            const url = 'api/sheriff/leave?id='+this.leaveToDelete.id;
            const options = {headers:{'Authorization' :'Bearer '+this.token}}
            this.$http.delete(url, options)
                .then(response => {
                    console.log(response)
                    console.log('delete success')
                    const index = this.assignedLeaves.findIndex(assignedleave=>{if(assignedleave.id == this.leaveToDelete.id) return true;})
                    if(index>=0) this.assignedLeaves.splice(index,1);
                    this.$emit('change');
                }, err=>{
                    const errMsg = err.response.data.error;
                    this.leaveErrorMsg = errMsg.slice(0,60) + (errMsg.length>60?' ...':'');
                    this.leaveErrorMsgDesc = errMsg;
                    this.leaveError = true;
                });
        
           
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