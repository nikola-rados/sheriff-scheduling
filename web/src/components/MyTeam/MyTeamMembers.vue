<template>
    <b-card bg-variant="white">
        <b-row>
            <b-col cols="11">
                <page-header :pageHeaderText="'My Team - ' + this.commonInfo.location.name"></page-header>
            </b-col>
            <b-col style="padding: 0;">
                <b-button style="max-height: 40px;" size="sm" variant="warning" @click="AddMember()"><b-icon-plus/>Add a User</b-button>
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
                    <div class="card h-100">
                        <div class="card-body">
                            <user-summary-template :userBadgeNumber="teamMember.badgeNumber" :userName="teamMember.fullName" :userRole="teamMember.rank" :userImage="teamMember.image" :editMode="false" />
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
            <b-card v-if="isUserDataMounted">
                <b-row>
                    <b-col cols="4">
                        <user-summary-template :userBadgeNumber="user.badgeNumber" :userName="user.firstName + ' ' + user.lastName" :userRole="user.rank" :userImage="user.image" :editMode="editMode"/>
                    </b-col>
                    <b-col>
                        <b-card no-body>
                            <b-tabs card v-model="tabIndex">
                                <b-tab title="Identification" >
                                    
                                    <b-form-group v-if="createMode"><label>IDIR User Name<span class="text-danger">*</span></label>
                                        <b-form-input v-model="user.idirUserName" placeholder="Enter IDIR User Name" :state = "idirUserNameState?null:false"></b-form-input>
                                    </b-form-group>

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
                                    
                                </b-tab>

                                <b-tab title="Locations">                                    
                                </b-tab>
                                <b-tab title="Leaves">                                    
                                </b-tab>
                                <b-tab title="Training"> 
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
                ><b-icon-x font-scale="1.5" style="padding:0; vertical-align: middle; margin-right: 0.25rem;"></b-icon-x>Cancel</b-button>
                <b-button                 
                  variant="success" 
                  @click="saveMemberProfile('testing updates')"
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
    import PageHeader from "@components/common/PageHeader.vue";
    import UserSummaryTemplate from "./UserSummaryTemplate.vue";
    import "@store/modules/CommonInformation";  
    import {commonInfoType} from '../../types/common';
    import {teamMemberInfoType} from '../../types/MyTeam';
    import {teamMemberJsonType} from '../../types/MyTeam/jsonTypes';  
    const commonState = namespace("CommonInformation");

    enum gender {'Male'=0, 'Female', 'Other'}

    @Component({
        components: {
            PageHeader,
            UserSummaryTemplate
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

        sectionHeader = "";
        showMemberDetails = false;
        showCancelWarning = false;
        //TODO: get user role and Make sure only super-admin can see the "add a user" yellow button
        user = {} as teamMemberInfoType;
        userJson = {} as teamMemberJsonType;
        genderOptions = [{text:"Male", value: gender.Male}, {text:"Female", value: gender.Female}, {text:"Other", value: gender.Other}]
        genderValues = [0, 1, 2]        
        idirUsernameState = true;
        firstNameState = true;
        lastNameState = true;
        emailState = true;
        selectedGenderState = true;
        badgeNumberState = true;
        selectedRankState = true;
        idirUserNameState = true;

        tabIndex = 0;
        errorCode = 0;
        errorText = '';
        isUserDataMounted = false;
        editMode = false;
        createMode = false;

        isMyTeamDataMounted = false;
        myTeamData: teamMemberInfoType[] =[];
        
        @Watch('commonInfo.location.id', { immediate: true })
        locationChange()
        {
            this.GetSheriffs()
        }

        mounted() {
            this.sectionHeader = "My Team - " + this.commonInfo.location.name;
            this.GetSheriffs();
        }

        public GetSheriffs()
        {
            this.isMyTeamDataMounted = false;

            const url = 'api/sheriff?locationId=' + this.commonInfo.location.id
            const options = {headers:{'Authorization' :'Bearer '+this.token}}
            console.log(options)
            this.$http.get(url, options)
                .then(response => {
                    if(response.data){
                        console.log(response.data)
                        this.ExtractMyTeam(response.data);                        
                    }
                    this.isMyTeamDataMounted = true;
                });
        }

        public ExtractMyTeam(data: any)
        {
            this.myTeamData = [];
            
            for(const myteaminfo of data)
            {
                const myteam: teamMemberInfoType = {id:'',idirUserName:'', rank:'', firstName:'', lastName:'', email:'', badgeNumber:'', gender:'' }
                myteam.fullName = this.getFullNameOfPerson(myteaminfo.firstName ,myteaminfo.lastName);
                myteam.gender = myteaminfo.gender;
                myteam.rank = myteaminfo.rank;
                myteam.badgeNumber = myteaminfo.badgeNumber;

                this.myTeamData.push(myteam);
            }
        }


        public getFullNameOfPerson(first: string, last: string)
        {            
            return Vue.filter('capitilize')(first) + ' ' + Vue.filter('capitilize')(last);
        }

        public OpenMemberDetails(data)
        {
            console.log(data)
            // TODO: pass data to modal
            this.createMode = false;
            this.editMode = true;
            this.showMemberDetails=true;
            this.loadUserDetails("1234");
        }

        public closeProfileWindow() 
        {         
            if(this.isEmpty(this.user))
            {
                this.showMemberDetails = false;
                this.resetProfileWindowState();
            }   
            else
                this.showCancelWarning = true;
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
            this.user = {} as teamMemberInfoType;
        }

        public AddMember()
        {            
            // TODO: pass data to modal
            this.createMode = true;
            this.editMode = false;
            this.isUserDataMounted = true;
            this.showMemberDetails=true;
        }

        public loadUserDetails(userId): void {
            console.log("loading user info" + userId)

            // this.editMode = true;
            // this.errorCode = 0;
            // this.$http.get('/api/sheriff/' + userId)
            //     .then(Response => Response.json(), err => {this.errorCode= err.status;this.errorText= err.statusText;console.log(err);}        
            //     ).then(data => {
            //         if(data){
            //             this.userJson = data
            //             this.extractUserInfo();
            //             this.isUserDataMounted = true;                        
            //         }
                    
            //     });
        }

        // public extractUserInfo(): void {
        //     this.user.firstName = this.userJson.firstName;
        //     this.user.lastName = this.userJson.lastName;

        // }

        public saveMemberProfile() {            
            const requiredErrorTab: number[] = [];

            if (!this.user.idirUserName) {
                this.idirUserNameState = false;
                requiredErrorTab.push(0);
            } else {
                this.idirUserNameState = true;
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
                this.resetProfileWindowState();
                
                //SAVE

                this.showMemberDetails = false;

            } else {                
                this.tabIndex= requiredErrorTab[0];
            }             
        }


        public updateProfile(): void {
            console.log("updating profile")

        }

        public createProfile() {
            console.log("creating profile")
            const body = {
                homeLocationId: this.commonInfo.location.id,               
                gender: this.user.gender,
                badgeNumber: this.user.badgeNumber,
                rank: this.user.rank,
                idirName: this.user.idirUserName,
                firstName:this.user.firstName,
                lastName: this.user.lastName,
                email: this.user.email
            }
            console.log(body)
            const url = 'api/sheriff';
            const options = {headers:{'Authorization' :'Bearer '+this.token}}
            
            this.$http.post(url, body, options )
                .then(data => {
                    if(data){
                        console.log(data) 
                        this.GetSheriffs();                     
                    }
                });            
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