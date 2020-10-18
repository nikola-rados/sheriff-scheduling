<template>
    <div>
        <b-card  style="height:400px;overflow: auto;" no-body>                                        
            <h2 v-if="locationError" class="mx-1 mt-0"><b-badge v-b-tooltip.hover :title="locationErrorMsgDesc"  variant="danger"> {{locationErrorMsg}} <b-icon class="ml-3" icon = x-square-fill @click="locationError = false" /></b-badge></h2>

            <b-card body-class="m-0 p-0">
                <b-row class="mt-3" style="background-color:#addcde;width: 31rem;margin-left: auto;margin-right: auto;border-radius: 0.75rem;" >
                    <b style="margin: 1.35rem 0.25rem 0 1.25rem">Home Location:</b>
                    <b-form-group style="width: 20rem; margin-top: 1rem"> 
                        <b-form-select                                                                                                           
                            v-model="selectedHomeLocation"
                            @change="homeLocationChanged">                            
                                <b-form-select-option
                                    v-for="homelocation in locationList" 
                                    :key="homelocation.id"
                                    :value="homelocation">
                                        {{homelocation.name}}
                                </b-form-select-option>    
                        </b-form-select>
                    </b-form-group>
                </b-row>
                <b-button v-if="!addNewLocation" style="transform:translate(0px,-5px);" size="sm" variant="success" @click="addNewLocation = true"> <b-icon icon="plus" /> Add </b-button>
            </b-card> 

            <b-card v-if="addNewLocation" class="my-3" border-variant="light" no-body>
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
            </b-card>

            <div>
                <b-card no-body border-variant="white" bg-variant="white" v-if="!assignedAwayLocations.length">
                        <span class="text-muted ml-4 my-5">No locations have been assigned.</span>
                </b-card>

                <b-card v-else  no-body border-variant="light" bg-variant="white">
                    <b-table
                        :items="assignedAwayLocations"
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
                            <template v-slot:cell(locationId)="data" >
                                <span 
                                    class="text-primary"
                                    v-b-tooltip.hover.right                                
                                    :title="data.item.locationNm.nameFull"> 
                                        {{data.item.locationNm.name}}
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
                            <template v-slot:cell(edit)="data" >                                       
                                <b-button class="my-0 py-0" size="sm" variant="transparent" @click="deleteLocation(data.item)"><b-icon icon="trash-fill" font-scale="1.25" variant="danger"/></b-button>
                                <b-button class="my-0 py-0" size="sm" variant="transparent" @click="editLocation(data.item)"><b-icon icon="pencil-square" font-scale="1.25" variant="primary"/></b-button>
                            </template>
                            
                    </b-table> 
                </b-card> 
            </div>                                     
        </b-card>
        <!-- <b-row class="m-1 mb-3" border-variant="white">
             <b-button
                    style="margin:0 0.5rem 0 auto; width:105px"
                    variant="secondary" 
                    @click="closeProfileWindow()"                  
            ><b-icon-x font-scale="1.5" style="padding:0; vertical-align: middle; margin-right: 0.25rem;"></b-icon-x>Cancel</b-button>                
            <b-button
                style="margin:0 0 0 0; width:105px"
                variant="success" 
                @click="saveMemberProfile()"
            ><b-icon-check2 style="padding:0; vertical-align: middle; margin-right: 0.25rem;"></b-icon-check2>Save</b-button>
        </b-row>


        <b-modal v-model="showCancelWarning" id="bv-modal-team-cancel-warning" header-class="bg-warning text-light">            
            <template v-slot:modal-title>                
                 <h2 v-if="editMode" class="mb-0 text-light"> Unsaved Profile Changes </h2>
                 <h2 v-else-if="createMode" class="mb-0 text-light"> Unsaved New Profile </h2>                
            </template>
            <p>Are you sure you want to cancel without saving your changes?</p>
            <template v-slot:modal-footer>
                <b-button variant="secondary" @click="$bvModal.hide('bv-modal-team-cancel-warning')"                   
                >No</b-button>
                <b-button variant="success" @click="closeWarningWindow()"
                >Yes</b-button>
            </template>            
            <template v-slot:modal-header-close>                 
                 <b-button variant="outline-warning" class="text-light closeButton" @click="$bvModal.hide('bv-modal-team-cancel-warning')"
                 >&times;</b-button>
            </template>
        </b-modal> -->
    </div>
