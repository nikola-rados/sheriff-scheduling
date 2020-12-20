<template>
    <div>
        <b-card  style="height:400px;overflow: auto;" no-body> 
            <b-card id="LocationError" no-body>                                       
                <h2 v-if="locationError" class="mx-1 mt-2"><b-badge v-b-tooltip.hover :title="locationErrorMsgDesc" style="word-break: break-word;white-space: normal;"  variant="danger"> {{locationErrorMsg}} <b-icon class="ml-3" icon = x-square-fill @click="locationError = false" /></b-badge></h2>
            </b-card>
            
            <b-card v-if="!addNewLocationForm && hasPermissionToEditUsers">
                <b-button size="sm" variant="success" @click="addNewLocation"> <b-icon icon="plus" /> Add </b-button>
            </b-card> 

            <b-card v-if="addNewLocationForm" id="addLocationForm" class="my-3" :border-variant="addFormColor" style="border:2px solid" body-class="m-0 px-0 py-1">
                <add-location-form :formData="{}" :isCreate="true" v-on:submit="saveAwayLocation" v-on:cancel="closeLocationForm" />              
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
                                    :title="data.item.locationNm"> 
                                        {{data.item.locationNm | truncate(20)}}
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
                                <b-button v-if="hasPermissionToEditUsers" class="my-0 py-0" size="sm" variant="transparent" @click="confirmDeleteLocation(data.item)"><b-icon icon="trash-fill" font-scale="1.25" variant="danger"/></b-button>
                                <b-button v-if="hasPermissionToEditUsers" class="my-0 py-0" size="sm" variant="transparent" @click="editLocation(data)"><b-icon icon="pencil-square" font-scale="1.25" variant="primary"/></b-button>
                            </template>

                            <template v-slot:row-details="data">
                                <b-card :id="'Lo-Date-'+data.item.startDate.substring(0,10)" body-class="m-0 px-0 py-1" :border-variant="addFormColor" style="border:2px solid">
                                    <add-location-form :formData="data.item" :isCreate="false" v-on:submit="saveAwayLocation" v-on:cancel="closeLocationForm" />
                                </b-card>
                            </template>
                            
                    </b-table> 
                </b-card> 
            </div>                                     
        </b-card>

        <b-modal v-model="confirmDelete" id="bv-modal-confirm-delete" header-class="bg-warning text-light">
            <template v-slot:modal-title>
                    <h2 class="mb-0 text-light">Confirm Delete Location</h2>                    
            </template>
            <h4>Are you sure you want to delete the "{{locationToDelete.locationNm}}" location?</h4>
            <b-form-group style="margin: 0; padding: 0; width: 20rem;"><label class="ml-1">Reason for Deletion:</label> 
                <b-form-select
                    size = "sm"
                    v-model="locationDeleteReason">
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
                <b-button variant="danger" @click="deleteLocation()" :disabled="locationDeleteReason.length == 0">Confirm</b-button>
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
    import { Component, Vue} from 'vue-property-decorator';
    import moment from 'moment-timezone';
    import {teamMemberInfoType, awayLocationInfoType} from '../../../types/MyTeam';
    import {locationInfoType, userInfoType} from '../../../types/common';
    import AddLocationForm from './AddForms/AddLocationForm.vue'
    import { namespace } from 'vuex-class';
    import "@store/modules/CommonInformation";
    const commonState = namespace("CommonInformation");
    import "@store/modules/TeamMemberInformation"; 
    const TeamMemberState = namespace("TeamMemberInformation");

    @Component({
        components: {
            AddLocationForm
        }        
    })    

    export default class LocationTab extends Vue {

        @commonState.State
        public allLocationList!: locationInfoType[];

        @commonState.State
        public userDetails!: userInfoType;

        @TeamMemberState.State
        public userToEdit!: teamMemberInfoType;

        hasPermissionToEditUsers = false;
        addNewLocationForm = false;
        locationError = false;
        locationErrorMsg = '';
        locationErrorMsgDesc = '';

        confirmDelete = false;
        locationToDelete = {} as awayLocationInfoType;

        addFormColor = 'secondary';
        latestEditData;
        isEditOpen = false;
        locationDeleteReason = '';
        
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
            this.hasPermissionToEditUsers = this.userDetails.permissions.includes("EditUsers");                         
            this.extractAwayLocations();          
        }

        public extractAwayLocations ()
        {
            this.assignedAwayLocations = this.userToEdit.awayLocation? this.userToEdit.awayLocation: [];
            for(const inx in this.assignedAwayLocations)
            {
                const location = this.getLocation(this.assignedAwayLocations[inx].locationId)
                this.assignedAwayLocations[inx]['locationNm'] = location? location.name : '';

                if(Vue.filter('isDateFullday')(this.assignedAwayLocations[inx].startDate,this.assignedAwayLocations[inx].endDate)){ 
                    this.assignedAwayLocations[inx]['isFullDay'] = true;
                    this.assignedAwayLocations[inx]['_cellVariants'] = {isFullDay:'danger'}                 
                }else{
                    this.assignedAwayLocations[inx]['isFullDay'] = false;
                    this.assignedAwayLocations[inx]['_cellVariants'] = {isFullDay:'success'}                    
                }
                const timezone = location? location.timezone : 'UTC';
                this.assignedAwayLocations[inx].startDate = moment(this.assignedAwayLocations[inx].startDate).tz(timezone).format();
                this.assignedAwayLocations[inx].endDate = moment(this.assignedAwayLocations[inx].endDate).tz(timezone).format();               
            }
        }

        public confirmDeleteLocation(location) {
            //console.log(location)
            this.locationToDelete = location;           
            this.confirmDelete=true; 
        }

         public cancelDeletion() {
            this.confirmDelete = false;
            this.locationDeleteReason = '';
        }

        public deleteLocation(){
            if (this.locationDeleteReason.length) {
                this.confirmDelete = false;
                this.locationError = false; 
                const url = 'api/sheriff/awaylocation?id='+this.locationToDelete.id+'&expiryReason='+this.locationDeleteReason;
                this.$http.delete(url)
                    .then(response => {
                        //console.log(response)
                        // console.log('delete success')
                        const index = this.assignedAwayLocations.findIndex(assignedlocation=>{if(assignedlocation.id == this.locationToDelete.id) return true;})
                        if(index>=0) this.assignedAwayLocations.splice(index,1);
                        this.$emit('change');
                    }, err=>{
                        const errMsg = err.response.data.error;
                        this.locationErrorMsg = errMsg;//.slice(0,60) + (errMsg.length>60?' ...':'');
                        this.locationErrorMsgDesc = errMsg;
                        this.locationError = true;
                        location.href = '#LocationError'
                    });
                    this.locationDeleteReason = '';
            }
        }
        
        public addNewLocation(){
            if(this.isEditOpen){
                location.href = '#Lo-Date-'+this.latestEditData.item.startDate.substring(0,10)
                this.addFormColor = 'danger'
            }else{
                this.addNewLocationForm = true;
                this.$nextTick(()=>{location.href = '#addLocationForm';})
            }
        }

        public editLocation(data){
            console.log('edit location')
            //console.log(data)
            if(this.addNewLocationForm){
                location.href = '#addLocationForm'
                this.addFormColor = 'danger'
            }else if(this.isEditOpen){
                location.href = '#Lo-Date-'+this.latestEditData.item.startDate.substring(0,10)
                this.addFormColor = 'danger'               
            }else if(!this.isEditOpen && !data.detailsShowing){
                data.toggleDetails();
                this.isEditOpen = true;
                this.latestEditData = data
                Vue.nextTick().then(()=>{
                    location.href = '#Lo-Date-'+this.latestEditData.item.startDate.substring(0,10)
                });
            }                        
        }

        public saveAwayLocation(body, iscreate){
            this.locationError  = false;                        
            body['sheriffId']= this.userToEdit.id;
            //console.log(body)
            //console.log(iscreate)
            const method = iscreate? 'post' :'put';
            const url = 'api/sheriff/awaylocation'  
            const options = { method: method, url:url, data:body}
            this.$http(options)
                .then(response => {                    
                    if(iscreate) 
                        this.addToAssignedLocationList(response.data);
                    else
                        this.modifyAssignedLocationList(response.data);
                    this.closeLocationForm();
                }, err=>{   
                    console.log(err.response.data);
                    const errMsg = err.response.data.error;
                    this.locationErrorMsg = errMsg;//.slice(0,60) + (errMsg.length>60?' ...':'');
                    this.locationErrorMsgDesc = errMsg;
                    this.locationError = true;
                    location.href = '#LocationError'
                });                
        }

        public modifyAssignedLocationList(modifiedLocationInfo){

            const index = this.assignedAwayLocations.findIndex(assignedlocation =>{ if(assignedlocation.id == modifiedLocationInfo.id) return true})
            if(index>=0){  
                const location = this.getLocation(modifiedLocationInfo.locationId);
                const timezone = location? location.timezone : 'UTC';          
                this.assignedAwayLocations[index].locationId =  modifiedLocationInfo.locationId;
                this.assignedAwayLocations[index].startDate = moment(modifiedLocationInfo.startDate).tz(timezone).format();
                this.assignedAwayLocations[index].endDate = moment(modifiedLocationInfo.endDate).tz(timezone).format();                
                this.assignedAwayLocations[index]['locationNm'] = location? location.name :'' ;
                this.assignedAwayLocations[index].comment = modifiedLocationInfo.comment? modifiedLocationInfo.comment :'' ;

                if(Vue.filter('isDateFullday')( this.assignedAwayLocations[index].startDate, this.assignedAwayLocations[index].endDate)){ 
                    this.assignedAwayLocations[index]['isFullDay'] = true;
                    this.assignedAwayLocations[index]['_cellVariants'] = {isFullDay:'danger'}                 
                }else{
                    this.assignedAwayLocations[index]['isFullDay'] = false;
                    this.assignedAwayLocations[index]['_cellVariants'] = {isFullDay:'success'}                    
                }

                this.$emit('change');
            } 
        }

        public addToAssignedLocationList(addedLocationInfo){

            const location = this.getLocation(addedLocationInfo.locationId);
            const timezone = location? location.timezone : 'UTC';
            const assignedAwayLocation: awayLocationInfoType =
            {
                id: addedLocationInfo.id,
                sheriffId : addedLocationInfo.sheriffId,    
                locationId: addedLocationInfo.locationId,
                startDate: moment(addedLocationInfo.startDate).tz(timezone).format(),
                endDate: moment(addedLocationInfo.endDate).tz(timezone).format(), 
                comment: addedLocationInfo.comment,              
            }
            
            assignedAwayLocation['locationNm'] = location? location.name :''
            if(Vue.filter('isDateFullday')(assignedAwayLocation.startDate,assignedAwayLocation.endDate)){ 
                assignedAwayLocation['isFullDay'] = true;
                assignedAwayLocation['_cellVariants'] = {isFullDay:'danger'}                 
            }else{
                assignedAwayLocation['isFullDay'] = false;
                assignedAwayLocation['_cellVariants'] = {isFullDay:'success'}                    
            }

            this.assignedAwayLocations.push(assignedAwayLocation); 
            this.$emit('change');                     
        }

        public closeLocationForm(){        
            this.addNewLocationForm= false; 
            this.addFormColor = 'secondary'
            if(this.isEditOpen){
                this.latestEditData.toggleDetails();
                this.isEditOpen = false;
            } 
        }

        public getLocation(locationId: number|null){
            const index = this.allLocationList.findIndex(location=>{if(location.id == locationId)return true})
            if(index>=0) return this.allLocationList[index]; else return "";
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