<template>
    <div>
        <b-card  style="height:400px;overflow: auto;" no-body> 
            <b-card id="LocationError" no-body>                                       
                <h2 v-if="locationError" class="mx-1 mt-2"><b-badge v-b-tooltip.hover :title="locationErrorMsgDesc"  variant="danger"> {{locationErrorMsg}} <b-icon class="ml-3" icon = x-square-fill @click="locationError = false" /></b-badge></h2>
            </b-card>
            
            <b-card v-if="!addNewLocationForm">
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
                                <b-button class="my-0 py-0" size="sm" variant="transparent" @click="confirmDeleteLocation(data.item)"><b-icon icon="trash-fill" font-scale="1.25" variant="danger"/></b-button>
                                <b-button class="my-0 py-0" size="sm" variant="transparent" @click="editLocation(data)"><b-icon icon="pencil-square" font-scale="1.25" variant="primary"/></b-button>
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
            <p>Are you sure you want to delete the "{{locationToDelete.locationNm?locationToDelete.locationNm.nameFull:''}}" location?</p>
            <template v-slot:modal-footer>
                <b-button variant="danger" @click="deleteLocation()">Delete</b-button>
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
    import { Component, Vue, Prop } from 'vue-property-decorator';
    import moment from 'moment-timezone';
    import {teamMemberInfoType, awayLocationInfoType} from '../../../types/MyTeam';
    import {locationInfoType} from '../../../types/common';
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
        public locationList!: locationInfoType[];

        @TeamMemberState.State
        public userToEdit!: teamMemberInfoType;

        addNewLocationForm = false;
        locationError = false;
        locationErrorMsg = '';
        locationErrorMsgDesc = '';

        confirmDelete = false;
        locationToDelete = {} as awayLocationInfoType;

        addFormColor = 'secondary';
        latestEditData;
        isEditOpen = false;
        
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

        public confirmDeleteLocation(location) {
            //console.log(location)
            this.locationToDelete = location;           
            this.confirmDelete=true; 
        }

        public deleteLocation(){
            console.log('delete location')
            this.confirmDelete = false;

            this.locationError = false; 
            const url = 'api/sheriff/awaylocation?id='+this.locationToDelete.id;
            this.$http.delete(url)
                .then(response => {
                    //console.log(response)
                    console.log('delete success')
                    const index = this.assignedAwayLocations.findIndex(assignedlocation=>{if(assignedlocation.id == this.locationToDelete.id) return true;})
                    if(index>=0) this.assignedAwayLocations.splice(index,1);
                    this.$emit('change');
                }, err=>{
                    const errMsg = err.response.data.error;
                    this.locationErrorMsg = errMsg.slice(0,60) + (errMsg.length>60?' ...':'');
                    this.locationErrorMsgDesc = errMsg;
                    this.locationError = true;
                    location.href = '#LocationError'
                });
        }
        
        public addNewLocation(){
            if(this.isEditOpen){
                location.href = '#Lo-Date-'+this.latestEditData.item.startDate.substring(0,10)
                this.addFormColor = 'danger'
            }else
                this.addNewLocationForm = true
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
                    //console.log(response)
                    console.log('save success')
                    if(iscreate) 
                        this.addToAssignedLocationList(response.data);
                    else
                        this.modifyAssignedLocationList(response.data);
                    this.closeLocationForm();
                }, err=>{   
                    console.log(err.response.data);
                    const errMsg = err.response.data.error;
                    this.locationErrorMsg = errMsg.slice(0,60) + (errMsg.length>60?' ...':'');
                    this.locationErrorMsgDesc = errMsg;
                    this.locationError = true;
                    location.href = '#LocationError'
                });                
        }

        public modifyAssignedLocationList(modifiedLocationInfo){

            const index = this.assignedAwayLocations.findIndex(assignedlocation =>{ if(assignedlocation.id == modifiedLocationInfo.id) return true})
            if(index>=0){            
                this.assignedAwayLocations[index].locationId =  modifiedLocationInfo.locationId
                this.assignedAwayLocations[index].startDate = modifiedLocationInfo.startDate
                this.assignedAwayLocations[index].endDate = modifiedLocationInfo.endDate 
                this.assignedAwayLocations[index]['locationNm'] = this.getLocationName(modifiedLocationInfo.locationId);
                if(this.isDateFullday( this.assignedAwayLocations[index].startDate, this.assignedAwayLocations[index].endDate)){ 
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

        public closeLocationForm(){
            console.log('close form')  
            //console.log(this.latestEditData)          
            this.addNewLocationForm= false; 
            this.addFormColor = 'secondary'
            if(this.isEditOpen){
                this.latestEditData.toggleDetails();
                this.isEditOpen = false;
            } 
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