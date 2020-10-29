<template>
    <b-card bg-variant="light">
        <b-overlay :show= "true"> 
            <b-card  style="min-height: 100px;"/>                   
            <template v-slot:overlay>               
            <div> 
                <spinner/> 
                <p id="loading-label">Loading ...</p>
            </div>                
            </template> 
        </b-overlay> 
    </b-card>
</template>

<script lang="ts">
    import { Component, Vue } from 'vue-property-decorator';
    import Spinner from "@components/Spinner.vue";
@Component({
        components: {           
            Spinner
        }        
    })    
    export default class LoadingSpinner extends Vue {
    }
</script>

<style scoped>   
    .card {
        border: white;
    }
</style>
