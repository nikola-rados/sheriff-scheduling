import {} from '../common';
import {} from '../DutyRoster/jsonTypes';

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


