<template>
    <b-card bg-variant="white">
        <b-row>
            <b-col cols="11">
                <page-header :pageHeaderText="sectionHeader"></page-header>
            </b-col>
            <b-col style="padding: 0;">
                <b-button v-if="hasPermissionToAddNewRoles" style="max-height: 40px;" size="sm" variant="success" @click="AddRole()"><b-icon-plus/>Add Role</b-button>
            </b-col>
        </b-row>

        <loading-spinner v-if= "!isRolesDataMounted" />
            
        <b-card v-else bg-variant="light">

            <b-card no-body border-variant="white" bg-variant="white" v-if="!roleData.length">
                    <span class="text-muted ml-4 mb-5">No roles.</span>
            </b-card>

            <b-card v-else no-body border-variant="light" bg-variant="white">
                <b-table  :items="roleData"
                        :fields="roleFields"
                        class="mx-4"
                        borderless
                        striped
                        small 
                        responsive="sm"
                        >
                        <template v-slot:cell(edit)="row">
                        <b-button v-if="hasPermissionToExpireRoles" size="sm" variant="transparent" @click="removeRole(row.item, row.index)">
                            <b-icon-trash-fill font-scale="1.75" variant="danger"></b-icon-trash-fill>                    
                        </b-button>
                        <b-button size="sm" variant="transparent" @click="openRoleDetails(row.item.id)">
                            <b-icon-pencil-square font-scale="1.75" variant="primary"></b-icon-pencil-square>                    
                        </b-button>
                        </template>
                        <template v-slot:cell(name)="row">                  
                            <span>{{row.item.name}}</span>
                        </template>
                        <template v-slot:cell(description)="row">                  
                            <span>{{ row.item.description}}</span>
                        </template>
                </b-table>
            </b-card>
        </b-card>

        <b-modal size="xl" v-model="showRoleDetails" id="bv-modal-role-details" header-class="bg-primary text-light">            
            <template v-slot:modal-title>                
                 <h2 v-if="editMode" class="mb-0 text-light"> Updating Role </h2>
                 <h2 v-else-if="createMode" class="mb-0 text-light"> Creating Role </h2>                
            </template>
            <b-card  v-if="isRoleDetailsMounted">
                <b-row class="mx-1"> 
                    <b-form-group class="mr-1" style="width: 20rem"><label>Name<span class="text-danger">*</span></label>
                        <b-form-input v-model="role.name" placeholder="Enter Name" :state = "nameState?null:false"></b-form-input>
                    </b-form-group>                                    
                    <b-form-group class="ml-1" style="width: 45rem"><label>Description<span class="text-danger">*</span></label>
                        <b-form-input v-model="role.description" placeholder="Enter Description" :state = "descriptionState?null:false"></b-form-input>
                    </b-form-group>
                </b-row>
                <h2 class="mx-1 mt-0"><b-badge v-if="duplicateRole" variant="danger"> Duplicate Role</b-badge></h2>
                <b-row class="mx-1 mt-1">
                    <b-card no-body style="height:300px;width: 66rem;overflow-y: auto; overflow-x:hidden;">
                        <b-form-checkbox-group :state="permissionState?null:false" v-model="selectedPermissions"><label>Permissions<span class="text-danger">*</span></label>
                            <b-row class="mb-2 text-primary" style="font-weight:bold">
                                <b-col cols="4"> Name </b-col>
                                <b-col cols="8"> Description </b-col>                            
                            </b-row>

                            <b-row v-for="permission in sortPermissions(permissions) " :key="permission.value" class="mb-1">
                                <b-col cols="4">   
                                    <b-form-checkbox                                                                                 
                                        class="mt-1"                                        
                                        @change="permissionChanged" 
                                        :value="permission.value">
                                            {{permission.text}}
                                    </b-form-checkbox>
                                </b-col>
                                <b-col cols="8" style="width: 40rem">
                                    {{permission.desc}}
                                </b-col>
                            </b-row>
                        </b-form-checkbox-group>
            
                    </b-card>
                </b-row>                
                
            </b-card>
            <template v-slot:modal-footer>
                <b-button
                  variant="secondary" 
                  @click="closeRoleWindow()"                  
                ><b-icon-x font-scale="1.5" style="padding:0; vertical-align: middle; margin-right: 0.25rem;"></b-icon-x>Cancel</b-button>
                <b-button                 
                  variant="success"
                  :disabled="!hasPermissionToEditRoles && editMode" 
                  @click="saveRole('testing updates')"
                ><b-icon-check2 style="padding:0; vertical-align: middle; margin-right: 0.25rem;"></b-icon-check2>Save</b-button>
            </template>            
            <template v-slot:modal-header-close>                 
                 <b-button
                  variant="outline-primary"
                  class="text-light closeButton"
                  @click="$bvModal.hide('bv-modal-team-member-details')"                  
                  >
                  &times;</b-button>
            </template>           
        </b-modal>

        <b-modal v-model="confirmDelete" id="bv-modal-confirm-delete" header-class="bg-warning text-light">
            <template v-slot:modal-title>
                    <h2 class="mb-0 text-light">Confirm Delete Role</h2>                    
            </template>
            <p>Are you sure you want to delete the "{{roleToDelete.name}}" role?</p>
            <template v-slot:modal-footer>
                <b-button variant="danger" @click="confirmRemoveRole()">Delete</b-button>
                <b-button variant="primary" @click="$bvModal.hide('bv-modal-confirm-delete')">Cancel</b-button>
            </template>            
            <template v-slot:modal-header-close>                 
                 <b-button variant="outline-warning" class="text-light closeButton" @click="$bvModal.hide('bv-modal-confirm-delete')"
                 >&times;</b-button>
            </template>
        </b-modal>

        <b-modal v-model="showCancelWarning" id="bv-modal-role-cancel-warning" header-class="bg-warning text-light">            
            <template v-slot:modal-title>                
                 <h2 v-if="editMode" class="mb-0 text-light"> Unsaved Role Changes </h2>
                 <h2 v-else-if="createMode" class="mb-0 text-light"> Unsaved New Role </h2>                
            </template>
            <p>Are you sure you want to cancel without saving your changes?</p>
            <template v-slot:modal-footer>
                <b-button variant="secondary" @click="$bvModal.hide('bv-modal-role-cancel-warning')"                   
                >No</b-button>
                <b-button variant="success" @click="closeWarningWindow()"
                >Yes</b-button>
            </template>            
            <template v-slot:modal-header-close>                 
                 <b-button variant="outline-warning" class="text-light closeButton" @click="$bvModal.hide('bv-modal-role-cancel-warning')"
                 >&times;</b-button>
            </template>
        </b-modal>

        <b-modal v-model="openErrorModal" header-class="bg-warning text-light">
            <b-card class="h4 mx-2 py-2">
				<span class="p-0">{{errorText}}</span>
            </b-card>                        
            <template v-slot:modal-footer>
                <b-button variant="primary" @click="openErrorModal=false">Ok</b-button>
            </template>            
            <template v-slot:modal-header-close>                 
                <b-button variant="outline-warning" class="text-light closeButton" @click="openErrorModal=false"
                >&times;</b-button>
            </template>
        </b-modal> 

    </b-card>
