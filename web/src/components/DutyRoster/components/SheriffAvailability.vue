<template>
    <div v-if="availabilityInfo.length>0 && isMounted" id="shiftBox" ref="shiftBox" :key="updateBoxes">             
        <b-card
            v-for="block in availabilityBlocks"
            :key="block.key"
            :id="'availCard'+block.key"
            :style=" 'background-color:'+block.color+'; float:left; position: relative; left:'+ block.startTime +'%; width:' + block.timeDuration+'%; height:4rem;'"             
            no-body>
                <span v-if="blockSize(block)>30" @mousedown="cardSelected(block)"> 
                    <h6 :style="'background-color:'+block.headerColor+'; color:white; text-align: center; font-size:10px; line-height: 16px;'"
                        v-b-tooltip.hover                                
                        :title="block.title + ' ' + block.timeStamp">
                        <font-awesome-icon v-if="block.title.includes('Loaned')" style="transform: rotate(180deg); font-size: .55rem;"  icon="sign-out-alt" /> 
                        <font-awesome-icon v-if="block.title=='Leave'" style="font-size: .55rem;"  icon="suitcase"/> 
                        <font-awesome-icon v-if="block.title=='Training'" style="font-size: .5rem;" icon="graduation-cap"/> 
                        <b-icon-person-fill v-if="block.title=='Shift'"/>
                        <b-icon-calendar2-x v-if="block.title=='Unavailable'"/>
                    </h6>
                    <span style="text-align: center;font-size:10px; line-height: 12px; display: block;">{{block.timeStamp}}</span>
                </span>
                <span v-else @mousedown="cardSelected(block)">
                    <h6 
                        v-b-tooltip.hover                                
                        :title="block.title + ' ' + block.timeStamp"
                        :style="'background-color:'+block.headerColor+'; color:white; text-align: center; font-size:6px; line-height: 16px;'">
                        ...
                    </h6>                    
                </span>
        </b-card>
    </div>
</template>

<script lang="ts">
    import { Component, Vue, Prop } from 'vue-property-decorator';
    import { namespace } from "vuex-class";    
    import * as _ from 'underscore';

    @Component
    export default class SheriffAvailability extends Vue {

        @Prop({required: true})
        sheriffAvailabilityInfo!: any;

        @Prop({required: true})
        sheriffId!: string;

        availabilityBlocks: any[] = [];

        blockDrop = false;
        updateBoxes =0;

        isMounted = false;

        mounted()
        {
            // console.log(this.scheduleInfo)
            this.isMounted = false;

            const sortedSheriffAvailabilityInfo = _.sortBy(this.sheriffAvailabilityInfo,'startInMinutes')
           
            this.availabilityBlocks = []
            let widthOtherElements =0;
            for(const availability of sortedSheriffAvailabilityInfo){

                    this.availabilityBlocks.push({
                        key: this.sheriffId+availability.date +'Z' +availability.startTime,
                        startTime: availability.startInMinutes *5 /72 -widthOtherElements,
                        timeDuration: availability.timeDuration * 5 /72,
                        timeStamp: availability.startTime +'-'+availability.endTime,
                        color: '#AAAAA'
                    })
                    widthOtherElements += (availability.timeDuration * 5 /72);
                    //console.log(this.scheduleBlocks)                            
            }
            this.isMounted = true; 
        }

        public blockSize(block){
            //console.log('availCard'+block.key)
            const el = document.getElementById('availCard'+block.key)
            //console.log(el)
            if(el){
                return el.clientWidth
            }else{
                return 0
            }            
        }

        created() {
            window.addEventListener("resize", this.onResize);
        }

        destroyed() {
            window.removeEventListener("resize", this.onResize);
        }

        public onResize() {          
            this.updateBoxes++;
        }

    }
</script>

<style scoped>   

    .card {
        border: white;
    }

</style>