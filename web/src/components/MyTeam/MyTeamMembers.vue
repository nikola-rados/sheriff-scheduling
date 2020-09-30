<template>
    <b-card bg-variant="white">
        <b-row>
            <b-col cols="11">
                <page-header :pageHeaderText="'My Team - ' + this.location.name"></page-header>
            </b-col>
            <b-col>
                    <b-button variant="primary"> Add User </b-button>
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
                 <h2 class="mb-0 text-light"> Updating User Profile </h2>                
            </template>
            <b-card>
                <b-row>
                    <b-col cols="4">
                        <user-summary-template userId="teamMemberId" userName="teamMemberName" userRole="teamMemberRole" :editMode="true"/>
                    </b-col>
                    <b-col>
                        <b-card no-body>
                            <b-tabs card v-model="tabIndex">
                                <b-tab title="Identification" >
                                    <b-form>
                                        <b-form-group label="First Name" label-for="firstNameField">
                                            <b-form-input id="firstNameField" :v-model="user.firstName" placeholder="Enter First Name" :state = "firstNameState?null:false"></b-form-input>
                                        </b-form-group>
                                        <b-form-group label="Last Name" label-for="lastNameField">
                                            <b-form-input id="lastNameField" :v-model="user.lastName" placeholder="Enter Last Name" :state = "lastNameState?null:false"></b-form-input>
                                        </b-form-group>
                                        <b-form-group label="Gender" label-for="genderField">
                                            <b-form-select id="genderField" :v-model="user.gender" :options="genderOptions" :state = "selectedGenderState?null:false"></b-form-select>
                                        </b-form-group>
                                        <b-form-group label="Badge Number" label-for="badgeNumberField">
                                            <b-form-input id="badgeNumberField" :v-model="user.badgeNumber" placeholder="Enter Badge Number" :state = "badgeNumberState?null:false"></b-form-input>
                                        </b-form-group>
                                        <b-form-group label="Rank" label-for="rankField">
                                            <b-form-select id="rankField" :v-model="user.rank" :options="rankOptions" :state = "selectedRankState?null:false"></b-form-select>
                                        </b-form-group>
                                    </b-form>
                                </b-tab>

                                <b-tab title="Locations">
                                    <b-form>
                                        <b-form-group label="Home Location" label-for="homeLocationField">
                                            <b-form-select id="homeLocationField" :v-model="user.homeLocation" :options="locationOptions" :state = "selectedRankState?null:false"></b-form-select>
                                        </b-form-group>
                                    </b-form>
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
                                    </b-table>
                                    <b-card border-variant="white" no-body>
                                        <b-input-group class="mb-3">
                                            <b-form-select
                                                style="height: 100%;"
                                                v-model="fullDayLocation"               
                                                >
                                                <b-form-select-option
                                                v-for="location in locationList"
                                                :key="location.id"                  
                                                :value="location">{{location.name}}
                                                </b-form-select-option>                  
                                            </b-form-select>                           
                                            <b-form-datepicker placeholder="Select Start Date"
                                                v-model="fullDayStartDate"
                                                right
                                                locale="en-US"
                                            ></b-form-datepicker>                                            
                                            <b-form-datepicker placeholder="Select End Date"
                                                v-model="fullDayEndDate"
                                                right
                                                locale="en-US"
                                            ></b-form-datepicker>
                                            <b-button @click="AddCell()"><b-icon-plus></b-icon-plus></b-button>                           
                                        </b-input-group> 
                                    </b-card>                                    
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
                  @click="$bvModal.hide('bv-modal-team-member-details')"                  
                ><b-icon-x font-scale="1.5" style="padding:0; vertical-align: middle; margin-right: 0.25rem;"></b-icon-x>Cancel</b-button>
                <b-button                 
                  variant="success" 
                  @click="UpdateMemberProfile('testing updates')"
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
    import UserDetailsModal from "./UserDetailsModal.vue";
    import UserSummaryTemplate from "./UserSummaryTemplate.vue";
    import "@store/modules/CommonInformation";  
    import {locationInfoType} from '../../types/common';
    import {teamMemberInfoType} from '../../types/MyTeam';
    import {teamMemberJsonType} from '../../types/MyTeam/jsonTypes';  
    const commonState = namespace("CommonInformation");

    @Component({
        components: {
            PageHeader,
            UserDetailsModal,
            UserSummaryTemplate
        }
    })
    export default class MyTeamMembers extends Vue {

        @commonState.State
        public location!: locationInfoType;

        sectionHeader = "";
        showMemberDetails = false;

        user = {} as teamMemberInfoType;
        userJson = {} as teamMemberJsonType;

        genderOptions = ["Male", "Female", "Other"]
        rankOptions = ["Deputy Sheriff"]
        firstNameState = true;
        lastNameState = true;
        selectedGenderState = true;
        badgeNumberState = true;
        selectedRankState = true;

        tabIndex = 1;
        errorCode = 0;
        errorText = '';
        isUserDataMounted = false;


        mounted() {
            this.sectionHeader = "My Team - " + this.location.name;
        }

        public OpenMemberDetails(data)
        {
            console.log(data)
            // TODO: pass data to modal
            this.showMemberDetails=true;
            this.loadUserDetails("1234");

        }

        public loadUserDetails(userId): void {

            this.errorCode = 0;
            this.$http.get('/api/sheriff/' + userId)
                .then(Response => Response.json(), err => {this.errorCode= err.status;this.errorText= err.statusText;console.log(err);}        
                ).then(data => {
                    if(data){
                        this.userJson = data
                        this.extractUserInfo();
                        
                    }
                    this.isUserDataMounted = true;
                });
        }

        public extractUserInfo(): void {
            this.user.firstName = this.userJson.firstName;
            this.user.lastName = this.userJson.lastName;

        }

        public UpdateMemberProfile() {
            console.log("saving on submit")
            const requiredErrorTab: number[] = [];
            if (!this.user.firstName) {
                this.firstNameState = false;
                requiredErrorTab.push(0);
            }
            if (!this.user.lastName) {
                this.lastNameState = false;
                requiredErrorTab.push(0);
            }
            if (!this.user.gender) {
                this.selectedGenderState = false;
                requiredErrorTab.push(0);
            }
            if (!this.user.badgeNumber) {
                this.badgeNumberState = false;
                requiredErrorTab.push(0);
            }
            if (!this.user.rank) {
                this.selectedRankState = false;
                requiredErrorTab.push(0);
            }
            
            if (requiredErrorTab.length == 0) {
                //SAVE

                this.showMemberDetails = false;

            } else {
                
                this.tabIndex= requiredErrorTab[0];

            }

             
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

</style>