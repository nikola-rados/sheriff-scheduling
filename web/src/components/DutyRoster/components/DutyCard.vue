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
        <span v-if="localTime.isTodayInView" :style="{borderLeft:'3px solid red', gridColumnStart: localTime.timeSlot+1,gridColumnEnd:(localTime.timeSlot+2), gridRow:'1/7'}"></span>
    
        <b-modal v-model="assignDutyError" id="bv-modal-assign-duty-error" header-class="bg-warning text-light">
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
            <h4 style="line-height:1.5rem" >Are you sure you want to assign a duty that extends the team member's shift?</h4>
            <template v-slot:modal-footer>
                <b-button variant="danger" @click="confirmedAssignOverTimeDuty()">Confirm</b-button>
                <b-button variant="primary" @click="$bvModal.hide('bv-modal-confirm-assign-overtime')">Cancel</b-button>
            </template>            
            <template v-slot:modal-header-close>                 
                <b-button variant="outline-warning" class="text-light closeButton" @click="$bvModal.hide('bv-modal-confirm-assign-overtime')"
                >&times;</b-button>
            </template>
        </b-modal>

        <b-modal v-model="showEditDutyDetails" id="bv-modal-edit-duty-details" centered header-class="bg-primary text-light">
			<template v-slot:modal-title>
				<div class="h2 mb-2 text-light"> Editing Duty: </div> 
                <div style="float:left;" class="h4 ml-4 p-0 mb-0">{{assignmentName}}</div>             
                <div style="float:left;" class="h4 text-warning ml-2 p-0 mb-0"> {{fullDutyStartTime}} - {{fullDutyEndTime}}</div>                 
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

                <div>
                    <b-card no-body border-variant="light" bg-variant="white">
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
                                
                            <template v-slot:cell(title)="data" >
                                <div
                                    :style="(data.value=='Not Available'||data.value=='Not Required')?'color:'+data.item.color:''"
                                    v-b-tooltip.hover.right                                
                                    :title="data.item.title>20? data.item.title:''">
                                    {{data.item.title | truncate(20)}}</div>
                            </template>

                            <template v-slot:cell(note)="data" >
                                <div                                                                        
                                    :style="{fontSize:'12px',margin:'.11rem 0', color:data.item.color }">
                                    {{data.value}}</div>
                            </template>

                            <template v-slot:cell(editDutySlot)="data" >          
                                <b-button style="width:1.2rem;float:right" class="ml-1 mr-0 my-0 py-0" size="sm" :disabled="data.item.sheriffId?false:true" variant="transparent" @click="confirmUnassignDutySlot(data.item)"><b-icon v-if="data.item.sheriffId" icon="trash-fill" font-scale="1.25" variant="danger" style="transform: translate(-8px,0);"/></b-button>
                                <b-button style="width:.75rem;float:right" class="mx-1 my-0 py-0" size="sm" :disabled="data.item.note?true:false" variant="transparent" @click="editDutySlotInfo(data)"><b-icon v-if="!data.item.note" icon="pencil-square" font-scale="1.25" variant="primary" style="transform: translate(-8px,0);"/></b-button>
                            </template>

                            <template v-slot:row-details="data">
                                <b-card :id="'Le-Date-'+data.item.startTimeString.substring(0,10)" body-class="m-0 px-0 py-1" :border-variant="addFormColor" style="border:2px solid">
                                    <add-duty-slot-form 
                                        :formData="data.item" 
                                        :dutyBlocks="dutyBlocks"
                                        :fullDutyStartTime="fullDutyStartTime"
                                        :fullDutyEndTime="fullDutyEndTime"
                                        :startOfDay="dutyDate"
                                        :timezone="dutyTimezone" 
                                        v-on:submit="assignDuty" 
                                        v-on:cancel="closeDutySlotForm" />
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

        <b-modal v-model="confirmUnassign" id="bv-modal-confirm-unassign" header-class="bg-warning text-light">
            <template v-slot:modal-title>
                    <h2 class="mb-0 text-light">Confirm Unassign Duty</h2>                    
            </template>
            
            <h4>Are you sure you want to unassign the "{{assignmentName}}" duty from {{dutySlotToUnassign.lastName}}, {{dutySlotToUnassign.firstName}}?</h4>
            
            <template v-slot:modal-footer>
                <b-button variant="danger" @click="unassignDutySlot()">Confirm</b-button>
                <b-button variant="primary" @click="$bvModal.hide('bv-modal-confirm-unassign')">Cancel</b-button>
            </template>            
            <template v-slot:modal-header-close>                 
                <b-button variant="outline-warning" class="text-light closeButton" @click="$bvModal.hide('bv-modal-confirm-unassign')"
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
    import {dutySlotInfoType, assignDutySlotsInfoType, assignDutyInfoType, assignmentCardInfoType, dutyBlockInfoType, myTeamShiftInfoType } from '../../../types/DutyRoster';
    import {localTimeInfoType} from '../../../types/common';

    import { namespace } from "vuex-class";
    import "@store/modules/CommonInformation";
    const commonState = namespace("CommonInformation"); 
    import "@store/modules/DutyRosterInformation";   
    const dutyState = namespace("DutyRosterInformation");

    @Component({
        components: {
            AddDutySlotForm
        }        
    })  
    export default class DutyCard extends Vue {

        @commonState.State
        public localTime!: localTimeInfoType;

        @Prop({required: true})
        dutyRosterInfo!: assignmentCardInfoType;

        @dutyState.State
        public shiftAvailabilityInfo!: myTeamShiftInfoType[];

        dutyBlocks: dutyBlockInfoType[] = []; 

        fullDutyStartTime=''
        fullDutyEndTime=''

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
        dutyTimezone='';

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
        
        confirmUnassign = false;
        dutySlotToUnassign = {} as dutyBlockInfoType;

        fields = [      
            {key:'title', label:'Sheriff',sortable:false, tdClass: 'border-top'  },
            {key:'startTimeString', label:'Start Time',  sortable:false, tdClass: 'border-top', thClass:'h6 align-middle',},
            {key:'endTimeString',   label:'End Time',  sortable:false, tdClass: 'border-top', thClass:'h6 align-middle',},  
            {key:'note', label:'Note',sortable:false, tdClass: 'border-top', thClass:'h6 align-middle'  },
            {key:'editDutySlot', label:'',  sortable:false, tdClass: 'border-top', thClass:'',},       
        ];

        mounted()
        {
            //console.log(this.dutyRosterInfo)
            this.assignmentName = this.getDutyName()
            this.dutyId = (this.dutyRosterInfo && this.dutyRosterInfo.attachedDuty)? this.dutyRosterInfo.attachedDuty.id:0;
            this.isMounted = false;
            this.dutyBlocks = [];
            this.extractDuty();
            console.log(this.dutyRosterInfo.assignment)
            //if(this.dutyRosterInfo.assignment == store) Vue.nextTick(()=>this.editDuty())
            
        }

        public getDutyName(){
            if (this.dutyRosterInfo)
                return Vue.filter('capitalize')(this.dutyRosterInfo.type.name +' - '+this.dutyRosterInfo.code) +' ('+this.dutyRosterInfo.name+')'; 
            else return '';
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
        
        public confirmUnassignDutySlot(slotinfo){
            console.log(slotinfo)
            this.dutySlotToUnassign = slotinfo
            this.confirmUnassign = true;
        }

        public unassignDutySlot(){
            this.confirmUnassign = false;
            if(this.dutySlotToUnassign){
                const editedDutySlots: assignDutySlotsInfoType[] =[{
                    startDate: '',
                    endDate: '',
                    shiftId:null,
                    dutySlotId:this.dutySlotToUnassign.dutySlotId
                }]
                this.assignDuty(this.dutySlotToUnassign.sheriffId, editedDutySlots, true);        
            }
        }

		public closeEditDutyWindow(){
			this.closeDutySlotForm();
			this.showEditDutyDetails = false;
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

        public extractDuty(){
            if(this.dutyRosterInfo.attachedDuty){
                const dutyInfo = this.dutyRosterInfo.attachedDuty;

                const dutyStartTime = moment(dutyInfo.startDate).tz(dutyInfo.timezone);
                const startOfDay = moment(dutyStartTime).startOf("day");
                this.dutyDate = startOfDay.format();
                this.dutyTimezone = dutyInfo.timezone;

                this.fullDutyStartTime  = dutyStartTime.format('HH:mm')
                this.fullDutyEndTime = moment(dutyInfo.endDate).tz(dutyInfo.timezone).format('HH:mm')

                const dutyBin = this.getTimeRangeBins(dutyInfo.startDate, dutyInfo.endDate,startOfDay, dutyInfo.timezone);
                let unassignedArray = this.fillInArray(Array(96).fill(0),1,dutyBin.startBin, dutyBin.endBin); 
                              
                for(const dutySlot of dutyInfo.dutySlots){
                    // console.log(dutySlot)
                    let id = 1000;
                    const assignedDutyBin = this.getTimeRangeBins(dutySlot.startDate, dutySlot.endDate,startOfDay, dutySlot.timezone)
                    unassignedArray = this.fillInArray(unassignedArray,0,assignedDutyBin.startBin, assignedDutyBin.endBin);
                    const sheriff = this.shiftAvailabilityInfo.filter(sheriff=>{if(sheriff.sheriffId==dutySlot.sheriffId)return true})[0];
                    const isOvertime = this.getOverTime(dutySlot.shiftId, dutySlot.isNotAvailable, dutySlot.isNotRequired, dutySlot.isOvertime);                    
                    const isNotRequiredOrAvailable = (dutySlot.isNotAvailable || dutySlot.isNotRequired)
                    const isNotRequiredOrAvailableTitle = dutySlot.isNotRequired? 'Not Required':'Not Available'
                    const isNotRequiredOrAvailableSheriffId = dutySlot.isNotRequired? '00000-00000-11111':'00000-00000-22222'
                    //console.log(isNotRequiredOrAvailable)                    

                    this.dutyBlocks.push({
                        id: 'dutySlot'+dutySlot.id+'i'+dutyInfo.id+'n'+id++,                    
                        startTime: 1+ assignedDutyBin.startBin,
                        endTime: 1+ assignedDutyBin.endBin,                    
                        color: this.getDutyColor(this.dutyRosterInfo.type.colorCode, dutySlot.isNotAvailable,dutySlot.isNotRequired, isOvertime),
                        height: '2/6',
                        title: this.getTitle(sheriff,dutySlot.isNotAvailable,dutySlot.isNotRequired),
                        lastName: isNotRequiredOrAvailable? isNotRequiredOrAvailableTitle: Vue.filter('capitalize')(sheriff.lastName),
                        firstName: isNotRequiredOrAvailable? '' : Vue.filter('capitalize')(sheriff.firstName),
                        sheriffId: isNotRequiredOrAvailable? isNotRequiredOrAvailableSheriffId: sheriff.sheriffId,
                        startTimeString: moment(dutySlot.startDate).tz(dutySlot.timezone).format('HH:mm'),
                        endTimeString: moment(dutySlot.endDate).tz(dutySlot.timezone).format('HH:mm'),
                        timezone: dutySlot.timezone, 
                        shiftId: dutySlot.shiftId, 
                        dutySlotId: dutySlot.id,
                        note: isOvertime? 'Overtime' :''
                    })                
                }

                this.extractUnassignedArrays(unassignedArray);

                for(const unassignInx in this.unassignedArray){
                    //console.log(unassignInx)
                    // console.log(this.unassignedArray[unassignInx])
                    const unassignedBin = this.getArrayRangeBins(this.unassignedArray[unassignInx]);
                    const unassignedSlotTime = this.convertTimeRangeBinsToTime(startOfDay, unassignedBin.startBin, unassignedBin.endBin);
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
                        startTimeString: unassignedSlotTime.startTime.substring(11,16),
                        endTimeString: unassignedSlotTime.endTime.substring(11,16),
                        timezone: '',
                        shiftId: null,
                        dutySlotId: null,
                        note: ''
                    })                
                }

                this.dutyBlocks = _.sortBy(this.dutyBlocks,'startTime')
                //console.log(unassignedArray)

                if(this.dutyBlocks.length>1)
                    for(const dutyBlockInx in this.dutyBlocks){
                        this.dutyBlocks[dutyBlockInx].height = (Number(dutyBlockInx)%2)?'2/4':'4/6'; 
                    }

                
            }
            //console.log(this.dutyBlocks)

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
            if(isNotAvailable) return 'Not Available';
            else if(isNotRequired) return  'Not Required';
            else if(sheriff) return Vue.filter('capitalize')(sheriff.lastName)+', '+Vue.filter('capitalize')(sheriff.firstName);
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
                    const editedDutySlots: assignDutySlotsInfoType[] =[{
                        startDate: timeRangeDate.startTime,
                        endDate: timeRangeDate.endTime,
                        shiftId:null,
                        dutySlotId:null
                    }]
                    this.assignDuty(sheriffId, editedDutySlots, false);
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
                    const editedDutySlots: assignDutySlotsInfoType[] =[]

                    const discontinuity = this.findDiscontinuity(unionUnassignAvail);
                    const iterationNum = Math.floor((this.sumOfArrayElements(discontinuity) +1)/2);
                    console.log(iterationNum);
                    console.log(discontinuity);

                    for(let i=0; i< iterationNum; i++){
                        const inx1 = discontinuity.indexOf(1)
                        let inx2 = discontinuity.indexOf(2)
                        discontinuity[inx1]=0
                        if(inx2>=0) discontinuity[inx2]=0; else inx2=discontinuity.length 
                        console.error(inx1 + ' ' +inx2) 
                        //console.log(unionSelectedRangeAvail.slice(inx1,inx2).includes(1))
                        //console.log(unionSelectedRangeAvail[inx1])
                        const slotTime = this.convertTimeRangeBinsToTime(this.dutyDate, inx1, inx2)
                        editedDutySlots.push({
                            startDate: slotTime.startTime,
                            endDate: slotTime.endTime,                        
                            shiftId: unionUnassignAvail[inx1],
                            dutySlotId: null,
                        })
                    }
                    console.log(editedDutySlots)
                    this.assignDuty(sheriffId, editedDutySlots, false)
                    
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

            const editedDutySlots: assignDutySlotsInfoType[] =[{
                startDate: this.overTimeTimeRangeDate.startTime,
                endDate: this.overTimeTimeRangeDate.endTime,
                shiftId:null,
                dutySlotId:null
            }]

            this.assignDuty(this.overTimeSheriffId, editedDutySlots, false);
        }

        public unionArrays(arrayA, arrayB){
            return arrayA.map((arr,index) =>{return arr*arrayB[index]});
        }

        public sumOfArrayElements(arrayA){
            return arrayA.reduce((acc, arr) => acc + (arr>0?1:0), 0)
        }

        public fillInArray(array, fillInNum, startBin, endBin){
            return array.map((arr,index) =>{if(index>=startBin && index<endBin) return fillInNum; else return arr;});
        }

        public findDiscontinuity(array){
            return array.map((arr,index)=>{
                if((index==0 && arr>0)||(arr>0 && array[index-1]==0)) return 1 ;
                else if(index>0 && arr==0 && array[index-1]>0) return 2 ;
                else return 0
            })
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

        public assignDuty(sheriffId: string|null, editedDutySlots: assignDutySlotsInfoType[], unassignSheriff: boolean ) {

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
                
                const dutySlots: dutySlotInfoType[] = [];
                const dutySlotIds: number[] = [];
                console.log(dutyInfo)
                
                for(const dutySlot of editedDutySlots){
                    console.log(dutySlot.startDate)
                    console.log(dutySlot.endDate)
                    console.log(dutySlot.shiftId)
                    console.log(dutySlot.dutySlotId)
                    if(dutySlot.dutySlotId) dutySlotIds.push(dutySlot.dutySlotId)

                    if(!unassignSheriff)
                        dutySlots.push( { 
                            id: dutySlot.dutySlotId,                      
                            startDate: dutySlot.startDate,
                            endDate: dutySlot.endDate,
                            dutyId: dutyInfo.id,
                            sheriffId: sheriffId,
                            shiftId: dutySlot.shiftId,
                            timezone: dutyInfo.timezone,
                            isNotRequired: isNotRequired,
                            isNotAvailable: isNotAvailable,
                            isOvertime: false
                        })
                    
                }

                for(const dutySlot of dutyInfo.dutySlots){
                    if(dutySlotIds.includes(dutySlot.id)){
                        continue;
                    }
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

    .grid > div { 
        padding: 0 0;
        border: 1px dotted rgb(212, 212, 212);
    }

</style>