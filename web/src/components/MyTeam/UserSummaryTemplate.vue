<template> 
    <b-card class="text-center" border-variant="secondary">
            <!-- <b-card-img
                src="https://picsum.photos/600/300/?image=25"
                style="max-width: 20rem;"
                class="mb-2"
            ></b-card-img> -->
            <b-icon-person-circle class="mb-3" variant="secondary" font-scale="7.5"></b-icon-person-circle>
            <b-card no-body class="mb-5 mt-2" v-if="editMode"><b-button variant="outline-secondary">Browse for File</b-button></b-card>
            <b-card-sub-title>{{userId}}</b-card-sub-title>
            <b-card-title>{{userName}}</b-card-title>
            <b-card-sub-title>{{userRole}}</b-card-sub-title>
    </b-card>   
</template>

<script lang="ts">
    import { Component, Vue, Prop } from 'vue-property-decorator';
    import { namespace } from 'vuex-class';
    
    import "@store/modules/CommonInformation";  
    import {locationInfoType} from '../../types/common';  
    const commonState = namespace("CommonInformation");

    @Component
    export default class UserSummaryTemplate extends Vue {

        @commonState.State
        public location!: locationInfoType;

        @Prop({required: true})
        userId!: string;

        @Prop({required: true})
        userName!: string;

        @Prop({required: true})
        userRole!: string;

        @Prop({required: true})
        editMode!: boolean;



        // sectionHeader = "";

        mounted() {
            // this.sectionHeader = "My Team - " + this.location.name;
        }

    }
</script>

<style scoped>   

    .card {
        border: white;
    }

</style>