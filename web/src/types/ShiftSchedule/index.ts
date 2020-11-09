import {} from '../common';
import {} from '../DutyRoster/jsonTypes/index';

export interface shiftInfoType {
    id: number;
    startDate: string;
    endDate: string;    
    timezone: string;
    locationId: string;     
    sheriffId: string;
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
    endDate: string;
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
    homeLocation: {id:number, name:string};
}

export interface conflictsInfoType {
    id?: string;
    dayOffset:number; 
    date:string; 
    startTime:string;
    endTime:string;
    startInMinutes:number;
    timeDuration: number; 
    type: string; 
    fullday: boolean;   
}

export interface scheduleBlockInfoType {
    id?: string;
    key: string;
    startTime:number;
    timeStamp: string;
    timeDuration: number; 
    title: string;
    color: string;
    originalColor: string; 
    headerColor: string;  
    selected: boolean;   
}

export interface dayOptionsInfoType {
    name:string;
    diff: number;
    fullday: boolean; 
    conflicts: {
        Training: conflictsInfoType[],
        Leave: conflictsInfoType[],
        Loaned: conflictsInfoType[],
        Shift: conflictsInfoType[],
        Unavailable: conflictsInfoType[]
    }
}