export interface locationInfoType {
    name: string;
    id: number;
    regionId: number| null;
    agencyId?: string;
    concurrencyToken?: number;
    justinCode?: string;
    timezone: string;
}

export interface leaveInfoType {
    code: string;
    id: number;
    description?: string;
}

export interface trainingInfoType {
    code: string;
    id: number;
    description?: string;
}

export interface userInfoType {
    firstName: string;
    lastName: string;
    roles: string[];
    homeLocationId: number;
    permissions: string[];
}

export interface commonInfoType {
    sheriffRankList: sheriffRankInfoType[]    
}

export interface sheriffRankInfoType {
    id: number;
    name: string;
}

export interface localTimeInfoType {
    timeString: string;
    timeSlot: number;
    dayOfWeek: number;
    isTodayInView: boolean;
}
