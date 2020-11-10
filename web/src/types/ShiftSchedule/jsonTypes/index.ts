import { sortOrderForLocationJsonType } from '@/types/common/jsonTypes';
import { sheriffRoleJsonType, userLocationJsonType } from '@/types/MyTeam/jsonTypes';

export interface sheriffsAvailabilityJsonType {    
    start: string,
    end: string,
    conflicts: conflictJsonType[],
    sheriff: {
        gender: string,
        badgeNumber: string,
        rank: string,
        awayLocation: availabilityAwayLocationJsonType[],
        leave: availabilityLeaveJsonType[],
        training: availabilityTrainingJsonType[],
        photoUrl: string,
        lastPhotoUpdate: string,
        id: string,
        isEnabled: true,
        firstName: string,
        lastName: string,
        email: string,
        homeLocationId: number,
        homeLocation: userLocationJsonType,
        activeRoles: sheriffRoleJsonType[],
        roles: sheriffRoleJsonType[],
        concurrencyToken: number
    },
     sheriffId: string,
     timezone: string
}

export interface conflictJsonType {
     sheriffId: string,
     conflict: string,
     start: string,
     end: string,
     locationId: number
}

export interface availabilityTrainingJsonType  {
    trainingType: {
        id: string,
        type: string,
        code: string,
        subCode: string,
        description: string,
        effectiveDate: string,
        expiryDate: string,
        location: userLocationJsonType,
        locationId: number,
        concurrencyToken: number,
        sortOrderForLocation: sortOrderForLocationJsonType
    },
    trainingTypeId: number,
    trainingCertificationExpiry: string,
    id: string,
    startDate:  string,
    endDate: string,
    expiryDate:  string,
    expiryReason: string,
    sheriffId:  string,
    comment: string,
    timezone: string,
    concurrencyToken: number
}

export interface availabilityAwayLocationJsonType {
    location: userLocationJsonType,
    locationId: number,
    id: number,
    startDate: string,
    endDate: string,
    expiryDate: string,
    expiryReason: string,
    sheriffId: string,
    comment: string,
    timezone: string,
    concurrencyToken: number
}

export interface availabilityLeaveJsonType {
    leaveType: {
        id: number,
        type:  string,
        code: string,
        subCode: string,
        description: string,
        effectiveDate: string,
        expiryDate: string,
        location: userLocationJsonType,
        locationId: number,
        concurrencyToken: number,
        sortOrderForLocation: sortOrderForLocationJsonType,
    },
    leaveTypeId: number,
    id: number,
    startDate: string,
    endDate: string,
    expiryDate: string,
    expiryReason: string,
    sheriffId:   string,
    comment: string,
    timezone: string,
    concurrencyToken: number
}
