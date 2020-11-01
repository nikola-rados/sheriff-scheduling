<template>
    <b-card bg-variant="white" class="home" no-body>    
        <b-row class="m-0 p-0" cols="2" >            
            <b-col class="pl-1 " cols="1" style="overflow: auto;">
                <team-member-card v-for="member in teamMembers" :key="member.badgeNumber" :badgeNumber="member.badgeNumber"/>
            </b-col>
            <b-col class="m-0 p-0" cols="11" style="overflow: auto;  background-color: yellow;">

                <schedule-header />

                <b-table
                    :items="shiftSchedules" 
                    :fields="fields"
                    head-row-variant="primary"   
                    bordered
                    fixed>
                        <template v-slot:cell(Sun) = "data" >  
                            <ScheduleCard :shiftInfo="data.item.Sun"/>
                        </template>

                        <template v-slot:cell(Mon) = "data" >  
                            <ScheduleCard :shiftInfo="data.item.Mon"/>
                        </template>

                       

                </b-table>

                <b-card><br></b-card>  
            </b-col>
        </b-row>
        
    </b-card>
</template>

<script lang="ts">
    import { Component, Vue } from 'vue-property-decorator';
    import { weekShiftInfoType } from '../../types/ShiftSchedule/index'
    import ScheduleCard from './components/ScheduleCard.vue'
    import TeamMemberCard from './components/TeamMemberCard.vue'
    import ScheduleHeader from './components/ScheduleHeader.vue'

    @Component({
        components: {
            ScheduleCard,
            TeamMemberCard,
            ScheduleHeader
        }
    })
    export default class ManageSchedule extends Vue {

        isDataReady = false;

        teamMembers = [
            {badgeNumber:1231},
            {badgeNumber:4561},
            {badgeNumber:7891},
            {badgeNumber:9991},
        ]

        fields=[
            {key:'Sun', label:'', tdClass:'px-0 mx-0'},
            {key:'Mon', label:'', tdClass:'px-0 mx-0'},
            {key:'Tue', label:'', tdClass:'px-0 mx-0'},
            {key:'Wed', label:'', tdClass:'px-0 mx-0'},
            {key:'Thu', label:'', tdClass:'px-0 mx-0'},
            {key:'Fri', label:'', tdClass:'px-0 mx-0'},
            {key:'Sat', label:'', tdClass:'px-0 mx-0'}
        ]

        shiftSchedules: weekShiftInfoType[] =[
            {
                Sun:{date:'2020-1-1', startTime:60 ,timeDuration: 20, type: 'Court', subType:'', color: 'info', timeStamp: '08:00 - 16:00', assignee:''},
                Mon:{},
                Tue:{},
                Wed:{},
                Thu:{},
                Fri:{},
                Sat:{},
            },           
        ]

        mounted()
        {
            const currentdate = this.startOfWeek(new Date());
            this.fields[0].label = this.fields[0].key +' ' + Vue.filter('beautify-date')(currentdate.toISOString());
        }

        public startOfWeek(date){
            const diff = date.getDate() - date.getDay();        
            return new Date(date.setDate(diff));
        }

    }
</script>

<style scoped>   

    .card {
        border: white;
    }

</style>