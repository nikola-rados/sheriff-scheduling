import { sortOrderForLocationJsonType } from '@/types/common/jsonTypes';

export interface lookupCodeJsonType {
    code: string;
    concurrencyToken: number;
    id: number;
    locationId: number;
    sortOrderForLocation: sortOrderForLocationJsonType;
    type: string;  
}