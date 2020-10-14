<template>
    <div>
        <b-card  style="height:400px;overflow: auto;" no-body>                                        
            <h2 v-if="locationError" class="mx-1 mt-0"><b-badge  variant="danger"> Location changes unsuccessful <b-icon class="ml-3" icon = x-square-fill @click="locationError = false" /></b-badge></h2>

            <b-card no-body class="mx-2 my-0 p-0" border-variant="light"><label class="ml-1 p-0">Home Location:</label>
                <b-form-group style="width: 20rem"> 
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
            </b-card>
            
            
            <b-card class="mb-3" border-variant="light" no-body>
                <b-table-simple small borderless >
                    <b-tbody style="background-color:#BBB">
                        <b-tr>

                            <b-td rowspan="2">
                                <b-form-select
                                    class="mr-1"
                                    style="width: 20rem"                                                       
                                    v-model="selectedLocation"
                                    :state = "locationState?null:false">
                                        <b-form-select-option :value="{}">
                                            Select a location
                                        </b-form-select-option>
                                        <b-form-select-option
                                            v-for="location in locationList" 
                                            :key="location.id"
                                            :value="location">
                                                {{location.name}}
                                        </b-form-select-option>     
                                </b-form-select>
                            </b-td>
                            <b-td>
                                <b-form-datepicker
                                    class="mr-1"
                                    v-model="selectedStartDate"
                                    placeholder="Start Date"
                                    locale="en-US" 
                                    :date-format-options="{ year: 'numeric', month: 'short', day: '2-digit' }"
                                    :state = "startDateState?null:false">
                                </b-form-datepicker>
                            </b-td>
                            <b-td>                                
                                <b-form-datepicker
                                    class="mr-1"
                                    v-model="selectedEndDate"
                                    placeholder="End Date"
                                    locale="en-US"
                                    :date-format-options="{ year: 'numeric', month: 'short', day: '2-digit' }"
                                    :state = "endDateState?null:false">
                                </b-form-datepicker> 
                            </b-td>                            
                            <b-td rowspan="2" >
                                <b-button
                                    variant="success"                        
                                    @click="saveAwayLocation()">
                                    Save
                                </b-button>   
                            </b-td>
                        </b-tr>    
                        <b-tr>                            
                            <b-td>
                                <b-form-timepicker 
                                    v-model="startTime"
                                    placeholder="Start time" 
                                    locale="en">
                                </b-form-timepicker>
                            </b-td>                            
                            <b-td>
                                <b-form-timepicker 
                                    v-model="endTime"
                                    placeholder="End time" 
                                    locale="en">
                                </b-form-timepicker>
                            </b-td>
                        </b-tr>
                        
                    </b-tbody>
                </b-table-simple>
                                      
               
            </b-card>

<!-- 
            <b-card no-body border-variant="white" bg-variant="white" v-if="!assignedRoles.length">
                    <span class="text-muted ml-4 mb-5">No roles have been assigned.</span>
            </b-card>

            <b-card v-else no-body border-variant="light" bg-variant="white">
                <b-table
                    :items="assignedRoles"
                    :fields="roleFields"
                    :key="refreshTable"
                    striped
                    borderless
                    small
                    responsive="sm"
                    >  
                        <template v-slot:cell(effDate)="data" >
                            <span>{{data.value | beautify-date}}</span> 
                        </template>
                        <template v-slot:cell(expDate)="data" >
                            <span>{{data.value | beautify-date}}</span> 
                        </template>
                        <template v-slot:cell(editRole)="data" >                                       
                            <span><b-button variant="transparent" @click="deleteRole(data.item)"><b-icon icon="trash-fill" font-scale="1.75" variant="danger"/></b-button></span>
                            <span><b-button variant="transparent" @click="editRole(data.item)"><b-icon icon="pencil-square" font-scale="1.75" variant="primary"/></b-button></span> 
                        </template>
                        
                </b-table> 
            </b-card>                                       -->
        </b-card>
    </div>
</template>

