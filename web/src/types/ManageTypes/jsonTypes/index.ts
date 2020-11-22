import { sortOrderForLocationJsonType } from '@/types/common/jsonTypes';
import {} from '../../common';

export interface lookupCodeJsonType {
    code: string;
    concurrencyToken: number;
    id: number;
    locationId: number;
    sortOrderForLocation: sortOrderForLocationJsonType;
    type: string;  
}