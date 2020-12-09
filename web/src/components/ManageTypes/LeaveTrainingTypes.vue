<template>
    <b-card bg-variant="white" class="home">    
        <b-row class="bg-white">
            <b-col cols="8">
                <page-header :pageHeaderText="selectedLeaveTrainingType.label + 's'"></page-header>                
            </b-col>
            <b-col cols="2">
                <b-form-group style="margin: 0.25rem 0 0 0.5rem;width: 15rem"> 
                    <b-form-select
                        size = "lg"
                        @change="changeLeaveTraining"
                        v-model="selectedLeaveTrainingType">                            
                            <b-form-select-option
                                v-for="leaveTrainingType in leaveTrainingTypeTabs" 
                                :key="leaveTrainingType.name"                                
                                :value="leaveTrainingType">
                                    {{leaveTrainingType.label}}
                            </b-form-select-option>     
                    </b-form-select>
                </b-form-group>
            </b-col>
            <b-col cols="2" >
                <div :class="expiredViewChecked?'bg-warning':''" :style="(expiredViewChecked?'width: 10.75rem;':'width: 12.75rem;')+'margin: .75rem 1rem 0 .5rem;'">
                    <b-form-checkbox class="ml-2" v-model="expiredViewChecked"  @change="getLeaveTraining()" size="lg"  switch>
                        {{viewStatus}}
                    </b-form-checkbox>
                </div>
            </b-col>
        </b-row>

        <loading-spinner v-if= "!isLeaveTrainingDataMounted" />

        <b-card v-else no-body style="width: 50rem; margin: 0 auto 8rem auto">                                        
            <b-card id="LeaveTrainingError" no-body>
                <h2 v-if="leaveTrainingError" class="mx-1 mt-2"><b-badge v-b-tooltip.hover :title="leaveTrainingErrorMsgDesc"  variant="danger"> {{leaveTrainingErrorMsg}} <b-icon class="ml-3" icon = x-square-fill @click="leaveTrainingError = false" /></b-badge></h2>
            </b-card>

            <div>
                <b-card  v-if="!addNewLeaveTrainingForm">                
                    <b-button size="sm" variant="success" @click="addNewLeaveTraining"> <b-icon icon="plus" /> Add </b-button>
                </b-card>

                <b-card v-else id="addLeaveTrainingForm" class="my-3" :border-variant="addFormColor" style="border:2px solid" body-class="m-0 px-0 py-1">
                    <add-leave-training-form :type="selectedLeaveTrainingType.label" :formData="{}" :isCreate="true" :sortOrder="sortIndex" v-on:submit="saveLeaveTraining" v-on:cancel="closeLeaveTrainingForm" />              
                </b-card>
            </div>

            <div>
                <b-card no-body border-variant="white" bg-variant="white" v-if="!leaveTrainingList.length" style="width: 50rem; margin: 0 auto 8rem auto">
                    <span class="text-muted ml-4 my-5">No {{selectedLeaveTrainingType.label}}s exist.</span>
                </b-card>

                <b-card v-else  no-body border-variant="light" bg-variant="white" style="width: 50rem; margin: 0 auto 8rem auto">
                    <b-table
                        :key="updateTable"
                        :items="leaveTrainingList"
                        :fields="fields"                        
                        sort-icon-left
                        head-row-variant="primary"
                        :striped="!expiredViewChecked"
                        borderless
                        small
                        v-sortLeaveTrainingType
                        responsive="sm"> 

                            <template v-slot:table-colgroup>
                                <col style="width:4rem">
                                <col>
                                <col style="width:6rem">
                            </template>
                                              
                            <template v-slot:head(code) >
                                <span>{{selectedLeaveTrainingType.label}}</span> 
                            </template>

                            <template v-slot:cell(sortOrder)= "data" >
                                <span v-if="!data.item['_rowVariant']"><b-icon class="handle ml-3" icon="arrows-expand"/></span> 
                            </template>

                            <template v-slot:cell(edit)="data" >                                       
                                <b-button v-if="userIsAdmin && !data.item['_rowVariant']" 
                                    class="ml-2 px-1"
                                    style="padding: 1px 2px 1px 2px;" 
                                    size="sm"
                                    v-b-tooltip.hover
                                    title="Expire"  
                                    variant="warning" 
                                    @click="confirmDeleteLeaveTraining(data.item)">
                                    <b-icon icon="clock" 
                                        font-scale="1" 
                                        variant="white"/>
                                </b-button>
                                <b-button v-if="userIsAdmin && data.item['_rowVariant']" class="my-0 ml-2 py-0 px-1" size="sm" variant="warning" @click="confirmUnexpireLeaveTraining(data.item)"><b-icon icon="arrow-counterclockwise" font-scale="1.25" variant="danger"/></b-button>
                                <b-button v-if="userIsAdmin" :disabled="data.item['_rowVariant']?true:false" class="my-0 py-0" size="sm" variant="transparent" @click="editLeaveTraining(data)"><b-icon icon="pencil-square" font-scale="1.25" variant="primary"/></b-button>
                            </template>

                            <template v-slot:row-details="data">
                                <b-card :id="'#LeaveTraining-'+data.item.code" body-class="m-0 px-0 py-1" :border-variant="addFormColor" style="border:2px solid">
                                    <add-leave-training-form :type="selectedLeaveTrainingType.label" :formData="data.item" :sortOrder="data.item.sortOrder" :isCreate="false" v-on:submit="saveLeaveTraining" v-on:cancel="closeLeaveTrainingForm" />
                                </b-card>
                            </template>
                    </b-table>
                </b-card> 
            </div>                                     
        </b-card>

        <b-modal v-model="confirmDelete" id="bv-modal-confirm-delete" header-class="bg-warning text-light">
            <template v-slot:modal-title>
                    <h2 v-if="deleteType == 'expire'" class="mb-0 text-light">Confirm Expire {{selectedLeaveTrainingType.label}}</h2>
                    <h2 v-else class="mb-0 text-light">Confirm Unexpire {{selectedLeaveTrainingType.label}}</h2>                     
            </template>
            <h4 v-if="deleteType == 'expire'">Are you sure you want to expire the "{{selectedLeaveTrainingType.label}}: {{leaveTrainingToDelete.code?leaveTrainingToDelete.code:''}}"?</h4>
            <h4 v-else>Are you sure you want to Unexpire the "{{selectedLeaveTrainingType.label}}: {{leaveTrainingToDelete.code?leaveTrainingToDelete.code:''}}"?</h4>
            <template v-slot:modal-footer>
                <b-button variant="danger" @click="deleteLeaveTraining()">Confirm</b-button>
                <b-button variant="primary" @click="$bvModal.hide('bv-modal-confirm-delete')">Cancel</b-button>
            </template>            
            <template v-slot:modal-header-close>                 
                <b-button variant="outline-warning" class="text-light closeButton" @click="$bvModal.hide('bv-modal-confirm-delete')"
                >&times;</b-button>
            </template>
        </b-modal>    

    </b-card>
