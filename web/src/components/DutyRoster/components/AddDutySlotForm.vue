<template>
    <div>
        <b-table-simple small borderless>
            <b-tbody>
                <b-tr>
                    <b-td>
                        <b-tr>
                            <b-form-group style="margin: 0.25rem 0 0 0.5rem;width: 15rem">                            
                                <label class="h6 m-0 p-0">Sheriff<span class="text-danger">*</span></label>
                                <b-form-select 
                                    size="sm"
                                    v-model="selectedSheriff.sheriffId"
                                    :state = "sheriffState?null:false">
                                        <b-form-select-option
                                            v-for="sheriff in shiftAvailabilityInfo"
                                            :key="sheriff.sheriffId"
                                            :value="sheriff.sheriffId">
                                                    {{sheriff.firstName}} {{sheriff.lastName}}
                                        </b-form-select-option>
                                </b-form-select>
                            </b-form-group>
                        </b-tr>                        
                    </b-td>
                    <b-td >
                        <b-tr>
                            <b-td>
                                <b-form-group style="margin: 0.25rem 0 0 0; width: 4.5rem">
                                    <label class="h6 m-0 p-0">From<span class="text-danger">*</span></label>
                                    <b-form-input
                                        v-model="selectedStartTime"
                                        @click="startTimeState=true"
                                        size="sm"
                                        type="text"
                                        autocomplete="off"
                                        @paste.prevent
                                        :formatter="timeFormat"
                                        placeholder="HH:MM"
                                        :state = "startTimeState?null:false"
                                    ></b-form-input>
                                </b-form-group>
                            </b-td>
                            <b-td>
                                <b-form-group style="margin: 0.25rem 0 0 0; width: 4.5rem;">
                                    <label class="h6 m-0 p-0">To<span class="text-danger">*</span></label>
                                    <b-form-input
                                        v-model="selectedEndTime"
                                        @click="endTimeState=true"
                                        size="sm"
                                        type="text"
                                        autocomplete="off"
                                        @paste.prevent
                                        :formatter="timeFormat"
                                        placeholder="HH:MM"
                                        :state = "endTimeState?null:false"
                                    ></b-form-input>
                                </b-form-group>
                            </b-td>	
                        </b-tr>
                        <b-tr>
                            <b-td>	
                                <b-button                                    
                                    style="margin: 1.25rem .5rem 0 0 ; padding:0 .5rem 0 .5rem; "
                                    variant="secondary"
                                    @click="closeForm()">
                                    Cancel
                                </b-button>
                            </b-td>
                            <b-td>		       
                                <b-button                                    
                                    style="margin: 1.25rem 0 0 0; padding:0 0.7rem 0 0.7rem; "
                                    variant="success"                        
                                    @click="saveForm()">
                                    Save
                                </b-button>
                            </b-td>	
                        </b-tr>
                    </b-td>                   
                </b-tr>   
            </b-tbody>
        </b-table-simple>  

        <b-modal v-model="showCancelWarning" id="bv-modal-duty-slot-cancel-warning" header-class="bg-warning text-light">            
            <template v-slot:modal-title>
                <h2 v-if="isCreate" class="mb-0 text-light"> Unsaved New Duty Slot </h2>                
                <h2 v-else class="mb-0 text-light"> Unsaved Duty Slot Changes </h2>                                 
            </template>
            <p>Are you sure you want to cancel without saving your changes?</p>
            <template v-slot:modal-footer>
                <b-button variant="secondary" @click="$bvModal.hide('bv-modal-duty-slot-cancel-warning')"                   
                >No</b-button>
                <b-button variant="success" @click="confirmedCloseForm()"
                >Yes</b-button>
            </template>            
            <template v-slot:modal-header-close>                 
                 <b-button variant="outline-warning" class="text-light closeButton" @click="$bvModal.hide('bv-modal-duty-slot-cancel-warning')"
                 >&times;</b-button>
            </template>
        </b-modal>             
    </div>
