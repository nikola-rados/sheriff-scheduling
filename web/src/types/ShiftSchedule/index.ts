import {} from '../common';
import {} from '../DutyRoster/jsonTypes/index';

export interface shiftInfoType {
    id: number,
    startDate: string,
    endDate: string,    
    type: string,
    subType: string,     
    sheriffId: string,
}

export interface weekShiftInfoType {

    Sun: shiftInfoType | {};
    Mon: shiftInfoType | {};
    Tue: shiftInfoType | {};
    Wed: shiftInfoType | {};
    Thu: shiftInfoType | {};
    Fri: shiftInfoType | {};
    Sat: shiftInfoType | {};    
}



