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

    myteam:sheriffAvailabilityInfoType;
    Sun: shiftInfoType | {};
    Mon: shiftInfoType | {};
    Tue: shiftInfoType | {};
    Wed: shiftInfoType | {};
    Thu: shiftInfoType | {};
    Fri: shiftInfoType | {};
    Sat: shiftInfoType | {};    
}

export interface shiftRangeInfoType {
    startDate: string;
    endDate: string
}

export interface shiftSubTypeInfoType {
    code: string;
    id: number;
}

export interface sheriffAvailabilityInfoType {
    sheriffId: string;
    conflicts: conflictsInfoType[];
    firstName: string;
    lastName: string;
    badgeNumber: string;
    rank: string;
}

export interface conflictsInfoType {
    dayOffset:number, 
    date:string, 
    startTime:string,
    endTime:string,
    startInMinutes:number,
    timeDuration: number, 
    type: string, 
    fullday: boolean   
}

export interface dayOptionsInfoType {
    name:string,
    diff: number,
    fullday: boolean, 
    conflicts: {
        Training: conflictsInfoType[],
        Leave: conflictsInfoType[],
        Loaned: conflictsInfoType[],
        Shift: conflictsInfoType[]
    }
}