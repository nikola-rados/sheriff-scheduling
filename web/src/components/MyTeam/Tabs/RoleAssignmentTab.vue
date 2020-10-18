
<template>
    <div>
        <b-card  style="height:400px;overflow: auto;" >                                        
            <h2 class="mx-1 mt-0"><b-badge v-if="roleAssignError" variant="danger"> Role Assignment Unsuccessful <b-icon class="ml-3" icon = x-square-fill @click="roleAssignError = false" /></b-badge></h2>

            <b-card class="mb-3" border-variant="light">
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
            </b-card>                                      
        </b-card>
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

        @commonState.State
        public token!: string;

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

        assignedRoles: roleOptionInfoType[] = [];

        roleFields =  
        [           
            {key:'text',    label:'Role',sortable:false, tdClass: 'border-top',  }, 
            {key:'effDate', label:'Effective Date',   sortable:false, tdClass: 'border-top', thClass:'',},
            {key:'expDate', label:'Expiry Date',      sortable:false, tdClass: 'border-top', thClass:'',}, 
            {key:'editRole',  sortable:false, tdClass: 'border-top', thClass:'text-white',},       
        ];

        mounted()
        {
           // console.log('role') 
            this.GetRoles();
        }
   
        public GetRoles(){
            const url = '/api/role'
            const options = {headers:{'Authorization' :'Bearer '+this.token}}
            this.$http.get(url, options)
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
                    const url = 'api/sheriff/assignroles' //:'api/sheriff/unassignroles' 
                    const options = {headers:{'Authorization' :'Bearer '+this.token}}
                    this.$http.put(url, body, options)
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

        public deleteRole(role){
            this.roleAssignError = false; 
            const body = 
            [{
                "userId": this.userId,
                "roleId": role.value,                        
            }]
            const url = 'api/sheriff/unassignroles' 
            const options = {headers:{'Authorization' :'Bearer '+this.token}}
            this.$http.put(url, body, options)
                .then(response => {
                    console.log(response)
                    console.log('unassign success')
                   
                    this.getUserRoles();
                                                     
                }, err=>{this.roleAssignError = true;});
        }

        public getUserRoles()
        {
            const url = 'api/sheriff/' + this.userId
            const options = {headers:{'Authorization' :'Bearer '+this.token}}
            this.$http.get(url, options)
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