<template>
    <div>    
        <b-card 
            style=" width:100%; height:4.5rem;" 
            bg-variant="white"            
            class="mx-0 my-1 p-0" 
            body-class="m-0 p-0" 
            header-class=" h6 m-0 p-0">
                <b-row style="margin-left:2px;font-size:12px; line-height: 16px;">#{{badgeNumber}}</b-row>
                <b-row style="margin-left:2px;font-size:10px; line-height: 13px;">Deputy Sheriff</b-row>
                <b-row style="margin-left:2px;font-size:14px; line-height: 16px; font-weight: bold; text-transform: uppercase;">Cop, Good</b-row>
                <b-row style="margin-left:0px;font-size:9px;">
                    <b-badge variant="info" >S</b-badge>
                    <b-badge variant="white" >M</b-badge>
                    <b-badge variant="info">T</b-badge>
                    <b-badge variant="info">W</b-badge>
                    <b-badge variant="white">T</b-badge>
                    <b-badge variant="danger"><b-icon font-scale="1.5" icon="x"/></b-badge>
                    <b-badge variant="info"><b-icon class="m-0 p-0" font-scale="2" icon="x"/></b-badge>
                </b-row>                              
                <!-- <b class="h7 m-0 p-0"> {{shiftInfo.assignee}} </b> -->
        </b-card>
       
    </div>
</template>

<script lang="ts">
    import { Component, Vue, Prop } from 'vue-property-decorator';

    @Component
    export default class TeamMemberCard extends Vue {

        @Prop({required: true})
        badgeNumber!: number;

    }
</script>

<style scoped>   

    .card {
        border:2px solid rgb(124, 136, 136);
    }

    .badge {
        width: .75rem;
        margin:0.5rem .04rem 0 .04rem;
        padding: .0125rem 0 0 0; 
        border: solid 1px rgb(53, 56, 53); 
        border-radius: 4px;
        background-image: linear-gradient(to bottom right, rgb(95, 174, 211), rgb(240, 240, 229));
    }

</style>