</template>

<script lang="ts">
    import { Component, Vue } from 'vue-property-decorator';
    import { namespace } from 'vuex-class';
    import moment from 'moment-timezone';
    import "@store/modules/CommonInformation";
    const commonState = namespace("CommonInformation");
    import * as _ from 'underscore';    
    import PageHeader from "@components/common/PageHeader.vue";  
    import {userInfoType} from '../../types/common';
    import {roleInfoType, permissionOptionInfoType} from '../../types/MyTeam';
    import {roleJsonType} from '../../types/MyTeam/jsonTypes';


    @Component({
        components: {
            PageHeader
        }
    })
    export default class DefineRolesAccess extends Vue {

        @commonState.State
        public userDetails!: userInfoType;
        
        showRoleDetails = false;
        showCancelWarning = false;
        role = {} as roleInfoType;
        originalRole = {} as roleInfoType;
        roleJson;
        nameState = true;
        descriptionState = true;
        permissionState = true;
        duplicateRole = false;        
        isRolesDataMounted = false;
        isRoleDetailsMounted = false;
        editMode = false;
        createMode = false;
        sectionHeader = "";
        errorCode = 0;
        errorText = '';
        //userIsAdmin = false;
        hasPermissionToAddNewRoles = false;
        hasPermissionToEditRoles = false;
        hasPermissionToExpireRoles = false;
        selectedPermissions: string[] = [];
        originalSelectedPermissions: string[] = [];
        permissions: permissionOptionInfoType[] = []

        openErrorModal=false;

        roleFields = [
          { key: 'name', label: 'Name'},
          { key: 'description', label: 'Description'},
          { key: 'edit', thClass: 'd-none'}
        ];

        confirmDelete= false;        
        roleToDelete: roleInfoType = {id:'',name:'', description:'', expiryDate: '', permissions: []};
        indexToDelete = -1;
        roleToEditId = 0;
        
        roleData: roleInfoType[] = [];

        mounted() {
            this.hasPermissionToAddNewRoles = this.userDetails.permissions.includes("CreateAndAssignRoles");
            this.hasPermissionToEditRoles = this.userDetails.permissions.includes("EditRoles");
            this.hasPermissionToExpireRoles = this.userDetails.permissions.includes("ExpireRoles");
            
            //this.userIsAdmin = true;
            this.getRoles();
            this.sectionHeader = "Manage System Roles and Access";
        }

        public getRoles()
        {
            this.isRolesDataMounted = false;
            const url = 'api/role';
            this.$http.get(url)
                .then(response => {
                    if(response.data){
                        // console.log(response.data)
                        this.extractRolesInfo(response.data);                        
                    }                    
                },err => {
                    this.errorText = this.errorText=err.response.statusText+' '+err.response.status+ '  - ' + moment().format();
                    if (err.response.status != '401') {
                        this.openErrorModal=true;
                    }  
                    this.isRolesDataMounted = true;
                })
        }

        public extractRolesInfo(data: roleJsonType[])
        {
            this.roleData = [];            
            for(const roleInfo of data)
            {
                const role: roleInfoType = {id:'',name:'', description:'', expiryDate: '', permissions: []}
                role.id = roleInfo.id;
                role.name = roleInfo.name;
                role.description = roleInfo.description;
                role.expiryDate = roleInfo.expiryDate;
                const permissions: string[] = []               
                for(const permissionInfo of roleInfo.rolePermissions)
                {                   
                    permissions.push(permissionInfo.permissionId)
                }
                role.permissions = permissions;
                this.roleData.push(role);
            }
            this.isRolesDataMounted = true;
        }

        public openRoleDetails(roleId)
        {
            this.createMode = false;
            this.editMode = true;
            this.nameState = true;
            this.duplicateRole = false;
            this.roleToEditId = roleId;
            this.getPermissions()
        }

        public sortPermissions(permissions) {
            return _.sortBy(permissions,'selected').reverse()        
        }

        public getPermissions(): void {

            const url = 'api/permission';
            this.$http.get(url)
                .then(response => {
                    if(response.data){
                        this.extractPermissions(response.data);                                               
                    }                    
                },err => {
                    this.errorText=err.response.statusText+' '+err.response.status + '  - ' + moment().format(); 
                    if (err.response.status != '401') {
                        this.openErrorModal=true;
                    }  
                    this.isRolesDataMounted = true;
                });
        }

        public closeWarningWindow() {   
            this.resetRoleWindowState();         
            this.showCancelWarning = false;
            this.showRoleDetails = false;
        }

        public loadRoleDetails(roleId): void {
            this.editMode = true;            
            const url = 'api/role/' + roleId
            this.$http.get(url)
                .then(response => {
                    if(response.data){                        
                        this.extractRoleInfo(response.data);                 
                    }                    
                },err => {
                    this.errorText=err.response.statusText+' '+err.response.status + '  - ' + moment().format(); 
                    if (err.response.status != '401') {
                        this.openErrorModal=true;
                    } 
                    this.isRolesDataMounted = true;
                });
        }

        public extractRoleInfo(roleData){
            this.role = {};
            this.selectedPermissions = [];
            this.originalRole = {};
            this.originalSelectedPermissions = [];
            this.role.id = this.originalRole.id = roleData.id;
            this.role.name = this.originalRole.name = roleData.name;
            this.role.description = this.originalRole.description = roleData.description;            
            for(const rolePermission of roleData.rolePermissions) {
                const index = this.permissions.findIndex(permission => {if (permission.value == rolePermission.permission.id) return true; else return false;})
                if (index >= 0) {
                    this.selectedPermissions.push(this.permissions[index].value);
                    this.originalSelectedPermissions.push(this.permissions[index].value)
                    this.permissions[index].selected = true;                    
                }
            }
            this.isRoleDetailsMounted = true;
            this.showRoleDetails=true;   
        }
        
        
        public permissionChanged(){
            Vue.nextTick().then(()=>{
                for(const permissionInx in this.permissions)
                    this.permissions[permissionInx].selected = this.selectedPermissions.includes(this.permissions[permissionInx].value)
                
            });
        }

        public extractPermissions(permissionsData){
            this.permissions=[];
            this.selectedPermissions = [];
            if (this.createMode) {
                for(const permission of permissionsData) {
                    this.permissions.push({text:permission.name, desc:permission.description , value:permission.id, selected:false})
                }
                this.isRoleDetailsMounted = true;
                this.showRoleDetails=true; 
            }
            if (this.editMode) {
                for(const permission of permissionsData) {
                    this.permissions.push({text:permission.name, desc:permission.description , value:permission.id, selected:false})
                }
                this.loadRoleDetails(this.roleToEditId);
            }        
        }

        public closeRoleWindow() 
        {                    
            if(this.createMode && this.isEmpty(this.role) && this.selectedPermissions.length < 1)
            {
                this.showRoleDetails = false;
                this.resetRoleWindowState();
            }             
            else if(this.editMode && !this.changesMade())
            {
                this.showRoleDetails = false;
                this.resetRoleWindowState();
            }    
            else
                this.showCancelWarning = true;
        }

        public changesMade(): boolean {            
            return (!_.isEqual(this.originalRole, this.role) || 
            !_.isEqual(this.originalSelectedPermissions, this.selectedPermissions))
        }

        public isEmpty(obj){
            for(const prop in obj) 
                if(obj[prop] != null)
                    return false;
            return true;
        }        

        public resetRoleWindowState() {
            this.createMode = false;
            this.editMode = false;
            this.nameState = true;
            this.descriptionState = true;
            this.permissionState = true;            
            this.duplicateRole = false;
            this.role = {} as roleInfoType;
            this.originalRole = {} as roleInfoType;
            this.selectedPermissions = [];
            this.originalSelectedPermissions = [];
        }

        public AddRole()
        {  
            this.createMode = true;
            this.editMode = false;
            this.getPermissions();            
        }

        public saveRole() { 

            let requiredErrors = 0;
            if (!this.role.name) {
                this.nameState = false;
                requiredErrors+=1;
            } else {
                this.nameState = true;
            }
            if (!this.role.description) {
                this.descriptionState = false;
                requiredErrors+=1;
            } else {
                this.descriptionState = true;
            }
            
            if (!(this.selectedPermissions.length > 0)) {
                this.permissionState = false;
                requiredErrors+=1;
            } else {
                this.permissionState = true;
            }            
            if (requiredErrors == 0) {
                if (this.editMode) this.updateRole();
                if (this.createMode) this.createRole();
            }           
        }

        public updateRole(): void {
            const body = {
                role: { 
                    id: this.role.id,
                    name: this.role.name,               
                    description: this.role.description
                },
                permissionIds: this.selectedPermissions                
            }
            // console.log(body)

            const url = 'api/role'; 
            this.$http.put(url, body)
                .then(response => {
                    if(response.data){
                        this.resetRoleWindowState();
                        this.showRoleDetails = false;
                        this.getRoles();                     
                    }                    
                }, err => {
                    this.errorText = err.response.data.error                    
                    if(this.errorText.includes('already exists'))
                    {
                        this.nameState = false;
                        this.duplicateRole = true;
                    }
                });
        }

        public createRole() {
            const body = {
                role: { 
                    name: this.role.name,               
                    description: this.role.description
                },
                permissionIds: this.selectedPermissions                
            }
            // console.log(body)
            const url = 'api/role';
            this.$http.post(url, body)
                .then(response => {
                    if(response.data){
                        this.resetRoleWindowState();
                        this.showRoleDetails = false;
                        this.getRoles();                     
                    }
                }, err => {
                    this.errorText = err.response.data.error                    
                    if(this.errorText.includes('already exists'))
                    {
                        this.nameState = false;
                        this.duplicateRole = true;
                    }
                })   
        }        

        public removeRole(role: roleInfoType, index: number) {
            //console.log(role)
            this.roleToDelete = role;
            this.indexToDelete = index;            
            this.confirmDelete=true;      
        }

        public confirmRemoveRole() {
            this.$http.delete('/api/role?id='+ this.roleToDelete.id)
            .then(response => {
                    if(response.status == 204){
                        this.confirmDelete=false;
                        this.getRoles();                                            
                    }                    
                })
                 
        }

    }
</script>

<style scoped>   

    .card {
        border: white;
    }

</style>