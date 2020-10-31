<template>
    <div>    
        <b-card 
            :style=" 'position: relative; left:'+ startTime +'%; width:' + timeDuration+'%; height:6rem;'" 
            :bg-variant="color" 
            class="m-0 p-0" 
            body-class="m-0 py-0" 
            header-class=" h6 m-0 p-0" 
            header="Court">                                
                hi
        </b-card>
       
    </div>
</template>

<script lang="ts">
    import { Component, Vue, Prop } from 'vue-property-decorator';
    import { namespace } from 'vuex-class'

    @Component
    export default class ScheduleCard extends Vue {

        @Prop({required: true})
        startTime!: string;

        @Prop({required: true})
        timeDuration!: string;

        @Prop({required: true})
        color!: string;



    }
</script>

<style scoped>   

    .card {
        border: white;
    }

</style>