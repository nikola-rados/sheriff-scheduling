export interface locationInfoType {
    "name": string,
    "id": string
}

export interface commonInfoType {
    "token": string,
    "location": locationInfoType,
    "sheriffRankList": string[]
}
