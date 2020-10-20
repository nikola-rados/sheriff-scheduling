
<template>
    <div>
        <b-card  style="height:400px;overflow: auto;" >                                        
            <h2 class="mx-1 mt-0"><b-badge v-if="roleAssignError" variant="danger"> Role Assignment Unsuccessful <b-icon class="ml-3" icon = x-square-fill @click="roleAssignError = false" /></b-badge></h2>

            <b-card class="mb-3">
                <b-input-group >
                    <b-form-select
                        class="mr-1"                                                       
                        v-model="selectedRole"
                        :state = "roleState?null:false"         
                        placeholder="Select a role">
                            <b-form-select-option
                                v-for="role in roles" 
                                :key="role.value"
                                :value="role">
                                        {{role.text}}
                            </b-form-select-option>    
                    </b-form-select>
                    <b-form-datepicker
                        class="mr-1"
                        v-model="selectedEffectiveDate"
                        placeholder="Eff. Date"
                        locale="en-US" 
                        :date-format-options="{ year: 'numeric', month: 'short', day: '2-digit' }"
                        :state = "effDateState?null:false">
                    </b-form-datepicker>
                    <b-form-datepicker
                        class="mr-1"
                        v-model="selectedExpiryDate"
                        placeholder="Exp. Date"
                        reset-button
                        locale="en-US"
                        :date-format-options="{ year: 'numeric', month: 'short', day: '2-digit' }"
                        >
                    </b-form-datepicker> 
                    <b-button
                        variant="success"
                        :disabled="roles.length==0"
                        @click="saveRole()">
                        <b-icon icon="plus" /> Save 
                    </b-button>                           
                </b-input-group> 
            </b-card>

            <b-card no-body v-if="!assignedRoles.length">
                    <span class="text-muted ml-4 mb-5">No roles have been assigned.</span>
            </b-card>

            <b-card v-else no-body >
                <b-table
                    :items="assignedRoles"
                    :fields="roleFields"
                    :key="refreshTable"
                    head-row-variant="primary"
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
                            <span><b-button class="my-0 py-0" size="sm" variant="transparent" @click="confirmDeleteRole(data.item)"><b-icon icon="trash-fill" font-scale="1.75" variant="danger"/></b-button></span>
                            <span><b-button class="my-0 py-0" size="sm" variant="transparent" @click="editRole(data.item)"><b-icon icon="pencil-square" font-scale="1.75" variant="primary"/></b-button></span> 
                        </template>
                        
                </b-table> 
            </b-card>                                      
        </b-card>

         <b-modal v-model="confirmDelete" id="bv-modal-confirm-delete" header-class="bg-warning text-light">
            <template v-slot:modal-title>
                    <h2 class="mb-0 text-light">Confirm Delete Role</h2>                    
            </template>
            <p>Are you sure you want to delete the "{{roleToDelete.desc}}" role?</p>
            <template v-slot:modal-footer>
                <b-button variant="danger" @click="deleteRole()">Delete</b-button>
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
    import {roleOptionInfoType} from '../../../types/MyTeam';
    import { namespace } from 'vuex-class';
    import "@store/modules/CommonInformation";
    const commonState = namespace("CommonInformation");

    @Component
    export default class RoleAssignmentTab extends Vue {

        @Prop({required: true})
        userId!: string;

        @Prop({required: true})
        userAllRoles!: any;

        selectedRole = {} as roleOptionInfoType;
        selectedEffectiveDate =''
        selectedExpiryDate =''
        roleState = true
        effDateState = true

        refreshTable = 0;

        roles: roleOptionInfoType[] = []
        roleAssignError = false;

        rolesJson;

        confirmDelete = false;
        roleToDelete = {} as roleOptionInfoType;

        assignedRoles: roleOptionInfoType[] = [];

        roleFields =  
        [           
            {key:'text',    label:'Role',sortable:false, tdClass: 'border-top',  }, 
            {key:'effDate', label:'Effective Date',   sortable:false, tdClass: 'border-top', thClass:'',},
            {key:'expDate', label:'Expiry Date',      sortable:false, tdClass: 'border-top', thClass:'',}, 
            {key:'editRole', label:'', sortable:false, tdClass: 'border-top', thClass:'text-white',},       
        ];

        mounted()
        {
           // console.log('role') 
            this.GetRoles();
        }
   
        public GetRoles(){
            const url = '/api/role';
            this.$http.get(url)
                .then(response => {
                    if(response.data){
                        this.rolesJson = response.data
                        this.extractRoles(this.rolesJson, this.userAllRoles);                        
                    }                                   
                })
        }
      
        public extractRoles(rolesJson, userAllRoles ){
            this.roles=[];
            this.assignedRoles =[];
            this.selectedRole = {} as roleOptionInfoType;
            this.roleAssignError = false; 


            //console.log(userAllRoles)
            for(const allRole of userAllRoles) 
            { 
                this.assignedRoles.push({
                    text:allRole.role.name, 
                    desc: allRole.role.description, 
                    value:allRole.role.id, 
                    effDate:allRole.effectiveDate, 
                    expDate:allRole.expiryDate
                })
            }
            this.refreshTable++;
            
            for(const role of rolesJson)
            {
                const index = this.assignedRoles.findIndex(assignrole =>{if(assignrole.value == role.id) return true;else return false});
                if(index < 0)
                {             
                    this.roles.push({text:role.name, desc: role.description, value:role.id, effDate:'', expDate:''})           
                }
            }
        }        

        public saveRole(){
                
                this.roleState = true;
                this.effDateState = true;
                this.roleAssignError = false; 

                if(!this.selectedRole)
                {
                    this.roleState = false;
                }
                else if(this.selectedEffectiveDate == "")
                {
                    this.roleState = true;
                    this.effDateState =  false;
                }
                else 
                {
                    this.roleState = true;
                    this.effDateState = true;

                    const body = 
                    [{
                        "userId": this.userId,
                        "roleId": this.selectedRole.value,
                        "effectiveDate": this.selectedEffectiveDate,
                        "expiryDate": this.selectedExpiryDate
                    }]
                    const url = 'api/sheriff/assignroles' 
                    this.$http.put(url, body)
                        .then(response => {
                            console.log(response)
                            console.log('assign success')
                            
                            this.selectedRole = {} as roleOptionInfoType;
                            this.selectedEffectiveDate ='';
                            this.selectedExpiryDate ='';
                            this.getUserRoles();

                        }, err=>{this.roleAssignError = true;});
                }
           
        }

        public confirmDeleteRole(role) {
            this.roleToDelete = role;           
            this.confirmDelete = true; 
        }

        public deleteRole(){
            this.roleAssignError = false;
            this.confirmDelete = false; 
            const body = 
            [{
                "userId": this.userId,
                "roleId": this.roleToDelete.value,                        
            }]
            const url = 'api/sheriff/unassignroles' ;
            this.$http.put(url, body)
                .then(response => {
                    console.log(response)
                    console.log('unassign success')
                   
                    this.getUserRoles();
                                                     
                }, err=>{this.roleAssignError = true;});
        }

        public getUserRoles()
        {
            const url = 'api/sheriff/' + this.userId;
            this.$http.get(url)
                .then(response => {
                    if(response.data){
                        console.log(response.data)                        
                        this.extractRoles(this.rolesJson,response.data.roles);                                                                
                    }                    
                });
        }
           
        
        public editRole(role){
            console.log('edit role')
        }

    }
</script>

<style scoped>
    .card {
        border: white;
    }
</style>