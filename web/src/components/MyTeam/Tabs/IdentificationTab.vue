<template>
    <div :key="refreshPage">
        <b-form-group v-if="createMode"><label>IDIR User Name<span class="text-danger">*</span></label>
            <b-form-input v-model="user.idirUserName" placeholder="Enter IDIR User Name" :state = "idirUserNameState?null:false"></b-form-input>
        </b-form-group>
        <h2 class="mx-1 mt-0"><b-badge v-if="duplicateIdir" variant="danger"> Duplicate IDIR</b-badge></h2>

        <b-row class="mx-1"> 
            <b-form-group class="mr-1" style="width: 20rem"><label>First Name<span class="text-danger">*</span></label>
                <b-form-input v-model="user.firstName" placeholder="Enter First Name" :state = "firstNameState?null:false"></b-form-input>
            </b-form-group>                                    
            <b-form-group class="ml-1" style="width: 20rem"><label>Last Name<span class="text-danger">*</span></label>
                <b-form-input v-model="user.lastName" placeholder="Enter Last Name" :state = "lastNameState?null:false"></b-form-input>
            </b-form-group>
        </b-row>

        <b-row class="mx-1">   
            <b-form-group class="mr-1" style="width: 10rem"><label>Gender<span class="text-danger">*</span></label>
                <b-form-select v-model="user.gender" :options="genderOptions" :state = "selectedGenderState?null:false"></b-form-select>
            </b-form-group>
            <b-form-group class="ml-1" style="width: 30rem"><label>Email<span class="text-danger">*</span></label>
                <b-form-input v-model="user.email" placeholder="Enter Email" :state = "emailState?null:false" type="email"></b-form-input>
            </b-form-group>
        </b-row>

        <b-row class="mx-1">
            <b-form-group class="mr-1" style="width: 20rem"><label>Badge Number<span class="text-danger">*</span></label>
                <b-form-input v-model="user.badgeNumber" placeholder="Enter Badge Number" :state = "badgeNumberState?null:false"></b-form-input>
            </b-form-group>                                            
            <b-form-group class="ml-1" style="width: 15rem"><label>Rank<span class="text-danger">*</span></label>
                <b-form-select v-model="user.rank" placeholder="Select Rank" :options="commonInfo.sheriffRankList" :state = "selectedRankState?null:false"></b-form-select>
            </b-form-group>
        </b-row>
        <h2 v-if="duplicateBadge" class="mx-1 mt-0"><b-badge variant="danger"> Duplicate Badge</b-badge></h2>

        <b-row  v-if="createMode" class="mx-1">
            <b-form-group class="mr-1" style="width: 25rem"><label>Home Location<span class="text-danger">*</span></label> 
                <b-form-select                                                                                                           
                    v-model="user.homeLocationId"
                    :state = "homeLocationState?null:false"
                    >                            
                        <b-form-select-option
                            v-for="homelocation in locationList" 
                            :key="homelocation.id"
                            :value="homelocation.id">
                                {{homelocation.name}}
                        </b-form-select-option>    
                </b-form-select>
            </b-form-group>
        </b-row>

        <b-modal v-model="showCancelWarning" id="bv-modal-team-cancel-warning" header-class="bg-warning text-light">            
            <template v-slot:modal-title>                
                 <h2 v-if="editMode" class="mb-0 text-light"> Unsaved Profile Changes </h2>
                 <h2 v-else-if="createMode" class="mb-0 text-light"> Unsaved New Profile </h2>                
            </template>
            <p>Are you sure you want to {{cancelMessage}} without saving your changes?</p>
            <template v-slot:modal-footer>
                <b-button variant="secondary" @click="closeWarningWindow(false);"                   
                >No</b-button>
                <b-button variant="success" @click="closeWarningWindow(true)"
                >Yes</b-button>
            </template>            
            <template v-slot:modal-header-close>                 
                 <b-button variant="outline-warning" class="text-light closeButton" @click="$bvModal.hide('bv-modal-team-cancel-warning')"
                 >&times;</b-button>
            </template>
        </b-modal>
    </div>
</template>

