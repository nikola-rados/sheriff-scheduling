<template>
    <b-card bg-variant="white">
        <b-row>
            <b-col cols="11">
                <page-header :pageHeaderText="sectionHeader"></page-header>
            </b-col>
            <b-col style="padding: 0;">
                <b-button v-if="userIsAdmin" style="max-height: 40px;" size="sm" variant="warning" @click="AddRole()"><b-icon-plus/>Add a Role</b-button>
            </b-col>
        </b-row>

    </b-card>
</template>

<script lang="ts">
    import { Component, Vue } from 'vue-property-decorator';
    import { namespace } from 'vuex-class'    
    import PageHeader from "@components/common/PageHeader.vue";
    import "@store/modules/CommonInformation";  
    import {commonInfoType, locationInfoType, userInfoType} from '../../types/common';
    const commonState = namespace("CommonInformation");

    @Component({
        components: {
            PageHeader
        }
    })
    export default class DefineRolesAccess extends Vue {

        @commonState.State
        public userDetails!: userInfoType;

        @commonState.State
        public token!: string;

        @commonState.Action
        public UpdateToken!: (newToken: string) => void
        
        errorCode = 0;
        errorText = '';
        isRoleDataMounted = false;
        editMode = false;
        createMode = false;
        sectionHeader = "";
        userIsAdmin = false;

        isRolesDataMounted = false;  


        mounted() {
            this.userIsAdmin = (this.userDetails.roles.indexOf("Administrator") > -1) || (this.userDetails.roles.indexOf("System Administrator") > -1);
            // this.getRoles();
            this.sectionHeader = "Manage System Roles and Access";
        }

        // public getRoles()
        // {
        //     this.isRolesDataMounted = false;
        //     const url = 'api/sheriff?locationId=' + this.location.id
        //     const options = {headers:{'Authorization' :'Bearer '+this.token}}
        //     this.$http.get(url, options)
        //         .then(response => {
        //             if(response.data){
        //                 // console.log(response.data)
        //                 this.extractMyTeam(response.data);                        
        //             }
        //             this.isMyTeamDataMounted = true;
        //         })
        // }

    }
</script>

<style scoped>   

    .card {
        border: white;
    }

</style>