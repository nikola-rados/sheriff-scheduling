<template>
    <div>    
        <b-card class="mb-5">
            <b-card style="height: 60px" border-variant="white" no-body>
                <b-alert
                    :show="warningDismissCountDown"
                    dismissible
                    fade
                    variant="warning" 
                    @dismiss-count-down="warningCountDownChanged"               
                    >
                    {{warningMsg}}
                </b-alert>
                <b-alert
                    :show="successDismissCountDown"
                    dismissible
                    fade
                    variant="success" 
                    @dismiss-count-down="successCountDownChanged"               
                    >
                    {{successMsg}}
                </b-alert>
                <b-alert
                    :show="errorDismissCountDown"
                    dismissible
                    fade
                    variant="danger" 
                    @dismiss-count-down="errorCountDownChanged"               
                    >
                    {{errorMsg}}
                </b-alert>
            </b-card>
            <b-tabs  nav-wrapper-class = "bg-light text-dark"
                        active-nav-item-class="text-white bg-primary"                     
                         >
                <b-tab 
                v-for="tabMapping in tabs"                 
                :key="tabMapping.key"                 
                :title="tabMapping.label"                 
                v-on:click="activetab = tabMapping; getInformation(tabMapping.call)" 
                v-bind:class="[ activetab.key === tabMapping.key ? 'active' : '' ]"
                >
                    <b-card border-variant="white">
                        <h4>
                            {{selectedLocation.name}} {{activetab.label}}
                        </h4>
                        <hr  style="border-top: 1px dashed gray"/>      
                    </b-card>
                    <b-table
                        :items="callInformation"
                        :fields="fields"
                        :sort-by.sync="sortBy"
                        :sort-desc.sync="sortDesc"
                        :no-sort-reset="true"
                        sort-icon-left
                        striped
                        borderless
                        small
                        responsive="sm"
                        >  
                        
                        <template v-slot:head(locationId)="data">                         
                            <b v-if="activetab.key !=='CourtRooms'" >{{data.label}}</b>
                            <b v-else></b>
                        </template>

                        <template v-slot:cell(locationId)="data">                         
                            <span v-if="activetab.key !='CourtRooms'" >
                                <b-dropdown variant="bg-white" no-caret :text="data.item.locationId?'Custom Role':'Default Role'">
                                    <b-dropdown-item @click="setScope(data,'Custom Role')">Custom Role</b-dropdown-item>
                                    <b-dropdown-item @click="setScope(data,'Default Role')">Default Role</b-dropdown-item>
                                </b-dropdown>
                            </span>
                            
                            <b v-else></b>
                        </template>

                        <template v-slot:head(name)>
                            <b v-if="activetab.key =='CourtRooms'">Courtroom </b>
                            <b v-else>Type</b>
                        </template> 

                        <template v-slot:cell(name)="data" >
                            <span v-if="activetab.key =='CourtRooms'">{{data.item.name}}</span>
                            <span v-else-if="activetab.key =='EscortRuns'">{{data.item.title}}</span>
                            <span v-else>{{data.item.description}}</span> 
                        </template>

                        <template v-slot:cell(expire)="data" >
                            <b-button size="sm" variant="warning" @click="ExpireCell(data)">
                                <b-icon icon="clock">
                                </b-icon>
                            </b-button> 
                        </template>

                        <template v-slot:cell(delete)="data" >
                            <b-button size="sm" variant="danger" @click="DeleteCell(data)">
                                <b-icon icon="trash">
                                </b-icon>
                            </b-button> 
                        </template>
                        <!-- <template v-for="(field,index) in fields" v-slot:[`head(${field.key})`]="data">
                            <b v-bind:key="index" :class="field.headerStyle" > {{ data.label }}</b>
                        </template> 
        
                        <template v-slot:cell(Status)="data" >
                                <b-badge  
                                    v-for="(field,index) in data.value"
                                    :key="index" 
                                    class="mr-1 mt-2"
                                    :style="data.field.cellStyle"
                                    v-b-tooltip.hover 
                                    :title='field.key' > 
                                    {{ field.abbr }} 
                                </b-badge>
                        </template> -->
                    </b-table>
                    <b-card border-variant="white" no-body>
                        <b-input-group class="mb-3">
                            <b-form-input                                
                                v-model="inputSortOrder"
                                type="number"                                
                                placeholder="Sort Order"
                                autocomplete="off" 
                                class="mr-2"                               
                            ></b-form-input>                            
                            <b-form-input                                
                                v-model="inputName"
                                type="text"                                
                                :placeholder="activetab.key =='CourtRooms'?'Courtroom':'Type'"
                                autocomplete="off" 
                                class="mr-2"                               
                            ></b-form-input>
                            <b-form-input                                
                                v-model="inputCode"
                                type="text"                                
                                placeholder="Code"
                                autocomplete="off" 
                                class="mr-2"
                                :state="inputCodeStatus?null:false"                               
                            ></b-form-input>
                            <b-form-select 
                                v-if="activetab.key !='CourtRooms'"                              
                                v-model="inputScope"
                                type="text"                                
                                placeholder="Scope"
                                autocomplete="off"
                                class="mr-2" 
                                :options="['Custom Role','Default Role']"                               
                            ></b-form-select> 
                            <b-button
                            @click="AddCell()">
                                <b-icon icon="plus">
                                </b-icon>
                                    Add {{activetab.name}}
                            </b-button>                           
                        </b-input-group> 
                    </b-card>
                </b-tab>
            </b-tabs>
        </b-card>

        <b-modal v-model="showConfirmation" id="bv-modal-confirmation" hide-footer>
            <template v-slot:modal-title>
                <h2 class="mb-0"> Are you sure you want to delete: </h2>
            </template>
            <b-row>
                <b-col>
                    <h3 class="text-danger"> {{activetab.name}} Code: </h3>
                </b-col>
                <b-col>
                    <h2 class="text-danger"> {{deletecode}} </h2>
                </b-col>
            </b-row>           
            <!-- <b-table                
                :items="deleteCellInfo.item"
                fields="{'sortOrder','name','code'}"                
                borderless
                small                
                > 
                
            </b-table>            -->
            
            <b-button class="mt-3 mx-5" variant="success" @click="$bvModal.hide('bv-modal-confirmation')">Cancel</b-button>
            <b-button class="mt-3 mx-5" variant="danger" @click="DeleteInformation();$bvModal.hide('bv-modal-confirmation')">Delete</b-button>
            
        </b-modal> 
    </div>
