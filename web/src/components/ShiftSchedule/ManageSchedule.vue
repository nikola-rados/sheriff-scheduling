<template>
    <b-card bg-variant="white" class="home" no-body>    
        <b-row cols="1" >            
            <b-col md="1" cols="2" style="overflow: auto;">
                <schedule-side-panel v-if="isDataReady"/> 
            </b-col>
            <b-col col md="11" cols="11" style="overflow: auto;">

                <schedule-header v-if="isDataReady"/>

                <b-table
                    :items="shiftSchedules" 
                    :fields="fields"   
                    bordered
                    fixed>

                        <template v-slot:head(dateSA) = "data" >  
                            {{data}}
                        </template>

                        <template v-slot:cell(dateSU) = "data" >  
                            <ScheduleCard :startTime="data.item.timeStartSU" :timeDuration="data.item.timeDurationSU" color="info"/>
                        </template>

                        <template v-slot:cell(dateMO) = "data" >  
                            <ScheduleCard :startTime="data.item.timeStartMO" :timeDuration="data.item.timeDurationMO" color="warning"/>
                        </template>

                </b-table>

                <b-card><br></b-card>  
            </b-col>
        </b-row>
        
    </b-card>
</template>

<script lang="ts">
    import { Component, Vue } from 'vue-property-decorator';
    import { namespace } from 'vuex-class'
    import ScheduleCard from './components/ScheduleCard.vue'

    @Component({
        components: {
            ScheduleCard
        }
    })
    export default class ManageSchedule extends Vue {

        isDataReady = false;
        fields=[
            {key:'dateSU', tdClass:'px-0 mx-0'},
            {key:'dateMO', tdClass:'px-0 mx-0'},
            {key:'dateTU', tdClass:'px-0 mx-0'},
            {key:'dateWE', tdClass:'px-0 mx-0'},
            {key:'dateTH', tdClass:'px-0 mx-0'},
            {key:'dateFR', tdClass:'px-0 mx-0'},
            {key:'dateSA', tdClass:'px-0 mx-0'}
        ]

        shiftSchedules =[
            {dateSU:'2020-1-1', timeStartSU:60 ,timeDurationSU: 33,
            dateMO:'2020-1-1', timeStartMO:5 ,timeDurationMO: 25,},
            {dateSU:'2020-1-1', timeStartSU:10 ,timeDurationSU: 33,
            dateMO:'2020-1-1', timeStartMO:5 ,timeDurationMO: 25,},
        ]

    }
</script>

<style scoped>   

    .card {
        border: white;
    }

</style>