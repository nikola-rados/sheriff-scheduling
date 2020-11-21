<template>
    <div class="grid">
        <div v-for="i in 96" :key="i" :style="{backgroundColor: '#F9F9F9', gridColumnStart: i,gridColumnEnd:(i+1), gridRow:'1/7'}"></div>
        <div
            @click="editDuty()"
            v-for="block in dutyBlocks" 
            :key="block.id"
            :id="block.id"
            :style="{gridColumnStart: block.startTime, gridColumnEnd:block.endTime, gridRow:block.height,  backgroundColor: block.color, fontSize:'9px', textAlign: 'center', margin:0, padding:0  }"
            v-b-tooltip.hover                            
            :title="block.title" 
            @dragover.prevent
            @drop.prevent="drop" >
                <b style="text-transform: capitalize; margin:  0 padding:0; color:white;">
                    {{block.title|truncate(block.endTime - block.startTime-1)}}
                </b>     
        </div>
    
        <b-modal v-model="assignDutyError" size="lg" id="bv-modal-assign-duty-error" header-class="bg-warning text-light">
            <b-row id="AssignDutyError" class="h4 mx-2 py-2">
				<span class="p-0">{{assignDutyErrorMsg}}</span>
            </b-row>
            <template v-slot:modal-title>
                <h2 class="mb-0 text-light">Assign Duty Issues</h2>                   
            </template>            
            <template v-slot:modal-footer>
                <b-button variant="primary" @click="$bvModal.hide('bv-modal-assign-duty-error')">Ok</b-button>
            </template>            
            <template v-slot:modal-header-close>                 
                <b-button variant="outline-warning" class="text-light closeButton" @click="$bvModal.hide('bv-modal-assign-duty-error')"
                >&times;</b-button>
            </template>
        </b-modal>

        <b-modal v-model="confirmAssignOverTimeDuty" id="bv-modal-confirm-assign-overtime" header-class="bg-warning text-light">
			<b-row v-if="assignDutyError" id="AssignDutyError" class="h4 mx-2">
				<b-badge class="mx-1 mt-2"
					style="width: 20rem;"
					v-b-tooltip.hover
					:title="assignDutyErrorMsg"
					variant="danger"> {{assignDutyErrorMsg | truncate(40)}}
					<b-icon class="ml-3"
						icon = x-square-fill
						@click="assignDutyError = false"
				/></b-badge>                    
            </b-row>            
            <template v-slot:modal-title>
                <h2 class="mb-0 text-light">Confirm Assign Overtime</h2>                   
            </template>
            <h4 >Are you sure you want to assign a duty that extends the team member's shift?</h4>
            <template v-slot:modal-footer>
                <b-button variant="danger" @click="confirmedAssignOverTimeDuty()">Confirm</b-button>
                <b-button variant="primary" @click="$bvModal.hide('bv-modal-confirm-assign-overtime')">Cancel</b-button>
            </template>            
            <template v-slot:modal-header-close>                 
                <b-button variant="outline-warning" class="text-light closeButton" @click="$bvModal.hide('bv-modal-confirm-assign-overtime')"
                >&times;</b-button>
            </template>
        </b-modal>

        <b-modal v-model="showEditDutyDetails" id="bv-modal-edit-duty-details" header-class="bg-primary text-light">
			<template v-slot:modal-title>
				<h2 class="mb-0 text-light"> Editing Duty </h2>
			</template>

			<b-card v-if="isDutyDataMounted" no-body style="font-size: 14px;user-select: none;" >

				<b-card id="EditDutyError" no-body>
					<h2 v-if="editDutyError" class="mx-1 mt-2"
						><b-badge v-b-tooltip.hover
							:title="editDutyMsg"
							variant="danger"> {{editDutyMsg | truncate(40)}}
							<b-icon class="ml-3"
								icon = x-square-fill
								@click="editDutyError = false"
					/></b-badge></h2>
				</b-card>             
                <b-card  v-if="!addNewDutySlotForm">                
                    <b-button size="sm" variant="success" @click="addNewDutySlot"> <b-icon icon="plus" /> Add </b-button>
                </b-card>

                <b-card v-if="addNewDutySlotForm" id="addDutySlotForm" class="my-3" :border-variant="addFormColor" style="border:2px solid" body-class="m-0 px-0 py-1">
                    <add-duty-slot-form :formData="{}" :isCreate="true" v-on:submit="saveDutySlot" v-on:cancel="closeDutySlotForm" />              
                </b-card>

                <div>
                    <b-card no-body border-variant="white" bg-variant="white" v-if="!dutyBlocks.length">
                            <span class="text-muted ml-4 my-5">No Sheriffs have been assigned.</span>
                    </b-card>

                    <b-card v-else  no-body border-variant="light" bg-variant="white">
                        <b-table
                            :items="dutyBlocks"
                            :fields="fields"
                            head-row-variant="primary"
                            striped
                            borderless
                            small
                            sort-by="startTimeString"
                            responsive="sm"
                            >  
                                
                            <template v-slot:cell(sheriffName)="data" >
                                <span
                                    v-b-tooltip.hover.right                                
                                    :title="data.item.title"
                                    class="text-primary">
                                    {{data.item.title | truncate(20)}}</span>
                            </template>
                            <template v-slot:cell(editDutySlot)="data" >                                       
                                <b-button class="my-0 py-0" size="sm" variant="transparent" @click="confirmDeleteDutySlot(data.item)"><b-icon icon="trash-fill" font-scale="1.25" variant="danger"/></b-button>
                                <b-button class="my-0 py-0" size="sm" variant="transparent" @click="editDutySlotInfo(data)"><b-icon icon="pencil-square" font-scale="1.25" variant="primary"/></b-button>
                            </template>

                            <template v-slot:row-details="data">
                                <b-card :id="'Le-Date-'+data.item.startTimeString.substring(0,10)" body-class="m-0 px-0 py-1" :border-variant="addFormColor" style="border:2px solid">
                                    <add-duty-slot-form :formData="data.item" :isCreate="false" v-on:submit="saveDutySlot" v-on:cancel="closeDutySlotForm" />
                                </b-card>
                            </template>
                        </b-table> 
                    </b-card> 
                </div>
			</b-card>

			<template v-slot:modal-footer>
				<b-button
						size="sm"
						variant="danger"
						class="mr-auto"
						@click="confirmDeleteDuty()"
				><b-icon-trash-fill style="padding:0; vertical-align: middle; margin-right: 0.25rem;"></b-icon-trash-fill>Delete</b-button>
				<b-button
						size="sm"
						variant="secondary"
						@click="closeEditDutyWindow()"
				><b-icon-x style="padding:0; vertical-align: middle; margin-right: 0.25rem;"></b-icon-x>Cancel</b-button>
				<!-- <b-button
						size="sm"
						variant="success"
						@click="saveDuty()"
				><b-icon-check2 style="padding:0; vertical-align: middle; margin-right: 0.25rem;"></b-icon-check2>Save</b-button> -->
			</template>
			<template v-slot:modal-header-close>
				<b-button
					variant="outline-primary"
					class="text-light closeButton"
					@click="closeEditDutyWindow()"
					>
					&times;</b-button>
			</template>
		</b-modal>

        <b-modal v-model="confirmDelete" id="bv-modal-confirm-delete" header-class="bg-warning text-light">
            <template v-slot:modal-title>
                    <h2 class="mb-0 text-light">Confirm Delete Duty</h2>                    
            </template>

            <b-card id="DeleteError" no-body>
					<h2 v-if="deleteError" class="mx-1 mt-2"
						><b-badge v-b-tooltip.hover
							:title="deleteErrorMsg"
							variant="danger"> {{deleteErrorMsg | truncate(40)}}
							<b-icon class="ml-3"
								icon = x-square-fill
								@click="deleteError = false"
					/></b-badge></h2>
			</b-card>



            <h4>Are you sure you want to delete the "{{assignmentName}}" duty?</h4>
            
            <template v-slot:modal-footer>
                <b-button variant="danger" @click="deleteDuty()">Confirm</b-button>
                <b-button variant="primary" @click="cancelDeletion()">Cancel</b-button>
            </template>            
            <template v-slot:modal-header-close>                 
                <b-button variant="outline-warning" class="text-light closeButton" @click="cancelDeletion()"
                >&times;</b-button>
            </template>
        </b-modal> 


    </div>
