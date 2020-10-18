<template>
    <b-card bg-variant="white">
        <b-row class="bg-white">
            <b-col cols="10">
                <page-header :pageHeaderText="sectionHeader"></page-header>
                <b-card>  
                    <b-form-group class="mr-1" style="width: 20rem"><label class="ml-1">Searching keyword:</label>
                        <b-form-input v-model="searchPhrase" placeholder="Enter Keyword"></b-form-input>
                        <b-form-text class="text-light font-italic"> Name/Rank/Location/Badge Number </b-form-text>
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
                        <div class="card-header bg-dark border-dark mb-0 pb-0 " >
                            <b-row class="ml-4">                                                
                                <user-location-summary v-if="teamMember.loanedOut.length>0" :homeLocation="teamMember.homeLocationNm" :loanedJson="teamMember.loanedOut" :index="teamMember.badgeNumber"/>
                                <user-training-summary class="mx-2" v-if="teamMember.training.length>0" :trainingJson="teamMember.training" :index="teamMember.badgeNumber"/>
                            </b-row>
                        </div>
                        <div @click="openMemberDetails(teamMember.id)" class="card-body my-1 py-0">
                            <user-summary-template v-on:photoChange="photoChanged" :user="teamMember" :editMode="false" />
                        </div>
                        <div class="card-footer text-white bg-dark border-dark mt-0 pt-0" >                                                
                            <expire-sheriff-profile :disabled="!userIsAdmin" :userID="teamMember.id" :userIsEnable="teamMember.isEnabled" @change="getSheriffs()" />                        
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
                        <user-summary-template v-on:photoChange="photoChanged" :user="userToEdit" :editMode="editMode"/>
                    </b-col>
                    <b-col cols="9">
                        <b-card no-body >
                            <b-tabs  card v-model="tabIndex" @activate-tab="onTabChanged">
                                <b-tab title="Identification">
                                    <identification-tab 
                                        :runMethod="identificationTabMethods"                                         
                                        v-on:closeMemberDetails="closeMemberDetailWindow()" 
                                        v-on:profileUpdated="getSheriffs()"   
                                        v-on:changeTab="changeTab"                                     
                                        :createMode="createMode" 
                                        :editMode="editMode" />
                                </b-tab>

                                <b-tab v-if="editMode" title="Locations" class="p-0"> 
                                    <location-tab 
                                        v-on:change="getSheriffs()"
                                        v-on:closeMemberDetails="closeProfileWindow()"/>                                   
                                </b-tab>

                                <b-tab v-if="editMode" title="Leaves">                                    
                                </b-tab>

                                <b-tab v-if="editMode" title="Training"> 
                                </b-tab>

                                <b-tab v-if="userIsAdmin & editMode" title="Roles" class="p-0">
                                    <role-assignment-tab :userId="userToEdit.id" :userAllRoles="userAllRoles" />
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

        
    </b-card>
</template>

