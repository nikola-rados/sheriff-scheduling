<template>
    <div>
        <b-table-simple small borderless>
            <b-tbody>
                <b-tr>
                    <b-td>
                        <b-form-group style="margin: 0.25rem 0 0 0.5rem;width: 18rem">
                            <label class="h6 ml-1 mb-0 pb-0" > Assignment: </label> 
                            <b-form-input
                                size = "sm"
                                v-model="selectedAssignment"
                                type="text"
                                :placeholder="'Enter '+ type"
                                :state = "assignmentState?null:false">                                           
                            </b-form-input>
                        </b-form-group>           
                    </b-td>
                    <b-td>
                        <b-form-group style="margin: 0.25rem 0 0 0.5rem;width: 18rem">
                            <label class="h6 ml-1 mb-0 pb-0" > Location specification: </label> 
                            <b-form-select
                                size = "sm"
                                v-model="selectedLocationScope"
                                :options ="locationSpecifics">
                            </b-form-select>
                        </b-form-group>                                            
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

        <b-modal v-model="showCancelWarning" id="bv-modal-assignment-cancel-warning" header-class="bg-warning text-light">            
            <template v-slot:modal-title>
                <h2 v-if="isCreate" class="mb-0 text-light"> Unsaved New Assignment </h2>                
                <h2 v-else class="mb-0 text-light"> Unsaved Assignment Changes </h2>                                 
            </template>
            <p>Are you sure you want to cancel without saving your changes?</p>
            <template v-slot:modal-footer>
                <b-button variant="secondary" @click="$bvModal.hide('bv-modal-assignment-cancel-warning')"                   
                >No</b-button>
                <b-button variant="success" @click="confirmedCloseForm()"
                >Yes</b-button>
            </template>            
            <template v-slot:modal-header-close>                 
                 <b-button variant="outline-warning" class="text-light closeButton" @click="$bvModal.hide('bv-modal-assignment-cancel-warning')"
                 >&times;</b-button>
            </template>
        </b-modal>             
    </div>
</template>

<script lang="ts">
    import { Component, Vue, Prop } from 'vue-property-decorator';
    import {assignmentTypeInfoType}  from '../../types/ManageTypes/index';
    import {locationInfoType} from '../../types/common'; 
    
    import { namespace } from 'vuex-class';
    import "@store/modules/CommonInformation";
    const commonState = namespace("CommonInformation");

    @Component
    export default class AddAssignmentForm extends Vue {        

        @commonState.State
        public location!: locationInfoType;

        @Prop({required: true})
        type!: string;
       
        @Prop({required: true})
        formData!: assignmentTypeInfoType;

        @Prop({required: true})
        isCreate!: boolean;

        originalAssignment = '';
        selectedAssignment = '';
        assignmentState = true;

        originalLocationScope = -1;
        selectedLocationScope = -1;  
        

        formDataId = 0;
        showCancelWarning = false;

        locationSpecifics = [
            {value: -1, text: 'Province'}
        ]
        
        mounted()
        { 
            this.clearSelections();

            this.locationSpecifics = [{value: -1, text: 'Province'}]
            this.locationSpecifics.push({value:this.location.id, text:this.location.name})
            this.selectedLocationScope = this.location.id;

            console.log(this.formData)
            if(this.formData.id) {
                this.extractFormInfo();
            }               
        }        

        public extractFormInfo(){
            this.formDataId = this.formData.id? this.formData.id:0;
            this.originalAssignment = this.selectedAssignment = this.formData.code            
            this.originalLocationScope = this.selectedLocationScope =  (this.formData.locationId == this.location.id) ? this.location.id : -1;

            console.log(this.formDataId)
            console.log(this.originalAssignment)
            console.log(this.originalLocationScope)
        }

        public saveForm(){                
            this.assignmentState   = true;

            if(!this.selectedAssignment ){
                this.assignmentState  = false;
            }else{
                this.assignmentState  = true;

                const body = {
                    code: this.selectedAssignment,
                    locationId: this.selectedLocationScope == -1 ? null: this.selectedLocationScope,
                    id: this.formDataId
                } 
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
                if( this.selectedAssignment ||
                    this.selectedLocationScope == -1) return true;
                return false;
            }else{
                if( (this.originalAssignment != this.selectedAssignment)|| 
                    (this.originalLocationScope != this.selectedLocationScope)) return true;
                return false;
            }
        }

        public confirmedCloseForm(){           
            this.clearSelections();
            this.$emit('cancel');
        }

        public clearSelections(){
            this.selectedAssignment = '';
            this.selectedLocationScope = -1;
            this.assignmentState = true;            
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