import {} from '../common';

export interface placeHolderInfoType {

    "ID": string,
    "Role": string,
    "First Name": string,
    "Last Name": string,
    "Name": string,
    "Index": number
}

export interface assignmentTypeInfoType {
    code: string;
    concurrencyToken?: number;
    id: number;
    locationId: number;
    type: string;
    sortOrder: number;
}

