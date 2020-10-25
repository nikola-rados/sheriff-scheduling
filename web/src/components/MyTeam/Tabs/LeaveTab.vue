<template>
    <div>
        <b-card v-if="leaveTabDataReady" style="height:400px;overflow: auto;" no-body>                                        
            <b-card id="LeaveError" no-body>
                <h2 v-if="leaveError" class="mx-1 mt-2"><b-badge v-b-tooltip.hover :title="leaveErrorMsgDesc"  variant="danger"> {{leaveErrorMsg}} <b-icon class="ml-3" icon = x-square-fill @click="leaveError = false" /></b-badge></h2>
            </b-card>
            <b-card  v-if="!addNewLeaveForm">                
                <b-button size="sm" variant="success" @click="addNewLeave"> <b-icon icon="plus" /> Add </b-button>
            </b-card>

            <b-card v-if="addNewLeaveForm" id="addLeaveForm" class="my-3" :border-variant="addFormColor" style="border:2px solid" body-class="m-0 px-0 py-1">
                <add-leave-form :formData="{}" :isCreate="true" :leaveTypeInfoList="leaveTypeInfoList" v-on:submit="saveLeave" v-on:cancel="closeLeaveForm" />              
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
                                <b-button class="my-0 py-0" size="sm" variant="transparent" @click="editLeave(data)"><b-icon icon="pencil-square" font-scale="1.25" variant="primary"/></b-button>
                            </template>

                            <template v-slot:row-details="data">
                                <b-card :id="'Le-Date-'+data.item.startDate.substring(0,10)" body-class="m-0 px-0 py-1" :border-variant="addFormColor" style="border:2px solid">
                                    <add-leave-form :formData="data.item" :isCreate="false" :leaveTypeInfoList="leaveTypeInfoList" v-on:submit="saveLeave" v-on:cancel="closeLeaveForm" />
                                </b-card>
                            </template>
                    </b-table> 
                </b-card> 
            </div>                                     
        </b-card>
        <b-modal v-model="confirmDelete" id="bv-modal-confirm-delete" header-class="bg-warning text-light">
            <template v-slot:modal-title>
                    <h2 class="mb-0 text-light">Confirm Delete Leave</h2>                    
            </template>
            <h4>Are you sure you want to delete the "{{leaveToDelete.leaveType?leaveToDelete.leaveType.description:''}}" leave?</h4>
            <b-form-group style="margin: 0; padding: 0; width: 20rem;"><label class="ml-1">Reason for Deletion:</label> 
                <b-form-select
                    size = "sm"
                    v-model="leaveDeleteReason">
                        <b-form-select-option value="OPERDEMAND">
                            Cover Operational Demands
                        </b-form-select-option>
                        <b-form-select-option value="PERSONAL">
                            Personal Decision
                        </b-form-select-option>
                        <b-form-select-option value="ENTRYERR">
                            Entry Error
                        </b-form-select-option>     
                </b-form-select>
            </b-form-group>
            <template v-slot:modal-footer>
                <b-button variant="danger" @click="deleteLeave()" :disabled="leaveDeleteReason.length == 0">Delete</b-button>
                <b-button variant="primary" @click="cancelDeletion()">Cancel</b-button>
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
    import AddLeaveForm from './AddForms/AddLeaveForm.vue'
    import "@store/modules/TeamMemberInformation";
    const TeamMemberState = namespace("TeamMemberInformation");
    import {teamMemberInfoType, userLeaveInfoType} from '../../../types/MyTeam';
    import {leaveInfoType} from '../../../types/common';
    import { leaveTypeJson } from '../../../types/common/jsonTypes';

    @Component({
        components: {
            AddLeaveForm
        }        
    })  
    export default class LeaveTab extends Vue {        

        @TeamMemberState.State
        public userToEdit!: teamMemberInfoType;                

        leaveTypeInfoList: leaveInfoType[] = [];
        leaveTabDataReady = false;
        

        addNewLeaveForm = false;
        addFormColor = 'secondary';
        latestEditData;
        isEditOpen = false;

        leaveError = false;
        leaveErrorMsg = '';
        leaveErrorMsgDesc = '';
        updateTable=0;

        confirmDelete = false;
        leaveToDelete = {} as userLeaveInfoType;
        
        assignedLeaves: userLeaveInfoType[] = [];
        timezone = 'UTC';
        leaveDeleteReason = '';

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
            this.timezone = this.userToEdit.homeLocation? this.userToEdit.homeLocation.timezone :'UTC';
            this.leaveTabDataReady = false;
            this.extractLeaves();                     
        }

        public extractLeaves () {   
                     
            const assignedLeavesJson = this.userToEdit.leave? this.userToEdit.leave: [];
            for(const leaveJson of assignedLeavesJson){                
                const leave = {} as userLeaveInfoType;
                leave.id = leaveJson.id;
                leave.leaveType = leaveJson.leaveType;
                leave.leaveTypeId = leaveJson.leaveTypeId;
                leave.leaveName = leaveJson.leaveType?leaveJson.leaveType.description: '';
                leave.comment = leaveJson.comment?leaveJson.comment:'';               

                if(Vue.filter('isDateFullday')(leaveJson.startDate,leaveJson.endDate)){ 
                    leave.isFullDay = true;
                    leave['_cellVariants'] = {isFullDay:'danger'}                 
                } else{
                    leave.isFullDay = false;
                    leave['_cellVariants'] = {isFullDay:'success'}                    
                }
                
                leave.startDate = moment(leaveJson.startDate).tz(this.timezone).format();
                leave.endDate = moment(leaveJson.endDate).tz(this.timezone).format();
                this.assignedLeaves.push(leave);               
            }
            
            this.loadLeaveTypes();
        }

        public loadLeaveTypes() {
            const url = 'api/managetypes?codeType=LeaveType';
            this.$http.get(url)
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

        public addNewLeave(){
            if(this.isEditOpen){
                location.href = '#Le-Date-'+this.latestEditData.item.startDate.substring(0,10)
                this.addFormColor = 'danger'
            }else{
                this.addNewLeaveForm = true;
                this.$nextTick(()=>{location.href = '#addLeaveForm';})
            }
        }

        public saveLeave(body, iscreate) {
            this.leaveError  = false; 
            body['sheriffId']= this.userToEdit.id;
            const method = iscreate? 'post' :'put';            
            const url = 'api/sheriff/leave'  
            const options = { method: method, url:url, data:body}
            
            this.$http(options)
                .then(response => {
                    if(iscreate) 
                        this.addToLeaveList(response.data);
                    else
                        this.modifyAssignedLeaveList(response.data);
                    this.closeLeaveForm();
                }, err=>{
                    const errMsg = err.response.data.error;
                    this.leaveErrorMsg = errMsg.slice(0,60) + (errMsg.length>60?' ...':'');
                    this.leaveErrorMsgDesc = errMsg;
                    this.leaveError = true;
                    location.href = '#LeaveError'
                });
        }

        public modifyAssignedLeaveList(modifiedLeaveInfo){

            const index = this.assignedLeaves.findIndex(assignedleave =>{ if(assignedleave.id == modifiedLeaveInfo.id) return true})
            if(index>=0){
                this.assignedLeaves[index].id =  modifiedLeaveInfo.id;
                this.assignedLeaves[index].startDate = moment(modifiedLeaveInfo.startDate).tz(this.timezone).format();
                this.assignedLeaves[index].endDate = moment(modifiedLeaveInfo.endDate).tz(this.timezone).format();
                this.assignedLeaves[index].leaveTypeId = modifiedLeaveInfo.leaveTypeId; 
                const leaveType = this.getLeaveType(this.assignedLeaves[index].leaveTypeId)               
                this.assignedLeaves[index].leaveType = leaveType;                
                this.assignedLeaves[index].leaveName = leaveType.description;
                this.assignedLeaves[index].comment = modifiedLeaveInfo.comment?modifiedLeaveInfo.comment:'';
                if(Vue.filter('isDateFullday')( this.assignedLeaves[index].startDate, this.assignedLeaves[index].endDate)){ 
                    this.assignedLeaves[index]['isFullDay'] = true;
                    this.assignedLeaves[index]['_cellVariants'] = {isFullDay:'danger'}                 
                }else{
                    this.assignedLeaves[index]['isFullDay'] = false;
                    this.assignedLeaves[index]['_cellVariants'] = {isFullDay:'success'}                    
                }
                this.$emit('change');
            } 
        }

        public addToLeaveList(addedLeaveInfo){
            const leave = {} as userLeaveInfoType;
            leave.id = addedLeaveInfo.id;
            leave.leaveType = addedLeaveInfo.leaveType;
            leave.leaveTypeId = addedLeaveInfo.leaveTypeId; 
            leave.leaveName = addedLeaveInfo.leaveType.description;
            leave.startDate = moment(addedLeaveInfo.startDate).tz(this.timezone).format();
            leave.endDate = moment(addedLeaveInfo.endDate).tz(this.timezone).format();
            leave.comment = addedLeaveInfo.comment? addedLeaveInfo.comment:'';
            
            if(Vue.filter('isDateFullday')(leave.startDate,leave.endDate)){ 
                leave.isFullDay = true;
                leave['_cellVariants'] = {isFullDay:'danger'}                 
            }else{
                leave.isFullDay = false;
                leave['_cellVariants'] = {isFullDay:'success'}                    
            }

            this.assignedLeaves.push(leave); 
            this.$emit('change');                     
        }

        public closeLeaveForm() {                     
            this.addNewLeaveForm= false; 
            this.addFormColor = 'secondary'
            if(this.isEditOpen){
                this.latestEditData.toggleDetails();
                this.isEditOpen = false;
            } 
        }

        public getLeaveType(leaveTypeId: number|null){
            const index = this.leaveTypeInfoList.findIndex(leave=>{if(leave.id == leaveTypeId)return true})
            if(index>=0){
                return this.leaveTypeInfoList[index]
            }
            else
                return {} as leaveInfoType;
        }       

        public editLeave(data) {
            if(this.addNewLeaveForm){
                location.href = '#addLeaveForm'
                this.addFormColor = 'danger'
            }else if(this.isEditOpen){
                location.href = '#Le-Date-'+this.latestEditData.item.startDate.substring(0,10)
                this.addFormColor = 'danger'               
            }else if(!this.isEditOpen && !data.detailsShowing){
                data.toggleDetails();
                this.isEditOpen = true;
                this.latestEditData = data
                Vue.nextTick().then(()=>{
                    location.href = '#Le-Date-'+this.latestEditData.item.startDate.substring(0,10)
                });
            }   
        }

        public confirmDeleteLeave(leave) {
            this.leaveToDelete = leave;           
            this.confirmDelete=true; 
        }

        public cancelDeletion() {
            this.confirmDelete = false;
            this.leaveDeleteReason = '';
        }

        public deleteLeave() {
            if (this.leaveDeleteReason.length) {
                this.confirmDelete = false;
                this.leaveError = false; 
                const url = 'api/sheriff/leave?id='+this.leaveToDelete.id+'&expiryReason='+this.leaveDeleteReason;
                this.$http.delete(url)
                    .then(response => {
                        const index = this.assignedLeaves.findIndex(assignedleave=>{if(assignedleave.id == this.leaveToDelete.id) return true;})
                        if(index>=0) this.assignedLeaves.splice(index,1);
                        this.$emit('change');
                    }, err=>{
                        const errMsg = err.response.data.error;
                        this.leaveErrorMsg = errMsg.slice(0,60) + (errMsg.length>60?' ...':'');
                        this.leaveErrorMsgDesc = errMsg;
                        this.leaveError = true;
                    });
                    this.leaveDeleteReason = '';
            }           
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