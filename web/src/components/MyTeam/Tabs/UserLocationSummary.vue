<template> 
    <b-card v-if="displayLoanedIn || displayLoanedOut" no-body class="bg-dark text-white"> 
        <b-icon-box-arrow-left v-if="displayLoanedOut" :id="'loanedOutIcon'+index" font-scale="1.5"></b-icon-box-arrow-left>
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
        <b-icon-box-arrow-right v-if="displayLoanedIn" :id="'loanedInIcon'+index" font-scale="1.5"></b-icon-box-arrow-right>
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
    </b-card>   
</template>

<script lang="ts">
    import { Component, Vue, Prop } from 'vue-property-decorator';
    import { namespace } from 'vuex-class';
    import {loanedLocationInfoType} from '../../../types/MyTeam';
    import {awayLocationsJsontype} from '../../../types/MyTeam/jsonTypes';
    import "@store/modules/CommonInformation";  
    const commonState = namespace("CommonInformation");

    @Component
    export default class UserLocationSummary extends Vue {       

        @Prop({required: true})
        index!: number;

        @Prop({required: true})
        loanedInJson!: awayLocationsJsontype[];
        
        @Prop({required: true})
        loanedOutJson!: awayLocationsJsontype[]; 

        @commonState.State
        public token!: string;        
        
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
            if (this.loanedInJson.length > 0 ) {
                
                this.userLoanedInInfo = [];          
                for(const loanedInInfoJson of this.loanedInJson)
                {
                    const loanedInInfo = {} as loanedLocationInfoType;
                    loanedInInfo.locationId = loanedInInfoJson.locationId;
                    loanedInInfo.locationName = loanedInInfoJson.location.name; 
                    loanedInInfo.isFullDay = loanedInInfoJson.isFullDay;
                    if (loanedInInfoJson.isFullDay) {
                        loanedInInfo.startDate = Vue.filter('beautify-date')(loanedInInfoJson.startDate);
                        loanedInInfo.endDate = Vue.filter('beautify-date')(loanedInInfoJson.endDate);
                    } else {
                        loanedInInfo.startDate = Vue.filter('beautify-date-time')(loanedInInfoJson.startDate);
                        loanedInInfo.endDate = Vue.filter('beautify-date-time')(loanedInInfoJson.endDate);
                    }
                    this.userLoanedInInfo.push(loanedInInfo);
                }
                this.displayLoanedIn = true;
                 
            } else {
                this.displayLoanedIn = false;
            }

            if (this.loanedOutJson.length > 0 ) {
                
                this.userLoanedOutInfo = [];          
                for(const loanedOutInfoJson of this.loanedOutJson)
                {
                    const loanedOutInfo = {} as loanedLocationInfoType;
                    loanedOutInfo.locationId = loanedOutInfoJson.locationId;
                    loanedOutInfo.locationName = loanedOutInfoJson.location.name; 
                    loanedOutInfo.isFullDay = loanedOutInfoJson.isFullDay;
                    if (loanedOutInfoJson.isFullDay) {
                        loanedOutInfo.startDate = Vue.filter('beautify-date')(loanedOutInfoJson.startDate);
                        loanedOutInfo.endDate = Vue.filter('beautify-date')(loanedOutInfoJson.endDate);
                    } else {
                        loanedOutInfo.startDate = Vue.filter('beautify-date-time')(loanedOutInfoJson.startDate);
                        loanedOutInfo.endDate = Vue.filter('beautify-date-time')(loanedOutInfoJson.endDate);
                    }
                    this.userLoanedOutInfo.push(loanedOutInfo);
                }
                this.displayLoanedOut = true;
                 
            } else {
                this.displayLoanedOut = false;
            }
        }

        


    }
</script>

 <style scoped>

    .tooltip >>> .tooltip-inner{
        max-width: 500px !important;
        width: 400px !important;
    }

</style>