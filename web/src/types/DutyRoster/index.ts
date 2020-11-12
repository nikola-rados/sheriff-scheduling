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