</template>

<script lang="ts">
import { Component,  Vue } from 'vue-property-decorator';
import axios from 'axios'
import * as _ from 'underscore';

import { namespace } from 'vuex-class';
import "../../store/modules/Common";
const commonState = namespace("Common");

type IdType = string;
export interface Courtroom {
    id?: IdType;
    locationId?: IdType;
    code: IdType;
    name?: string;
    description?: string;
    effectiveDate?: string;
    expiryDate?: string;
    sortOrder?: number;
    isExpired?: boolean; // Only used on the client-side
}
export interface AddInformation {
    id?: IdType;
    locationId?: IdType;
    code: IdType;
    name?: string;
    title?: string;
    description?: string;
    effectiveDate?: string;
    expiryDate?: string;
    sortOrder?: number;
    isExpired?: boolean; // Only used on the client-side
    createdBy: string;
    updatedBy: string;
    createdDtm: string;
    updatedDtm: string;
}
@Component
export default class AssignmentTypes extends Vue {

    @commonState.State
    public selectedLocation!: any;

    mounted () 
    {  
        this.activetab = this.tabs[0];
        this.getInformation(this.tabs[0].call) 
    }

    tabs = [
        {key:'CourtRooms',     label:'Courtrooms',       call:'courtrooms', name:'Courtroom' },
        {key:'CourtRoles',     label:'Court Roles',      call:'codes/courtroles', name:'Court Role' },
        {key:'JailRoles',      label:'Jail Roles',       call:'codes/jailroles', name:'Jail Role' },
        {key:'EscortRuns',     label:'Escort Runs',      call:'escort-runs', name:'Escort Run' },
        {key:'OtherAssignment',label:'Other Assignments',call:'codes/otherassign', name:'Other Assignment' },

    ];
    activetab = {key:'',     label:'',       call:'' };
   // loginSuccess = false;
   // hasLocations = false;
    inputSortOrder: number|null = null;
    inputName ='';
    inputCode ='';
    inputScope ='Default Role';
    inputCodeStatus = true;

    warningDismissCountDown = 0;
    warningMsg = '';
    successDismissCountDown = 0;
    successMsg = '';
    errorDismissCountDown = 0;
    errorMsg = '';
    showConfirmation = false;

    callInformation: Courtroom[] = [];
    sortBy = 'sortOrder';
    sortDesc = false;
    active = true
    fields =  
    [
        {key:'sortOrder', sortable:true,  tdClass: 'border-top', },
        {key:'name',      sortable:false, tdClass: 'border-top',  },
        {key:'code',      sortable:false, tdClass: 'border-top', },
        {key:'locationId', label:'Scope', sortable:false, tdClass: 'border-top', },
        {key:'expire',    sortable:false, tdClass: 'border-top', thClass:'text-white',},
        {key:'Delete',    sortable:false, tdClass: 'border-top', thClass:'text-white',},        
    ];

    public getInformation(callName: string) 
    {        
        console.log(callName)
        console.log(this.selectedLocation)
        this.inputSortOrder = null;
        this.inputName = '';
        this.inputCode = '';
        this.inputScope = 'Default Role';
        this.inputCodeStatus = true;
        this.callInformation = [];

        const url = '/api/v1/'+callName+'?locationId='+ this.selectedLocation.id   
        axios.get(url, {
            headers: 
            {
                'Authorization' :localStorage.getItem('token')||'',
                'Content-Type': 'application/javascript'                    
            }
        })        
        .then(response => {
            console.log(response.data)
            if(response.data.length>0)
            {                
                this.callInformation = response.data;
                console.log(this.callInformation)
            }
        })
        .catch(err => {
            console.log(err.response);
        });        
    }

