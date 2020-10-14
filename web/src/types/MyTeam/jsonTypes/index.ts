import {} from '../../common';

export interface teamMemberJsonType {
    
        "gender": string,
        "badgeNumber": string,
        "rank": string,
        "awayLocations": awayLocationsJsontype[],
        "leaves": [
          {
            "id": number,
            "leaveTypeId": number,
            "leaveType": {
              "id": number,
              "type": "CourtRoom",
              "code": string,
              "subCode": string,
              "description": string,
              "effectiveDate": string,
              "expiryDate": string,
              "sortOrder": number,
              "locationId": number,
              "location": {
                "id": number,
                "code": string,
                "name": string,
                "justinId": number,
                "justinCode": string,
                "parentLocationId": number,
                "region": {
                  "id": number,
                  "code": string,
                  "name": string
                },
                "createdById": string,
                "createdBy": {
                  "id": string,
                  "preferredUsername": string,
                  "idirId": string,
                  "isDisabled": boolean,
                  "firstName": string,
                  "lastName": string,
                  "email": string,
                  "roles": [
                    null
                  ],
                  "permissions": [
                    null
                  ],
                  "lastLogin": string,
                  "createdById": string,
                  "createdOn": string,
                  "updatedById": string,
                  "updatedOn": string,
                  "rowVersion": string
                },
                "createdOn": string,
                "updatedById": string,
                "updatedBy": {
                  "id": string,
                  "preferredUsername": string,
                  "idirId": string,
                  "isDisabled": boolean,
                  "firstName": string,
                  "lastName": string,
                  "email": string,
                  "roles": [
                    null
                  ],
                  "permissions": [
                    null
                  ],
                  "lastLogin": string,
                  "createdById": string,
                  "createdOn": string,
                  "updatedById": string,
                  "updatedOn": string,
                  "rowVersion": string
                },
                "updatedOn": string,
                "rowVersion": string
              },
              "createdById": string,
              "createdBy": {
                "id": string,
                "preferredUsername": string,
                "idirId": string,
                "isDisabled": boolean,
                "firstName": string,
                "lastName": string,
                "email": string,
                "roles": [
                  null
                ],
                "permissions": [
                  null
                ],
                "lastLogin": string,
                "createdById": string,
                "createdOn": string,
                "updatedById": string,
                "updatedOn": string,
                "rowVersion": string
              },
              "createdOn": string,
              "updatedById": string,
              "updatedBy": {
                "id": string,
                "preferredUsername": string,
                "idirId": string,
                "isDisabled": boolean,
                "firstName": string,
                "lastName": string,
                "email": string,
                "roles": [
                  null
                ],
                "permissions": [
                  null
                ],
                "lastLogin": string,
                "createdById": string,
                "createdOn": string,
                "updatedById": string,
                "updatedOn": string,
                "rowVersion": string
              },
              "updatedOn": string,
              "rowVersion": string
            },
            "startDate": string,
            "endDate": string,
            "isFullDay": boolean,
            "sheriffId": string,
            "createdById": string,
            "createdBy": {
              "id": string,
              "preferredUsername": string,
              "idirId": string,
              "isDisabled": boolean,
              "firstName": string,
              "lastName": string,
              "email": string,
              "roles": [
                null
              ],
              "permissions": [
                null
              ],
              "lastLogin": string,
              "createdById": string,
              "createdOn": string,
              "updatedById": string,
              "updatedOn": string,
              "rowVersion": string
            },
            "createdOn": string,
            "updatedById": string,
            "updatedBy": {
              "id": string,
              "preferredUsername": string,
              "idirId": string,
              "isDisabled": boolean,
              "firstName": string,
              "lastName": string,
              "email": string,
              "roles": [
                null
              ],
              "permissions": [
                null
              ],
              "lastLogin": string,
              "createdById": string,
              "createdOn": string,
              "updatedById": string,
              "updatedOn": string,
              "rowVersion": string
            },
            "updatedOn": string,
            "rowVersion": string
          }
        ],
        "training": [
          {
            "id": number,
            "trainingTypeId": number,
            "trainingType": {
              "id": number,
              "type": "CourtRoom",
              "code": string,
              "subCode": string,
              "description": string,
              "effectiveDate": string,
              "expiryDate": string,
              "sortOrder": number,
              "locationId": number,
              "location": {
                "id": number,
                "code": string,
                "name": string,
                "justinId": number,
                "justinCode": string,
                "parentLocationId": number,
                "region": {
                  "id": number,
                  "code": string,
                  "name": string
                },
                "createdById": string,
                "createdBy": {
                  "id": string,
                  "preferredUsername": string,
                  "idirId": string,
                  "isDisabled": boolean,
                  "firstName": string,
                  "lastName": string,
                  "email": string,
                  "roles": [
                    null
                  ],
                  "permissions": [
                    null
                  ],
                  "lastLogin": string,
                  "createdById": string,
                  "createdOn": string,
                  "updatedById": string,
                  "updatedOn": string,
                  "rowVersion": string
                },
                "createdOn": string,
                "updatedById": string,
                "updatedBy": {
                  "id": string,
                  "preferredUsername": string,
                  "idirId": string,
                  "isDisabled": boolean,
                  "firstName": string,
                  "lastName": string,
                  "email": string,
                  "roles": [
                    null
                  ],
                  "permissions": [
                    null
                  ],
                  "lastLogin": string,
                  "createdById": string,
                  "createdOn": string,
                  "updatedById": string,
                  "updatedOn": string,
                  "rowVersion": string
                },
                "updatedOn": string,
                "rowVersion": string
              },
              "createdById": string,
              "createdBy": {
                "id": string,
                "preferredUsername": string,
                "idirId": string,
                "isDisabled": boolean,
                "firstName": string,
                "lastName": string,
                "email": string,
                "roles": [
                  null
                ],
                "permissions": [
                  null
                ],
                "lastLogin": string,
                "createdById": string,
                "createdOn": string,
                "updatedById": string,
                "updatedOn": string,
                "rowVersion": string
              },
              "createdOn": string,
              "updatedById": string,
              "updatedBy": {
                "id": string,
                "preferredUsername": string,
                "idirId": string,
                "isDisabled": boolean,
                "firstName": string,
                "lastName": string,
                "email": string,
                "roles": [
                  null
                ],
                "permissions": [
                  null
                ],
                "lastLogin": string,
                "createdById": string,
                "createdOn": string,
                "updatedById": string,
                "updatedOn": string,
                "rowVersion": string
              },
              "updatedOn": string,
              "rowVersion": string
            },
            "startDate": string,
            "endDate": string,
            "isFullDay": boolean,
            "sheriffId": string,
            "createdById": string,
            "createdBy": {
              "id": string,
              "preferredUsername": string,
              "idirId": string,
              "isDisabled": boolean,
              "firstName": string,
              "lastName": string,
              "email": string,
              "roles": [
                null
              ],
              "permissions": [
                null
              ],
              "lastLogin": string,
              "createdById": string,
              "createdOn": string,
              "updatedById": string,
              "updatedOn": string,
              "rowVersion": string
            },
            "createdOn": string,
            "updatedById": string,
            "updatedBy": {
              "id": string,
              "preferredUsername": string,
              "idirId": string,
              "isDisabled": boolean,
              "firstName": string,
              "lastName": string,
              "email": string,
              "roles": [
                null
              ],
              "permissions": [
                null
              ],
              "lastLogin": string,
              "createdById": string,
              "createdOn": string,
              "updatedById": string,
              "updatedOn": string,
              "rowVersion": string
            },
            "updatedOn": string,
            "rowVersion": string
          }
        ],
        "photo": string,
        "id": string,
        "isDisabled": boolean,
        "homeLocation": {
          "id": number,
          "code": string,
          "name": string,
          "justinId": number,
          "justinCode": string,
          "parentLocationId": number,
          "region": {
            "id": number,
            "code": string,
            "name": string
          },
          "createdById": string,
          "createdBy": {
            "id": string,
            "preferredUsername": string,
            "idirId": string,
            "isDisabled": boolean,
            "firstName": string,
            "lastName": string,
            "email": string,
            "roles": [
              null
            ],
            "permissions": [
              null
            ],
            "lastLogin": string,
            "createdById": string,
            "createdOn": string,
            "updatedById": string,
            "updatedOn": string,
            "rowVersion": string
          },
          "createdOn": string,
          "updatedById": string,
          "updatedBy": {
            "id": string,
            "preferredUsername": string,
            "idirId": string,
            "isDisabled": boolean,
            "firstName": string,
            "lastName": string,
            "email": string,
            "roles": [
              null
            ],
            "permissions": [
              null
            ],
            "lastLogin": string,
            "createdById": string,
            "createdOn": string,
            "updatedById": string,
            "updatedOn": string,
            "rowVersion": string
          },
          "updatedOn": string,
          "rowVersion": string
        },
        "firstName": string,
        "lastName": string,
        "email": string,
        "roles": [
          {
            "id": number,
            "userId": string,
            "user": {
              "id": string,
              "preferredUsername": string,
              "idirId": string,
              "isDisabled": boolean,
              "firstName": string,
              "lastName": string,
              "email": string,
              "roles": [
                null
              ],
              "permissions": [
                null
              ],
              "lastLogin": string,
              "createdById": string,
              "createdOn": string,
              "updatedById": string,
              "updatedOn": string,
              "rowVersion": string
            },
            "roleId": number,
            "role": {
              "id": number,
              "name": string,
              "description": string,
              "rolePermissions": [
                {
                  "id": number,
                  "permission": {
                    "id": number,
                    "name": string,
                    "description": string,
                    "createdById": string,
                    "createdBy": {
                      "id": string,
                      "preferredUsername": string,
                      "idirId": string,
                      "isDisabled": boolean,
                      "firstName": string,
                      "lastName": string,
                      "email": string,
                      "roles": [
                        null
                      ],
                      "permissions": [
                        null
                      ],
                      "lastLogin": string,
                      "createdById": string,
                      "createdOn": string,
                      "updatedById": string,
                      "updatedOn": string,
                      "rowVersion": string
                    },
                    "createdOn": string,
                    "updatedById": string,
                    "updatedBy": {
                      "id": string,
                      "preferredUsername": string,
                      "idirId": string,
                      "isDisabled": boolean,
                      "firstName": string,
                      "lastName": string,
                      "email": string,
                      "roles": [
                        null
                      ],
                      "permissions": [
                        null
                      ],
                      "lastLogin": string,
                      "createdById": string,
                      "createdOn": string,
                      "updatedById": string,
                      "updatedOn": string,
                      "rowVersion": string
                    },
                    "updatedOn": string,
                    "rowVersion": string
                  },
                  "createdById": string,
                  "createdBy": {
                    "id": string,
                    "preferredUsername": string,
                    "idirId": string,
                    "isDisabled": boolean,
                    "firstName": string,
                    "lastName": string,
                    "email": string,
                    "roles": [
                      null
                    ],
                    "permissions": [
                      null
                    ],
                    "lastLogin": string,
                    "createdById": string,
                    "createdOn": string,
                    "updatedById": string,
                    "updatedOn": string,
                    "rowVersion": string
                  },
                  "createdOn": string,
                  "updatedById": string,
                  "updatedBy": {
                    "id": string,
                    "preferredUsername": string,
                    "idirId": string,
                    "isDisabled": boolean,
                    "firstName": string,
                    "lastName": string,
                    "email": string,
                    "roles": [
                      null
                    ],
                    "permissions": [
                      null
                    ],
                    "lastLogin": string,
                    "createdById": string,
                    "createdOn": string,
                    "updatedById": string,
                    "updatedOn": string,
                    "rowVersion": string
                  },
                  "updatedOn": string,
                  "rowVersion": string
                }
              ],
              "users": [
                null
              ],
              "createdById": string,
              "createdBy": {
                "id": string,
                "preferredUsername": string,
                "idirId": string,
                "isDisabled": boolean,
                "firstName": string,
                "lastName": string,
                "email": string,
                "roles": [
                  null
                ],
                "permissions": [
                  null
                ],
                "lastLogin": string,
                "createdById": string,
                "createdOn": string,
                "updatedById": string,
                "updatedOn": string,
                "rowVersion": string
              },
              "createdOn": string,
              "updatedById": string,
              "updatedBy": {
                "id": string,
                "preferredUsername": string,
                "idirId": string,
                "isDisabled": boolean,
                "firstName": string,
                "lastName": string,
                "email": string,
                "roles": [
                  null
                ],
                "permissions": [
                  null
                ],
                "lastLogin": string,
                "createdById": string,
                "createdOn": string,
                "updatedById": string,
                "updatedOn": string,
                "rowVersion": string
              },
              "updatedOn": string,
              "rowVersion": string
            },
            "createdById": string,
            "createdBy": {
              "id": string,
              "preferredUsername": string,
              "idirId": string,
              "isDisabled": boolean,
              "firstName": string,
              "lastName": string,
              "email": string,
              "roles": [
                null
              ],
              "permissions": [
                null
              ],
              "lastLogin": string,
              "createdById": string,
              "createdOn": string,
              "updatedById": string,
              "updatedOn": string,
              "rowVersion": string
            },
            "createdOn": string,
            "updatedById": string,
            "updatedBy": {
              "id": string,
              "preferredUsername": string,
              "idirId": string,
              "isDisabled": boolean,
              "firstName": string,
              "lastName": string,
              "email": string,
              "roles": [
                null
              ],
              "permissions": [
                null
              ],
              "lastLogin": string,
              "createdById": string,
              "createdOn": string,
              "updatedById": string,
              "updatedOn": string,
              "rowVersion": string
            },
            "updatedOn": string,
            "rowVersion": string
          }
        ],
        "permissions": [
          {
            "id": number,
            "name": string,
            "description": string,
            "createdById": string,
            "createdBy": {
              "id": string,
              "preferredUsername": string,
              "idirId": string,
              "isDisabled": boolean,
              "firstName": string,
              "lastName": string,
              "email": string,
              "roles": [
                null
              ],
              "permissions": [
                null
              ],
              "lastLogin": string,
              "createdById": string,
              "createdOn": string,
              "updatedById": string,
              "updatedOn": string,
              "rowVersion": string
            },
            "createdOn": string,
            "updatedById": string,
            "updatedBy": {
              "id": string,
              "preferredUsername": string,
              "idirId": string,
              "isDisabled": boolean,
              "firstName": string,
              "lastName": string,
              "email": string,
              "roles": [
                null
              ],
              "permissions": [
                null
              ],
              "lastLogin": string,
              "createdById": string,
              "createdOn": string,
              "updatedById": string,
              "updatedOn": string,
              "rowVersion": string
            },
            "updatedOn": string,
            "rowVersion": string
          }
        ],
        "lastLogin": string,
        "createdById": string,
        "createdBy": {
          "id": string,
          "preferredUsername": string,
          "idirId": string,
          "isDisabled": boolean,
          "firstName": string,
          "lastName": string,
          "email": string,
          "roles": [
            null
          ],
          "permissions": [
            null
          ],
          "lastLogin": string,
          "createdById": string,
          "createdOn": string,
          "updatedById": string,
          "updatedOn": string,
          "rowVersion": string
        },
        "createdOn": string,
        "updatedById": string,
        "updatedBy": {
          "id": string,
          "preferredUsername": string,
          "idirId": string,
          "isDisabled": boolean,
          "firstName": string,
          "lastName": string,
          "email": string,
          "roles": [
            null
          ],
          "permissions": [
            null
          ],
          "lastLogin": string,
          "createdById": string,
          "createdOn": string,
          "updatedById": string,
          "updatedOn": string,
          "rowVersion": string
        },
        "updatedOn": string,
        "rowVersion": string
      
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