<script lang="ts">
    import { Component, Vue, Prop } from 'vue-property-decorator';
    import {teamMemberInfoType} from '../../../types/MyTeam';
    import {locationInfoType} from '../../../types/common';
    import { namespace } from 'vuex-class';
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
        // selectedRole = {} as roleOptionInfoType;
        
        selectedLocation = {} as locationInfoType | undefined;
        locationState = true;

        selectedEndDate = ''
        endDateState = true

        selectedStartDate = ''
        startDateState = true

        startTime = ''
        endTime = ''

        
        // refreshTable = 0;

        // roles: roleOptionInfoType[] = []
        locationError = false;

        // rolesJson;

        // assignedRoles: roleOptionInfoType[] = [];

        // roleFields =  
        // [           
        //     {key:'text',    label:'Role',sortable:false, tdClass: 'border-top',  }, 
        //     {key:'effDate', label:'Effective Date',   sortable:false, tdClass: 'border-top', thClass:'',},
        //     {key:'expDate', label:'Expiry Date',      sortable:false, tdClass: 'border-top', thClass:'',}, 
        //     {key:'editRole',  sortable:false, tdClass: 'border-top', thClass:'text-white',},       
        // ];

        mounted()
        {
            console.log('locationTab') 
            this.selectedHomeLocation = this.userToEdit.homeLocation;
            //{id: this.user.homeLocation.id, name: this.user.homeLocation.name, regionId: this.user.homeLocation.regionId}
            //this.user.homeLocation;
            console.log(this.selectedHomeLocation)
            //console.log(this.user)
        }

        public homeLocationChanged()
        {
            Vue.nextTick().then(()=>{
                console.log(this.selectedHomeLocation)

                const body = {
                    homeLocationId: this.selectedHomeLocation? this.selectedHomeLocation.id: '',
                    gender: this.userToEdit.gender,
                    badgeNumber: this.userToEdit.badgeNumber,
                    rank: this.userToEdit.rank,
                    idirName: this.userToEdit.idirUserName,
                    firstName:this.userToEdit.firstName,
                    lastName: this.userToEdit.lastName,
                    email: this.userToEdit.email,
                    id: this.userToEdit.id 
                }

                const url = 'api/sheriff';
                const options = {headers:{'Authorization' :'Bearer '+this.token}} 
                this.$http.put(url, body, options)
                    .then(response => {
                        console.log(response) 
                        this.updateUser();
                                          
                    }, err => {
                        console.log(err)
                        this.locationError = true;
                    });
            });
        }

        updateUser()
        {
            const user = this.userToEdit
            user.homeLocation = this.selectedHomeLocation;
            user.homeLocationNm = this.selectedHomeLocation? this.selectedHomeLocation.name: '';
            user.homeLocationId = this.selectedHomeLocation? this.selectedHomeLocation.id: 0;
            this.UpdateUserToEdit(user);
            this.$emit('change') 
        }
   
        // public GetRoles(){
        //     const url = '/api/role'
        //     const options = {headers:{'Authorization' :'Bearer '+this.token}}
        //     this.$http.get(url, options)
        //         .then(response => {
        //             if(response.data){
        //                 this.rolesJson = response.data
        //                 this.extractRoles(this.rolesJson, this.userAllRoles);                        
        //             }                                   
        //         })
        // }
      
        // public extractRoles(rolesJson, userAllRoles ){
        //     this.roles=[];
        //     this.assignedRoles =[];
        //     this.selectedRole = {} as roleOptionInfoType;
        //     this.roleAssignError = false; 


        //     console.log(userAllRoles)
        //     for(const allRole of userAllRoles) 
        //     { 
        //         this.assignedRoles.push({
        //             text:allRole.role.name, 
        //             desc: allRole.role.description, 
        //             value:allRole.role.id, 
        //             effDate:allRole.effectiveDate, 
        //             expDate:allRole.expiryDate
        //         })
        //     }
        //     this.refreshTable++;
            
        //     for(const role of rolesJson)
        //     {
        //         const index = this.assignedRoles.findIndex(assignrole =>{if(assignrole.value == role.id) return true;else return false});
        //         if(index < 0)
        //         {             
        //             this.roles.push({text:role.name, desc: role.description, value:role.id, effDate:'', expDate:''})           
        //         }
        //     }
        // }        

        public saveAwayLocation(){
            console.log('save away location')
                
        //         this.roleState = true;
        //         this.effDateState = true;
        //         this.roleAssignError = false; 

        //         if(!this.selectedRole)
        //         {
        //             this.roleState = false;
        //         }
        //         else if(this.selectedEffectiveDate == "")
        //         {
        //             this.roleState = true;
        //             this.effDateState =  false;
        //         }
        //         else 
        //         {
        //             this.roleState = true;
        //             this.effDateState = true;

        //             const body = 
        //             [{
        //                 "userId": this.userId,
        //                 "roleId": this.selectedRole.value,
        //                 "effectiveDate": this.selectedEffectiveDate,
        //                 "expiryDate": this.selectedExpiryDate
        //             }]
        //             const url = 'api/sheriff/assignroles' //:'api/sheriff/unassignroles' 
        //             const options = {headers:{'Authorization' :'Bearer '+this.token}}
        //             this.$http.put(url, body, options)
        //                 .then(response => {
        //                     console.log(response)
        //                     console.log('assign success')
                            
        //                     this.selectedRole = {} as roleOptionInfoType;
        //                     this.selectedEffectiveDate ='';
        //                     this.selectedExpiryDate ='';
        //                     this.getUserRoles();

        //                 }, err=>{this.roleAssignError = true;});
        //         }
           
        }

        // public deleteRole(role){
        //     this.roleAssignError = false; 
        //     const body = 
        //     [{
        //         "userId": this.userId,
        //         "roleId": role.value,                        
        //     }]
        //     const url = 'api/sheriff/unassignroles' 
        //     const options = {headers:{'Authorization' :'Bearer '+this.token}}
        //     this.$http.put(url, body, options)
        //         .then(response => {
        //             console.log(response)
        //             console.log('unassign success')
                   
        //             this.getUserRoles();
                                                     
        //         }, err=>{this.roleAssignError = true;});
        // }

        // public getUserRoles()
        // {
        //     const url = 'api/sheriff/' + this.userId
        //     const options = {headers:{'Authorization' :'Bearer '+this.token}}
        //     this.$http.get(url, options)
        //         .then(response => {
        //             if(response.data){
        //                 console.log(response.data)                        
        //                 this.extractRoles(this.rolesJson,response.data.roles);                                                                
        //             }                    
        //         });
        // }
           
        
        // public editRole(role){
        //     console.log('edit role')
        // }

       

     

       
    }
</script>

<style scoped>
    .b-td {
        background-color: lightsalmon;
    }
</style>