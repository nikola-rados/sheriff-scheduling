<template>
    <b-card bg-variant="white">
        <b-row>
            <b-col cols="11">
                <page-header :pageHeaderText="sectionHeader"></page-header>
            </b-col>
            <b-col style="padding: 0;">
                <b-button v-if="userIsAdmin" style="max-height: 40px;" size="sm" variant="warning" @click="AddMember()"><b-icon-plus/>Add a User</b-button>
            </b-col>
        </b-row>
        
        <b-card bg-variant="light" v-if= "!isMyTeamDataMounted" >
            <b-overlay :show= "true"> 
                <b-card  style="min-height: 100px;"/>                   
                <template v-slot:overlay>               
                <div> 
                    <loading-spinner/> 
                    <p id="loading-label">Loading ...</p>
                </div>                
                </template> 
            </b-overlay> 
        </b-card>
      

        <div v-else class="container mb-5" id="app">
            <div class="row">
                <div v-for="teamMember in myTeamData" :key="teamMember.badgeNumber" class="col-3  my-1">
                    <div  class="card h-100">
                        <div @click="openMemberDetails(teamMember.id)" class="card-body">
                            <user-summary-template :userBadgeNumber="teamMember.badgeNumber" :userName="teamMember.fullName" :userRole="teamMember.rank" :userImage="teamMember.image" :editMode="false" />
                        </div>
                        <div class="card-footer bg-light border-light" >                                                
                            <expire-sheriff-profile :userID="teamMember.id" :userIsEnable="teamMember.isEnabled"/>                        
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <b-modal size="xl" v-model="showMemberDetails" id="bv-modal-team-member-details" header-class="bg-primary text-light">            
            <template v-slot:modal-title>                
                 <h2 v-if="editMode" class="mb-0 text-light"> Updating User Profile </h2>
                 <h2 v-else-if="createMode" class="mb-0 text-light"> Creating User Profile </h2>                
            </template>
            <b-card v-if="isUserDataMounted" no-body>
                <b-row>
                    <b-col cols="3">
                        <user-summary-template :userBadgeNumber="user.badgeNumber" :userName="user.firstName + ' ' + user.lastName" :userRole="user.rank" :userImage="user.image" :editMode="editMode"/>
                    </b-col>
                    <b-col cols="9">
                        <b-card no-body>
                            <b-tabs card v-model="tabIndex">
                                <b-tab title="Identification" >
                                    
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
                                    
                                </b-tab>

                                <b-tab title="Locations">                                    
                                </b-tab>
                                <b-tab title="Leaves">                                    
                                </b-tab>
                                <b-tab title="Training"> 
                                </b-tab>
                                <b-tab v-if="userIsAdmin & editMode" title="Roles" class="p-0">
                                    
                                    <b-card  style="height:400px;overflow: auto;" >                                        
                                        <h2 class="mx-1 mt-0"><b-badge v-if="roleAssignError" variant="danger"> Role Assignment Unsuccessful <b-icon class="ml-3" icon = x-square-fill @click="roleAssignError = false" /></b-badge></h2>
                                               
                                        <b-form-group >
                                            <b-form-checkbox-group v-model="selectedRoles">
                                                <b-row class="mb-2 text-primary" style="font-weight:bold">
                                                    <b-col cols="5"> Role </b-col>
                                                    <b-col class="mx-2" > Effective Date </b-col>
                                                    <b-col class="mx-2" > Expiry Date </b-col>
                                                </b-row>
                                                <b-row v-for="(rol,rolInx) in roles" :key="rolInx" class="mb-1">
                                                    <b-col cols="5">   
                                                        <b-form-checkbox 
                                                            class="mt-1"
                                                            v-b-tooltip.hover.left
                                                            :title="rol.desc"
                                                            @change="roleChanged(rolInx)" 
                                                            :value="rol.value">
                                                                {{rol.text}}
                                                        </b-form-checkbox>
                                                    </b-col>
                                                    <b-col >
                                                        <b-form-datepicker
                                                        class="p-0 m-0"
                                                        v-model="roles[rolInx].effDate"
                                                        placeholder="Eff. Date"
                                                        locale="en-US" 
                                                        :date-format-options="{ year: 'numeric', month: 'short', day: '2-digit' }"
                                                        :state = "rol.effState?null:false"
                                                        ></b-form-datepicker>
                                                    </b-col>
                                                    <b-col >
                                                        <b-form-datepicker
                                                        v-model="roles[rolInx].expDate"
                                                        placeholder="Exp. Date"
                                                        locale="en-US"
                                                        :date-format-options="{ year: 'numeric', month: 'short', day: '2-digit' }"
                                                        :state = "rol.expState?null:false"
                                                        ></b-form-datepicker>
                                                    </b-col>
                                                </b-row> 
                                          
                                            </b-form-checkbox-group>
                                        </b-form-group>                                       
                                    </b-card>
                                </b-tab>
                                

                            </b-tabs>
                        </b-card>
                    </b-col>
                </b-row>        
            </b-card>
            <template v-slot:modal-footer>
                <b-button
                    variant="secondary" 
                    @click="closeProfileWindow()"                  
                ><b-icon-x font-scale="1.5" style="padding:0; vertical-align: middle; margin-right: 0.25rem;"></b-icon-x>{{getCancelLabel}}</b-button>
                <b-button     
                    v-if="tabIndex<1"
                    variant="success" 
                    @click="saveMemberProfile()"
                ><b-icon-check2 style="padding:0; vertical-align: middle; margin-right: 0.25rem;"></b-icon-check2>Save</b-button>
            </template>            
            <template v-slot:modal-header-close>                 
                 <b-button
                  variant="outline-primary"
                  class="text-light closeButton"
                  @click="$bvModal.hide('bv-modal-team-member-details')"                  
                  >
                  &times;</b-button>
            </template>           
        </b-modal>

        <b-modal v-model="showCancelWarning" id="bv-modal-team-cancel-warning" header-class="bg-warning text-light">            
            <template v-slot:modal-title>                
                 <h2 v-if="editMode" class="mb-0 text-light"> Unsaved Profile Changes </h2>
                 <h2 v-else-if="createMode" class="mb-0 text-light"> Unsaved New Profile </h2>                
            </template>
            <p>Are you sure you want to cancel without saving your changes?</p>
            <template v-slot:modal-footer>
                <b-button variant="secondary" @click="$bvModal.hide('bv-modal-team-cancel-warning')"                   
                >No</b-button>
                <b-button variant="success" @click="closeWarningWindow()"
                >Yes</b-button>
            </template>            
            <template v-slot:modal-header-close>                 
                 <b-button variant="outline-warning" class="text-light closeButton" @click="$bvModal.hide('bv-modal-team-cancel-warning')"
                 >&times;</b-button>
            </template>
        </b-modal>
    </b-card>
