<template>
    <b-card bg-variant="white" class="home">    
        <b-row class="bg-white">
            <b-col cols="9">
                <page-header :pageHeaderText="location.name+' '+sectionHeader"></page-header>                
            </b-col>
            <b-col cols="2">
                <b-form-group style="margin: 0.25rem 0 0 0.5rem;width: 15rem"> 
                    <b-form-select
                        size = "lg"
                        @change="getAssignments"
                        v-model="selectedAssignmentType">                            
                            <b-form-select-option
                                v-for="assignmentType in assignmentTypeTabs" 
                                :key="assignmentType.name"                                
                                :value="assignmentType">
                                    {{assignmentType.label}}
                            </b-form-select-option>     
                    </b-form-select>
                </b-form-group>
            </b-col>
        </b-row>

        <loading-spinner v-if= "!isAssignmentDataMounted" />

        <b-card v-else no-body style="width: 50rem; margin: 0 auto 8rem auto">                                        
            <b-card id="AssignmentError" no-body>
                <h2 v-if="assignmentError" class="mx-1 mt-2"><b-badge v-b-tooltip.hover :title="assignmentErrorMsgDesc"  variant="danger"> {{assignmentErrorMsg}} <b-icon class="ml-3" icon = x-square-fill @click="assignmentError = false" /></b-badge></h2>
            </b-card>
            <b-card  v-if="!addNewAssignmentForm">                
                <b-button size="sm" variant="success" @click="addNewAssignment"> <b-icon icon="plus" /> Add </b-button>
            </b-card>

            <b-card v-if="addNewAssignmentForm" id="addAssignmentForm" class="my-3" :border-variant="addFormColor" style="border:2px solid" body-class="m-0 px-0 py-1">
                <add-assignment-form :type="selectedAssignmentType.label" :formData="{}" :isCreate="true" v-on:submit="saveAssignment" v-on:cancel="closeAssignmentForm" />              
            </b-card>

            <div>
                <b-card no-body border-variant="white" bg-variant="white" v-if="!assignmentList.length" style="width: 50rem; margin: 0 auto 8rem auto">
                    <span class="text-muted ml-4 my-5">No {{selectedAssignmentType.name}} assignment exist.</span>
                </b-card>

                <b-card v-else  no-body border-variant="light" bg-variant="white" style="width: 50rem; margin: 0 auto 8rem auto">
                    <b-table
                        :key="updateTable"
                        :items="assignmentList"
                        :fields="fields"                        
                        sort-icon-left
                        head-row-variant="primary"
                        striped
                        border
                        small
                        v-sortAssignmentType 
                        id="mytable"
                        responsive="sm"> 

                            <template v-slot:table-colgroup>
                                <col style="width:4rem">
                            </template>
                                              
                            <template v-slot:head(code) >
                                <span>{{selectedAssignmentType.label}}</span> 
                            </template>

                            <template v-slot:head(scope) = "data" >
                                <span v-if="selectedAssignmentType.name != 'CourtRoom'">{{data.label}}</span> 
                                <span v-else></span>
                            </template>

                            <template v-slot:cell(sortOrder)= "data" >
                                <span><b-icon class="handle mr-3" icon="arrows-expand"/>{{data.value}}</span> 
                            </template>

                            <template v-slot:cell(edit)="data" >                                       
                                <b-button class="my-0 py-0" size="sm" variant="transparent" @click="confirmDelete(data.item)"><b-icon icon="trash-fill" font-scale="1.25" variant="danger"/></b-button>
                                <b-button class="my-0 py-0" size="sm" variant="transparent" @click="editAssignment(data)"><b-icon icon="pencil-square" font-scale="1.25" variant="primary"/></b-button>
                            </template>

                            <template v-slot:row-details="data">
                                <b-card :id="'#Assignment-'+data.item.code" body-class="m-0 px-0 py-1" :border-variant="addFormColor" style="border:2px solid">
                                    <add-assignment-form :type="selectedAssignmentType.label" :formData="data.item" :isCreate="false" v-on:submit="saveAssignment" v-on:cancel="closeAssignmentForm" />
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
    import AddAssignmentForm from "../ManageTypes/AddAssignmentForm.vue"
    import {locationInfoType} from '../../types/common'; 
    import {assignmentTypeInfoType}  from '../../types/ManageTypes/index'
    import * as _ from 'underscore';
    import sortAssignmentType from './utils/sortAssignmentType';
    
    @Component({
        components: {
            PageHeader,
            AddAssignmentForm
        },
        directives: {     
            sortAssignmentType
        }
    })
    export default class AssignmentTypes extends Vue {

        @commonState.State
        public location!: locationInfoType;

        @manageTypesState.State
        public sortingAssignmentInfo!: {prvIndex: number; newIndex: number};

        sectionHeader = 'Assignments';
        isAssignmentDataMounted = false;
        isEditOpen = false;
        latestEditData;

        assignmentError = false;
        assignmentErrorMsg = '';
        assignmentErrorMsgDesc = '';

        addNewAssignmentForm = false;
        addFormColor = 'dark';
        updateTable =0;

        assignmentList: assignmentTypeInfoType[] = [];
        selectedAssignmentType = {name:'CourtRoom', label:'Court Room'};
        
        assignmentTypeTabs = 
        [
            {name:'CourtRoom', label:'Court Room'},
            {name:'CourtRole', label:'Court Assignment'},
            {name:'JailRole', label:'Jail Assignment'},
            {name:'EscortRun', label:'Escort Assignment'},
            {name:'OtherAssignment', label:'Other Assignments'},
        ]

        fields =  
        [     
            {key:'sortOrder', label:'', sortable:false, tdClass: 'border-top' },       
            {key:'code', label:'', sortable:false, tdClass: 'border-top'  },
            {key:'scope',   label:'Scope',  sortable:false, tdClass: 'border-top', thClass:'',},  
            {key:'edit', label:'',  sortable:false, tdClass: 'border-top', thClass:'',},       
        ];

        @Watch('sortingAssignmentInfo', { immediate: true })
        sortingAssignmentInfoChange()
        {
            if (this.isAssignmentDataMounted) {
                for(const listIndex in this.assignmentList)
                    if(Number(listIndex) == this.sortingAssignmentInfo.prvIndex)
                        this.assignmentList[listIndex].sortOrder = this.sortingAssignmentInfo.newIndex*2 + Math.sign(this.sortingAssignmentInfo.newIndex-this.sortingAssignmentInfo.prvIndex)
                    else
                        this.assignmentList[listIndex].sortOrder *=2;
                this.refineSortOrders();
            }         
        } 
        
        @Watch('location.id', { immediate: true })
        locationChange()
        {
            if (this.isAssignmentDataMounted) {
                this.getAssignments()
            }            
        } 

        mounted () 
        {  
            console.log('assignment type')
            this.getAssignments()       
        }

        public getAssignments() {            
            this.isAssignmentDataMounted = false;
            const url = 'api/managetypes?codeType='+this.selectedAssignmentType.name;
            console.log(url)
            this.$http.get(url)
                .then(response => {
                    if(response.data){
                        console.log(response.data)
                        this.extractAssignments(response.data);                        
                    }
                    this.isAssignmentDataMounted = true;
                })        
        }

        public extractAssignments(assignmentsJson) {

            this.assignmentList = [];
            let sortIndex = assignmentsJson.length? assignmentsJson.length : 0;
            for(const assignmentJson of assignmentsJson)
            {
                if(assignmentJson.locationId == this.location.id)
                {
                    const assignment = {} as assignmentTypeInfoType;
                    assignment.id = assignmentJson.id;
                    assignment.code = assignmentJson.code;
                    assignment.locationId = assignmentJson.locationId;
                    assignment.sortOrder = assignmentJson.sortOrder?assignmentJson.sortOrder:(sortIndex++);
                    assignment.type = assignmentJson.type;
                    this.assignmentList.push(assignment)
                }
            }
            this.refineSortOrders();
        }

        public refineSortOrders(){
            this.assignmentList = _.sortBy(this.assignmentList,'sortOrder');
            for(const listIndex in this.assignmentList)
                this.assignmentList[listIndex].sortOrder = Number(listIndex);
            this.updateTable++;
        }
    
        public addNewAssignment(){
            if(this.isEditOpen){
                location.href = '#Assignment-'+this.latestEditData.item.code
                this.addFormColor = 'danger'
            }else
            {
                this.addNewAssignmentForm = true;
                this.$nextTick(()=>{location.href = '#addAssignmentForm';})
            }
        }

        public confirmDelete(assignment){
            console.log("deleting")
        }

        public editAssignment(assignment){
            console.log(assignment)
            if(this.addNewAssignmentForm){
                location.href = '#addAssignmentForm'
                this.addFormColor = 'danger'
            }else if(this.isEditOpen){
                location.href = '#Assignment-'+this.latestEditData.item.code
                this.addFormColor = 'danger'               
            }else if(!this.isEditOpen && !assignment.detailsShowing){
                assignment.toggleDetails();
                this.isEditOpen = true;
                this.latestEditData = assignment
                Vue.nextTick().then(()=>{
                    location.href = '#Assignment-'+this.latestEditData.item.code
                });
            }  
        }

        public saveAssignment(body, iscreate){
            this.assignmentError = false;
            console.log(body) 
            // body['sheriffId']= this.userToEdit.id;
            // const method = iscreate? 'post' :'put';            
            // const url = 'api/sheriff/training'  
            // const options = { method: method, url:url, data:body}
            
            // this.$http(options)
            //     .then(response => {
            //         if(iscreate) 
            //             this.addToAssignedTrainingList(response.data);
            //         else
            //             this.modifyAssignedTrainingList(response.data);
                    
            //         this.closeTrainingForm();
            //     }, err=>{
            //         const errMsg = err.response.data.error;
            //         this.trainingErrorMsg = errMsg.slice(0,60) + (errMsg.length>60?' ...':'');
            //         this.trainingErrorMsgDesc = errMsg;
            //         this.trainingError = true;
            //         location.href = '#TrainingError'
            //     });                
        }

        public closeAssignmentForm() {                     
            this.addNewAssignmentForm = false; 
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