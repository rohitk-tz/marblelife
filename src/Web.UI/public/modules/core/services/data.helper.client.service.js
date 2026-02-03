'use strict';

var DataHelper = {
    InvoiceItemTypes: {
        Service: 91,
        Royalty: 92,
        AdFund: 93,
        Discount: 94,
        LateFee: 123,
        InterestRate: 124,
        ServiceFee: 95,
        getValue: function (id) {
            switch (id) {
                case 91:
                    return "Service";
                    break;
                case 92:
                    return "Royalty";
                    break;
                case 93:
                    return "AdFund";
                    break;
                case 94:
                    return "Discount";
                    break;
                case 123:
                    return "LateFee";
                    break;
                case 124:
                    return "InterestRate";
                    break;
                case 95:
                    return "ServiceFee";
                    break;
            }
        }
    },

    Role: {
        SuperAdmin: 1,
        FranchiseeAdmin: 2,
        SalesRep: 3,
        Technician: 4,
        FrontOfficeExecutive: 5,
        OperationsManager: 6,
        Equipment: 7,
        getValue: function (id) {
            switch (id) {
                case 1:
                    return "SuperAdmin";
                    break;
                case 2:
                    return "FranchiseeAdmin";
                    break;
                case 3:
                    return "SalesRep";
                    break;
                case 4:
                    return "Technician";
                    break;
                case 5:
                    return "FrontOfficeExecutive";
                    break;
                case 6:
                    return "OperationsManager";
                    break;
                case 7:
                    return "Equipment";
                    break;
            }
        }
    },

    InvoiceStatus: {
        Paid: 81,
        Unpaid: 82,
        PartialPaid: 83,
        Canceled: 84
    },

    InstrumentType: {
        ChargeCard: 41,
        ECheck: 42,
        Cash: 43,
        Check: 44,
        OnFileChargeCard: 45,
        OnFileECheck: 46
    },

    SalesDataUploadStatus: {
        Uploaded: 71,
        Parsed: 72,
        Failed: 73,
        ParseInProgress: 74
    },

    ServiceTypeCategory: {
        Restoration: 101,
        Maintenance: 102,
        FRONTOFFICECALLMANAGEMENT: 254
    },

    FeeProfile: {
        Weekly: 31,
        Monthly: 32
    },

    AuditStatus: {
        Approved: 151,
        Rejected: 152,
        Pending: 153
    },
    ScheduleType: {
        Job: 145,
        Estimate: 146,
        Holiday: 147,
        Vacation: 148,
        Meeting: 149
    },
    ServiceFeeType: {
        Bookkeeping: 172,
        PayRollProcessing: 173,
        Recruiting: 174,
        OneTimeProject: 175,
        Loan: 171,
        NationalCharge: 176,
        BookkeepingVar: 178,
        FranchiseeTechMail: 214
    },
    RepeatFrequency: {
        Daily: 183,
        Weekly: 181,
        Monthly: 182,
        Custom: 184,
    },
    DocumentType: {
        FranchiseeManagementDocument: 201,
        NationalAccountDocuments: 202
    },
    Status: {
        All: 1,
        Active: 2,
        Inactive: 3,
        getValue: function (id) {
            switch (id) {
                case 1:
                    return "All";
                    break;
                case 2:
                    return "Active";
                    break;
                case 3:
                    return "Inactive";
                    break;
            }
        }
    },
    CountryId: {
        USA: 1,
        Canada: 2,
        Bahamas: 3,
        CaymanIslands: 4,
        SouthAfrica: 5,
        UnitedArabEmirates: 6,
        getValue: function (id) {
            switch (id) {
                case 1:
                    return "USA";
                    break;
                case 2:
                    return "Canada";
                    break;
                case 3:
                    return "Bahamas";
                    break;
                case 4:
                    return "CaymanIslands";
                    break;
                case 5:
                    return "SouthAfrica";
                    break;
                case 6:
                    return "UnitedArabEmirates";
                    break;
            }
        }
    },
    DocumentName: {
        W9: 1,
        LoanAgreement: 2,
        AnnualTaxFilling: 3,
        FranchiseeContract: 4,
        COI: 5,
        EMPLOYEEHANDBOOK: 6,
        HAZOMMANUAL: 7,
        LICENSE: 8,
        UPLOADTAXES: 9,
        FRANCHISEAGREEMENTSRENEWALS: 10,
        RESALECERTIFICATE: 11,
        NCA: 12,
        NAA: 13,
        Others: 14
    },
    ReportTypes: {
        Type6: 1,
        Type1C: 2,
        Type5: 3,
        Type1B: 4,
        Type1A: 5,
        Type14: 6,
        Type4: 7,
        Type1F: 8,
        Type1D: 9,
        Type3: 10,
        Type2A: 11,
        Type2B: 12,
        Type7: 13,
        Type8: 14,
        Type9: 15,
        Type10B: 16,
        Type11: 17,
        Type12: 18,
        Type13: 19,
        Type13B: 20,
        Type16: 21,
        Type1: 22,
        Type17: 23,
        Type17A: 24,
        Type17B: 25,
        Type17C: 26,
        Type17D: 27,
        Type17E: 28,
        Type18A: 29,
        Type18B: 30,
        Type18C: 31,
        Type18D: 32,
        Type18E: 33,
        Type1H: 34,
        Type5A: 35,
        Type4B: 36,
        Type5B: 37,
        Type4A: 38,
    },
    BeforeAfterImages: {
        Before: 203,
        After: 204,
        During: 205,
        BuildingExterior: 206,
    },
    ServiceTypes: {
        GROUTLIFE: 3,
        CONCRETECOUNTERTOPS: 34,
        CONCRETEOVERLAYMENTS: 35,
        CONCRETECOATINGS: 33,
        STONELIFE: 1,
        getValue: function (id) {
            switch (id) {
                case 1:
                    return "STONELIFE";
                    break;
            }
        }
    },
    MarketingTypes: {
        RESIDENTIAL: 4
    },

    FranchiseeCategories: {
        FRONTOFFICE: 210,
        OFFICEPERSON: 211,
        RESPONDWHENAVAILABLE: 212,
        RESPONDSNEXTDAY: 213,
        getValue: function (id) {
            switch (id) {
                case 210:
                    return "FRONTOFFICE";
                    break;
                case 211:
                    return "OFFICEPERSON";
                    break;
                case 212:
                    return "RESPONDWHENAVAILABLE";
                    break;
                case 213:
                    return "RESPONDSNEXTDAY";
                    break;
            }
        }
    },
    Months: {
        January: 1,
        February: 2,
        March: 3,
        April: 4,
        May: 5,
        June: 6,
        July: 7,
        August: 8,
        September: 9,
        October: 10,
        November: 11,
        December: 12,
        getValue: function (month) {
            switch (month) {
                case "January":
                    return 1;
                    break;
                case "February":
                    return 2;
                    break;
                case "March":
                    return 3;
                    break;
                case "April":
                    return 4;
                    break;
                case "May":
                    return 5;
                    break;
                case "June":
                    return 6;
                    break;
                case "July":
                    return 7;
                    break;
                case "August":
                    return 8;
                    break;
                case "September":
                    return 9;
                    break;
                case "October":
                    return 10;
                    break;
                case "November":
                    return 11;
                    break;
                case "December":
                    return 12;
                    break;
            }
        }
    },
    Confirmation: {
        confirmed: 218,
        alreadyConfirmed: 2,
        errorInConfirming: 3,
        invalidId: 4,
        pastScheduler: 5,
        notConfirmed: 217,
        notResponded: 216,
        getValue: function (confirmEnum) {
            switch (confirmEnum) {
                case 218:
                    return "confirmed";
                    break;
                case 2:
                    return "alreadyConfirmed";
                    break;
                case 3:
                    return "errorInConfirming";
                    break;
                case 4:
                    return "invalidId";
                    break;
                case 5:
                    return "pastScheduler";
                    break;
                case 217:
                    return "notConfirmed";
                    break;
                case 216:
                    return "notresponded";
                    break;
            }
        }
    },

    PerformanceParamter: {
        seocost: 219,
        ppsspend: 220,
        getValue: function (confirmEnum) {
            switch (confirmEnum) {
                case 219:
                    return "seocost";
                    break;
                case 220:
                    return "ppsspend";
                    break;
                default:
                    return "ppsspend";
                    break;
            }
        }
    },

    MLFSStatus: {
        MaxMinValue: 1,
        WholeNumber: 2,
        BlankValue: 3,
        GreaterValue: 4,
        ColorBlank: 5,
        LessThan0: 6,
        getValue: function (id) {
            switch (id) {
                case 1:
                    return "MaxMinValue";
                    break;
                case 2:
                    return "WholeNumber";
                    break;
                case 3:
                    return "BlankValue";
                    break;
                case 4:
                    return "GreaterValue";
                    break;
                case 5:
                    return "ColorBlank";
                    break;
                case 5:
                    return "LessThan0";
                    break;

            }
        }
    },
    NonResidentialCommercialClass: {
        RESIDENTIAL: 4,
        UNCLASSIFIED: 11,
        INTERIORDESIGN: 16,
        FLOORING: 15,
        RESIDENTIALPROPERTYMGMT: 21,
        HOMEMANAGEMENT: 26,
        getValue: function (id) {
            switch (id) {
                case 4:
                    return "RESIDENTIAL";
                    break;
                case 16:
                    return "INTERIORDESIGN";
                    break;
                case 15:
                    return "FLOORING";
                    break;
                case 21:
                    return "RESIDENTIALPROPERTYMGMT";
                    break;
                case 26:
                    return "HOMEMANAGEMENT";
                    break;
                case 11:
                    return "UNCLASSIFIED";
                    break;
                default:
                    return "NOTFOUND";
                    break;

            }
        }
    },
    FranchiseeNotes: {
        FRANCHISEEDURATION: 237,
        NOTESFROMOWNER: 238,
        NOTESFROMCALLCENTER: 239,
        getValue: function (id) {
            switch (id) {
                case 237:
                    return "FRANCHISEEDURATION";
                    break;
                case 238:
                    return "NOTESFROMOWNER";
                    break;
                case 239:
                    return "NOTESFROMCALLCENTER";
                    break;
                default:
                    return "NOTFOUND";
                    break;

            }
        }
    },
    CustomerSignatureType: {
        BEFORECOMPLETITION: 289,
        AFTERCOMPLETITION: 290,
        getValue: function (id) {
            switch (id) {
                case 289:
                    return "BEFORECOMPLETITION";
                    break;
                case 290:
                    return "AFTERCOMPLETITION";
                    break;
                default:
                    return "NOTFOUND";
                    break;

            }
        }
    },
    UnitType: {
        LINEARFT: 292,
        TIME: 295,
        MAINTAINANCE: 294,
        PRODUCTPRICE: 293,
        AREA: 291,
        getValue: function (id) {
            switch (id) {
                case 283:
                    return "ft.";
                    break;
                case 285:
                    return "sq. ft.";
                    break;
                case 286:
                    return "sq. ft.";
                    break;
                case 287:
                    return "per unit";
                    break;
                case 282:
                    return "sq. ft.";
                    break;
                default:
                    return "";
                    break;
            }
        },
    },
    ServiceTypeInvoice: {
        LINEARFT: 283,
        TIME: 285,
        MAINTAINANCE: 286,
        PRODUCTPRICE: 287,
        AREA: 282,
        TAXRATE: 288,
        EVENT:284
    }
};