</template>

<script lang="ts">
    import { Component, Vue, Prop } from 'vue-property-decorator';
   
    import * as _ from 'underscore';
    import moment from 'moment-timezone';
    import AddDutySlotForm from './AddDutySlotForm.vue'
    import { assignDutyInfoType, assignmentCardInfoType, dutyBlockInfoType, myTeamShiftInfoType } from '../../../types/DutyRoster';

    import { namespace } from "vuex-class"; 
    import "@store/modules/DutyRosterInformation";   
    const dutyState = namespace("DutyRosterInformation");

    @Component({
        components: {
            AddDutySlotForm
        }        
    })  
    export default class DutyCard extends Vue {

        @Prop({required: true})
        dutyRosterInfo!: assignmentCardInfoType;

        @dutyState.State
        public shiftAvailabilityInfo!: myTeamShiftInfoType[];

        dutyBlocks: dutyBlockInfoType[] = []; 

        addNewDutySlotForm = false;
        addFormColor = 'secondary';

        isEditOpen = false;
        updateTable=0;

        assignDutyError = false;
        assignDutyErrorMsg = '';

        isMounted = false;

        overTimeSheriffId = '';
        overTimeTimeRangeDate = {startTime: '', endTime: ''}
        confirmAssignOverTimeDuty = false;

        assignedArray: number[] = [];
        unassignedArray: number[][] = [];
        dutyDate ='';

        assignmentName = '';
        dutyId = 0;

        showEditDutyDetails = false;
        isDutyDataMounted = false;
        showEditCancelWarning = false;
        confirmDelete = false;
        dutySlotToDelete = {} as dutyBlockInfoType;
        latestEditData;
        timezone = 'UTC';

        editDutyError = false;
        editDutyErrorMsg = '';

        deleteErrorMsg = '';
		deleteError = false;        

        fields = [      
            {key:'title', label:'Sheriff',sortable:false, tdClass: 'border-top'  },
            {key:'startTimeString', label:'Start Time',  sortable:false, tdClass: 'border-top', thClass:'h6 align-middle',},
            {key:'endTimeString',   label:'End Time',  sortable:false, tdClass: 'border-top', thClass:'h6 align-middle',},  
            {key:'editDutySlot', label:'',  sortable:false, tdClass: 'border-top', thClass:'',},       
        ];

        mounted()
        {
            console.log(this.dutyRosterInfo)
            this.assignmentName = (this.dutyRosterInfo && this.dutyRosterInfo.attachedDuty && this.dutyRosterInfo.attachedDuty.assignment)? this.dutyRosterInfo.attachedDuty.assignment.name: '';
            this.dutyId = (this.dutyRosterInfo && this.dutyRosterInfo.attachedDuty)? this.dutyRosterInfo.attachedDuty.id:0;
            this.isMounted = false;
            this.dutyBlocks = [];
            this.extractDuty();
        }

        public editDuty(){			
			this.isDutyDataMounted = false;
            console.log(this.dutyBlocks);
            this.showEditDutyDetails = true;
            this.isDutyDataMounted = true;					           
        }

        public confirmDeleteDuty(){
			this.deleteError = false;
			this.confirmDelete = true;
        }
        
        public cancelDeletion() {
            this.deleteError = false;
			this.confirmDelete = false;
        }

        public deleteDuty(){
			
            this.confirmDelete = false;
            this.deleteError = false;
            const url = 'api/dutyroster?id=' + this.dutyId;
        
            this.$http.delete(url)
                .then(response => {
                    // console.log(response);
                    this.confirmDelete = false;
                    this.$emit('change');                    
                }, err=>{
                    const errMsg = err.response.data.error;
                    console.log(err.response)
                    this.deleteErrorMsg = errMsg;
                    this.deleteError = true;
                });		
			
		}
        
        public isChanged(){
            //TODO
            return false;
		}

		public closeEditDutyWindow(){
			if(this.isChanged())
                this.showEditCancelWarning = true;
            else
                this.confirmedCloseEditAssignmentWindow();
		}

		public confirmedCloseEditAssignmentWindow(){
            this.resetDutyWindowState();
            this.showEditCancelWarning = false;
			this.showEditDutyDetails = false;
        }

		public resetDutyWindowState() {
            //TODO
            this.closeDutySlotForm();
        }
        
        public addNewDutySlot(){
            if(this.isEditOpen){
                console.log(this.latestEditData)
                location.href = '#Ds-Time-'+this.latestEditData.item.startTimeString.substring(0,10)
                this.addFormColor = 'danger'
            }else{
                this.addNewDutySlotForm = true;
                this.$nextTick(()=>{location.href = '#addDutySlotForm';})
            }
        }

        public closeDutySlotForm() {                     
            this.addNewDutySlotForm= false; 
            this.addFormColor = 'secondary';
            if(this.isEditOpen){
                this.latestEditData.toggleDetails();
                this.isEditOpen = false;
            } 
        }

        public editDutySlotInfo(data) {
            if(this.addNewDutySlotForm){
                location.href = '#addDutySlotForm'
                this.addFormColor = 'danger'
            }else if(this.isEditOpen){
                location.href = '#Ds-Time-'+this.latestEditData.item.startTimeString.substring(0,10)
                this.addFormColor = 'danger'               
            }else if(!this.isEditOpen && !data.detailsShowing){
                data.toggleDetails();
                this.isEditOpen = true;
                this.latestEditData = data
                Vue.nextTick().then(()=>{
                    location.href = '#Ds-Time-'+this.latestEditData.item.startTimeString.substring(0,10)
                });
            }   
        }

        public confirmDeleteDutySlot(dutySlot) {
            console.log(dutySlot)
            this.dutySlotToDelete = dutySlot;           
            this.confirmDelete=true; 
        }

        public cancelDutySlotDeletion() {
            this.confirmDelete = false;
        }

        public deleteDutySlot() {
            //TODO: remove the dutyslot and update the duty with remaining slots
            this.confirmDelete = false;
            this.editDutyError = false; 
            // const url = 'api/sheriff/leave?id='+this.leaveToDelete.id+'&expiryReason='+this.leaveDeleteReason;
            // this.$http.delete(url)
            //     .then(response => {
            //         const index = this.assignedLeaves.findIndex(assignedleave=>{if(assignedleave.id == this.leaveToDelete.id) return true;})
            //         if(index>=0) this.assignedLeaves.splice(index,1);
            //         this.$emit('change');
            //     }, err=>{
            //         const errMsg = err.response.data.error;
            //         this.editDutyErrorMsg = errMsg;
            //         this.editDutyError = true;
            //     });            
        }

        public saveDutySlot(body, iscreate) {
            this.editDutyError  = false;
            // TODO: add data to body 
            //body['sheriffId']= this.userToEdit.id;
                   
            const url = 'api/dutyroster'  
            
            
            this.$http.put(url, body)
                .then(response => {
                    if(iscreate) 
                        this.addToDutySlotList(response.data);
                    else
                        this.modifyDutySlotList(response.data);
                    this.closeDutySlotForm();
                }, err=>{
                    const errMsg = err.response.data.error;
                    this.editDutyErrorMsg = errMsg;
                    this.editDutyError = true;
                    location.href = '#DutyError'
                });
        }

        public modifyDutySlotList(modifiedDutySlotInfo){

            const index = this.dutyBlocks.findIndex(dutyBlock =>{ if(dutyBlock.id == modifiedDutySlotInfo.id) return true})
            if(index>=0){
                //TODO: populate edited duty slot info
                // this.assignedLeaves[index].id =  modifiedLeaveInfo.id;
                // this.assignedLeaves[index].startDate = moment(modifiedLeaveInfo.startDate).tz(this.timezone).format();
                // this.assignedLeaves[index].endDate = moment(modifiedLeaveInfo.endDate).tz(this.timezone).format();
                // this.assignedLeaves[index].leaveTypeId = modifiedLeaveInfo.leaveTypeId; 
                // const leaveType = this.getLeaveType(this.assignedLeaves[index].leaveTypeId)               
                // this.assignedLeaves[index].leaveType = leaveType;                
                // this.assignedLeaves[index].leaveName = leaveType.description;
                // this.assignedLeaves[index].comment = modifiedLeaveInfo.comment?modifiedLeaveInfo.comment:'';
                // if(Vue.filter('isDateFullday')( this.assignedLeaves[index].startDate, this.assignedLeaves[index].endDate)){ 
                //     this.assignedLeaves[index]['isFullDay'] = true;
                //     this.assignedLeaves[index]['_cellVariants'] = {isFullDay:'danger'}                 
                // }else{
                //     this.assignedLeaves[index]['isFullDay'] = false;
                //     this.assignedLeaves[index]['_cellVariants'] = {isFullDay:'success'}                    
                // }
                this.$emit('change');
            } 
        }

        public addToDutySlotList(addedSlotInfo){
            const dutySlot = {} as dutyBlockInfoType;
            //TODO: populate new slot into 
            // leave.id = addedLeaveInfo.id;
            // leave.leaveType = addedLeaveInfo.leaveType;
            // leave.leaveTypeId = addedLeaveInfo.leaveTypeId; 
            // leave.leaveName = addedLeaveInfo.leaveType.description;
            // leave.startDate = moment(addedLeaveInfo.startDate).tz(this.timezone).format();
            // leave.endDate = moment(addedLeaveInfo.endDate).tz(this.timezone).format();
            // leave.comment = addedLeaveInfo.comment? addedLeaveInfo.comment:'';
            
            

            this.dutyBlocks.push(dutySlot); 
            this.$emit('change');                     
        }

        public extractDuty(){
            if(this.dutyRosterInfo.attachedDuty){
                const dutyInfo = this.dutyRosterInfo.attachedDuty;

                const dutyStartTime = moment(dutyInfo.startDate).tz(dutyInfo.timezone);
                const startOfDay = moment(dutyStartTime).startOf("day");
                this.dutyDate = startOfDay.format();

                const dutyBin = this.getTimeRangeBins(dutyInfo.startDate, dutyInfo.endDate,startOfDay, dutyInfo.timezone);
                let unassignedArray = this.fillInArray(Array(96).fill(0),1,dutyBin.startBin, dutyBin.endBin); 
                              
                for(const dutySlot of dutyInfo.dutySlots){
                    // console.log(dutySlot)
                    let id = 1000;
                    const assignedDutyBin = this.getTimeRangeBins(dutySlot.startDate, dutySlot.endDate,startOfDay, dutySlot.timezone)
                    unassignedArray = this.fillInArray(unassignedArray,0,assignedDutyBin.startBin, assignedDutyBin.endBin);
                    const sheriff = this.shiftAvailabilityInfo.filter(sheriff=>{if(sheriff.sheriffId==dutySlot.sheriffId)return true})[0];
                    const isOvertime = this.getOverTime(dutySlot.shiftId, dutySlot.isNotAvailable, dutySlot.isNotRequired, dutySlot.isOvertime);                    
                    this.dutyBlocks.push({
                        id: 'dutySlot'+dutySlot.id+'i'+dutyInfo.id+'n'+id++,                    
                        startTime: 1+ assignedDutyBin.startBin,
                        endTime: 1+ assignedDutyBin.endBin,                    
                        color: this.getDutyColor(this.dutyRosterInfo.type.colorCode,dutySlot.isNotAvailable,dutySlot.isNotRequired, isOvertime),
                        height: '2/6',
                        title: this.getTitle(sheriff,dutySlot.isNotAvailable,dutySlot.isNotRequired),
                        lastName: sheriff.lastName,
                        firstName: sheriff.firstName,
                        sheriffId: sheriff.sheriffId,
                        startTimeString: moment(dutySlot.startDate).tz(dutySlot.timezone).format('HH:mm'),
                        endTimeString: moment(dutySlot.endDate).tz(dutySlot.timezone).format('HH:mm'),
                        timezone: dutySlot.timezone
                    })                
                }

                this.extractUnassignedArrays(unassignedArray);

                for(const unassignInx in this.unassignedArray){
                    //console.log(unassignInx)
                    // console.log(this.unassignedArray[unassignInx])
                    const unassignedBin = this.getArrayRangeBins(this.unassignedArray[unassignInx]);
                    this.dutyBlocks.push({
                        id:'dutySlot'+dutyInfo.id+'n'+unassignInx,                    
                        startTime: 1+ unassignedBin.startBin,
                        endTime: 1+ unassignedBin.endBin,                    
                        color: this.dutyRosterInfo.type.colorCode,
                        height: '2/6',
                        title: '',
                        sheriffId: '',
                        firstName: '',
                        lastName: '',
                        startTimeString: '',
                        endTimeString: '',
                        timezone: ''
                    })                
                }

                this.dutyBlocks = _.sortBy(this.dutyBlocks,'startTime')
                //console.log(unassignedArray)

                if(this.dutyBlocks.length>1)
                    for(const dutyBlockInx in this.dutyBlocks){
                        this.dutyBlocks[dutyBlockInx].height = (Number(dutyBlockInx)%2)?'2/4':'4/6'; 
                    }

                
            }
            console.log(this.dutyBlocks)

            this.isMounted = true; 
        }

        public extractUnassignedArrays(unassignedArray){
            let startBin = 0;
            for(let valueInx=0; valueInx<unassignedArray.length; valueInx++){

                if((unassignedArray[valueInx]>0 && valueInx==0)||(unassignedArray[valueInx]>0 && unassignedArray[valueInx-1]==0)) startBin = valueInx;
                
                if((unassignedArray[valueInx]>0 && valueInx==(unassignedArray.length-1))||(unassignedArray[valueInx]>0 && unassignedArray[valueInx+1]==0)){
                    //console.log(startBin)
                    //console.log(valueInx)
                    const array = this.fillInArray(Array(96).fill(0),1, startBin, valueInx+1)
                    //console.log(array)
                    this.unassignedArray.push(array)
                    //console.log(this.unassignedArray)
                } 
            }
        }

        public getDutyColor(color, isNotAvailable, isNotRequired, isOverTime) {
            if(isOverTime) return '#e85a0e';
            else if(isNotRequired) return  '#28a745';
            else if(isNotAvailable) return '#dc3545';
            else return color;
        }

        public getTitle(sheriff,isNotAvailable,isNotRequired){
            if(isNotAvailable) return 'isNotAvailable';
            else if(isNotRequired) return  'isNotRequired';
            else if(sheriff) return sheriff.lastName+', '+sheriff.firstName;
            else return ' ';
        }

        public getOverTime(shiftId, isNotAvailable, isNotRequired, isOverTime) {
            if(isOverTime) return true;
            else if(isNotRequired || isNotAvailable) return  false;
            else if(!shiftId) return true;
            else return false;
        }
        
        public drop(event: any) 
        {
            console.log(event.target.id)
            if(event.target.id){
                const cardid = event.dataTransfer.getData('text');
                const blockId: string = event.target.id;
                const unassignedBlockId = Number(blockId.substring(blockId.indexOf('n')+1));
                console.log(unassignedBlockId)
                if(unassignedBlockId>=1000) return
                // console.log(cardid)
                const sheriffId = cardid.slice(7)
                // console.log(sheriffId)
                const unassignedArray = this.unassignedArray[unassignedBlockId]

                if(sheriffId=='00000-00000-11111'||sheriffId=='00000-00000-22222'){
                    const timeRangeBins = this.getArrayRangeBins(unassignedArray);
                    const timeRangeDate = this.convertTimeRangeBinsToTime(this.dutyDate, timeRangeBins.startBin, timeRangeBins.endBin);
                    this.assignDuty(sheriffId, timeRangeDate.startTime, timeRangeDate.endTime, null);
                    return
                }

                const sheriff = this.shiftAvailabilityInfo.filter(sheriff=>{if(sheriff.sheriffId==sheriffId)return true})[0];
                const availability = sheriff.availability
                const duties = sheriff.duties
                // console.log(sheriff)
                // console.log(unassignedArray)
                // console.log(availability)
                // console.log(duties)                

                const unionUnassignAvail = this.unionArrays(unassignedArray, availability)
                console.log(unionUnassignAvail)
                
                if(this.sumOfArrayElements(unionUnassignAvail)>0){
                    console.log('call assign')
                    const timeRangeBins = this.getArrayRangeBins(unionUnassignAvail);
                    const timeRangeDate = this.convertTimeRangeBinsToTime(this.dutyDate, timeRangeBins.startBin, timeRangeBins.endBin);
                    this.assignDuty(sheriffId, timeRangeDate.startTime, timeRangeDate.endTime, timeRangeBins.binValue)
                    
                }else{
                    const unionUnassignDuties = this.unionArrays(unassignedArray, duties)
                    if(this.sumOfArrayElements(unionUnassignDuties)>0){
                        console.log('overtime conflicts')
                        this.assignDutyErrorMsg = "This team member is already assigned to conflicting duties.";
                        this.assignDutyError = true;
                    }else{
                        console.log('call assign overtime')
                        const timeRangeBins = this.getArrayRangeBins(unassignedArray);
                        this.overTimeTimeRangeDate = this.convertTimeRangeBinsToTime(this.dutyDate, timeRangeBins.startBin, timeRangeBins.endBin);
                        this.overTimeSheriffId = sheriffId;
                        this.assignDutyError = false;
                        this.assignDutyErrorMsg = '';
                        this.confirmAssignOverTimeDuty = true;
                    }
                }
            }
        }

        public confirmedAssignOverTimeDuty() {

            this.assignDuty(this.overTimeSheriffId, this.overTimeTimeRangeDate.startTime, this.overTimeTimeRangeDate.endTime, null);
        }

        public unionArrays(arrayA, arrayB){
            return arrayA.map((arr,index) =>{return arr*arrayB[index]});
        }

        // public subtractUnionOfArrays(arrayA, arrayB){
        //     return arrayA.map((arr,index) =>{return arr&&(arrayB[index]>0?0:1)});
        // }

        // public addArrays(arrayA, arrayB){
        //     return arrayA.map((arr,index) =>{return arr+arrayB[index]});
        // }

        public sumOfArrayElements(arrayA){
            return arrayA.reduce((acc, arr) => acc + arr, 0)
        }

        public fillInArray(array, fillInNum, startBin, endBin){
            return array.map((arr,index) =>{if(index>=startBin && index<endBin) return fillInNum; else return arr;});
        }

        public getArrayRangeBins(arrayOriginal){
            const array = _.clone(arrayOriginal);
            const startBin: number = array.findIndex(arr=>{if(arr>0) return true });
            const binValue: number = startBin>=0? array[startBin]: 1;
            //console.log(startBin)
            //console.log(array[startBin])
            return({
                startBin: startBin ,
                endBin: (96-array.reverse().findIndex(arr=>{if(arr>0) return true })),
                binValue: binValue
            })
        }

        public convertTimeRangeBinsToTime(dutyDate, startBin, endBin){            
            const startTime = moment(dutyDate).add(startBin*15, 'minutes').format();
            const endTime = moment(dutyDate).add(endBin*15, 'minutes').format();
            return( {startTime: startTime, endTime:endTime } )
        }

        public getTimeRangeBins(startDate, endDate, startOfDay, timezone){
            const startTime = moment(startDate).tz(timezone);
            const endTime = moment(endDate).tz(timezone);
            const startBin = moment.duration(startTime.diff(startOfDay)).asMinutes()/15;
            const endBin = moment.duration(endTime.diff(startOfDay)).asMinutes()/15;
            return( {startBin: startBin, endBin:endBin } )
        }

        public assignDuty(sheriffId, startDate, endDate, shiftId ) {

            if(this.dutyRosterInfo.attachedDuty){
                const dutyInfo = this.dutyRosterInfo.attachedDuty;
                let isNotRequired = false;
                let isNotAvailable = false;
                if(sheriffId=='00000-00000-11111'){
                    sheriffId = null;
                    isNotRequired = true;
                }else if(sheriffId=='00000-00000-22222'){
                    sheriffId = null;
                    isNotAvailable = true;
                }
                console.log(startDate)
                console.log(endDate)
                console.log(shiftId)
                console.log(dutyInfo)

                const dutySlots = [ { 
                        id: null,                      
                        startDate: startDate,
                        endDate: endDate,
                        dutyId: dutyInfo.id,
                        sheriffId: sheriffId,
                        shiftId: shiftId,
                        timezone: dutyInfo.timezone,
                        isNotRequired: isNotRequired,
                        isNotAvailable: isNotAvailable,
                        isOvertime: false
                    }
                ]

                for(const dutySlot of dutyInfo.dutySlots){
                    dutySlots.push({  
                        id: dutySlot.id,                                              
                        startDate: dutySlot.startDate,
                        endDate: dutySlot.endDate,
                        dutyId: dutySlot.dutyId,
                        sheriffId: dutySlot.sheriffId,
                        shiftId: dutySlot.shiftId,
                        timezone: dutySlot.timezone,
                        isNotRequired: dutySlot.isNotRequired,
                        isNotAvailable: dutySlot.isNotAvailable,
                        isOvertime: dutySlot.isOvertime                           
                    })                    
                }
                
                const body: assignDutyInfoType[] = [
                    {
                        id: dutyInfo.id,
                        startDate: dutyInfo.startDate,
                        endDate: dutyInfo.endDate,
                        locationId: dutyInfo.locationId,
                        assignmentId: dutyInfo.assignmentId,
                        dutySlots: dutySlots,
                        timezone: dutyInfo.timezone
                    }
                ];
                
                console.log(body)
                const url = 'api/dutyroster';
                this.$http.put(url, body )
                    .then(response => {
                        if(response.data){
                            // Update the duty bar with name;
                            this.$emit('change');
                        }
                    }, err => {
                        const errMsg = err.response.data.error;
                        this.assignDutyErrorMsg = errMsg;
                        this.assignDutyError = true;
                    })
            }
        }
        
            // console.log(this.unionArrays([0,2,2],[1,0,1]))
            // console.log(this.addArrays([0,0,2],[-1,0,0]))
            // console.log(this.sumOfArrayElements([0,1,2]))

            // console.log(this.fillInArray(Array(6).fill(0), 5 , 2,4))
            // console.log(this.subtractUnionOfArrays([1,1,1,1,0],[0,2,2,0,0]))

    }

    
</script>

<style scoped>   

    .card {
        border: white;
    }

    .grid {        
        display:grid;
        grid-template-columns: repeat(96, 1.041666%);
        grid-template-rows: repeat(6,.5rem);
        inline-size: 100%; 
        background-color: #EEEEEE; 
        height: 2.9rem; 
        margin: 0; 
        padding: 0;
        column-gap: 0;
        row-gap: 0;
           
    }

    .grid > * { 
        padding: 0 0;
        border: 1px dotted rgb(212, 212, 212);
    }

</style>