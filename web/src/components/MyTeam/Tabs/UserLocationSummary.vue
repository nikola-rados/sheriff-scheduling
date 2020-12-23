<template> 
    <b-card v-if="displayLoanedIn || displayLoanedOut" no-body class="bg-dark text-white">
        <b-row> 
            <b-icon-box-arrow-left class="mx-2" v-if="displayLoanedOut" :id="'loanedOutIcon'+index" font-scale="1.5"></b-icon-box-arrow-left>
                <b-tooltip v-if="displayLoanedOut" :target="'loanedOutIcon'+index" variant="warning" show.sync ="true" triggers="hover">
                    <h2 class="text-danger">On loan to:</h2>                
                    <b-table  
                        :items="userLoanedOutInfo"
                        :fields="userAwayLocationFields"
                        sort-by="startDate"                
                        borderless
                        striped
                        small 
                        responsive="sm"
                        class="my-0 py-0"
                        >
                        <template v-slot:cell(startDate)="data" >
                            <span v-if="data.item.isFullDay">{{data.value | beautify-date}}</span>
                            <span v-else>{{data.value | beautify-date-time}}</span> 
                        </template>
                        <template v-slot:cell(endDate)="data" >
                            <span v-if="data.item.isFullDay">{{data.value | beautify-date}}</span>
                            <span v-else>{{data.value | beautify-date-time}}</span> 
                        </template>
                    </b-table>                                        
                </b-tooltip>
            <b-icon-box-arrow-in-right class="mx-2" v-if="displayLoanedIn" :id="'loanedInIcon'+index" font-scale="1.5"></b-icon-box-arrow-in-right>
                <b-tooltip v-if="displayLoanedIn" :target="'loanedInIcon'+index" variant="warning" show.sync ="true" triggers="hover">
                    <h2 class="text-danger">Loaned in from:</h2>                
                    <b-table  
                        :items="userLoanedInInfo"
                        :fields="userAwayLocationFields"  
                        sort-by="startDate"              
                        borderless
                        striped
                        small 
                        responsive="sm"
                        class="my-0 py-0"
                        >
                        <template v-slot:cell(startDate)="data" >
                            <span v-if="data.item.isFullDay">{{data.value | beautify-date}}</span>
                            <span v-else>{{data.value | beautify-date-time}}</span> 
                        </template>
                        <template v-slot:cell(endDate)="data" >
                            <span v-if="data.item.isFullDay">{{data.value | beautify-date}}</span>
                            <span v-else>{{data.value | beautify-date-time}}</span> 
                        </template>
                    </b-table>                                        
                </b-tooltip>
        </b-row>
    </b-card>   
</template>

<script lang="ts">
    import { Component, Vue, Prop } from 'vue-property-decorator';
    import moment from 'moment-timezone';
    import {loanedLocationInfoType} from '../../../types/MyTeam';
    import {awayLocationsJsontype} from '../../../types/MyTeam/jsonTypes';
    import {locationInfoType} from '../../../types/common';
    import { namespace } from 'vuex-class';
    import "@store/modules/CommonInformation";
    const commonState = namespace("CommonInformation");

    @Component
    export default class UserLocationSummary extends Vue {       

        @Prop({required: true})
        index!: number;

        @Prop({required: true})
        homeLocation!: string;

        @Prop({required: true})
        awayJson!: awayLocationsJsontype[];  
        
        @commonState.State
        public location!: locationInfoType;
        
        userLoanedInInfo: loanedLocationInfoType[] = [];
        userLoanedOutInfo: loanedLocationInfoType[] = [];
        displayLoanedIn = false;
        displayLoanedOut = false;
        userAwayLocationFields = [
          { key: 'locationName', label: 'Location', thClass: 'text-primary h3', tdClass: 'font-weight-bold'},
          { key: 'startDate', label: 'Start', thClass: 'text-primary h3'},
          { key: 'endDate', label: 'End', thClass: 'text-primary h3'},
          { key: 'comment', label: 'Comment', thClass: 'text-primary h3', tdClass:'text-success'},
        ];

        mounted()
        {  
            this.extractLoanedInfo();
        }

        public extractLoanedInfo()
        {
            this.displayLoanedOut = false;
            this.displayLoanedIn = false;

            if (this.awayJson.length > 0 ) {
                
                this.userLoanedOutInfo = [];
                this.userLoanedInInfo = [];          
                for(const loanedInfoJson of this.awayJson)
                {
                    const loanedInfo = {} as loanedLocationInfoType;
                    loanedInfo.locationId = loanedInfoJson.locationId;
                    const timezone = loanedInfoJson.location.timezone;

                    loanedInfo.startDate = moment(loanedInfoJson.startDate).tz(timezone).format();
                    loanedInfo.endDate = moment(loanedInfoJson.endDate).tz(timezone).format();
                    loanedInfo.isFullDay = Vue.filter('isDateFullday')(loanedInfo.startDate, loanedInfo.endDate);                    
                    loanedInfo.comment = loanedInfoJson.comment? loanedInfoJson.comment : '';
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

    }
</script>

 <style scoped>

    .tooltip >>> .tooltip-inner{
        max-width: 700px !important;
        width: 600px !important;
    }

</style>