</template>

<script lang="ts">
    import { Component, Vue, Watch } from 'vue-property-decorator';
    import { namespace } from 'vuex-class';
    import "@store/modules/CommonInformation";
    const commonState = namespace("CommonInformation");
    import "@store/modules/ManageTypesInformation";
    const manageTypesState = namespace("ManageTypesInformation");
    import PageHeader from "@components/common/PageHeader.vue"; 
    import AddLeaveTrainingForm from "../ManageTypes/AddLeaveTrainingForm.vue"
    import {locationInfoType, userInfoType} from '../../types/common'; 
    import {leaveTrainingTypeInfoType}  from '../../types/ManageTypes/index'
    import * as _ from 'underscore';
    import sortLeaveTrainingType from './utils/sortLeaveTrainingType';
    
    @Component({
        components: {
            PageHeader,
            AddLeaveTrainingForm
        },
        directives: {     
            sortLeaveTrainingType
        }
    })
    export default class LeaveTrainingTypes extends Vue {

        @commonState.State
        public location!: locationInfoType;

        @commonState.State
        public userDetails!: userInfoType;

        @manageTypesState.State
        public sortingLeaveTrainingInfo!: {prvIndex: number; newIndex: number};


        userIsAdmin = false;
        // sectionHeader = 'Leave/Training';
        isLeaveTrainingDataMounted = false;
        isEditOpen = false;
        latestEditData;

        leaveTrainingError = false;
        leaveTrainingErrorMsg = '';
        leaveTrainingErrorMsgDesc = '';

        addNewLeaveTrainingForm = false;
        addFormColor = 'dark';
        updateTable =0;

        sortIndex = 0;
        expiredViewChecked = false;

        saveOrderFlag = false;

        confirmDelete = false;
        deleteType = 'expire'; // 'unexpire'
        leaveTrainingToDelete = {} as leaveTrainingTypeInfoType;

        leaveTrainingList: leaveTrainingTypeInfoType[] = [];
        selectedLeaveTrainingType = {name:'LeaveType', label:'Leave'};
        previousSelectedLeaveTrainingType = {name:'LeaveType', label:'Leave'};
        
        leaveTrainingTypeTabs = 
        [
            {name:'LeaveType', label:'Leave'},
            {name:'TrainingType', label:'Training'}
        ]

        fields =  
        [     
            {key:'sortOrder', label:'', sortable:false, tdClass: 'border-top' },       
            {key:'code', label:'', sortable:false, tdClass: 'border-top'  },
            {key:'edit', label:'',  sortable:false, tdClass: 'border-top', thClass:'',},       
        ];

        @Watch('sortingLeaveTrainingInfo', { immediate: true })
        sortingLeaveTrainingInfoChange()
        {
            if (this.isLeaveTrainingDataMounted) {
                for(const listIndex in this.leaveTrainingList)
                    if(Number(listIndex) == this.sortingLeaveTrainingInfo.prvIndex)
                        this.leaveTrainingList[listIndex].sortOrder = this.sortingLeaveTrainingInfo.newIndex*2 + Math.sign(this.sortingLeaveTrainingInfo.newIndex-this.sortingLeaveTrainingInfo.prvIndex)
                    else
                        this.leaveTrainingList[listIndex].sortOrder *=2;
                this.saveOrderFlag = true;
                this.refineSortOrders();
                
            }         
        } 
        
        @Watch('location.id', { immediate: true })
        locationChange()
        {
            if (this.isLeaveTrainingDataMounted) {
                this.getLeaveTraining()
            }            
        } 

        mounted () 
        {        
            // this.userIsAdmin = this.userDetails.roles.includes("Administrator");  
            this.userIsAdmin = true;   
            this.getLeaveTraining()       
        }

        public getLeaveTraining() { 
            this.closeLeaveTrainingForm(); 
            Vue.nextTick(()=>{         
                this.isLeaveTrainingDataMounted = false;
                const url = 'api/managetypes?codeType='+this.selectedLeaveTrainingType.name+'&showExpired='+this.expiredViewChecked;
                //console.log(url)
                this.$http.get(url)
                    .then(response => {
                        if(response.data){
                            // console.log(response.data)
                            this.extractLeaveTrainings(response.data);                        
                        }
                        this.isLeaveTrainingDataMounted = true;
                    }) 
            });       
        }

        public extractLeaveTrainings(leaveTrainingsJson) {

            this.leaveTrainingList = [];
            this.sortIndex = leaveTrainingsJson.length? 5000+leaveTrainingsJson.length : 0;
            for(const leaveTrainingJson of leaveTrainingsJson)
            {                
                const leaveTraining = {} as leaveTrainingTypeInfoType;
                leaveTraining.id = leaveTrainingJson.id;
                leaveTraining.code = leaveTrainingJson.code;
                
                leaveTraining['_rowVariant'] = '';
                let sortOrderOffset = 0;
                if(leaveTrainingJson.expiryDate){
                    leaveTraining['_rowVariant'] = 'info';
                    sortOrderOffset = 10000;
                }
                leaveTraining.sortOrder = leaveTrainingJson.sortOrderForLocation?leaveTrainingJson.sortOrderForLocation.sortOrder:(sortOrderOffset+this.sortIndex++);
                leaveTraining.type = leaveTrainingJson.type;
                this.leaveTrainingList.push(leaveTraining)                
            }

            // console.log(this.leaveTrainingList)
            this.refineSortOrders();
        }

        public refineSortOrders(){
            this.leaveTrainingList = _.sortBy(this.leaveTrainingList,'sortOrder');
            for(const listIndex in this.leaveTrainingList){
                if(!this.leaveTrainingList[listIndex]['_rowVariant'])
                    this.leaveTrainingList[listIndex].sortOrder = Number(listIndex);
            }
                
            this.updateTable++;
            if(this.saveOrderFlag) this.saveSortOrders();
        }

        public saveSortOrders(){
            const sortOrders: {lookupCodeId: number ; sortOrder: number}[] = [];

            for(const leaveTraining of this.leaveTrainingList ){
                if(!leaveTraining['_rowVariant'])
                    sortOrders.push({lookupCodeId: leaveTraining.id , sortOrder: leaveTraining.sortOrder})
            }

            const body = {
                sortOrderLocationId: null,
                sortOrders: sortOrders
            }

            //console.log(sortOrders)
            //console.log(body)
            const url = 'api/managetypes/updatesort' 

            this.$http.put(url, body)
                .then(response => {
                    // console.log(response)
                }, err=>{
                    const errMsg = err.response.data.error;
                    this.leaveTrainingErrorMsg = errMsg.slice(0,60) + (errMsg.length>60?' ...':'');
                    this.leaveTrainingErrorMsgDesc = errMsg;
                    this.leaveTrainingError = true;
                    location.href = '#LeaveTrainingError'
                });  

        }
    
        public addNewLeaveTraining(){
            if(this.isEditOpen){
                location.href = '#LeaveTraining-'+this.latestEditData.item.code
                this.addFormColor = 'danger'
            }else
            {
                this.addNewLeaveTrainingForm = true;
                this.$nextTick(()=>{location.href = '#addLeaveTrainingForm';})
            }
        }

        public confirmUnexpireLeaveTraining(leaveTraining){
            this.leaveTrainingToDelete = leaveTraining;           
            this.deleteType = 'unexpire';           
            this.confirmDelete = true;
        }

        public confirmDeleteLeaveTraining(leaveTraining){
            this.leaveTrainingToDelete = leaveTraining;
            this.deleteType = 'expire';           
            this.confirmDelete = true;
        }

        public deleteLeaveTraining(){
            this.confirmDelete = false; 
            const url = 'api/managetypes/'+this.leaveTrainingToDelete.id+'/'+this.deleteType;
            this.$http.put(url)
                .then(response => {
                    this.saveOrderFlag = true;
                    this.getLeaveTraining();
                }, err=>{
                    const errMsg = err.response.data.error;
                    this.leaveTrainingErrorMsg = errMsg.slice(0,60) + (errMsg.length>60?' ...':'');
                    this.leaveTrainingErrorMsgDesc = errMsg;
                    this.leaveTrainingError = true;
                    location.href = '#LeaveTrainingError'
                });
        }

        public editLeaveTraining(leaveTraining){
            if(this.addNewLeaveTrainingForm){
                location.href = '#addLeaveTrainingForm'
                this.addFormColor = 'danger'
            }else if(this.isEditOpen){
                location.href = '#LeaveTraining-'+this.latestEditData.item.code
                this.addFormColor = 'danger'               
            }else if(!this.isEditOpen && !leaveTraining.detailsShowing){
                leaveTraining.toggleDetails();
                this.isEditOpen = true;
                this.latestEditData = leaveTraining
                Vue.nextTick().then(()=>{
                    location.href = '#LeaveTraining-'+this.latestEditData.item.code
                });
            }  
        }

        public saveLeaveTraining(body, iscreate){
            this.leaveTrainingError = false;
            body['type'] = this.selectedLeaveTrainingType.name;
            body['description'] = body.code;
            const method = iscreate? 'post' :'put';            
            const url = 'api/managetypes'  
            const options = { method: method, url:url, data:body}
            
            this.$http(options)
                .then(response => {
                    if(iscreate) 
                        this.addLeaveTrainingToList(response.data);
                    else
                        this.modifyLeaveTrainingList(response.data);
                    
                    this.closeLeaveTrainingForm();
                }, err=>{
                    const errMsg = err.response.data.error;
                    this.leaveTrainingErrorMsg = errMsg.slice(0,60) + (errMsg.length>60?' ...':'');
                    this.leaveTrainingErrorMsgDesc = errMsg;
                    this.leaveTrainingError = true;
                    location.href = '#LeaveTrainingError'
                });                
        }

        public closeLeaveTrainingForm() {                     
            this.addNewLeaveTrainingForm = false; 
            this.addFormColor = 'secondary'
            if(this.isEditOpen){
                this.latestEditData.toggleDetails();
                this.isEditOpen = false;
            } 
        }

        public addLeaveTrainingToList(leaveTrainingJson){
            this.sortIndex++;
            const leaveTraining = {} as leaveTrainingTypeInfoType;
            leaveTraining.id = leaveTrainingJson.id;
            leaveTraining.code = leaveTrainingJson.code;
            leaveTraining.sortOrder = leaveTrainingJson.sortOrderForLocation? leaveTrainingJson.sortOrderForLocation.sortOrder :(this.sortIndex++);
            leaveTraining.type = leaveTrainingJson.type; 
            this.leaveTrainingList.push(leaveTraining);
            this.saveOrderFlag = true;
            this.refineSortOrders();
        }

        public modifyLeaveTrainingList(modifiedLeaveTrainingJson){            

            const index = this.leaveTrainingList.findIndex(leaveTraining =>{ if(leaveTraining.id == modifiedLeaveTrainingJson.id) return true})
            if(index>=0){
                this.leaveTrainingList[index].id =  modifiedLeaveTrainingJson.id;                
                this.leaveTrainingList[index].code = modifiedLeaveTrainingJson.code;
                this.leaveTrainingList[index].sortOrder = modifiedLeaveTrainingJson.sortOrderForLocation? modifiedLeaveTrainingJson.sortOrderForLocation.sortOrder :(this.sortIndex++);
                this.leaveTrainingList[index].type = modifiedLeaveTrainingJson.type;                
            }
            this.saveOrderFlag = true;
            this.refineSortOrders();
        }

        get viewStatus() {
            if(this.expiredViewChecked) return 'All '+this.selectedLeaveTrainingType.label+'s';else return 'Active '+this.selectedLeaveTrainingType.label+'s';
        }

        public changeLeaveTraining(){
            if(this.addNewLeaveTrainingForm){
                location.href = '#addLeaveTrainingForm'
                this.addFormColor = 'danger';
                this.selectedLeaveTrainingType = this.previousSelectedLeaveTrainingType;
            }else if(this.isEditOpen){
                location.href = '#LeaveTraining-'+this.latestEditData.item.code
                this.addFormColor = 'danger'
                this.selectedLeaveTrainingType =  this.previousSelectedLeaveTrainingType;              
            }else{
                this.previousSelectedLeaveTrainingType = this.selectedLeaveTrainingType;
                this.expiredViewChecked = false;
                this.getLeaveTraining();
            }
        }
    
}
</script>

<style scoped>   
    .card {
        border: white;
    }
</style>