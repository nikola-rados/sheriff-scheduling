<template>
    <div class="grid796c">
        <div v-for="i in 7" :key="i" :style="{backgroundColor: '#F9F9F9', gridColumnStart: ((i-1)*96)+1,gridColumnEnd:(i*96+2), gridRow:'1/7'}"></div>       
        <div
            @click="editDuty(block.day)"
            v-for="block in dutyBlocks" 
            :key="block.id"
            :id="block.id"
            :style="{borderLeft:block.borderLeft, borderRight: block.borderRight, gridColumnStart: block.startTime, gridColumnEnd:block.endTime, gridRow:block.height,  backgroundColor: block.color, fontSize:'9px', textAlign: 'center', margin:0, padding:0  }"
            v-b-tooltip.hover
            :title="block.title? block.title:''" 
            @dragover.prevent
            @drop.prevent="drop" >
                <!-- <b-icon-chat-square-text-fill v-if="block.comment" variant="white" font-scale=".8" class="m-0 p-0"/> -->
                <b :style="(dutyBlockStyle + 'font-size: 18px;')">
                    {{block.title|truncate(((block.endTime - block.startTime)/7)-1)}}
                </b>                    
        </div>
        <span v-if="localTime.isTodayInView" :style="currentTimeStyle"></span>

        <b v-for="commentBlock in selectedComments" 
            :key="'comment'+commentBlock.day+'t'+commentBlock.startTime"
            v-b-tooltip.hover.v-info="commentBlock.comment"  
            :style="{backgroundColor: '#AB0000', gridColumnStart: commentBlock.startTime, gridColumnEnd: commentBlock.endTime, gridRow:'1/3'}"> 
                <b-icon-chat-square-text-fill font-scale="0.8" variant="white" style="transform: translate(7px,-5px);"/>
        </b>
        
    
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
                <div style="float:left;" class="h5 ml-0 p-0 mb-0">{{assignmentName}}</div>             
                <div style="float:left; margin:.1rem 0 0 1rem;" class="h5 text-warning p-0"> {{dutyBlocksDay[0].fullDutyStartTime}} - {{dutyBlocksDay[0].fullDutyEndTime}}</div>
                <div style="float:left; margin:.1rem 0 0 1rem;" class="h6 text-white p-0"> {{dutyBlocksDay[0].dutyDate|beautify-date-weekday}}</div>                 
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
                            :items="dutyBlocksDay"
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
                                    :style="(data.value=='Not Available'||data.value=='Not Required'||data.value=='Closed')?'color:'+data.item.color:''"
                                    v-b-tooltip.hover.right                                
                                    :title="data.item.title>20? data.item.title:''">
                                    {{data.item.title | truncate(20)}}</div>
                            </template>

                            <template v-slot:cell(note)="data" >
                                <div                                                                        
                                    :style="{fontSize:'12px',margin:'.11rem 0', color:data.item.color }">
                                    {{data.value}}</div>
                            </template>

                            <template v-if="hasPermissionToEditDuty" v-slot:cell(editDutySlot)="data" >          
                                <b-button style="width:1.2rem;float:right" 
                                        class="ml-1 mr-0 my-0 py-0"
                                        size="sm" 
                                        :disabled="data.item.sheriffId?false:true" 
                                        variant="transparent" 
                                        @click="confirmUnassignDutySlot(data.item)"
                                        v-b-tooltip.hover                                
                                        title="Unassign"
                                        ><b-icon 
                                            v-if="data.item.sheriffId" 
                                            icon="person-x-fill" 
                                            font-scale="1.25" 
                                            variant="danger" 
                                            style="transform: translate(-8px,0);"/>
                                </b-button>
                                <b-button style="width:.75rem;float:right" 
                                        class="mx-1 my-0 py-0" 
                                        size="sm" 
                                        
                                        variant="transparent" 
                                        @click="editDutySlotInfo(data)"
                                        v-b-tooltip.hover                                
                                        title="Edit"
                                        ><b-icon 
                                            
                                            icon="pencil-square" 
                                            font-scale="1.25" 
                                            variant="primary" 
                                            style="transform: translate(-8px,0);"/>
                                </b-button>
                            </template>

                            <template v-slot:row-details="data">
                                <b-card :id="'Le-Date-'+data.item.startTimeString.substring(0,10)" body-class="m-0 px-0 py-1" :border-variant="addFormColor" style="border:2px solid">
                                    <add-duty-slot-week-form 
                                        :formData="data.item" 
                                        :dutyBlocks="dutyBlocksDay"
                                        :fullDutyStartTime="dutyBlocksDay[0].fullDutyStartTime"
                                        :fullDutyEndTime="dutyBlocksDay[0].fullDutyEndTime"
                                        :startOfDay="dutyBlocksDay[0].dutyDate"
                                        :timezone="dutyTimezone" 
                                        v-on:submit="assignDuty" 
                                        v-on:cancel="closeDutySlotForm" />
                                </b-card>
                            </template>
                        </b-table> 
                    </b-card> 
                </div>
                <b-row style="height:3rem; border-radius:4px;" class="mx-auto my-0 p-0 bg-primary text-white">
                    <label style="margin:1rem .3rem 0 0.5rem;" class="h6 p-0">Comment:</label>
                    <b-form-group class="mx-2 my-2" style="width: 13rem">                        
                        <b-form-input
                            v-model="selectedComment"
                            size="sm"
                            placeholder="Enter a comment"
                            type="text" 
                            :formatter="commentFormat"                           
                        ></b-form-input>
                    </b-form-group>
                    <b-button
                        variant="success"
                        class="p-1 my-2 mx-2"
                        @click="updateComment(dutyBlocksDay[0].dutyId,selectedComment)"
                        size="sm">
                        <b-icon-check2 style="padding:0; vertical-align: middle; margin-right: 0.25rem;"/>
                        Save Comment
                    </b-button>                                    
                </b-row>
			</b-card>

			<template v-slot:modal-footer>
				<b-button
                        :disabled="!hasPermissionToExpireDuty"
						size="sm"
						variant="danger"
						class="mr-auto"
						@click="confirmDeleteDuty()"
				><b-icon-trash-fill style="padding:0; vertical-align: middle; margin-right: 0.25rem;"></b-icon-trash-fill>Delete</b-button>
				<b-button
						size="sm"
						variant="secondary"
						@click="closeEditDutyWindow()"
				><b-icon-x style="padding:0; vertical-align: middle; margin-right: 0.25rem;"></b-icon-x>Close</b-button>
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

            <h4 v-if="dutyBlocksDay[0]">Are you sure you want to delete the "{{assignmentName}}" duty from {{dutyBlocksDay[0].dutyDate|beautify-date-weekday}}?</h4>
            
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
            
            <h4 v-if="dutyBlocksDay[0] && dutySlotToUnassign">Are you sure you want to unassign {{dutySlotToUnassign.lastName}}, {{dutySlotToUnassign.firstName}} from the "{{assignmentName}}" duty on {{dutyBlocksDay[0].dutyDate|beautify-date-weekday}}?</h4>
            
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
    import AddDutySlotWeekForm from './AddDutySlotWeekForm.vue'
    import {dutyRangeInfoType, dutySlotInfoType, assignDutySlotsInfoType, assignDutyInfoType, assignmentCardInfoType, dutyBlockWeekInfoType, myTeamShiftInfoType } from '../../../types/DutyRoster';
    import {localTimeInfoType, userInfoType} from '../../../types/common';

    import { namespace } from "vuex-class";
    import "@store/modules/CommonInformation";
    const commonState = namespace("CommonInformation"); 
    import "@store/modules/DutyRosterInformation";   
    const dutyState = namespace("DutyRosterInformation");

    @Component({
        components: {
            AddDutySlotWeekForm
        }        
    })  
    export default class DutyCardWeekView extends Vue {

        @dutyState.State
        public dutyRangeInfo!: dutyRangeInfoType;

        @commonState.State
        public localTime!: localTimeInfoType;

        @commonState.State
        public userDetails!: userInfoType;

        @Prop({required: true})
        dutyRosterInfo!: assignmentCardInfoType;

        @dutyState.State
        public shiftAvailabilityInfo!: myTeamShiftInfoType[];

        @dutyState.State
        public dutyToBeEdited!: string;

        @dutyState.Action
        public UpdateDutyToBeEdited!: (newDutyToBeEdited: string) => void

        dutyBlocks: dutyBlockWeekInfoType[] = [];
        
        dutyBlocksDay: dutyBlockWeekInfoType[] = []; 

        @dutyState.State
        public view24h!: boolean;
        

        //fullDutyStartTime=''
        //fullDutyEndTime=''

        addNewDutySlotForm = false;
        addFormColor = 'secondary';

        dutyBlockStyle = 'text-transform: capitalize; margin:  0 padding:0; color:white;';

        isEditOpen = false;
        updateTable=0;

        assignDutyError = false;
        assignDutyErrorMsg = '';

        isMounted = false;

        overTimeSheriffId = '';
        overTimeTimeRangeDate = {startTime: '', endTime: '', day: 0}
        confirmAssignOverTimeDuty = false;

        assignedArray: number[] = [];
        unassignedArray: number[][] = [];

        dutyTimezone='';

        assignmentName = '';

        showEditDutyDetails = false;
        isDutyDataMounted = false;
        hasPermissionToEditDuty = false;
        hasPermissionToExpireDuty = false;
        showEditCancelWarning = false;
        confirmDelete = false;
        dutySlotToDelete = {} as dutyBlockWeekInfoType;
        latestEditData;
        timezone = 'UTC';

        editDutyError = false;
        editDutyErrorMsg = '';

        deleteErrorMsg = '';
        deleteError = false; 

        selectedComments: any[] = [];
        selectedComment = '';
        //originalComment = '';
        commentSaved = false;
        
        confirmUnassign = false;
        dutySlotToUnassign = {} as dutyBlockWeekInfoType;

        dutyWeekDates: string[] = []

        fields = [      
            {key:'title', label:'Sheriff',sortable:false, tdClass: 'border-top'  },
            {key:'startTimeString', label:'Start Time',  sortable:false, tdClass: 'border-top', thClass:'h6 align-middle',},
            {key:'endTimeString',   label:'End Time',  sortable:false, tdClass: 'border-top', thClass:'h6 align-middle',},  
            {key:'note', label:'Note',sortable:false, tdClass: 'border-top', thClass:'h6 align-middle'  },
            {key:'editDutySlot', label:'',  sortable:false, tdClass: 'border-top', thClass:'',},       
        ];

        mounted()
        {
            this.hasPermissionToEditDuty = this.userDetails.permissions.includes("EditDuties");
            this.hasPermissionToExpireDuty = this.userDetails.permissions.includes("ExpireDuties");
            this.assignmentName = this.getDutyName()

            for(let day=0; day<7; day++)
                this.dutyWeekDates.push(moment(this.dutyRangeInfo.startDate).add(day,'days').format())
            
            this.isMounted = false;
            this.dutyBlocks = [];
            this.extractDuty();
            if(this.dutyRosterInfo.assignment == this.dutyToBeEdited.substring(0,8)) Vue.nextTick(()=>this.editDuty(this.dutyToBeEdited.substring(9)))
            
        }

        public getDutyName(){
            if (this.dutyRosterInfo)
                return Vue.filter('capitalize')(this.dutyRosterInfo.type.name +' - '+this.dutyRosterInfo.code) +' ('+this.dutyRosterInfo.name+')'; 
            else return '';
        }

        public editDuty(day){
            this.isDutyDataMounted = false;
            this.dutyBlocksDay = (this.dutyBlocks.filter(dutyBlock=>{if(dutyBlock.day==day)return true;}));
            this.selectedComment = this.dutyBlocksDay[0].comment
            this.UpdateDutyToBeEdited(this.dutyRosterInfo.assignment+'D'+day);
            this.showEditDutyDetails = true;
            this.isDutyDataMounted = true;
        }

        public confirmDeleteDuty(){
            if (this.hasPermissionToExpireDuty) {
                this.deleteError = false;
                this.confirmDelete = true;
            }
        }
        
        public cancelDeletion() {
            this.deleteError = false;
			this.confirmDelete = false;
        }

        public deleteDuty(){
			
            this.confirmDelete = false;
            this.deleteError = false;
            const body = [this.dutyBlocksDay[0].dutyId]
            const url = 'api/dutyroster?id=' + this.dutyBlocksDay[0].dutyId;
        
            this.$http.delete(url, {data:body})
                .then(response => {
                    this.confirmDelete = false;
                    this.UpdateDutyToBeEdited('');
                    this.$emit('change', this.scrollPositions());                    
                }, err=>{
                    const errMsg = err.response.data.error;
                    //console.log(err.response)
                    this.deleteErrorMsg = errMsg;
                    this.deleteError = true;
                });		
			
		}
        
        public confirmUnassignDutySlot(slotinfo){
            this.dutySlotToUnassign = slotinfo
            this.confirmUnassign = true;
        }

        public unassignDutySlot(){
            this.confirmUnassign = false;
            if(this.dutySlotToUnassign){
                const editedDutySlots: assignDutySlotsInfoType[] =[{
                    startDate: '',
                    endDate: '',
                    //shiftId:null,
                    isOvertime:false,
                    dutySlotId:this.dutySlotToUnassign.dutySlotId
                }]
                this.assignDuty(this.dutySlotToUnassign.sheriffId, editedDutySlots, true,this.dutyBlocksDay[0].day);        
            }
        }

		public closeEditDutyWindow(){
            this.closeDutySlotForm();
            this.UpdateDutyToBeEdited('');
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

            for(let day=0; day<7; day++){            
                if(this.dutyRosterInfo[day]){
                    const dutyInfo = this.dutyRosterInfo[day];
                    //console.log(dutyInfo)
                    const dutyStartTime = moment(dutyInfo.startDate).tz(dutyInfo.timezone);
                    const startOfDay = moment(dutyStartTime).startOf("day");
                    const dutyDate = startOfDay.format();
                    this.dutyTimezone = dutyInfo.timezone;

                    const fullDutyStartTime  = dutyStartTime.format('HH:mm')
                    const fullDutyEndTime = moment(dutyInfo.endDate).tz(dutyInfo.timezone).format('HH:mm')

                    const dutyBin = this.getTimeRangeBins(dutyInfo.startDate, dutyInfo.endDate,startOfDay, dutyInfo.timezone);
                
                    if(dutyInfo.comment){
                        const commentBin = this.convert12h24hView(dutyBin)
                        this.selectedComments.push({
                            day:day,
                            startTime: 1+(96*day)+ commentBin.startBin,
                            endTime: 1+(96*day)+ commentBin.startBin+13,
                            comment: dutyInfo.comment? dutyInfo.comment: ''
                        })
                    }
                   
                    let unassignedArray = this.fillInArray(Array(96).fill(0),1,dutyBin.startBin, dutyBin.endBin); 
                                
                    for(const dutySlot of dutyInfo.dutySlots){
                        let id = 1000;
                        const assignedDutyBin = this.getTimeRangeBins(dutySlot.startDate, dutySlot.endDate,startOfDay, dutySlot.timezone)
                        const assignedBins = this.convert12h24hView(assignedDutyBin);
                        
                        unassignedArray = this.fillInArray(unassignedArray,0,assignedDutyBin.startBin, assignedDutyBin.endBin);
                        const sheriff = this.shiftAvailabilityInfo.filter(sheriff=>{if(sheriff.sheriffId==dutySlot.sheriffId)return true})[0];
                        const isOvertime = this.getOverTime(dutySlot.shiftId, dutySlot.isNotAvailable, dutySlot.isNotRequired,dutySlot.isClosed, dutySlot.isOvertime);                    
                        const isNotRequiredOrAvailable = (dutySlot.isNotAvailable || dutySlot.isNotRequired || dutySlot.isClosed)
                        const isNotRequiredOrAvailableTitle = dutySlot.isNotRequired? 'Not Required':(dutySlot.isClosed?'Closed':'Not Available')
                        const isNotRequiredOrAvailableSheriffId = dutySlot.isNotRequired? '00000-00000-11111':(dutySlot.isClosed?'00000-00000-33333':'00000-00000-22222')

                        this.dutyBlocks.push({
                            id: 'dutySlot'+dutySlot.id+'i'+dutyInfo.id+'D'+day+'n'+id++, 

                            startTime: 1+(96*day)+ assignedBins.startBin,
                            endTime: 1+(96*day)+ assignedBins.endBin,
                            borderLeft: assignedBins.borderLeft,
                            borderRight: assignedBins.borderRight,                            
                            
                            color: this.getDutyColor(this.dutyRosterInfo.type.colorCode, dutySlot.isNotAvailable,dutySlot.isNotRequired, dutySlot.isClosed, isOvertime),
                            height: '2/6',
                            title: this.getTitle(sheriff,dutySlot.isNotAvailable,dutySlot.isNotRequired,dutySlot.isClosed),
                            lastName: isNotRequiredOrAvailable? isNotRequiredOrAvailableTitle: (sheriff? Vue.filter('capitalize')(sheriff.lastName):''),
                            firstName: isNotRequiredOrAvailable? '' : (sheriff? Vue.filter('capitalize')(sheriff.firstName):''),
                            sheriffId: isNotRequiredOrAvailable? isNotRequiredOrAvailableSheriffId: (sheriff? sheriff.sheriffId: ''),
                            startTimeString: moment(dutySlot.startDate).tz(dutySlot.timezone).format('HH:mm'),
                            endTimeString: moment(dutySlot.endDate).tz(dutySlot.timezone).format('HH:mm'),
                            timezone: dutySlot.timezone, 
                            shiftId: dutySlot.shiftId, 
                            dutySlotId: dutySlot.id,
                            note: isOvertime? 'Overtime' :'',
                            day: day,
                            dutyId: dutyInfo.id,
                            dutyDate: dutyDate,
                            fullDutyStartTime:fullDutyStartTime,
                            fullDutyEndTime:fullDutyEndTime,
                            comment: dutyInfo.comment? dutyInfo.comment: ''
                        })                
                    }
                    this.unassignedArray = [];
                    this.extractUnassignedArrays(unassignedArray);

                    for(const unassignInx in this.unassignedArray){

                        const unassignedBin = this.getArrayRangeBins(this.unassignedArray[unassignInx]);
                        const bins = this.convert12h24hView(unassignedBin);
                        const unassignedSlotTime = this.convertTimeRangeBinsToTime(startOfDay, unassignedBin.startBin, unassignedBin.endBin);
                        this.dutyBlocks.push({
                            id:'dutySlot'+dutyInfo.id+'D'+day+'n'+unassignInx,

                            startTime: 1+(96*day)+ bins.startBin,
                            endTime: 1+(96*day)+ bins.endBin, 
                            borderLeft: bins.borderLeft,
                            borderRight: bins.borderRight,

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
                            note: '',
                            day: day,
                            dutyId: dutyInfo.id,
                            dutyDate: dutyDate,
                            fullDutyStartTime:fullDutyStartTime,
                            fullDutyEndTime:fullDutyEndTime,
                            comment: dutyInfo.comment? dutyInfo.comment: ''
                        })                
                    }                    
                }
            }            
            this.isMounted = true;           
        }

        public convert12h24hView(time){            
            if(this.view24h) 
                return {startBin:time.startBin, endBin:time.endBin , borderLeft:'', borderRight:''}
            else{

                let start = (time.startBin-26) ;
                let end = (time.endBin-26);
                let borderLeft = ''
                let borderRight = ''
                if(start>=48)
                    start = 96;
                else if(start>=0)
                    start *=2;
                else{
                    start = 0;
                    borderLeft = '4px solid aqua'
                }

                if(end<=0)
                    end = 0;
                else if(end<=48)
                    end *=2;
                else{
                    end = 96;
                    borderRight = '4px solid aqua'
                }
                return {startBin:start, endBin:end , borderLeft:borderLeft, borderRight:borderRight}
            }
        }

        public extractUnassignedArrays(unassignedArray){
            let startBin = 0;
            for(let valueInx=0; valueInx<unassignedArray.length; valueInx++){

                if((unassignedArray[valueInx]>0 && valueInx==0)||(unassignedArray[valueInx]>0 && unassignedArray[valueInx-1]==0)) startBin = valueInx;
                
                if((unassignedArray[valueInx]>0 && valueInx==(unassignedArray.length-1))||(unassignedArray[valueInx]>0 && unassignedArray[valueInx+1]==0)){
                    
                    const array = this.fillInArray(Array(96).fill(0),1, startBin, valueInx+1)
                    this.unassignedArray.push(array)
                } 
            }
        }

        public getDutyColor(color, isNotAvailable, isNotRequired, isClosed, isOverTime) {
            if(isOverTime) return '#e85a0e';
            else if(isNotRequired) return  '#28a745';
            else if(isNotAvailable) return '#dc3545';
            else if (isClosed) return '#adb5bd';
            else return color;
        }

        public getTitle(sheriff,isNotAvailable,isNotRequired, isClosed){
            if(isNotAvailable) return 'Not Available';
            else if(isNotRequired) return  'Not Required';
            else if(isClosed) return  'Closed';
            else if(sheriff) return Vue.filter('capitalize')(sheriff.lastName)+', '+Vue.filter('capitalize')(sheriff.firstName);
            else return ' ';
        }

        public getOverTime(shiftId, isNotAvailable, isNotRequired, isClosed, isOverTime) {
            if(isNotRequired || isNotAvailable || isClosed) return  false;
            else if(isOverTime) return true;
            else return false;
        }
        
        public drop(event: any) 
        {
            if(event.target.id){
                const cardid = event.dataTransfer.getData('text');
                const blockId: string = event.target.id;
                const positionN = blockId.indexOf('n')
                const unassignedBlockId = Number(blockId.substring(positionN+1));
                const positionD = blockId.indexOf('D')
                const dutySlotDay = Number(blockId.substring(positionD+1,positionD+2));

                if(unassignedBlockId!=0) return
                if(this.dutyRosterInfo[dutySlotDay].dutySlots.length > 0) return

                const sheriffId = cardid.slice(7)
                if(sheriffId=='00000-00000-11111'||sheriffId=='00000-00000-22222'||sheriffId=='00000-00000-33333'){                    
                    const editedDutySlots: assignDutySlotsInfoType[] =[{
                        startDate: this.dutyRosterInfo[dutySlotDay].startDate,
                        endDate: this.dutyRosterInfo[dutySlotDay].endDate,
                        //shiftId:null,
                        isOvertime:false,
                        dutySlotId:null
                    }]
                    this.assignDuty(sheriffId, editedDutySlots, false, dutySlotDay);
                    return
                }
                
                const rangeBin = this.getTimeRangeBins(this.dutyRosterInfo[dutySlotDay].startDate, this.dutyRosterInfo[dutySlotDay].endDate, this.dutyWeekDates[dutySlotDay], this.timezone);
                const unassignedArray= this.fillInArray(Array(96).fill(0), 1 , rangeBin.startBin,rangeBin.endBin)
                
                const sheriff = this.shiftAvailabilityInfo.filter(sheriff=>{if(sheriff.sheriffId==sheriffId)return true})[0];
            
                let availability = this.getSheriffAvailability(sheriff, this.dutyWeekDates[dutySlotDay])
                const duties = this.getSheriffDuties(sheriff, this.dutyWeekDates[dutySlotDay])

                if(this.sumOfArrayElements(availability)==0 && this.sumOfArrayElements(duties)==0) return

                availability = this.subtractUnionOfArrays(availability,duties)         
                
                const unionUnassignAvail = this.unionArrays(unassignedArray, availability)
                
                if(this.sumOfArrayElements(unionUnassignAvail)>0){
                
                    const editedDutySlots: assignDutySlotsInfoType[] =[]

                    const discontinuity = this.findDiscontinuity(unionUnassignAvail);
                    const iterationNum = Math.floor((this.sumOfArrayElements(discontinuity) +1)/2);

                    for(let i=0; i< iterationNum; i++){
                        const inx1 = discontinuity.indexOf(1)
                        let inx2 = discontinuity.indexOf(2)
                        discontinuity[inx1]=0
                        if(inx2>=0) discontinuity[inx2]=0; else inx2=discontinuity.length 
    
                        const slotTime = this.convertTimeRangeBinsToTime(this.dutyWeekDates[dutySlotDay], inx1, inx2)
                        editedDutySlots.push({
                            startDate: slotTime.startTime,
                            endDate: slotTime.endTime, 
                            isOvertime:false,                       
                            //shiftId: unionUnassignAvail[inx1],
                            dutySlotId: null,
                        })
                    }
                   
                    this.assignDuty(sheriffId, editedDutySlots, false, dutySlotDay)
                    
                }else{
                    const unionUnassignDuties = this.unionArrays(unassignedArray, duties)
                    if(this.sumOfArrayElements(unionUnassignDuties)>0){                        
                        this.assignDutyErrorMsg = "This team member is already assigned to conflicting duties.";
                        this.assignDutyError = true;
                    }else{                       
                        const timeRangeBins = this.getArrayRangeBins(unassignedArray);
                        const slotTime = this.convertTimeRangeBinsToTime(this.dutyWeekDates[dutySlotDay], timeRangeBins.startBin, timeRangeBins.endBin);
                        this.overTimeTimeRangeDate.startTime = slotTime.startTime
                        this.overTimeTimeRangeDate.endTime = slotTime.endTime
                        this.overTimeTimeRangeDate.day = dutySlotDay
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
                isOvertime:true,
                //shiftId:null,
                dutySlotId:null
            }]

            this.assignDuty(this.overTimeSheriffId, editedDutySlots, false, this.overTimeTimeRangeDate.day);
        }

        public getSheriffAvailability(sheriff, startOfDay){
            if(sheriff){
                const shifts = sheriff.shifts.filter(shift=>{if(shift.startDate.substring(0,10)==startOfDay.substring(0,10))return true}) 
                let availability = Array(96).fill(0)
                for(const shift of shifts){
                    //console.log(shift)
                    if(shift.overtimeHours==0){
                        const rangeBin = this.getTimeRangeBins(shift.startDate, shift.endDate, startOfDay, this.timezone);
                        availability = this.fillInArray(availability, shift.id , rangeBin.startBin,rangeBin.endBin);
                    }
                }
                return availability
            }else{
                return Array(96).fill(0)
            }
        }

        public getSheriffDuties(sheriff,startOfDay){
            if(sheriff){
                const dutiesDetail = sheriff.dutiesDetail.filter(duty=>{if(duty.startTime.substring(0,10)==startOfDay.substring(0,10))return true})
                let duties = Array(96).fill(0)
                for(const duty of dutiesDetail){                    
                    duties = this.fillInArray(duties, duty.id , duty.startBin, duty.endBin)
                }
                return duties
            }else{
                return Array(96).fill(0)
            }
        }


        public unionArrays(arrayA, arrayB){
            return arrayA.map((arr,index) =>{return arr*arrayB[index]});
        }

        public sumOfArrayElements(arrayA){
            return arrayA.reduce((acc, arr) => acc + (arr>0?1:0), 0)
        }

        public subtractUnionOfArrays(arrayA, arrayB){
            return arrayA.map((arr,index) =>{return arr*(arrayB[index]>0?0:1)});
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

        public assignDuty(sheriffId: string|null, editedDutySlots: assignDutySlotsInfoType[], unassignSheriff: boolean, day: number ) {

            if(this.dutyRosterInfo[day]){
                const dutyInfo = this.dutyRosterInfo[day];
                let isNotRequired = false;
                let isNotAvailable = false;
                let isClosed = false;
                if(sheriffId=='00000-00000-11111'){
                    sheriffId = null;
                    isNotRequired = true;
                }else if(sheriffId=='00000-00000-22222'){
                    sheriffId = null;
                    isNotAvailable = true;
                }else if(sheriffId=='00000-00000-33333'){
                    sheriffId = null;
                    isClosed = true;
                }
                
                const dutySlots: dutySlotInfoType[] = [];
                const dutySlotIds: number[] = [];
                
                for(const dutySlot of editedDutySlots){

                    if(dutySlot.dutySlotId) dutySlotIds.push(dutySlot.dutySlotId)

                    if(!unassignSheriff)
                        dutySlots.push( { 
                            id: dutySlot.dutySlotId,                      
                            startDate: dutySlot.startDate,
                            endDate: dutySlot.endDate,
                            dutyId: dutyInfo.id,
                            sheriffId: sheriffId,
                            shiftId: null,//dutySlot.shiftId,
                            timezone: dutyInfo.timezone,
                            isNotRequired: isNotRequired,
                            isNotAvailable: isNotAvailable,
                            isClosed: isClosed,
                            isOvertime: dutySlot.isOvertime
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
                        isClosed: isClosed,
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
                        timezone: dutyInfo.timezone,
                        comment: dutyInfo.comment
                    }
                ];
                
                const url = 'api/dutyroster';
                this.$http.put(url, body )
                    .then(response => {
                        if(response.data){
                            // Update the duty bar with name;
                            this.$emit('change', this.scrollPositions());
                        }
                    }, err => {
                        const errMsg = err.response.data.error;
                        this.assignDutyErrorMsg = errMsg;
                        this.assignDutyError = true;
                    })
            }
        }

        get currentTimeStyle(){            
            let time = this.localTime.timeSlot
            if(!this.view24h){
                if(this.localTime.timeSlot-26<0) time = 0
                else  if(this.localTime.timeSlot-26>48) time = 96
                else time = (this.localTime.timeSlot-26)*2 
            }   
            const start = (this.localTime.dayOfWeek*96) +time+1
            const end =   (this.localTime.dayOfWeek*96) +time+2             
            return {borderLeft:'3px solid red', gridColumnStart: start,gridColumnEnd:end, gridRow:'1/7'}
        }

        public updateComment(id: number, comment: string){
            const url = 'api/dutyroster/updatecomment?dutyId='+id+'&comment='+comment;
            //console.log(url)
            this.$http.put(url)
                .then(response => {
                    //console.log(response)
                    if(response.status == 204){
                        // Update the duty bar with name;
                        this.commentSaved = true;
                        this.$emit('change', this.scrollPositions());
                    }else{
                        this.assignDutyErrorMsg = response.data? response.data: (response.status+' '+response.statusText);
                        this.assignDutyError = true;
                    }
                }, err => {
                    //console.log(err.response)
                    const errMsg = err.response.data.error? err.response.data.error: (err.response.data +'('+err.response.status +' '+err.response.statusText+')') ;
                    this.assignDutyErrorMsg = errMsg;
                    this.assignDutyError = true;
                })
        }

        public commentFormat(value) {
			return value.slice(0,100);
        }
        
        public scrollPositions(){
            const el = document.getElementsByClassName('b-table-sticky-header')
            const scrollDuty = el[0]? el[0].scrollTop : 0;
            const scrollGauge = el[1]? el[1].scrollTop : 0;

            const eltm = document.getElementById('dutyrosterteammember');
            const scrollTeamMember = eltm? eltm.scrollTop : 0;

            //console.log({scrollDuty: scrollDuty, scrollGauge: scrollGauge, scrollTeamMember:scrollTeamMember })

            return {scrollDuty: scrollDuty, scrollGauge: scrollGauge, scrollTeamMember:scrollTeamMember }
        }

    }

    
</script>

<style scoped>   

    .card {
        border: white;
    }

    .grid796c {        
        display:grid;
        grid-template-columns: repeat(672, 0.14881%);
        grid-template-rows: repeat(6,.5rem);
        inline-size: 100%; 
        background-color: #EEEEEE; 
        height: 2.9rem; 
        margin: 0; 
        padding: 0;
        column-gap: 0;
        row-gap: 0;
           
    }

    .grid796c > div { 
        padding: 0 0;
        border: 1px dotted rgb(212, 212, 212);
    }

</style>