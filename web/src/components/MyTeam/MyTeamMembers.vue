<template>
    <b-card bg-variant="white">
        <b-row class="bg-white">
            <b-col cols="10">
                <page-header :pageHeaderText="sectionHeader"></page-header>
                <b-card  >  
                    <b-form-group class="mr-1" style="width: 20rem"><label class="ml-1">Searching keyword:</label>
                        <b-form-input v-model="searchPhrase" placeholder="Enter Name"></b-form-input>
                        <b-form-text class="text-light font-italic"> Name/Rank/BadgeNumber/Location </b-form-text>
                    </b-form-group>
                </b-card> 
            </b-col>
            <b-col style="padding: 0;">
                <div :class="expiredViewChecked?'my-4 bg-warning':'my-4'" :style="expiredViewChecked?'max-width: 160px;':''">
                    <b-form-checkbox v-model="expiredViewChecked" :style="expiredViewChecked?'max-width: 160px; margin-left:10px;':''" @change="getSheriffs()" size="lg"  switch>
                        {{viewStatus}}
                    </b-form-checkbox>
                </div>                
                <b-button v-if="userIsAdmin" style="max-height: 40px;" size="sm" variant="success" @click="AddMember()" class="my-2"><b-icon-plus/>Add User</b-button>  
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

        <div v-else class="container mb-5" style="float: left;" id="app">
            <div class="row" :key="photokey">
                <div v-for="teamMember in myTeamData" :key="teamMember.badgeNumber" class="col-3  my-1">
                    <div  class="card h-100 bg-dark">
                        <div @click="openMemberDetails(teamMember.id)" class="card-body">
                            <user-summary-template v-on:photoChange="photoChanged" :user="teamMember" :editMode="false" />
                        </div>
                        <div class="card-footer text-white bg-dark border-dark" >                                                
                            <expire-sheriff-profile :userID="teamMember.id" :userIsEnable="teamMember.isEnabled" @change="getSheriffs()" />                        
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
                        <user-summary-template v-on:photoChange="photoChanged" :user="user" :editMode="editMode"/>
                    </b-col>
                    <b-col cols="9">
                        <b-card no-body>
                            <b-tabs card v-model="tabIndex">
                                <b-tab title="Identification" >
                                    <identification-tab 
                                        :runMethod="identificationTabMethods" 
                                        v-on:showWarning="$bvModal.show('bv-modal-team-cancel-warning')" 
                                        v-on:closeMemberDetails="closeWarningWindow()" 
                                        v-on:profileUpdated="getSheriffs()" 
                                        :originalUser="user"
                                        :createMode="createMode" 
                                        :editMode="editMode" />
                                </b-tab>

                                <b-tab title="Locations">                                    
                                </b-tab>

                                <b-tab title="Leaves">                                    
                                </b-tab>

                                <b-tab title="Training"> 
                                </b-tab>

                                <b-tab v-if="userIsAdmin & editMode" title="Roles" class="p-0">
                                    <role-assignment-tab :userId="user.id" :userAllRoles="userAllRoles" />
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
    import UserSummaryTemplate from "./utils/UserSummaryTemplate.vue";
    import "@store/modules/CommonInformation";  
    import {commonInfoType, locationInfoType, userInfoType} from '../../types/common';
    import {teamMemberInfoType, roleOptionInfoType} from '../../types/MyTeam';
    import {teamMemberJsonType} from '../../types/MyTeam/jsonTypes';  
    const commonState = namespace("CommonInformation");
    import store from '../../store'
    import ExpireSheriffProfile from './utils/ExpireSheriffProfile.vue'
    import RoleAssignmentTab from './utils/RoleAssignmentTab.vue'
    import IdentificationTab from './utils/IdentificationTab.vue'

    enum gender {'Male'=0, 'Female', 'Other'}

    @Component({
        components: {
            PageHeader,
            UserSummaryTemplate,
            ExpireSheriffProfile,
            RoleAssignmentTab,
            IdentificationTab
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
        
        expiredViewChecked = false;
        showMemberDetails = false;
        showCancelWarning = false;
        user = {} as teamMemberInfoType;
        userIsAdmin = false;

        tabIndex = 0;
        isUserDataMounted = false;
        editMode = false;
        createMode = false;
        sectionHeader = '';
        photokey=0;
        userAllRoles: any[] = [];

        searchPhrase = '';

        isMyTeamDataMounted = false;
    
        allMyTeamData: teamMemberInfoType[] =[];
        
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
        
        get viewStatus() {
            if(this.expiredViewChecked) return 'All Profiles';else return 'Active Profiles'
        }

        public getSheriffs()
        {
            this.isMyTeamDataMounted = false;
            const url = 'api/sheriff'//?locationId=' + this.location.id
            const options = {headers:{'Authorization' :'Bearer '+this.token}}
            this.$http.get(url, options)
                .then(response => {
                    if(response.data){
                        console.log(response.data)
                        this.extractMyTeamFromSheriffs(response.data);                        
                    }
                    this.isMyTeamDataMounted = true;
                })
        }

        public extractMyTeamFromSheriffs(data: any){
    
            this.allMyTeamData = [];            
            for(const myteaminfo of data)
            {
                const myteam: teamMemberInfoType = {id:'',idirUserName:'', rank:'', firstName:'', lastName:'', email:'', badgeNumber:'', gender:'' }
                myteam.fullName = Vue.filter('capitilize')(myteaminfo.firstName) + ' ' + Vue.filter('capitilize')(myteaminfo.lastName);
                myteam.firstName = myteaminfo.firstName;
                myteam.lastName = myteaminfo.lastName;
                myteam.rank = myteaminfo.rank;
                myteam.badgeNumber = myteaminfo.badgeNumber;
                myteam.id = myteaminfo.id;
                myteam.image = myteaminfo.photo? 'data:image/;base64,'+myteaminfo.photo: '';
                myteam.isEnabled = myteaminfo.isEnabled;
                myteam.homeLocationId = myteaminfo.homeLocationId;
                myteam.homeLocationNm = myteaminfo.homeLocation.name;
                this.allMyTeamData.push(myteam);
            }            
        }

        get myTeamData() {
            return this.allMyTeamData.filter(member => {
                if (this.expiredViewChecked)
                {
                    if(this.searchPhrase=='')
                    {
                        if(member.homeLocationId == this.location.id)
                            return true
                    }
                    else
                    { 
                        if(member.firstName && member.firstName.toLowerCase().startsWith(this.searchPhrase.toLowerCase()))
                            return true
                        if(member.lastName && member.lastName.toLowerCase().startsWith(this.searchPhrase.toLowerCase()))
                            return true
                        if(member.rank && member.rank.toLowerCase().startsWith(this.searchPhrase.toLowerCase()))
                            return true
                        if(member.badgeNumber && member.badgeNumber.toLowerCase().startsWith(this.searchPhrase.toLowerCase()))
                            return true
                        if(member.homeLocationNm && member.homeLocationNm.toLowerCase().startsWith(this.searchPhrase.toLowerCase()))
                            return true
                    }
                }
                else if(member.isEnabled){
                    if(this.searchPhrase=='')
                    {
                        if(member.homeLocationId == this.location.id)
                            return true
                    }
                    else
                    { 
                        if(member.firstName && member.firstName.toLowerCase().startsWith(this.searchPhrase.toLowerCase()))
                            return true
                        if(member.lastName && member.lastName.toLowerCase().startsWith(this.searchPhrase.toLowerCase()))
                            return true
                        if(member.rank && member.rank.toLowerCase().startsWith(this.searchPhrase.toLowerCase()))
                            return true
                        if(member.badgeNumber && member.badgeNumber.toLowerCase().startsWith(this.searchPhrase.toLowerCase()))
                            return true
                        if(member.homeLocationNm && member.homeLocationNm.toLowerCase().startsWith(this.searchPhrase.toLowerCase()))
                            return true
                    }
                }
            });
        }

        

        public photoChanged(id: string, image: string){
           
            const index = this.allMyTeamData.findIndex(myteam => {if(myteam.id == id) return true;else return false})
            if( index >=0 ){
                this.allMyTeamData[index].image = image;
                this.photokey++;
            }
        }      

        public openMemberDetails(userId){
            this.createMode = false;
            this.editMode = true;            
            this.loadUserDetails(userId);            
            this.editMode = true;
        }

        identificationTabMethods = new Vue();

        public closeProfileWindow(){
            this.identificationTabMethods.$emit('closeProfileWindow'); 
        }

        public closeWarningWindow() {
            this.showCancelWarning = false;
            this.showMemberDetails = false;
            this.resetProfileWindowState();            
        }

        public resetProfileWindowState() {
            this.createMode = false;
            this.editMode = false;
            this.user = {} as teamMemberInfoType;
        }

        public AddMember(){ 
            this.createMode = true;
            this.editMode = false;
            this.isUserDataMounted = true;
            this.showMemberDetails = true;
        }

        public loadUserDetails(userId): void {
            this.editMode = true;            
            const url = 'api/sheriff/' + userId
            const options = {headers:{'Authorization' :'Bearer '+this.token}}
            this.$http.get(url, options)
                .then(response => {
                    if(response.data){
                        console.log(response.data)                        
                        this.extractUserInfo(response.data);
                        this.isUserDataMounted = true;
                        this.showMemberDetails=true;                                              
                    }                    
                });
        }

        public extractUserInfo(userJson): void {
            console.log(userJson)
            this.user = {} as teamMemberInfoType;            
            this.user.idirUserName =  userJson.idirName;
            this.user.firstName = userJson.firstName;
            this.user.lastName = userJson.lastName;
            this.user.fullName = Vue.filter('capitilize')(userJson.firstName) + ' ' + Vue.filter('capitilize')(userJson.lastName);
            this.user.gender = gender[userJson.gender];
            this.user.rank = userJson.rank;
            this.user.email = userJson.email;
            this.user.badgeNumber = userJson.badgeNumber;
            this.user.id = userJson.id;
            this.user.image = userJson['photo']?'data:image/;base64,'+userJson['photo']:''; 
            console.log(this.user)
            this.userAllRoles = userJson.roles
        }

        get getCancelLabel(){
            if(this.tabIndex<1) return 'Cancel'; else return 'Close'
        }

        public saveMemberProfile() { 
            this.identificationTabMethods.$emit('saveMemberProfile');
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