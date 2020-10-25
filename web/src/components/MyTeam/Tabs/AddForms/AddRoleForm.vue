<template>
    <div>
        <b-table-simple small borderless>
            <b-tbody>
                <b-tr>
                    <b-td>   
                        <b-tr class="mt-1 mb-1 pb-0 bg-white">   
                            <h4 class="ml-3 my-1 pb-0">Role: </h4>                          
                        </b-tr>
                        <b-tr >                           
                            <b-form-group v-if="isCreate" style="padding: 0; margin: 0rem 0 0 .5rem;width: 15rem"> 
                                <b-form-select
                                    size = "sm"
                                    v-model="selectedRole"
                                    :state = "roleState?null:false">
                                        <b-form-select-option :value="{}">
                                            Select a Role*
                                        </b-form-select-option>
                                        <b-form-select-option
                                            v-for="role in roleTypeInfoList" 
                                            :key="role.value"                                            
                                            :value="role">
                                                    {{role.text}}
                                        </b-form-select-option>     
                                </b-form-select>                                
                            </b-form-group>
                            <b-form-group v-else style="border-radius: 4px; border:1px solid #bbbbbb; padding:0 0 .0 0;; margin: 0 0 0 0.5rem;width: 15rem">                                
                                <b-form-text class="h5 align-middle my-2 font-weight-normal ml-2">
                                    {{this.selectedRole.text}}
                                </b-form-text>
                            </b-form-group>
                        </b-tr>                        
                    </b-td>
                    <b-td>
                        <label class="h6 m-0 p-0"> Effective Date: </label>
                        <b-form-datepicker
                            class="mb-1"
                            size="sm"
                            v-model="selectedEffectiveDate"
                            placeholder="Eff. Date"
                            locale="en-US" 
                            :date-format-options="{ year: 'numeric', month: 'short', day: '2-digit' }"
                            :state = "effectiveDateState?null:false">
                        </b-form-datepicker>                       
                    </b-td>
                    <b-td>
                        <label class="h6 m-0 p-0"> Expiry Date: </label>
                        <b-form-datepicker
                            class="mb-1 mt-0 pt-0"
                            size="sm"
                            v-model="selectedExpiryDate"
                            placeholder="Exp. Date"
                            reset-button
                            locale="en-US"
                            :date-format-options="{ year: 'numeric', month: 'short', day: '2-digit' }"
                            >
                        </b-form-datepicker> 
                    </b-td>
                    <b-td >
                        <b-button                                    
                            style="margin: 1.5rem .5rem 0 0 ; padding:0 .5rem 0 .5rem; "
                            variant="secondary"
                            @click="closeForm()">
                            Cancel
                        </b-button>   
                        <b-button                                    
                            style="margin: 1.5rem 0 0 0; padding:0 0.7rem 0 0.7rem; "
                            variant="success"                                                    
                            @click="saveForm()">
                            Save
                        </b-button>  
                    </b-td>
                </b-tr>   
            </b-tbody>
        </b-table-simple>  

        <b-modal v-model="showCancelWarning" id="bv-modal-role-cancel-warning" header-class="bg-warning text-light">            
            <template v-slot:modal-title>
                <h2 v-if="isCreate" class="mb-0 text-light"> Unsaved New Role </h2>                
                <h2 v-else class="mb-0 text-light"> Unsaved Role Changes </h2>                                 
            </template>
            <p>Are you sure you want to cancel without saving your changes?</p>
            <template v-slot:modal-footer>
                <b-button variant="secondary" @click="$bvModal.hide('bv-modal-role-cancel-warning')"                   
                >No</b-button>
                <b-button variant="success" @click="confirmedCloseForm()"
                >Yes</b-button>
            </template>            
            <template v-slot:modal-header-close>                 
                 <b-button variant="outline-warning" class="text-light closeButton" @click="$bvModal.hide('bv-modal-role-cancel-warning')"
                 >&times;</b-button>
            </template>
        </b-modal>             
    </div>
</template>

