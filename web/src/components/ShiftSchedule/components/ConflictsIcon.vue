<template> 
    <div v-if="displayConflicts"  class="bg-transparent text-danger" style="margin:0 .01rem 0 .01rem;padding:0 0 0 .1rem; width:.8rem; height:.8rem">
        
        <div  v-if="type=='Shift'">
             <b-icon-person-fill style="transform:translate(0,-6px); margin:0; padding:0; color:blue;" font-scale="0.65"  :id="'conflictIcon'+type+index"/>
        </div>
        <div  v-if="type=='Loaned'">
           <font-awesome-icon  style="transform:translate(0,-6px) rotate(180deg); font-size: .6rem;"  icon="sign-out-alt"   :id="'conflictIcon'+type+index" />
        </div>
        <div v-if="type=='Leave'">
            <font-awesome-icon style="transform:translate(0px,-6px); font-size: .55rem;"  icon="suitcase" :id="'conflictIcon'+type+index"></font-awesome-icon> 
        </div>
        <div v-if="type=='Training'">
            <font-awesome-icon style="transform:translate(0,-6px); font-size: .5rem;" icon="graduation-cap" :id="'conflictIcon'+type+index"></font-awesome-icon> 
        </div>
        <div v-if="type=='Unavailable'">
            <b-icon-calendar2-x style="transform:translate(0,-6px); margin:0; padding:0; color:blue;" font-scale="0.65"  :id="'conflictIcon'+type+index"/>
        </div>

        <b-tooltip :target="'conflictIcon'+type+index" variant="warning" show.sync ="true" triggers="hover">
            <h2 class="text-danger">{{type}}:</h2>                
            <b-table  
                :items="conflictsInfo"
                :fields="conflictFields"
                sort-by="startDate"                
                borderless
                striped
                small 
                responsive="sm"
                class="my-0 py-0"
                >
                
                <template v-slot:cell(date)="data" >
                    <span> {{data.value | beautify-date}}</span>
                </template>
                <template v-slot:cell(fullday)="data" >
                    <span v-if="data.value">Full Day</span>
                    <span v-else>Partial Day</span> 
                </template>
            </b-table>                                        
        </b-tooltip>
    </div>   
</template>

<script lang="ts">
    import { Component, Vue, Prop } from 'vue-property-decorator';
    import moment from 'moment-timezone';
    import { conflictsInfoType } from '../../../types/ShiftSchedule';

    @Component
    export default class ConflictsIcon extends Vue {        
        
        @Prop({required: true})
        index!: number;

        @Prop({required: true})
        type!: string;
        
        @Prop({required: true})
        conflictsInfo!: conflictsInfoType[];
        
        displayConflicts = false;
        conflictFields = [
            { key: 'fullday', label: 'Type', thClass: 'text-primary h4'},
            { key: 'date', label: 'Date', thClass: 'text-primary h4'},
            { key: 'startTime', label: 'From', thClass: 'text-primary h4'},
            { key: 'endTime', label: 'To', thClass: 'text-primary h4'}
        ];

        mounted()
        {            
           this.displayConflicts = false;
           if(this.conflictsInfo.length)this.displayConflicts = true;
        }

    }
</script>

 <style scoped>

    .tooltip >>> .tooltip-inner{
        max-width: 500px !important;
        width: 400px !important;
    } 

    td {
        margin: 0rem 0rem 0rem 0rem;
        padding: 0rem 0rem 0rem 0rem;
        
        background-color: white ;
    }

</style>