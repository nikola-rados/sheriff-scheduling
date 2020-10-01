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
        
        <b-card no-body class="mx-3 mb-5">
            <b-card-group deck>    
                <b-card @click="OpenMemberDetails('testing ID')">
                    <user-summary-template userId="teamMemberId" userName="teamMemberName" userRole="teamMemberRole" :editMode="false" />
                </b-card>                                         
            </b-card-group>
        </b-card>

        <b-modal size="xl" v-model="showMemberDetails" id="bv-modal-team-member-details" header-class="bg-primary text-light">            
            <template v-slot:modal-title>                
                 <h2 v-if="editMode" class="mb-0 text-light"> Updating User Profile </h2>
                 <h2 v-else-if="createMode" class="mb-0 text-light"> Creating User Profile </h2>                
            </template>
            <b-card v-if="isUserDataMounted">
                <b-row>
                    <b-col cols="4">
                        <user-summary-template userId="teamMemberId" userName="teamMemberName" userRole="teamMemberRole" :editMode="editMode"/>
                    </b-col>
                    <b-col>
                        <b-card no-body>
                            <b-tabs card v-model="tabIndex">
                                <b-tab title="Identification" >
                                    <b-form>
                                        <b-form-group v-if="createMode"><label>IDIR User Name<span class="text-danger">*</span></label>
                                            <b-form-input v-model="user.idirUserName" placeholder="Enter IDIR User Name" :state = "idirUserNameState?null:false"></b-form-input>
                                        </b-form-group>
                                        <b-form-group><label>First Name<span class="text-danger">*</span></label>
                                            <b-form-input v-model="user.firstName" placeholder="Enter First Name" :state = "firstNameState?null:false"></b-form-input>
                                        </b-form-group>
                                        <b-form-group><label>Last Name<span class="text-danger">*</span></label>
                                            <b-form-input v-model="user.lastName" placeholder="Enter Last Name" :state = "lastNameState?null:false"></b-form-input>
                                        </b-form-group>
                                        <b-form-group><label>Gender<span class="text-danger">*</span></label>
                                            <b-form-select v-model="user.gender" :options="genderOptions" :state = "selectedGenderState?null:false"></b-form-select>
                                        </b-form-group>
                                        <b-form-group><label>Badge Number<span class="text-danger">*</span></label>
                                            <b-form-input v-model="user.badgeNumber" placeholder="Enter Badge Number" :state = "badgeNumberState?null:false"></b-form-input>
                                        </b-form-group>
                                        <b-form-group><label>Email<span class="text-danger">*</span></label>
                                            <b-form-input v-model="user.email" placeholder="Enter Email" :state = "emailState?null:false" type="email"></b-form-input>
                                        </b-form-group>
                                        <b-form-group><label>Rank<span class="text-danger">*</span></label>
                                            <b-form-select v-model="user.rank" placeholder="Select Rank" :options="commonInfo.sheriffRankList" :state = "selectedRankState?null:false"></b-form-select>
                                        </b-form-group>
                                    </b-form>
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
    </b-card>
</template>

<script lang="ts">
    import { Component, Vue } from 'vue-property-decorator';
    import { namespace } from 'vuex-class';
    import PageHeader from "@components/common/PageHeader.vue";
    import UserSummaryTemplate from "./UserSummaryTemplate.vue";
    import "@store/modules/CommonInformation";  
    import {commonInfoType} from '../../types/common';
    import {teamMemberInfoType} from '../../types/MyTeam';
    import {teamMemberJsonType} from '../../types/MyTeam/jsonTypes';  
    const commonState = namespace("CommonInformation");

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

        sectionHeader = "";
        showMemberDetails = false;
        //TODO: get user role and Make sure only super-admin can see the "add a user" yellow button
        user = {} as teamMemberInfoType;
        userJson = {} as teamMemberJsonType;
        //TODO: define gender as enum
        genderOptions = [{text:"Male", value: 0}, {text:"Female", value: 1}, {text:"Other", value: 2}]
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


        mounted() {
            this.sectionHeader = "My Team - " + this.commonInfo.location.name;
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

        public closeProfileWindow() {
            this.showMemberDetails = false;
            this.resetProfileWindowState();
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
            console.log("loading user info")

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

        public createProfile(): void {
            console.log("creating profile")
            
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