<script lang="ts">
import { Component, Vue, Prop } from 'vue-property-decorator';
import {teamMemberInfoType} from '../../../types/MyTeam';
import {commonInfoType, locationInfoType} from '../../../types/common';
import * as _ from 'underscore';

import { namespace } from 'vuex-class';
import "@store/modules/CommonInformation";
const commonState = namespace("CommonInformation"); 
import "@store/modules/TeamMemberInformation";
const TeamMemberState = namespace("TeamMemberInformation");

enum gender {'Male'=0, 'Female', 'Other'}

@Component
export default class IdentificationTab extends Vue {

    @commonState.State
    public token!: string;

    @commonState.State
    public commonInfo!: commonInfoType;

    @commonState.State
    public location!: locationInfoType;

    @commonState.State
    public locationList!: locationInfoType[];

    @Prop({required: true})
    createMode!: boolean;

    @Prop({required: true})
    editMode!: boolean;

    @Prop({required: true})
    runMethod!: any;

    @TeamMemberState.State
    public userToEdit!: teamMemberInfoType;

    @TeamMemberState.Action
    public UpdateUserToEdit!: (userToEdit: teamMemberInfoType) => void

    genderOptions = [{text:"Male", value: gender.Male}, {text:"Female", value: gender.Female}, {text:"Other", value: gender.Other}]
    genderValues = [0, 1, 2]
    firstNameState = true;
    lastNameState = true;
    emailState = true;
    selectedGenderState = true;
    badgeNumberState = true;
    selectedRankState = true;
    idirUserNameState = true;
    homeLocationState = true;
    duplicateBadge = false;
    duplicateIdir = false;

    showCancelWarning = false;
    cancelMessage ='cancel';

    selectedHomeLocation = {} as locationInfoType | undefined;

    errorCode = 0;
    errorText = '';
    refreshPage =0;

    user = {} as teamMemberInfoType;

    mounted(){
        console.log('identification')
        this.refreshTabInformation();
        //console.log(this.user)
        this.runMethod.$on('switchTab', this.switchTab)
        this.runMethod.$on('closeProfileWindow', this.closeProfileWindow)
        this.runMethod.$on('saveMemberProfile', this.saveMemberProfile)
    }

    public refreshTabInformation()
    {
        console.log('refresh identification tab')        
        this.ClearFormState();
        console.log(this.userToEdit)
        this.user = _.clone(this.userToEdit);
        this.refreshPage++;
    }

    public switchTab(){
        
        if(this.editMode && !this.changesMade()){
            this.$emit('changeTab', true);
            console.log('allow change tab')
        }    
        else
        {            
            this.cancelMessage = 'change tabs';
            this.showCancelWarning = true;
        }
    }

    public closeProfileWindow(){
        
        if(this.createMode && this.isEmpty(this.user)){
            this.$emit('closeMemberDetails');
            this.resetProfileWindowState();
        } 
        else if(this.editMode && !this.changesMade()){
            this.$emit('closeMemberDetails');
            this.resetProfileWindowState();
        }    
        else
        {
            this.cancelMessage = 'cancel';
            this.showCancelWarning = true;
        }
    }

    public closeWarningWindow(changingTab: boolean) {
        this.showCancelWarning = false;
        if(this.cancelMessage == 'cancel' && changingTab)
        { 
            this.$emit('closeMemberDetails');
            this.resetProfileWindowState();            
        }
        else{
            this.$emit('changeTab', changingTab);
        }
    }

    public changesMade(): boolean {
        if (this.editMode) {
            this.user.homeLocationId = this.userToEdit.homeLocationId;
            this.user.homeLocationNm = this.userToEdit.homeLocationNm;
            this.user.homeLocation = this.userToEdit.homeLocation;
        }
        return !_.isEqual(this.userToEdit, this.user)
    }

    public isEmpty(obj){
        for(const prop in obj) 
            if(obj[prop] != null)
                return false;
        return true;
    }
         
