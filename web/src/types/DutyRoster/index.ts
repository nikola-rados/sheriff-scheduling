import {} from '../common';
import { locationJsonType } from '../common/jsonTypes';
import { lookupCodeJsonType } from '../ManageTypes/jsonTypes';
import { shiftInfoType } from '../ShiftSchedule';
import {} from './jsonTypes';

export interface dutyRangeInfoType {
    startDate: string;
    endDate: string;
}

export interface myTeamShiftInfoType {
    sheriffId: string;
    shifts: shiftInfoType[];
    badgeNumber: number;
    firstName: string;
    lastName: string;
    rank: string;
    availability: number[];
    duties: number[];
    dutiesDetail: dutiesDetailInfoType[];
}

export interface dutiesDetailInfoType{
    id: number ; 
    startBin: number; 
    endBin: number;
    name: string;
    colorCode: string;
    color: string;
    type: string;
    code: string;
    startTime?: string; 
    endTime?: string;
}

export interface assignmentInfoType {
    id?: number;
    name: string;
    adhocStartDate: string | null;
    adhocEndDate: string | null;
    reoccuring: boolean;
    start: string;
    end: string;
    lookupCodeId: number;
    locationId: number;
    timezone: string;    
    type: string;
    subType: string;
    monday: boolean;
    tuesday: boolean;
    wednesday: boolean;
    thursday: boolean;
    friday: boolean;
    saturday: boolean;
    sunday: boolean;
    comment?: string;
}

export interface assignmentSubTypeInfoType {
    code: string;
    id: number;
}

export interface assignmentCardInfoType {
    FTEnumber: number;
    assignment: string;
    assignmentDetail: assignmentDetailInfoType;
    attachedDuty: attachedDutyInfoType | null;
    code: string;
    name: string;
    totalFTE: number;
    type: assignmentCardTypeInfoType;    
}

export interface assignmentCardWeekInfoType {
    FTEnumber: number;
    assignment: string;
    assignmentDetail: assignmentDetailInfoType;
    0: attachedDutyInfoType | null;
    1: attachedDutyInfoType | null;
    2: attachedDutyInfoType | null;
    3: attachedDutyInfoType | null;
    4: attachedDutyInfoType | null;
    5: attachedDutyInfoType | null;
    6: attachedDutyInfoType | null;
    code: string;
    name: string;
    totalFTE: number;
    type: assignmentCardTypeInfoType;    
}

export interface assignmentDetailInfoType {
    id: number;
    lookupCodeId: number;
    lookupCode: lookupCodeJsonType;
    location: locationJsonType;
    locationId: number;
    name: string;
    start: string;
    end: string;
    expiryDate: string | null;
    expiryReason: string | null;
    timezone: string;
    monday: boolean;
    tuesday: boolean;
    wednesday: boolean;
    thursday: boolean;
    friday: boolean;
    saturday: boolean;
    sunday: boolean;
    adhocStartDate: string | null;
    adhocEndDate: string | null;
    comment?: string;
}

export interface attachedDutyInfoType {
    assignment: assignmentDetailInfoType;
    assignmentId: number;
    concurrencyToken: number;
    dutySlots: any;
    endDate: string;
    id: number;
    location: locationJsonType;
    locationId: number;
    startDate: string;
    timezone: string;
    comment?: string;
}

export interface assignmentCardTypeInfoType {
    colorCode: string;
    name: string;
}

export interface assignDutyInfoType {
    id: number;
    startDate: string;
    endDate: string;
    locationId: number;
    assignmentId: number;
    dutySlots: dutySlotInfoType[];
    timezone: string;
    comment?: string;
}

export interface dutySlotInfoType {
    id: number|null;                        
    startDate: string;
    endDate: string;
    dutyId: number;
    sheriffId: string|null;
    shiftId: number|null;
    timezone: string;
    isNotRequired: boolean;
    isNotAvailable: boolean;
    isClosed: boolean;
    isOvertime: boolean;
}

export interface dutyBlockInfoType {
    color: string;
    endTime: number;
    endTimeString: string;
    height: string;
    id: string;
    firstName: string;
    lastName: string;
    sheriffId: string;
    startTime: number;
    startTimeString: string;
    title: string;
    timezone: string;
    shiftId: number|null;
    dutySlotId: number|null;
    note: string;
}

export interface viewDutyInfoType {       
    id: string;
    firstName: string;
    lastName: string;
    rank: string;
    rankOrder: number;
    displayName: string;
    sheriffId: string;    
    startTime: string;
    endTime: string;
    assignment: string;       
    note: string;
}

export interface dutyBlockWeekInfoType {
    color: string;
    endTime: number;
    endTimeString: string;
    height: string;
    id: string;
    firstName: string;
    lastName: string;
    sheriffId: string;
    startTime: number;
    startTimeString: string;
    title: string;
    timezone: string;
    shiftId: number|null;
    dutySlotId: number|null;
    note: string;
    day:number;
    dutyId:number;
    dutyDate:string;
    fullDutyStartTime: string;
    fullDutyEndTime: string;
    borderLeft: string;
    borderRight: string;
    comment: string;
}

export interface assignDutySlotsInfoType{
    startDate: string;
    endDate: string;
    isOvertime:boolean;
    dutySlotId: number|null;    
}