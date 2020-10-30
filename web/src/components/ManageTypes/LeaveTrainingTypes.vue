<template>
    <b-card bg-variant="white" class="home">    
        <b-row class="bg-white">
            <b-col cols="9">
                <page-header :pageHeaderText="selectedLeaveTrainingType.label + 's'"></page-header>                
            </b-col>
            <b-col cols="2">
                <b-form-group style="margin: 0.25rem 0 0 0.5rem;width: 15rem"> 
                    <b-form-select
                        size = "lg"
                        @change="getLeaveTraining"
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
                        striped
                        border
                        small
                        v-sortLeaveTrainingType 
                        id="mytable"
                        responsive="sm"> 

                            <template v-slot:table-colgroup>
                                <col style="width:4rem">
                            </template>
                                              
                            <template v-slot:head(code) >
                                <span>{{selectedLeaveTrainingType.label}}</span> 
                            </template>

                            <template v-slot:cell(sortOrder)= "data" >
                                <span><b-icon class="handle mr-3" icon="arrows-expand"/>{{data.value}}</span> 
                            </template>

                            <template v-slot:cell(edit)="data" >                                       
                                <b-button class="my-0 py-0" size="sm" variant="transparent" @click="confirmDelete(data.item)"><b-icon icon="trash-fill" font-scale="1.25" variant="danger"/></b-button>
                                <b-button class="my-0 py-0" size="sm" variant="transparent" @click="editLeaveTraining(data)"><b-icon icon="pencil-square" font-scale="1.25" variant="primary"/></b-button>
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
    import {locationInfoType} from '../../types/common'; 
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

        @manageTypesState.State
        public sortingLeaveTrainingInfo!: {prvIndex: number; newIndex: number};

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

        leaveTrainingList: leaveTrainingTypeInfoType[] = [];
        selectedLeaveTrainingType = {name:'LeaveType', label:'Leave'};
        
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
            this.getLeaveTraining()       
        }

        public getLeaveTraining() {            
            this.isLeaveTrainingDataMounted = false;
            const url = 'api/managetypes?codeType='+this.selectedLeaveTrainingType.name;
            console.log(url)
            this.$http.get(url)
                .then(response => {
                    if(response.data){
                        console.log(response.data)
                        this.extractLeaveTrainings(response.data);                        
                    }
                    this.isLeaveTrainingDataMounted = true;
                })        
        }

        public extractLeaveTrainings(leaveTrainingsJson) {

            this.leaveTrainingList = [];
            this.sortIndex = leaveTrainingsJson.length? leaveTrainingsJson.length : 0;
            for(const leaveTrainingJson of leaveTrainingsJson)
            {                
                const leaveTraining = {} as leaveTrainingTypeInfoType;
                leaveTraining.id = leaveTrainingJson.id;
                leaveTraining.code = leaveTrainingJson.code;
                leaveTraining.sortOrder = leaveTrainingJson.sortOrder?leaveTrainingJson.sortOrder:(this.sortIndex++);
                leaveTraining.type = leaveTrainingJson.type;
                this.leaveTrainingList.push(leaveTraining)                
            }

            console.log(this.leaveTrainingList)
            this.refineSortOrders();
        }

        public refineSortOrders(){
            this.leaveTrainingList = _.sortBy(this.leaveTrainingList,'sortOrder');
            for(const listIndex in this.leaveTrainingList)
                this.leaveTrainingList[listIndex].sortOrder = Number(listIndex);
            this.updateTable++;
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

        public confirmDelete(leaveTraining){
            console.log("deleting" + leaveTraining)
            console.log(this.sortIndex)
        }

        public editLeaveTraining(leaveTraining){
            console.log(leaveTraining)
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
            console.log(body)

            body['type']= this.selectedLeaveTrainingType.name;

            // body['sortOrderForLocation'] = {locationId: body.locationId, sortOrder: this.sortIndex}
            const method = iscreate? 'post' :'put';            
            const url = 'api/managetypes'  
            const options = { method: method, url:url, data:body}
            
            this.$http(options)
                .then(response => {
                    // if(iscreate) 
                    //     this.addToAssignedTrainingList(response.data);
                    // else
                    //     this.modifyAssignedTrainingList(response.data);
                    
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

    
}
</script>

<style scoped>   
    .card {
        border: white;
    }
</style>