    public saveMemberProfile() { 
        // console.log('save') 
        // console.log(this.user) 
        // console.log(this.userToEdit)    
        let requiredError = false;

        if (this.createMode && !this.user.idirUserName) {
            this.idirUserNameState = false;
            requiredError = true;
        } else {
            this.idirUserNameState = true;
            this.duplicateIdir = false;
        }
        if (!this.user.firstName) {
            this.firstNameState = false;
            requiredError = true;
        } else {
            this.firstNameState = true;
        }
        if (!this.user.lastName) {
            this.lastNameState = false;
            requiredError = true;
        } else {
            this.lastNameState = true;
        }
        if (this.genderValues.toString().indexOf(this.user.gender) == -1) {
            this.selectedGenderState = false;
            requiredError = true;
        } else {
            this.selectedGenderState = true;
        }
        if (!this.user.badgeNumber) {
            this.badgeNumberState = false;
            requiredError = true;
        } else {
            this.badgeNumberState = true;
            this.duplicateBadge = false;
        }
        if (!this.user.email) {
            this.emailState = false;
            requiredError = true;
        } else {
            this.emailState = true;
        }
        if (!this.user.rank) {
            this.selectedRankState = false;
            requiredError = true;
        } else {
            this.selectedRankState = true;
        }        
        if (!this.user.homeLocationId) {           
            requiredError = true;
            this.homeLocationState = false;
        } else{
            this.homeLocationState = true;
        }
        
        if (!requiredError) {
            if (this.editMode) this.updateProfile();
            if (this.createMode) this.createProfile();              

        } else { 
            this.UpdateUserToEdit(this.user);
            console.log('Error required')
        }             
    }

    public updateProfile(): void {
        console.log('update profile')
        const body = {               
            gender: this.user.gender,
            badgeNumber: this.user.badgeNumber,
            rank: this.user.rank,
            idirName: this.user.idirUserName,
            firstName:this.user.firstName,
            lastName: this.user.lastName,
            email: this.user.email,
            id: this.user.id
        }

        const url = 'api/sheriff';
        const options = {headers:{'Authorization' :'Bearer '+this.token}} 
        this.$http.put(url, body, options)
            .then(response => {
                if(response.data){
                    this.resetProfileWindowState();
                    this.$emit('closeMemberDetails');
                    this.$emit('profileUpdated')         
                }                    
            }, err => {
                console.log(err)
                this.errorText = err.response.data.error
                this.errorCode = err.response.status
                
                if(err.response.status == 400)
                {
                    if (this.errorText.includes('already has badge number')){
                        this.badgeNumberState = false;
                        this.duplicateBadge = true;
                    } else if (this.errorText.includes('has IDIR name')) {
                        this.idirUserNameState = false;
                        this.duplicateIdir = true;
                    }                        
                }
            });
    }

    public createProfile() {

        console.log('create profile')
        const body = {
            homeLocationId: this.user.homeLocationId,               
            gender: this.user.gender,
            badgeNumber: this.user.badgeNumber,
            rank: this.user.rank,
            idirName: this.user.idirUserName,
            firstName:this.user.firstName,
            lastName: this.user.lastName,
            email: this.user.email
        }
        // console.log(body)
        const url = 'api/sheriff';
        const options = {headers:{'Authorization' :'Bearer '+this.token}}           
        
        this.$http.post(url, body, options )
            .then(response => {
                if(response.data){
                    this.resetProfileWindowState();
                    this.$emit('closeMemberDetails');
                    this.$emit('profileUpdated')    
                }
            }, err => {
                this.errorText = err.response.data.error
                this.errorCode = err.response.status                     
                
                if(err.response.status == 400){
                    if (this.errorText.includes('already has badge number')){
                        this.badgeNumberState = false;
                        this.duplicateBadge = true;
                    } else if (this.errorText.includes('has IDIR name')) {
                        this.idirUserNameState = false;
                        this.duplicateIdir = true;
                    }                        
                }

            })   
    }

    public resetProfileWindowState() {
        this.user = {} as teamMemberInfoType;
        this.ClearFormState();
    }

    public ClearFormState(){        
        this.firstNameState = true;
        this.lastNameState = true;
        this.selectedGenderState = true;
        this.badgeNumberState = true;
        this.emailState = true;
        this.selectedRankState = true;
        this.idirUserNameState = true;
        this.duplicateBadge = false;
        this.duplicateIdir = false;
    }
    
}
</script>