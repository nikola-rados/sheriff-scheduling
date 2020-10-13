
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
        <h2 class="mx-1 mt-0"><b-badge v-if="duplicateBadge" variant="danger"> Duplicate Badge</b-badge></h2>

    </div>
</template>

<script lang="ts">
import { Component, Vue, Prop } from 'vue-property-decorator';
import {teamMemberInfoType} from '../../../types/MyTeam';
import {commonInfoType, locationInfoType} from '../../../types/common';
import * as _ from 'underscore';

import { namespace } from 'vuex-class';
const commonState = namespace("CommonInformation");
import store from '../../../store'

enum gender {'Male'=0, 'Female', 'Other'}

@Component
export default class IdentificationTab extends Vue {

    @commonState.State
    public token!: string;

    @commonState.State
    public commonInfo!: commonInfoType;

    @commonState.State
    public location!: locationInfoType;

    @Prop({required: true})
    originalUser!: teamMemberInfoType;

    @Prop({required: true})
    createMode!: boolean;

    @Prop({required: true})
    editMode!: boolean;

    @Prop({required: true})
    runMethod!: any;

    genderOptions = [{text:"Male", value: gender.Male}, {text:"Female", value: gender.Female}, {text:"Other", value: gender.Other}]
    genderValues = [0, 1, 2]
    firstNameState = true;
    lastNameState = true;
    emailState = true;
    selectedGenderState = true;
    badgeNumberState = true;
    selectedRankState = true;
    idirUserNameState = true;
    duplicateBadge = false;
    duplicateIdir = false;

    errorCode = 0;
    errorText = '';
    refreshPage =0;

    user = {} as teamMemberInfoType;

    mounted(){
        //console.log('role')
        this.ClearFormState();
        if(this.createMode) 
            this.user = {} as teamMemberInfoType;
        else
            this.user = _.clone(this.originalUser);

        this.refreshPage++;
        //console.log(this.user)
        this.runMethod.$on('closeProfileWindow', this.closeProfileWindow)
        this.runMethod.$on('saveMemberProfile', this.saveMemberProfile)                
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
            this.$emit('showWarning');
    }

    public changesMade(): boolean {
        return !_.isEqual(this.originalUser, this.user)
    }

    public isEmpty(obj){
        for(const prop in obj) 
            if(obj[prop] != null)
                return false;
        return true;
    }
         
    public saveMemberProfile() { 
        console.log('save')      
        const requiredErrorTab: number[] = [];

        if (this.createMode && !this.user.idirUserName) {
            this.idirUserNameState = false;
            requiredErrorTab.push(0);
        } else {
            this.idirUserNameState = true;
            this.duplicateIdir = false;
        }
        if (!this.user.firstName) {
            this.firstNameState = false;
            requiredErrorTab.push(0);
        } else {
            this.firstNameState = true;
        }
        if (!this.user.lastName) {
            this.lastNameState = false;
            requiredErrorTab.push(0);
        } else {
            this.lastNameState = true;
        }
        if (this.genderValues.toString().indexOf(this.user.gender) == -1) {
            this.selectedGenderState = false;
            requiredErrorTab.push(0);
        } else {
            this.selectedGenderState = true;
        }
        if (!this.user.badgeNumber) {
            this.badgeNumberState = false;
            requiredErrorTab.push(0);
        } else {
            this.badgeNumberState = true;
            this.duplicateBadge = false;
        }
        if (!this.user.email) {
            this.emailState = false;
            requiredErrorTab.push(0);
        } else {
            this.emailState = true;
        }
        if (!this.user.rank) {
            this.selectedRankState = false;
            requiredErrorTab.push(0);
        } else {
            this.selectedRankState = true;
        }
        
        if (requiredErrorTab.length == 0) {
            if (this.editMode) this.updateProfile();
            if (this.createMode) this.createProfile();              

        } else {                
            //this.tabIndex= requiredErrorTab[0]; __emit
        }             
    }

    public updateProfile(): void {
        const body = {
            homeLocationId: this.location.id,               
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
                    
                //if(this.errorText.includes('already has badge number'))
                // has IDIR name
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
        const body = {
            homeLocationId: this.location.id,               
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
        this.ClearFormState();
        this.user = {} as teamMemberInfoType;
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