<template>
    <b-card bg-variant="white">
        <page-header :pageHeaderText="sectionHeader"></page-header>
    </b-card>
</template>

<script lang="ts">
    import { Component, Vue } from 'vue-property-decorator';
    import { namespace } from 'vuex-class'
    import PageHeader from "@components/common/PageHeader.vue";

    @Component({
        components: {
            PageHeader
        }
    })
    export default class FindManageUsers extends Vue {

        sectionHeader = "";

        mounted() {
            this.sectionHeader = "Assign Provincial User Roles";
        }

    }
</script>

<style scoped>   

    .card {
        border: white;
    }

</style>