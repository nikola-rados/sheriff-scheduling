<template> 
    <b-card v-if="displayLoanedIn || displayLoanedOut" no-body class="bg-dark text-white">
        <b-row class="ml-4"> 
            <b-icon-box-arrow-left class="mr-2" v-if="displayLoanedOut" :id="'loanedOutIcon'+index" font-scale="1.5"></b-icon-box-arrow-left>
                <b-tooltip :target="'loanedOutIcon'+index" variant="warning" show.sync ="true" triggers="hover">
                    <h2 class="text-danger">On loan to:</h2>                
                    <b-table  
                        :items="userLoanedOutInfo"
                        :fields="userAwayLocationFields"                
                        borderless
                        striped
                        small 
                        responsive="sm"
                        class="my-0 py-0"
                        >
                    </b-table>                                        
                </b-tooltip>
            <b-icon-box-arrow-in-right class="mx-2" v-if="displayLoanedIn" :id="'loanedInIcon'+index" font-scale="1.5"></b-icon-box-arrow-in-right>
                <b-tooltip :target="'loanedInIcon'+index" variant="warning" show.sync ="true" triggers="hover">
                    <h2 class="text-danger">Loaned in from:</h2>                
                    <b-table  
                        :items="userLoanedInInfo"
                        :fields="userAwayLocationFields"                
                        borderless
                        striped
                        small 
                        responsive="sm"
                        class="my-0 py-0"
                        >
                    </b-table>                                        
                </b-tooltip>
        </b-row>
    </b-card>   
</template>

<script lang="ts">
    import { Component, Vue, Prop } from 'vue-property-decorator';
    import moment from 'moment-timezone';
    import { namespace } from 'vuex-class';
    import {loanedLocationInfoType} from '../../../types/MyTeam';
    import {awayLocationsJsontype} from '../../../types/MyTeam/jsonTypes';
    import {locationInfoType} from '../../../types/common';
    import "@store/modules/CommonInformation";  
    const commonState = namespace("CommonInformation");

    @Component
    export default class UserLocationSummary extends Vue {       

        @Prop({required: true})
        index!: number;

        @Prop({required: true})
        homeLocation!: string;

        @Prop({required: true})
        loanedJson!: awayLocationsJsontype[]; 

        @commonState.State
        public token!: string;  
        
        @commonState.State
        public location!: locationInfoType;
        
        userLoanedInInfo: loanedLocationInfoType[] = [];
        userLoanedOutInfo: loanedLocationInfoType[] = [];
        displayLoanedIn = false;
        displayLoanedOut = false;
        userAwayLocationFields = [
          { key: 'locationName', label: 'Location', thClass: 'text-primary h3', tdClass: 'font-weight-bold'},
          { key: 'startDate', label: 'Start', thClass: 'text-primary h3'},
          { key: 'endDate', label: 'End', thClass: 'text-primary h3'}
        ];

        mounted()
        {  
            this.extractLoanedInfo();
        }

        public extractLoanedInfo()
        {
            this.displayLoanedOut = false;
            this.displayLoanedIn = false;

            if (this.loanedJson.length > 0 ) {
                
                this.userLoanedOutInfo = [];
                this.userLoanedInInfo = [];          
                for(const loanedInfoJson of this.loanedJson)
                {
                    const loanedInfo = {} as loanedLocationInfoType;
                    loanedInfo.locationId = loanedInfoJson.locationId;

                    const startDate = moment(loanedInfoJson.startDate).tz("UTC").format();
                    const endDate = moment(loanedInfoJson.endDate).tz("UTC").format();
                    if(this.isDateFullday(startDate, endDate))                    {
                        loanedInfo.startDate = Vue.filter('beautify-date')(startDate);
                        loanedInfo.endDate = Vue.filter('beautify-date')(endDate);
                    }
                    else{
                        loanedInfo.startDate = Vue.filter('beautify-date-time')(startDate);
                        loanedInfo.endDate = Vue.filter('beautify-date-time')(endDate);
                    }

                    if(loanedInfo.locationId == this.location.id)
                    {
                        loanedInfo.locationName = this.homeLocation;
                        this.userLoanedInInfo.push(loanedInfo);
                    }                        
                    else
                    {
                        loanedInfo.locationName = loanedInfoJson.location.name;
                        this.userLoanedOutInfo.push(loanedInfo);
                    }
                        
                }
                if(this.userLoanedOutInfo.length) this.displayLoanedOut = true;
                if(this.userLoanedInInfo.length) this.displayLoanedIn = true;
                 
            } else {
                this.displayLoanedOut = false;
                this.displayLoanedIn = false;
            }
        }

        public isDateFullday(startDate, endDate){
            const start = moment(startDate); 
            const end = moment(endDate);
            const duration = moment.duration(end.diff(start));
            if(duration.asMinutes() < 1440 && duration.asMinutes()> -1440 )  return false;  else return true;
        }
    }
</script>

 <style scoped>

    .tooltip >>> .tooltip-inner{
        max-width: 500px !important;
        width: 400px !important;
    }

</style>