    public DeleteInformation() 
    {   
        this.ResetAlerts();
        const id = this.deleteCellInfo.item.id;
        const code =this.deleteCellInfo.item.code;

        const url = '/api/v1/'+this.activetab.call+'/'+id   
        axios.delete(url, {
            headers: 
            {
                'Authorization' :localStorage.getItem('token')||'',
                'Content-Type': 'application/javascript'                    
            }
        })        
        .then(response => {
            console.log(response)
            if(response.status == 204)
            {                 
                this.successMsg = ' Successfully deleted Code: \''+code+'\'. ';
                this.successDismissCountDown = 1;
                this.getInformation(this.activetab.call);
            }
            else
            {
                this.errorMsg = ' Error occurred while deleting Code: \''+code+'\'. ';
                this.errorDismissCountDown = 1;
            }
        })
        .catch(err => {
            console.log(err.response);
            this.errorMsg = ' Error occurred while deleting Code: \''+code+'\'. ';
            this.errorDismissCountDown = 1;
        });        
    }
    
    public AddInformation() 
    {   
        this.ResetAlerts();
        const date = new Date();    
        
        const postingData: AddInformation= 
        {            
            code: this.inputCode,
            createdBy: "DEV - FRONTEND",
            updatedBy: "DEV - FRONTEND",
            createdDtm: date.toISOString(),
            updatedDtm: date.toISOString(),            
        }

        if(this.inputScope=='Custom Role' || this.activetab.key =='CourtRooms')
            postingData['locationId'] = this.selectedLocation.id;  

        if(this.inputSortOrder != null) postingData['sortOrder'] = this.inputSortOrder;
       
       
        if(this.inputName)
        { 
            if(this.activetab.key == 'CourtRooms' ) postingData['name'] = this.inputName;
            else if(this.activetab.key == 'EscortRuns' ) postingData['title'] = this.inputName;
            else postingData['description'] = this.inputName;
        }
        console.log(postingData)

        const url = '/api/v1/'+this.activetab.call   
        axios.post(url, postingData, {
            headers: 
            {
                'Authorization' :localStorage.getItem('token')||'',
                'Content-Type': 'application/json'                    
            }
        })        
        .then(response => {
            console.log(response)
            if(response.status == 201)
            {                 
                this.successMsg = ' Successfully added Code: \''+this.inputCode+'\'. ';
                this.successDismissCountDown = 1;
                this.getInformation(this.activetab.call);
            }
            else
            {
                this.errorMsg = ' Error occurred while adding Code: \''+this.inputCode+'\'. ';
                this.errorDismissCountDown = 1;
            }
        })
        .catch(err => {
            console.log(err.response);
            this.errorMsg = ' Error occurred while adding Code: \''+this.inputCode+'\'. ';
            this.errorDismissCountDown = 1;
        });        
    }

deleteCellInfo: any;
deletecode=''
    // deleteCellInfo ={
    //     items:{
    //         'code':'',
    //         'sortOrder':'',
    //         'name':''
    //     }
    // };

    public DeleteCell(data: any)
    {
        console.log(data)
        console.log(data.item.id)
        this.deletecode = data.item.code
        this.deleteCellInfo = data; 
        this.showConfirmation = true;         
    }

    public ExpireCell(data: any)
    {
        console.log(data)        
    }

    public AddCell()
    {
        this.ResetAlerts();        

        if(this.inputCode)
        {
            this.inputCodeStatus = true;
            if(this.codeExist(this.inputCode)>=0)
            {
                this.inputCodeStatus = false;                
                this.warningMsg = ' The entered Code : \' '+this.inputCode+' \' already exists. ';
                this.warningDismissCountDown = 1;
            }
            else
            {
                this.AddInformation();
            }
        }
        else
        {
            this.inputCodeStatus = false;
        }
    }

    public codeExist(code: string)
    {
        return this.callInformation.findIndex(element =>element.code==code) ;
    }

    public ResetAlerts()
    {
        this.warningDismissCountDown = 0;
        this.successDismissCountDown = 0;
        this.errorDismissCountDown = 0;
    }

    public warningCountDownChanged(dismissCountDown: any)
    {
        this.warningDismissCountDown = dismissCountDown
    }
    
    public successCountDownChanged(dismissCountDown: any)
    {
        this.successDismissCountDown = dismissCountDown
    }

    public errorCountDownChanged(dismissCountDown: any)
    {
        this.errorDismissCountDown = dismissCountDown
    }

    public setScope(data: any,scope: string)
    {
        console.log(this.callInformation)
        console.log(data)
        console.log(scope)
        const index = this.callInformation.findIndex(element =>element.id==data.item.id)
        if(scope=='Custom Role') this.callInformation[index].locationId = this.selectedLocation.id ;
        else this.callInformation[index].locationId ='';
        console.log(this.callInformation[index])
    }

    
}
</script>