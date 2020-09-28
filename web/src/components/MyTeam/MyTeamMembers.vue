<template>
    <b-card bg-variant="white">
        <b-row>
            <b-col cols="11">
                <page-header :pageHeaderText="'My Team - ' + this.location.name"></page-header>
            </b-col>
            <b-col>
                <!-- <div> -->
                    <b-button variant="primary"> Add User </b-button>
                    <!-- <hr class="mb-2 bg-light" style="height: 5px;"/>  -->
                <!-- </div> -->
                
            </b-col>
        </b-row>
        
        <b-card no-body class="mx-3 mb-5">
            <b-card-group deck>    
                <b-card class="text-center" border-variant="secondary">                                    
                    <!-- <b-card-img
                        src="https://picsum.photos/600/300/?image=25"
                        style="max-width: 20rem;"
                        class="mb-2"
                    ></b-card-img> -->
                    <b-icon-person-circle class="mb-5" variant="secondary" font-scale="7.5"></b-icon-person-circle>
                    <b-card-sub-title>Lorem</b-card-sub-title>
                    <b-card-title>CHAD ERNEST FAST</b-card-title>
                    <b-card-sub-title>Inspector</b-card-sub-title>                
                </b-card>                            
            </b-card-group>
        </b-card>
    </b-card>
</template>

<script lang="ts">
    import { Component, Vue } from 'vue-property-decorator';
    import { namespace } from 'vuex-class';
    import PageHeader from "@components/common/PageHeader.vue";
    import "@store/modules/CommonInformation";  
    import {locationInfoType} from '../../types/common';  
    const commonState = namespace("CommonInformation");

    @Component({
        components: {
            PageHeader
        }
    })
    export default class MyTeamMembers extends Vue {

        @commonState.State
        public location!: locationInfoType;

        sectionHeader = "";

        mounted() {
            this.sectionHeader = "My Team - " + this.location.name;
        }

    }
</script>

<style scoped>   

    .card {
        border: white;
    }

</style>