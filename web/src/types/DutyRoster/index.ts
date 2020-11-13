import {} from '../common';
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

