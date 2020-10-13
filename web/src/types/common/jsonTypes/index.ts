export interface sheriffRankJsonType {
    "id": string,
    "type": string,
    "description": string 
}

export interface locationJsonType {
    id: number;
    agencyId: string;
    name: string;
    regionId: number|null;
    concurrencyToken: number;
}