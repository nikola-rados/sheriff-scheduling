export interface sheriffRankJsonType {
    id: string,
    type: string,
    description: string 
}

export interface locationJsonType {
    id: number;
    agencyId: string;
    name: string;
    regionId: number|null;
    concurrencyToken: number;
}

export interface leaveTypeJson {
    id: number,
    type: string,
    code: string,
    description: string,
    concurrencyToken: number
}

export interface trainingTypeJson {
    id: number,
    type: string,
    code: string,
    description: string,
    concurrencyToken: number
}