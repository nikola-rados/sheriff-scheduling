import {} from '../common';
import {} from '../DutyRoster/jsonTypes/index';

export interface shiftInfoType {
    id: number;
    startDate: string;
    endDate: string;    
    timezone: string;
    locationId: string;     
    sheriffId: string;
    comment?: string;
    overtimeHours: number;
}

export interface editedShiftInfoType {
    id: number;
    startDate: string;
    endDate: string;    
    timezone: string;
    locationId: number;     
    sheriffId: string;
    comment?: string;
}

export interface distributeTeamMemberInfoType {        
    sheriffId: string;
    name: string;
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
    location: string;
    dayOffset:number; 
    date:string; 
    startTime:string;
    endTime:string;
    startInMinutes:number;
    timeDuration: number; 
    type: string; 
    subType?: string;
    fullday: boolean;
    sheriffEventType?: string; 
    comment?: string;  
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
    type: string;
    subType?: string;
    comment?: string;
}

export interface dayOptionsInfoType {
    name:string;
    diff: number;
    fullday: boolean; 
    conflicts: {
        Training: conflictsInfoType[];
        Leave: conflictsInfoType[];
        Loaned: conflictsInfoType[];
        AllShifts:conflictsInfoType[]; 
        Shift: conflictsInfoType[];
        overTimeShift: conflictsInfoType[];
        Unavailable: conflictsInfoType[];
    }
}

export interface importConflictMessageType {
    ConflictFieldName: string
}

export interface weekScheduleInfoType {

    myteam:distributeScheduleInfoType;
    Sun: shiftInfoType | {};
    Mon: shiftInfoType | {};
    Tue: shiftInfoType | {};
    Wed: shiftInfoType | {};
    Thu: shiftInfoType | {};
    Fri: shiftInfoType | {};
    Sat: shiftInfoType | {};    
}

export interface distributeScheduleInfoType {
    sheriffId: string;
    conflicts: scheduleInfoType[];
    name: string;
    homeLocation: string;
}

export interface scheduleInfoType {
    id?: string;
    location: string;
    dayOffset:number; 
    date:string; 
    startTime:string;
    endTime:string;
    type: string;
    subType?: string;
    workSection: string; 
    workSectionColor: string;
}