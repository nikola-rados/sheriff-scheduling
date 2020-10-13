export interface locationInfoType {
    name: string;
    id: number;
    regionId: number| null;
    agencyId?: string;
    concurrencyToken?: number;
    justinCode?: string
}

export interface userInfoType {
    "roles": string[],
    "homeLocationId": number
}

export interface commonInfoType {
    "sheriffRankList": string[]    
}