</template>

<script lang="ts">
    import { Component, Vue, Watch } from 'vue-property-decorator';
    import { namespace } from 'vuex-class';
    import * as _ from 'underscore';
    import PageHeader from "@components/common/PageHeader.vue";
    import UserSummaryTemplate from "./UserSummaryTemplate.vue";
    import "@store/modules/CommonInformation";  
    import {commonInfoType, locationInfoType, userInfoType} from '../../types/common';
    import {teamMemberInfoType, roleOptionInfoType} from '../../types/MyTeam';
    import {teamMemberJsonType} from '../../types/MyTeam/jsonTypes';  
    const commonState = namespace("CommonInformation");
    import store from '../../store'
    import ExpireSheriffProfile from './utils/ExpireSheriffProfile.vue'

    enum gender {'Male'=0, 'Female', 'Other'}

    @Component({
        components: {
            PageHeader,
            UserSummaryTemplate,
            ExpireSheriffProfile
        }        
    })    
    export default class MyTeamMembers extends Vue {

        @commonState.State
        public commonInfo!: commonInfoType;

        @commonState.Action
        public UpdateCommonInfo!: (newCommonInfo: commonInfoType) => void

         @commonState.State
        public token!: string;

        @commonState.Action
        public UpdateToken!: (newToken: string) => void

        @commonState.State
        public location!: locationInfoType;

        @commonState.Action
        public UpdateLocation!: (newLocation: locationInfoType) => void

        @commonState.State
        public userDetails!: userInfoType;
        
        showMemberDetails = false;
        showCancelWarning = false;
        user = {} as teamMemberInfoType;
        originalUser = {} as teamMemberInfoType;
        userJson;
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
        userIsAdmin = false;

        tabIndex = 0;
        errorCode = 0;
        errorText = '';
        isUserDataMounted = false;
        editMode = false;
        createMode = false;
        sectionHeader = '';
        selectedRoles: string[] = []
        roles: roleOptionInfoType[] = []
        roleAssignError = false;


        isMyTeamDataMounted = false;
        myTeamData: teamMemberInfoType[] =[];
        
        @Watch('location.id', { immediate: true })
        locationChange()
        {
            this.getSheriffs()
            this.sectionHeader = "My Team - " + this.location.name;
        }  

        mounted() {
            this.userIsAdmin = (this.userDetails.roles.indexOf("Administrator") > -1) || (this.userDetails.roles.indexOf("System Administrator") > -1);
            this.getSheriffs();
            this.sectionHeader = "My Team - " + this.location.name;
        }

        public getSheriffs()
        {
            this.isMyTeamDataMounted = false;
            const url = 'api/sheriff?locationId=' + this.location.id
            const options = {headers:{'Authorization' :'Bearer '+this.token}}
            this.$http.get(url, options)
                .then(response => {
                    if(response.data){
                        // console.log(response.data)
                        this.extractMyTeam(response.data);                        
                    }
                    this.isMyTeamDataMounted = true;
                })
        }

        public extractMyTeam(data: any)
        {
            this.myTeamData = [];            
            for(const myteaminfo of data)
            {
                const myteam: teamMemberInfoType = {id:'',idirUserName:'', rank:'', firstName:'', lastName:'', email:'', badgeNumber:'', gender:'' }
                myteam.fullName = this.getFullNameOfPerson(myteaminfo.firstName ,myteaminfo.lastName);
                myteam.rank = myteaminfo.rank;
                myteam.badgeNumber = myteaminfo.badgeNumber;
                myteam.id = myteaminfo.id;
                myteam.isEnabled = myteaminfo.isEnabled;
                this.myTeamData.push(myteam);
            }
        }

        public getFullNameOfPerson(first: string, last: string)
        {            
            return Vue.filter('capitilize')(first) + ' ' + Vue.filter('capitilize')(last);
        }

        public openMemberDetails(userId)
        {
            this.createMode = false;
            this.editMode = true;
            this.badgeNumberState = true;
            this.idirUserNameState = true;
            this.duplicateBadge = false;
            this.duplicateIdir = false;
            this.GetRoles(userId);      
            
        }

        public closeProfileWindow() 
        {         
            if(this.createMode && this.isEmpty(this.user))
            {
                this.showMemberDetails = false;
                this.resetProfileWindowState();
            } 
            else if(this.editMode && !this.changesMade())
            {
                this.showMemberDetails = false;
                this.resetProfileWindowState();
            }    
            else
                this.showCancelWarning = true;
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

        public closeWarningWindow() {   
            this.resetProfileWindowState();         
            this.showCancelWarning = false;
            this.showMemberDetails = false;
        }

        public resetProfileWindowState() {
            this.createMode = false;
            this.editMode = false;
            this.firstNameState = true;
            this.lastNameState = true;
            this.selectedGenderState = true;
            this.badgeNumberState = true;
            this.emailState = true;
            this.selectedRankState = true;
            this.idirUserNameState = true;
            this.duplicateBadge = false;
            this.duplicateIdir = false;
            this.user = {} as teamMemberInfoType;
        }

        public AddMember()
        {  
            this.createMode = true;
            this.editMode = false;
            this.isUserDataMounted = true;
            this.showMemberDetails=true;
        }

        public GetRoles(userId){
            const url = '/api/role'
            const options = {headers:{'Authorization' :'Bearer '+this.token}}
            this.$http.get(url, options)
                .then(response => {
                    if(response.data){
                        this.extractRoles(response.data);                        
                    }
                    this.loadUserDetails(userId);                    
                })
        }

        public roleChanged(rolInx){
            Vue.nextTick().then(()=>{
                const selectedIndex = this.selectedRoles.indexOf(this.roles[rolInx].value);
                console.log(rolInx)
                console.log(this.roles[rolInx].value)
                this.roles[rolInx].effState = true;
                this.roles[rolInx].expState = true; 
                this.roleAssignError = false; 

                if(selectedIndex >=0 && this.roles[rolInx].effDate=="")
                {
                   this.roles[rolInx].effState = false; 
                   this.selectedRoles.splice(selectedIndex, 1);
                }
                else 
                {
                    const body = 
                    [{
                        "userId": this.user.id,
                        "roleId": this.roles[rolInx].value,
                        "effectiveDate": this.roles[rolInx].effDate,
                        "expiryDate": this.roles[rolInx].expDate
                    }]
                    const url = (selectedIndex >= 0)? 'api/sheriff/assignroles' :'api/sheriff/unassignroles' 
                    const options = {headers:{'Authorization' :'Bearer '+this.token}}
                    this.$http.put(url, body, options)
                        .then(response => {
                            console.log(response)
                            console.log('assign success')  
                            if(selectedIndex < 0)
                            {
                                this.roles[rolInx].effDate =""
                                this.roles[rolInx].expDate="" 
                            }                                              
                        }, err=>{this.roleAssignError = true;});
                }
            });
        }

        public extractRoles(data){
            this.roles=[];
            this.selectedRoles = [];
            this.roleAssignError = false; 
            
            for(const role of data)            
                this.roles.push({text:role.name, desc: role.description, value:role.id, effDate:'', expDate:'', effState:true, expState: true})           
        }

        public loadUserDetails(userId): void {
            this.editMode = true;            
            const url = 'api/sheriff/' + userId
            const options = {headers:{'Authorization' :'Bearer '+this.token}}
            this.$http.get(url, options)
                .then(response => {
                    if(response.data){
                        this.userJson = response.data;
                        this.extractUserInfo();
                        this.isUserDataMounted = true;
                        this.showMemberDetails=true; 
                        console.log(response.data)                       
                    }                    
                });
        }

        public extractUserInfo(): void {
            this.user = {} as teamMemberInfoType;            
            this.user.idirUserName = this.originalUser.idirUserName = this.userJson.idirName;
            this.user.firstName = this.originalUser.firstName = this.userJson.firstName;
            this.user.lastName = this.originalUser.lastName = this.userJson.lastName;
            this.user.gender = this.originalUser.gender = gender[this.userJson.gender];
            this.user.rank = this.originalUser.rank = this.userJson.rank;
            this.user.email = this.originalUser.email = this.userJson.email;
            this.user.badgeNumber = this.originalUser.badgeNumber = this.userJson.badgeNumber;
            this.user.id = this.originalUser.id = this.userJson.id;
            this.user.image = this.originalUser.image = this.userJson['image']? this.userJson['image'] :null ; 
            for(const activeRole of this.userJson.activeRoles) 
            {             
                const index = this.roles.findIndex(role =>{if(role.value == activeRole.role.id) return true;else return false});
                if(index >=0)
                { 
                    this.roles[index].effDate = activeRole.effectiveDate
                    this.roles[index].expDate = activeRole.expiryDate
                    this.selectedRoles.push(this.roles[index].value);
                }
            }
        }

        get getCancelLabel(){
            if(this.tabIndex<1) return 'Cancel'; else return 'Close'
        }

        public saveMemberProfile() {       
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
                this.tabIndex= requiredErrorTab[0];
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
                        this.showMemberDetails = false;
                        this.getSheriffs();                     
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
                        this.showMemberDetails = false;
                        this.getSheriffs();                     
                    }
                }, err => {
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

                })   
        }

    }
</script>

<style scoped>   

    .card {
        border: white;
    }

    .closeButton {
        font-size: 20px;
        padding: 0;
        margin: 0;
    }

    .form-group.required .label:after {
  content:"*";
  color:red;
}

</style>