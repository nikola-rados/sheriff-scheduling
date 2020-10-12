
<template>
    <div>
        <b-card  style="height:400px;overflow: auto;" >                                        
            <h2 class="mx-1 mt-0"><b-badge v-if="roleAssignError" variant="danger"> Role Assignment Unsuccessful <b-icon class="ml-3" icon = x-square-fill @click="roleAssignError = false" /></b-badge></h2>
                    
            <b-form-group >
                <b-form-checkbox-group v-model="selectedRoles">
                    <b-row class="mb-2 text-primary" style="font-weight:bold">
                        <b-col cols="5"> Role </b-col>
                        <b-col class="mx-2" > Effective Date </b-col>
                        <b-col class="mx-2" > Expiry Date </b-col>
                    </b-row>
                    <b-row v-for="(rol,rolInx) in roles" :key="rolInx" class="mb-1">
                        <b-col cols="5">   
                            <b-form-checkbox 
                                class="mt-1"
                                v-b-tooltip.hover.left
                                :title="rol.desc"
                                @change="roleChanged(rolInx)" 
                                :value="rol.value">
                                    {{rol.text}}
                            </b-form-checkbox>
                        </b-col>
                        <b-col >
                            <b-form-datepicker
                            class="p-0 m-0"
                            v-model="roles[rolInx].effDate"
                            placeholder="Eff. Date"
                            locale="en-US" 
                            :date-format-options="{ year: 'numeric', month: 'short', day: '2-digit' }"
                            :state = "rol.effState?null:false"
                            ></b-form-datepicker>
                        </b-col>
                        <b-col >
                            <b-form-datepicker
                            v-model="roles[rolInx].expDate"
                            placeholder="Exp. Date"
                            locale="en-US"
                            :date-format-options="{ year: 'numeric', month: 'short', day: '2-digit' }"
                            :state = "rol.expState?null:false"
                            ></b-form-datepicker>
                        </b-col>
                    </b-row> 
                
                </b-form-checkbox-group>
            </b-form-group>                                       
        </b-card>
    </div>
</template>

<script lang="ts">
    import { Component, Vue, Prop } from 'vue-property-decorator';
    import {roleOptionInfoType} from '../../../types/MyTeam';
    import { namespace } from 'vuex-class';
    const commonState = namespace("CommonInformation");
    import store from '../../../store'

    @Component
    export default class RoleAssignmentTab extends Vue {

        @commonState.State
        public token!: string;

        @Prop({required: true})
        userId!: string;

        @Prop({required: true})
        userActiveRoles!: any;

        selectedRoles: string[] = []
        roles: roleOptionInfoType[] = []
        roleAssignError = false;

        mounted()
        {
            console.log('role') 
            this.GetRoles();
        }

   
        public GetRoles(){
            const url = '/api/role'
            const options = {headers:{'Authorization' :'Bearer '+this.token}}
            this.$http.get(url, options)
                .then(response => {
                    if(response.data){
                        this.extractRoles(response.data);                        
                    }                                   
                })
        }
      
        public extractRoles(data){
            this.roles=[];
            this.selectedRoles = [];
            this.roleAssignError = false; 
            
            for(const role of data)            
                this.roles.push({text:role.name, desc: role.description, value:role.id, effDate:'', expDate:'', effState:true, expState: true})           
        
            for(const activeRole of this.userActiveRoles) 
            {             
                const index = this.roles.findIndex(role =>{if(role.value == activeRole.role.id) return true;else return false});
                if(index >=0)
                { 
                    this.roles[index].effDate = activeRole.effectiveDate
                    this.roles[index].expDate = activeRole.expiryDate
                    this.selectedRoles.push(this.roles[index].value);
                }
            }
        }        

        public roleChanged(rolInx){
            Vue.nextTick().then(()=>{
                const selectedIndex = this.selectedRoles.indexOf(this.roles[rolInx].value);
                console.log(rolInx)
                console.log(this.roles[rolInx].value)
                this.roles[rolInx].effState = true;
                this.roles[rolInx].expState = true; 
                this.roleAssignError = false; 

                if(selectedIndex >=0 && this.roles[rolInx].effDate=="")
                {
                   this.roles[rolInx].effState = false; 
                   this.selectedRoles.splice(selectedIndex, 1);
                }
                else 
                {
                    const body = 
                    [{
                        "userId": this.userId,
                        "roleId": this.roles[rolInx].value,
                        "effectiveDate": this.roles[rolInx].effDate,
                        "expiryDate": this.roles[rolInx].expDate
                    }]
                    const url = (selectedIndex >= 0)? 'api/sheriff/assignroles' :'api/sheriff/unassignroles' 
                    const options = {headers:{'Authorization' :'Bearer '+this.token}}
                    this.$http.put(url, body, options)
                        .then(response => {
                            console.log(response)
                            console.log('assign success')  
                            if(selectedIndex < 0)
                            {
                                this.roles[rolInx].effDate =""
                                this.roles[rolInx].expDate="" 
                            }                                              
                        }, err=>{this.roleAssignError = true;});
                }
            });
        }

       

     

       
    }
</script>