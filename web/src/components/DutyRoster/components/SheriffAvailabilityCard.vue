<template>
    <div class="gridsheriff">
        <div v-for="i in 96" :key="i" :style="{backgroundColor: '#F1FEF1', gridColumnStart: i,gridColumnEnd:(i+1), gridRow:'1/2'}"></div>
        <div 
            v-for="(block,index) in sheriffInfo.availabilityDetail"
            :key="index+2000"
            :style="{gridColumnStart: (97-block.startBin),gridColumnEnd:(97-block.endBin), gridRow:'1/1',  backgroundColor: block.color, fontSize:'9px', textAlign: 'center', margin:0, padding:0 }"
            v-b-tooltip.hover.right                             
            :title="block.name">
                <div style="text-transform: capitalize; margin:0 padding: 0;">
                    {{block.name|truncateleft(block.endTime - block.startTime-1)}}
                </div>                
        </div>
        <div 
            v-for="(block,index) in sheriffInfo.sheriff.dutiesDetail"
            :key="index+1000"
            :style="{gridColumnStart: (97-block.startBin),gridColumnEnd:(97-block.endBin), gridRow:'1/1',  backgroundColor: block.color, fontSize:'9px', textAlign: 'center', margin:0, padding:0, color:'white' }"
            v-b-tooltip.hover.right                             
            :title="block.name +'-'+ block.code">
                <div style="text-transform: capitalize; margin:0 padding: 0;">
                    {{getBlockTitle(block.name,block.code,block.endTime - block.startTime-1)}}
                </div>                
        </div>
    </div>
</template>

<script lang="ts">
    import { Component, Vue, Prop } from 'vue-property-decorator';
    import { myTeamShiftInfoType} from '../../../types/DutyRoster';
   
    import * as _ from 'underscore';
    import moment from 'moment-timezone';

    @Component
    export default class SheriffAvailabilityCard extends Vue {

        @Prop({required: true})
        sheriffInfo!: myTeamShiftInfoType;

        public getBlockTitle(name,code,len){
           return Vue.filter('truncateleft')( name+'-'+code,len)
        }

    }
</script>

<style scoped>

    .gridsheriff {        
        display:grid;
        grid-template-columns: repeat(96, 1.041666%);
        grid-template-rows: repeat(1,.9rem);
        inline-size: 100%; 
        background-color: #fcf5f5; 
        height: 1rem; 
        margin: 0; 
        padding: 0;
        column-gap: 0;
        row-gap: 0;
           
    }

    .gridsheriff > * { 
        padding: 0px 0;
        border: 1px dotted rgb(202, 202, 202);
    }

</style>