<script lang="ts">
    import { Component, Vue, Prop } from 'vue-property-decorator';
    import {teamMemberInfoType ,roleOptionInfoType, userRoleInfoType} from '../../../../types/MyTeam';
    // import {leaveInfoType} from '../../../../types/common';
    // import { leaveTypeJson } from '../../../../types/common/jsonTypes';
    import { namespace } from 'vuex-class';
    import "@store/modules/TeamMemberInformation"; 
    const TeamMemberState = namespace("TeamMemberInformation");

    @Component
    export default class AddRoleForm extends Vue {        

        @TeamMemberState.State
        public userToEdit!: teamMemberInfoType;

        @Prop({required: true})
        formData!: userRoleInfoType;

        @Prop({required: true})
        isCreate!: boolean;

        @Prop({required: true})
        roleTypeInfoList!: roleOptionInfoType[];       

        selectedRole = {} as roleOptionInfoType | undefined;
        roleState = true;      

        selectedExpiryDate = '';
        expiryDateState = true; 

        selectedEffectiveDate = '';
        effectiveDateState = true;

        // originalRole = {} as roleOptionInfoType | undefined;
        originalEffectiveDate = '';
        originalExpiryDate = '';

        formDataId = '';
        showCancelWarning = false;
        
        mounted()
        { 
            this.clearSelections();
            console.log(this.formData)
            console.log(this.roleTypeInfoList)
            if(this.formData.value) {
                this.extractFormInfo();
            }               
        }        

        public extractFormInfo(){
            this.formDataId = this.formData.value? this.formData.value:'';
            this.selectedRole = {text: this.formData.text, desc: this.formData.desc, value: this.formData.value};               
            this.originalEffectiveDate = this.selectedEffectiveDate = this.formData.effectiveDate.substring(0,10)            
            this.originalExpiryDate = this.selectedExpiryDate =  this.formData.expiryDate? this.formData.expiryDate.substring(0,10): '';
        }

        public saveForm(){
                this.roleState = true;
                this.effectiveDateState = true;                

                if(this.selectedRole && !this.selectedRole.value){
                    this.roleState = false;
                }
                else if(this.selectedEffectiveDate == "")
                {
                    this.roleState = true;
                    this.effectiveDateState =  false;
                }
                else 
                {
                    this.roleState = true;
                    this.effectiveDateState = true;

                    const timezone = this.userToEdit.homeLocation? this.userToEdit.homeLocation.timezone :'UTC';
                    const effectiveDate = Vue.filter('convertDate')(this.selectedEffectiveDate,"", 'StartTime',timezone);
                    const expiryDate =this.selectedExpiryDate? Vue.filter('convertDate')(this.selectedExpiryDate,"",'EndTime',timezone): '';

                    const body = [{                        
                        effectiveDate: effectiveDate,
                        expiryDate: expiryDate,
                        roleId: this.selectedRole?this.selectedRole.value:'',
                        text: this.selectedRole?this.selectedRole.text:'',
                        desc: this.selectedRole?this.selectedRole.desc:''
                    }] 
                    this.$emit('submit', body, this.isCreate);                  
                }
        }

        public closeForm(){
            if(this.isChanged())
                this.showCancelWarning = true;
            else
                this.confirmedCloseForm();
        }

        public isChanged(){
            if(this.isCreate){
                if((this.selectedRole && this.selectedRole.value) ||
                    this.selectedEffectiveDate || this.selectedExpiryDate) return true;
                return false;
            }else{
                if((this.originalEffectiveDate != this.selectedEffectiveDate)|| 
                    (this.originalExpiryDate != this.selectedExpiryDate)) return true;
                return false;
            }
        }

        public confirmedCloseForm(){           
            this.clearSelections();
            this.$emit('cancel');
        }

        public clearSelections(){
            this.selectedRole = {} as roleOptionInfoType;
            this.selectedEffectiveDate = '';
            this.selectedExpiryDate = '';
            this.roleState  = true;
            this.effectiveDateState   = true;
            this.expiryDateState = true;           
        }       
    }
</script>

<style scoped>
    td {
        margin: 0rem 0.5rem 0.1rem 0rem;
        padding: 0rem 0.5rem 0.1rem 0rem;
        
        background-color: white ;
    }
</style>