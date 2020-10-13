export interface locationInfoType {
    name: string;
    id: number;
    regionId: number|null;
}

export interface userInfoType {
    "roles": string[],
    "homeLocationId": number
}

export interface commonInfoType {
    "sheriffRankList": string[]    
}
