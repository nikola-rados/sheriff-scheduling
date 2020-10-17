import {} from '../../common';

export interface teamMemberJsonType {
    "loanedIn": awayLocationsJsontype[],
    "loanedOut": awayLocationsJsontype[],
    "gender": string,
    "badgeNumber": string,
    "rank": string,
    "awayLocation": awayLocationsJsontype[],
    "leave": leaveJsontype[],
    "training": trainingJsontype[],
    "photo": string,
    "id": string,
    "idirName": string,
    "isEnabled": boolean,
    "firstName": string,
    "lastName": string,
    "email": string,
    "homeLocationId": number,
    "homeLocation": {
      "id": number,
      "agencyId": string,
      "name": string,
      "justinCode": string,
      "parentLocationId": number,
      "expiryDate": string,
      "regionId": number,
      "timezone": string,
      "concurrencyToken": number
    },
    "activeRoles": [
      {
        "role": {
          "id": number,
          "name": string,
          "description": string,
          "rolePermissions": [
            {
              "id": number,
              "roleId": number,
              "permission": {
                "id": number,
                "name": string,
                "description": string,
                "concurrencyToken": number
              },
              "permissionId": number,
              "concurrencyToken": number
            }
          ],
          "concurrencyToken": number
        },
        "effectiveDate": string,
        "expiryDate": string
      }
    ],
    "roles": [
      {
        "role": {
          "id": number,
          "name": string,
          "description": string,
          "rolePermissions": [
            {
              "id": number,
              "roleId": number,
              "permission": {
                "id": number,
                "name": string,
                "description": string,
                "concurrencyToken": number
              },
              "permissionId": number,
              "concurrencyToken": number
            }
          ],
          "concurrencyToken": number
        },
        "effectiveDate": string,
        "expiryDate": string
      }
    ],
    "concurrencyToken": number
  
      
}

export interface permissionJsonType {
  
    "id": string,
    "name": string,
    "description": string,
    "concurrencyToken": number
  
}

export interface roleJsonType {  
    "id": string,
    "name": string,
    "description": string,
    "rolePermissions": rolePermissionsJsonType[],      
    "expiryDate": string,
    "concurrencyToken": number  
}

export interface rolePermissionsJsonType {
  "id": string,
  "roleId": string,
  "permission": permissionJsonType,
  "permissionId": string,
  "concurrencyToken": number
}

export interface awayLocationsJsontype {  
    "id": number,
    "location": {
      "id": number,
      "agencyId": string,
      "name": string,
      "justinCode": string,
      "parentLocationId": number,
      "expiryDate": string,
      "regionId": number,
      "timezone": string,
      "concurrencyToken": number
    },
    "locationId": number,
    "startDate": string,
    "endDate": string,
    "expiryDate": string,
    "isFullDay": boolean,
    "sheriffId": string,
    "concurrencyToken": number  
}

export interface trainingJsontype {  
    "id": number,
    "trainingType": {
      "id": number,
      "type": string,
      "code": string,
      "subCode": string,
      "description": string,
      "effectiveDate": string,
      "expiryDate": string,
      "sortOrder": number,
      "location": {
        "id": number,
        "agencyId": string,
        "name": string,
        "justinCode": string,
        "parentLocationId": number,
        "expiryDate": string,
        "regionId": number,
        "timezone": string,
        "concurrencyToken": number
      },
      "locationId": number,
      "concurrencyToken": number
    },
    "trainingTypeId": number,
    "startDate": string,
    "endDate": string,
    "expiryDate": string,
    "isFullDay": boolean,
    "sheriffId": string,
    "concurrencyToken": number
}


export interface leaveJsontype {
  "id": number,
  "leaveType": {
    "id": number,
    "type": string,
    "code": string,
    "subCode": string,
    "description": string,
    "effectiveDate": string,
    "expiryDate": string,
    "sortOrder": number,
    "location": {
      "id": number,
      "agencyId": string,
      "name": string,
      "justinCode": string,
      "parentLocationId": number,
      "expiryDate": string,
      "regionId": number,
      "timezone": string,
      "concurrencyToken": number
    },
    "locationId": number,
    "concurrencyToken": number
  },
  "leaveTypeId": number,
  "startDate": string,
  "endDate": string,
  "expiryDate": string,
  "comment": string,
  "sheriffId": string,
  "concurrencyToken": number 
}