</template>

<script lang="ts">
    import { Component, Vue, Prop } from 'vue-property-decorator';
    import { dutyBlockInfoType, myTeamShiftInfoType } from '../../../types/DutyRoster';
    import { namespace } from 'vuex-class';
    import "@store/modules/DutyRosterInformation";
import { shiftInfoType } from '../../../types/ShiftSchedule';
    const dutyState = namespace("DutyRosterInformation");

    @Component
    export default class AddDutySlotForm extends Vue {        

        @dutyState.State
        public shiftAvailabilityInfo!: myTeamShiftInfoType[];

        @Prop({required: true})
        formData!: dutyBlockInfoType;

        @Prop({required: true})
        isCreate!: boolean;
        

        selectedSheriff = {} as myTeamShiftInfoType | undefined;
        sheriffState = true;        

        selectedStartTime = '';
        startTimeState = true;

        selectedEndTime = '';
        endTimeState = true; 

        originalSheriff = {} as myTeamShiftInfoType | undefined;       
        originalStartTime = '';
        originalEndTime = '';


        formDataId = '0';
        showCancelWarning = false;
        
        mounted()
        { 
            this.clearSelections();
            if(this.formData.id) {
                this.extractFormInfo();
            }               
        }        

        public extractFormInfo(){
            this.formDataId = this.formData.id? this.formData.id:'0';
            
            const index = this.shiftAvailabilityInfo.findIndex(sheriff=>{if(sheriff.sheriffId == this.formData.sheriffId)return true})
            this.originalSheriff = this.selectedSheriff = (index>=0)? this.shiftAvailabilityInfo[index]: {} as myTeamShiftInfoType;            
              
            this.originalStartTime = this.selectedStartTime = this.formData.startTimeString;            
            this.originalEndTime = this.selectedEndTime = this.formData.endTimeString;
        }

        public autoCompleteTime(time){
            const tail="00:00";
            let result = "";
            
            if(time.length==1) result = '0'+time+ tail.slice(2);
            else if(time.length==4) {
                if(time.slice(3,4)=='2') result = time.slice(0,3)+'15';
                else result = time+(time.slice(-1)=='1' || time.slice(-1)=='4'?'5':'0');
            }
            else if(time.length==5){
                if(time.slice(3,4)=='2') result = time.slice(0,3)+'15';
                else result =time.slice(0,4)+(time.slice(3,4)=='1' || time.slice(3,4)=='4'?'5':'0');
            }
            else  result = time+ tail.slice(time.length);
            return result
        }

        public timeFormat(value , event) {
			if(isNaN(Number(value.slice(-1))) && value.slice(-1) != ':') return value.slice(0,-1)
			if(value.length!=3 && value.slice(-1) == ':') return value.slice(0,-1);
			if(value.length==2 && event.data && value.slice(0,1)>=3 && (value.slice(-1)>=5 || value.slice(-1)==2)) return value.slice(0,-1);
			if(value.length==2 && event.data && value.slice(0,1)>=3 && (value.slice(-1)==0 || value.slice(-1)==3)) return '0'+ value.slice(0,1)+':'+value.slice(1,2)+'0';
			if(value.length==2 && event.data && value.slice(0,1)>=3 && (value.slice(-1)==1 || value.slice(-1)==4)) return '0'+ value.slice(0,1)+':'+value.slice(1,2)+'5';
			if(value.length==2 && event.data && value.slice(0,1)==2 && value.slice(-1)>=5) return value.slice(0,-1);
			if(value.length==2 && event.data && value.slice(0,1)==2 && (value.slice(-1)==4 || value.slice(-1)==4)) return '02:45';
			if(value.length==2 && event.data && value.slice(-1)!=0 && value.slice(-1)!=1 && value.slice(-1)!=3 && value.slice(-1)!=4) return value.slice(0,2)+':';
			if(value.length==2 && event.data) return '0'+value.slice(0,1)+':'+value.slice(1,2);
			if(value.length==3 && value.slice(-1)!=':' ) return value.slice(0,2)+':';
			if(value.length==4 && event.data && (value.slice(0,1)>0||value.slice(1,2)>1) && (value.slice(-1)>=5 || value.slice(-1)==2)) return value.slice(0,-1);
			if(value.length==4 && event.data && value.slice(0,1)>0 && (value.slice(-1)==0 || value.slice(-1)==3)) return value.slice(0,4)+'0';
			if(value.length==4 && event.data && value.slice(0,1)>0 && (value.slice(-1)==1 || value.slice(-1)==4)) return value.slice(0,4)+'5';
			if(value.length==4 && event.data && value.slice(0,1)==0 && value.slice(1,2)<2 && value.slice(-1)>=5 ) return value.slice(1,2)+value.slice(3,4)+':';
			if(value.length==5 && (value.slice(0,2)>=24 || value.slice(3,5)>=60)) return '';
			if(value.length==5 && value.slice(0,2)>=3 && (value.slice(3,4)==0 || value.slice(3,4)==3)) return value.slice(0,4)+'0';
			if(value.length==5 && value.slice(0,2)>=3 && (value.slice(3,4)==1 || value.slice(3,4)==4)) return value.slice(0,4)+'5';
			if(value.length==6 && value.slice(0,1)==0 && (value.slice(4,6)==0||value.slice(4,6)==15||value.slice(4,6)==30||value.slice(4,6)==45) && (value.slice(1,2)+value.slice(3,4))<24) return value.slice(1,2)+value.slice(3,4)+':'+value.slice(4,5)+value.slice(5,6);
		
			if(value.length>5) return value.slice(0,5);
			if(value.length==5 && ( isNaN(value.slice(0,2)) || isNaN(value.slice(3,5)) || value.slice(2,3)!=':') )return '';
			if(value.length==4 && ( isNaN(value.slice(0,2)) || isNaN(value.slice(3,4)) || value.slice(2,3)!=':') )return '';
			return value
		}

        public saveForm(){
            this.sheriffState  = true;                
            this.startTimeState = true;
            this.endTimeState   = true;

            if(this.selectedSheriff && !this.selectedSheriff.sheriffId ){
                this.sheriffState  = false;
            
            }else if(this.selectedEndTime == "" && this.selectedStartTime != ""){
                this.sheriffState  = true;
                
                this.startTimeState = true;
                this.endTimeState   = false;
            }else if(this.selectedStartTime == "" && this.selectedEndTime != ""){
                this.sheriffState  = true;                    
                this.endTimeState   = true;
                this.startTimeState = false;
            }else{
                this.sheriffState  = true;                    
                this.endTimeState   = true;
                this.startTimeState = true;

                
                const body = {
                    sheriffId: this.selectedSheriff?this.selectedSheriff.sheriffId:0,
                    startTime: this.selectedStartTime,
                    endTime: this.selectedEndTime,
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
                if((this.selectedSheriff && this.selectedSheriff.sheriffId) ||
                    this.selectedStartTime || this.selectedEndTime) return true;
                return false;
            }else{
                if((this.originalSheriff && this.selectedSheriff && (this.originalSheriff.sheriffId != this.selectedSheriff.sheriffId)) ||
                    (this.originalStartTime != this.selectedStartTime) || 
                    (this.originalEndTime != this.selectedEndTime)) return true;
                return false;
            }
        }

        public confirmedCloseForm(){           
            this.clearSelections();
            this.$emit('cancel');
        }

        public clearSelections(){
            this.selectedSheriff = {} as myTeamShiftInfoType;           
            this.selectedStartTime = '';
            this.selectedEndTime = '';
            this.sheriffState  = true;
            this.startTimeState = true;
            this.endTimeState   = true;            
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