</template>

<script lang="ts">
    import { Component, Vue, Prop } from 'vue-property-decorator';
    import moment from 'moment-timezone';
    import {teamMemberInfoType, awayLocationInfoType} from '../../../types/MyTeam';
    import {locationInfoType} from '../../../types/common';
    import { namespace } from 'vuex-class';
    import "@store/modules/CommonInformation";
    const commonState = namespace("CommonInformation");
    import "@store/modules/TeamMemberInformation"; 
    const TeamMemberState = namespace("TeamMemberInformation");

    @Component
    export default class LocationTab extends Vue {

        @commonState.State
        public token!: string;

        @commonState.State
        public locationList!: locationInfoType[];

        @TeamMemberState.State
        public userToEdit!: teamMemberInfoType;

        @TeamMemberState.Action
        public UpdateUserToEdit!: (userToEdit: teamMemberInfoType) => void

        selectedHomeLocation = {} as locationInfoType | undefined;

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

        addNewLocation = false;
        locationError = false;
        locationErrorMsg = '';
        locationErrorMsgDesc = '';
        updateTable=0;
        
        assignedAwayLocations: awayLocationInfoType[] = [];

        fields =  
        [     
            {key:'isFullDay', label:'Type',sortable:false, tdClass: 'border-top', },       
            {key:'locationId',label:'Location',sortable:false, tdClass: 'border-top',  }, 
            {key:'startDate', label:'Start Date',  sortable:false, tdClass: 'border-top', thClass:'',},
            {key:'startTime', label:'Start Time',  sortable:false, tdClass: 'border-top', thClass:'h6 align-middle',},
            {key:'endDate',   label:'End Date',  sortable:false, tdClass: 'border-top', thClass:'',},
            {key:'endTime',   label:'End Time',  sortable:false, tdClass: 'border-top', thClass:'h6 align-middle',},  
            {key:'edit',      label:'',    sortable:false, tdClass: 'border-top', thClass:'',},       
        ];

        mounted()
        {             
            this.selectedHomeLocation = this.userToEdit.homeLocation;
    
            // console.log('locationTab')
            // console.log(this.selectedHomeLocation)
            // console.log(this.userToEdit)           
            // console.log(this.assignedAwayLocations)
            
            
            this.extractAwayLocations();          
        }

        public extractAwayLocations ()
        {
            this.assignedAwayLocations = this.userToEdit.awayLocation? this.userToEdit.awayLocation: [];
            for(const inx in this.assignedAwayLocations)
            {
                this.assignedAwayLocations[inx]['locationNm'] = this.getLocationName(this.assignedAwayLocations[inx].locationId);

                if(this.isDateFullday(this.assignedAwayLocations[inx].startDate,this.assignedAwayLocations[inx].endDate)){ 
                    this.assignedAwayLocations[inx]['isFullDay'] = true;
                    this.assignedAwayLocations[inx]['_cellVariants'] = {isFullDay:'danger'}                 
                }else{
                    this.assignedAwayLocations[inx]['isFullDay'] = false;
                    this.assignedAwayLocations[inx]['_cellVariants'] = {isFullDay:'success'}                    
                }
                
                this.assignedAwayLocations[inx].startDate = moment(this.assignedAwayLocations[inx].startDate).tz("UTC").format();
                this.assignedAwayLocations[inx].endDate = moment(this.assignedAwayLocations[inx].endDate).tz("UTC").format();               
            }
        }

        public isDateFullday(startDate, endDate){
            const start = moment(startDate); 
            const end = moment(endDate);
            const duration = moment.duration(end.diff(start));
            if(duration.asMinutes() < 1440 && duration.asMinutes()> -1440 )  return false;  else return true;
        }

        public homeLocationChanged()
        {
            Vue.nextTick().then(()=>{
                console.log(this.selectedHomeLocation)
               
                const homeLocationId= this.selectedHomeLocation? this.selectedHomeLocation.id: '';
                const url = 'api/sheriff/updatelocation?id='+this.userToEdit.id+'&locationId='+homeLocationId;
                const options = {headers:{'Authorization' :'Bearer '+this.token}} 
                this.$http.put(url, options)
                    .then(response => {
                        console.log(response) 
                        this.updateUser();                                            
                    }, err => {
                        console.log(err)
                        this.locationErrorMsg = 'Home-location change was unsuccessful';
                        this.locationErrorMsgDesc = "The change hasn't been applied";
                        this.locationError = true;
                    });
            });
        }

        public updateUser(){
            const user = this.userToEdit
            user.homeLocation = this.selectedHomeLocation;
            user.homeLocationNm = this.selectedHomeLocation? this.selectedHomeLocation.name: '';
            user.homeLocationId = this.selectedHomeLocation? this.selectedHomeLocation.id: 0;
            this.UpdateUserToEdit(user);
            console.log(user)
            this.$emit('change') 
        }

        public deleteLocation(location){
            console.log('delete location')
            // this.trainingError = false; 
            // const body = 
            // [{
            //     "userId": this.userId,
            //     "roleId": role.value,                        
            // }]
            // const url = 'api/sheriff/unassignroles' 
            // const options = {headers:{'Authorization' :'Bearer '+this.token}}
            // this.$http.put(url, body, options)
            //     .then(response => {
            //         console.log(response)
            //         console.log('unassign success')
                   
            //         this.getUserRoles();
                                                     
            //     }, err=>{this.roleAssignError = true;});
        }

        public editLocation(location){
            console.log('edit location')
        }

        public saveAwayLocation(){
                this.locationError  = false; 
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
                        isFullDay: isFullDay,
                        sheriffId: this.userToEdit.id,
                    }
                    const url = 'api/sheriff/awaylocation'  
                    const options = {headers:{'Authorization' :'Bearer '+this.token}}
                    this.$http.post(url, body, options)
                        .then(response => {
                            console.log(response)
                            console.log('assign success')
                            this.addToAssignedLocationList(response.data);
                            this.clearLocationSelection();
                        }, err=>{   
                            //console.log(err.response.data);
                            const errMsg = err.response.data.error;
                            this.locationErrorMsg = errMsg.slice(0,60) + (errMsg.length>60?' ...':'');
                            this.locationErrorMsgDesc = errMsg;
                            this.locationError = true;
                        });
                }
        }

        public addToAssignedLocationList(addedLocationInfo)
        {
            const assignedAwayLocation: awayLocationInfoType =
            {
                id: addedLocationInfo.id,
                sheriffId : addedLocationInfo.sheriffId,    
                locationId: addedLocationInfo.locationId,
                startDate: addedLocationInfo.startDate,
                endDate: addedLocationInfo.endDate,               
            }
            assignedAwayLocation['locationNm'] = this.getLocationName(addedLocationInfo.locationId);
            if(this.isDateFullday(assignedAwayLocation.startDate,assignedAwayLocation.endDate)){ 
                assignedAwayLocation['isFullDay'] = true;
                assignedAwayLocation['_cellVariants'] = {isFullDay:'danger'}                 
            }else{
                assignedAwayLocation['isFullDay'] = false;
                assignedAwayLocation['_cellVariants'] = {isFullDay:'success'}                    
            }

            this.assignedAwayLocations.push(assignedAwayLocation); 
            this.$emit('change');                     
        }

        public clearLocationSelection(){
            this.selectedLocation = {} as locationInfoType | undefined;
            this.selectedEndDate = '';
            this.selectedStartDate = '';
            this.selectedStartTime = '';
            this.selectedEndTime = '';
            this.addNewLocation= false;
        }

        public getLocationName(locationId: number|null){
            const index = this.locationList.findIndex(location=>{if(location.id == locationId)return true})
            if(index>=0){   
                let truncName = this.locationList[index].name.slice(0,20);
                if (this.locationList[index].name && this.locationList[index].name.length>23)
                    truncName = truncName +'...';
                else
                    truncName = this.locationList[index].name.slice(0,23);

                return {name:truncName, nameFull:this.locationList[index].name}
            }
            else
                return {name:'', nameFull:''}
        }

        get noHomeLocationList(){
            return this.locationList.filter(location =>{ if(this.selectedHomeLocation && this.selectedHomeLocation.id == location.id) return false;else return true})
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