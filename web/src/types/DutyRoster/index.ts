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

export interface assignmentDetailInfoType {
    id: number;
    lookupCodeId: number;
    lookupCode: lookupCodeJsonType;
    location: locationJsonType;
    locationId: number;
    name: string;
    start: string;
    end: string;
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
}

export interface assignmentCardTypeInfoType {
    colorCode: string;
    name: string;
}