<script lang="ts">
    import { Component, Vue, Watch } from 'vue-property-decorator';
    import { namespace } from 'vuex-class';
    import * as _ from 'underscore';
    import PageHeader from "@components/common/PageHeader.vue";
    
    import "@store/modules/CommonInformation"; 
    import "@store/modules/TeamMemberInformation"; 
    
    import {commonInfoType, locationInfoType, userInfoType} from '../../types/common';
    import {teamMemberInfoType, roleOptionInfoType} from '../../types/MyTeam';
    import {teamMemberJsonType} from '../../types/MyTeam/jsonTypes';  
    const commonState = namespace("CommonInformation");
    const TeamMemberState = namespace("TeamMemberInformation");
    import store from '../../store'
    import ExpireSheriffProfile from './Tabs/ExpireSheriffProfile.vue'
    import RoleAssignmentTab from './Tabs/RoleAssignmentTab.vue'
    import IdentificationTab from './Tabs/IdentificationTab.vue'
    import UserSummaryTemplate from "./Tabs/UserSummaryTemplate.vue";
    import LocationTab from './Tabs/LocationTab.vue';
    import UserLocationSummary from './Tabs/UserLocationSummary.vue';
    import UserTrainingSummary from './Tabs/UserTrainingSummary.vue';

    enum gender {'Male'=0, 'Female', 'Other'}

    @Component({
        components: {
            PageHeader,
            UserSummaryTemplate,
            UserLocationSummary,
            UserTrainingSummary,
            ExpireSheriffProfile,
            RoleAssignmentTab,
            IdentificationTab,
            LocationTab
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

        @TeamMemberState.State
        public userToEdit!: teamMemberInfoType;

        @TeamMemberState.Action
        public UpdateUserToEdit!: (userToEdit: teamMemberInfoType) => void

        identificationTabMethods = new Vue();
        
        expiredViewChecked = false;
        showMemberDetails = false;
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
        newTabIndex = 0;
        firstNavigation = true;
    
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
            const url = 'api/sheriff'
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
        
        public onTabChanged(newTabIndex , prevTabIndex, bvEvt)
        {
            this.newTabIndex = newTabIndex;            
            if(prevTabIndex == 0 && this.firstNavigation)
            {
                this.firstNavigation = false;
                bvEvt.preventDefault();
                this.identificationTabMethods.$emit('switchTab');   
            }            
        }

        public changeTab(changingTab) {
            if(changingTab) this.tabIndex = this.newTabIndex;
            Vue.nextTick().then(()=>{this.firstNavigation = true;});
        }

        public extractMyTeamFromSheriffs(data: any) {    
            this.allMyTeamData = [];            
            for(const myteaminfo of data)
            {
                const myteam = {} as teamMemberInfoType;
                myteam.fullName = Vue.filter('capitilize')(myteaminfo.firstName) + ' ' + Vue.filter('capitilize')(myteaminfo.lastName);
                myteam.firstName = myteaminfo.firstName;
                myteam.lastName = myteaminfo.lastName;
                myteam.rank = myteaminfo.rank;
                myteam.badgeNumber = myteaminfo.badgeNumber;
                myteam.id = myteaminfo.id;
                myteam.image = myteaminfo.photo? 'data:image/;base64,'+myteaminfo.photo: '';
                myteam.isEnabled = myteaminfo.isEnabled;
                myteam.homeLocationId = myteaminfo.homeLocationId;
                myteam.homeLocationNm = myteaminfo.homeLocation? myteaminfo.homeLocation.name: '';
                myteam.training = myteaminfo.training? myteaminfo.training: [];
                myteam.loanedOut = myteaminfo.loanedOut;
                if(myteaminfo.homeLocation)
                    myteam.homeLocation = {id: myteaminfo.homeLocation.id, name: myteaminfo.homeLocation.name, regionId: myteaminfo.homeLocation.regionId};
                this.allMyTeamData.push(myteam);
            }            
        }

        get myTeamData() {
            return this.allMyTeamData.filter(member => {
                if (this.expiredViewChecked || member.isEnabled)
                {
                    if(this.searchPhrase==''){
                        if(member.homeLocationId == this.location.id) return true
                        for(const loanInx in member.loanedOut)
                            if(member.loanedOut[loanInx].locationId == this.location.id ) return true
                    }
                    else{ 
                        if(member.firstName && member.firstName.toLowerCase().startsWith(this.searchPhrase.toLowerCase())) return true
                        if(member.lastName && member.lastName.toLowerCase().startsWith(this.searchPhrase.toLowerCase()))   return true
                        if(member.rank && member.rank.toLowerCase().startsWith(this.searchPhrase.toLowerCase()))           return true
                        if(member.badgeNumber && member.badgeNumber.toLowerCase().startsWith(this.searchPhrase.toLowerCase())) return true
                        if(member.homeLocationNm && member.homeLocationNm.toLowerCase().startsWith(this.searchPhrase.toLowerCase())) return true
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
            if(this.userIsAdmin)
            {
                this.createMode = false;
                this.editMode = true;            
                this.loadUserDetails(userId);
            }                        
        }

        public saveMemberProfile() { 
            this.identificationTabMethods.$emit('saveMemberProfile');
        }  

        public closeProfileWindow(){
            console.log(this.tabIndex)
            if(this.tabIndex ==0 || this.createMode)
            {  
                this.identificationTabMethods.$emit('closeProfileWindow');
            }
            else 
                this.closeMemberDetailWindow()
        }

        public closeMemberDetailWindow(){            
            this.showMemberDetails = false;
            this.resetProfileWindowState();            
        }

        public resetProfileWindowState(){
            this.createMode = false;
            this.editMode = false;
            this.tabIndex = 0;
            const user = {} as teamMemberInfoType;
            this.UpdateUserToEdit(user);  
        }

        public AddMember(){ 
            this.tabIndex = 0;
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
            const user = {} as teamMemberInfoType;            
            user.idirUserName =  userJson.idirName;
            user.firstName = userJson.firstName;
            user.lastName = userJson.lastName;
            user.fullName = Vue.filter('capitilize')(userJson.firstName) + ' ' + Vue.filter('capitilize')(userJson.lastName);
            user.gender = gender[userJson.gender];
            user.rank = userJson.rank;
            user.email = userJson.email;
            user.badgeNumber = userJson.badgeNumber;
            user.id = userJson.id;
            user.homeLocationId = userJson.homeLocationId;
            user.homeLocationNm = userJson.homeLocation? userJson.homeLocation.name: '';
            user.image = userJson['photo']?'data:image/;base64,'+userJson['photo']:'';
            if(userJson.homeLocation)
                user.homeLocation  = {id: userJson.homeLocation.id, name: userJson.homeLocation.name, regionId: userJson.homeLocation.regionId};
          
            if(userJson.awayLocation && userJson.awayLocation.length>0)
                user.awayLocation = userJson.awayLocation;
            //console.log(this.user)
            this.userAllRoles = userJson.roles
            this.UpdateUserToEdit(user);  
        }

        get getCancelLabel(){
            if(this.tabIndex<1) return 'Cancel'; else return 'Close'
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