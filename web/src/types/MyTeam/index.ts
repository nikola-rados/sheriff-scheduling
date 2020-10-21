import {leaveInfoType, locationInfoType, trainingInfoType} from '../common';
import {awayLocationsJsontype, leaveJsontype, trainingJsontype} from './jsonTypes';

export interface teamMemberInfoType {

    id?: string;
    idirUserName?: string;
    rank: string;
    firstName?: string;
    lastName?: string;
    email?: string;
    badgeNumber: string;
    gender: string;
    fullName?: string;
    image?: string | null;
    userRoles?: userRoleInfoType[];
    isEnabled?: boolean;
    homeLocationId?: number | null;
    homeLocationNm?: string | null;
    homeLocation?: locationInfoType;
    awayLocation?: awayLocationsJsontype[];
    training?: trainingJsontype[];
    leave?: leaveJsontype[];
    loanedOut?: awayLocationsJsontype[];
}

export interface userRoleInfoType{
    role: {
        id: number;
        name: string;
        description: string;
    };
    effectiveDate: string;
    expiryDate: string;
  }

export interface roleOptionInfoType{
    text: string;
    desc: string;
    value: string;
    effDate: string;
    expDate: string;
}

export interface permissionInfoType {
    id?: string;    
    name?: string;
    description?: string;
}

export interface roleInfoType {
    id?: string;    
    name?: string;
    description?: string;
    expiryDate?: string;
    permissions?: string[];
}

export interface permissionOptionInfoType{
    text: string;
    desc: string;
    value: string;
    selected: boolean;
}

export interface loanedLocationInfoType{
    id: number|null;    
    locationId: number|null;
    locationName: string;
    isFullDay: boolean;
    startDate: string;
    endDate: string;
    sheriffId: string      
}

export interface awayLocationInfoType{
    id: number|null;    
    locationId: number|null;
    startDate: string;
    endDate: string;
    sheriffId: string,
    concurrencyToken?: number      
}

export interface userTrainingInfoType{ 
    id: number;   
    trainingType: trainingInfoType;
    trainingTypeId?:number|null;
    trainingName?: string;
    sheriffId?: string;
    expiryDate: string;
    isFullDay?: boolean;
    startDate: string;
    endDate: string;
    comment?: string;    
}

export interface userLeaveInfoType{
    id: number;    
    leaveTypeId: number|null;
    leaveName?: string;
    leaveType?: leaveInfoType;
    comment: string,
    isFullDay: boolean;
    startDate: string;
    endDate: string;    
}