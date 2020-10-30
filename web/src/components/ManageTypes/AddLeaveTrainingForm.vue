<template>
    <div>
        <b-table-simple small borderless>
            <b-tbody>
                <b-tr>
                    <b-td>
                        <b-form-group style="margin: 0.25rem 0 0 0.5rem;width: 18rem">
                            <label class="h6 ml-1 mb-0 pb-0" > {{type}}: </label> 
                            <b-form-input
                                size = "sm"
                                v-model="selectedLeaveTraining"
                                type="text"
                                :placeholder="'Enter '+ type"
                                :state = "leaveTrainingState?null:false">                                           
                            </b-form-input>
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

        <b-modal v-model="showCancelWarning" id="bv-modal-leaveTraining-cancel-warning" header-class="bg-warning text-light">            
            <template v-slot:modal-title>
                <h2 v-if="isCreate" class="mb-0 text-light"> Unsaved New {{type}} </h2>                
                <h2 v-else class="mb-0 text-light"> Unsaved {{type}} Changes </h2>                                 
            </template>
            <p>Are you sure you want to cancel without saving your changes?</p>
            <template v-slot:modal-footer>
                <b-button variant="secondary" @click="$bvModal.hide('bv-modal-leaveTraining-cancel-warning')"                   
                >No</b-button>
                <b-button variant="success" @click="confirmedCloseForm()"
                >Yes</b-button>
            </template>            
            <template v-slot:modal-header-close>                 
                 <b-button variant="outline-warning" class="text-light closeButton" @click="$bvModal.hide('bv-modal-leaveTraining-cancel-warning')"
                 >&times;</b-button>
            </template>
        </b-modal>             
    </div>
</template>

<script lang="ts">
    import { Component, Vue, Prop } from 'vue-property-decorator';
    import {leaveTrainingTypeInfoType}  from '../../types/ManageTypes/index';
    import {locationInfoType} from '../../types/common'; 
    
    import { namespace } from 'vuex-class';
    import "@store/modules/CommonInformation";
    const commonState = namespace("CommonInformation");

    @Component
    export default class AddLeaveTrainingForm extends Vue {        

        @commonState.State
        public location!: locationInfoType;

        @Prop({required: true})
        type!: string;
       
        @Prop({required: true})
        formData!: leaveTrainingTypeInfoType;

        @Prop({required: true})
        isCreate!: boolean;

        @Prop({required: true})
        sortOrder!: number;

        originalLeaveTraining = '';
        selectedLeaveTraining = '';
        leaveTrainingState = true;

        formDataId = 0;
        showCancelWarning = false;

        // locationSpecifics = [
        //     {value: -1, text: 'Province'}
        // ]
        
        mounted()
        { 
            this.clearSelections();

            // this.locationSpecifics = [{value: -1, text: 'Province'}]
            // this.locationSpecifics.push({value:this.location.id, text:this.location.name})
            // this.selectedLocationScope = this.location.id;

            console.log(this.formData)
            if(this.formData.id) {
                this.extractFormInfo();
            }               
        }        

        public extractFormInfo(){
            this.formDataId = this.formData.id? this.formData.id:0;
            this.originalLeaveTraining = this.selectedLeaveTraining = this.formData.code            
            // this.originalLocationScope = this.selectedLocationScope =  (this.formData.locationId == this.location.id) ? this.location.id : -1;

            console.log(this.formDataId)
            console.log(this.originalLeaveTraining)
            // console.log(this.originalLocationScope)
        }

        public saveForm(){                
            this.leaveTrainingState   = true;

            if(!this.selectedLeaveTraining ){
                this.leaveTrainingState  = false;
            }else{
                this.leaveTrainingState  = true;
                
                const body = {
                    code: this.selectedLeaveTraining,
                    locationId: null,
                    sortOrderForLocation : {locationId: null, sortOrder: this.sortOrder}
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
                if( this.selectedLeaveTraining) return true;
                return false;
            }else{
                if(this.originalLeaveTraining != this.selectedLeaveTraining) return true;
                return false;
            }
        }

        public confirmedCloseForm(){           
            this.clearSelections();
            this.$emit('cancel');
        }

        public clearSelections(){
            this.selectedLeaveTraining = '';
            // this.selectedLocationScope = -1;
            this.leaveTrainingState = true;            
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