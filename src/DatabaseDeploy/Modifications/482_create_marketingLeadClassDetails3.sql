Create Table `MarketingLeadCallDetailV3`(
`Id` BIGINT(20) NOT NULL AUTO_INCREMENT,
`MarketingLeadCallDetailId` BIGINT(20) NOT NULL ,
`Sid` varchar(128) DEFAULT NULL,
`CallflowSetName` VARCHAR(400) default null,
`CallflowSetId` VARCHAR(400) default null,
`CallflowDestination` VARCHAR(400) default null,
`CallflowDestinationId` VARCHAR(400) default null,
`CallflowSource` VARCHAR(400) default null,
`CallflowSourceId` VARCHAR(400) default null,
`CallflowSourceQualified` VARCHAR(400) default null,
`CallflowRepeatSourceCaller` VARCHAR(400) default null,
`CallflowSourceCap` VARCHAR(400) default null,
`CallflowSourceRoute` VARCHAR(400) default null,
`CallflowSourceRouteId` VARCHAR(400) default null,
`CallflowSourceRouteQualified` VARCHAR(400) default null,
`CallflowState` VARCHAR(400) default null,
`CallflowEnteredZip` VARCHAR(400) default null,
`CallflowReroute` VARCHAR(400) default null,
`TransferToNumber` VARCHAR(400) default null,
`TransferToNumber_CallFlow` VARCHAR(400) default null,
`TransferType_CallFlow` VARCHAR(400) default null,
`CallTransferStatus_CallFlow` VARCHAR(400) default null,
`RingSeconds_CallFlow` VARCHAR(400) default null,
`RingCount_CallFlow` VARCHAR(400) default null,
`KeywordGroups_CallAnalytics` VARCHAR(400) default null,
`KeywordSpottingComplete_CallAnalytics` VARCHAR(400) default null,
`TranscriptionStatus_CallAnalytics` VARCHAR(400) default null,
`CallNote_CallAnalytics` VARCHAR(400) default null,
`RecordingUrl_Recording` VARCHAR(400) default null,
`RecordedSeconds_Recording` VARCHAR(400) default null,
`DialogAnalytics_Recording` VARCHAR(400) default null,
`FirstName_ReverseLookUp` VARCHAR(400) default null,
`LastName_ReverseLookUp` VARCHAR(400) default null,
`StreetLine1_ReverseLookUp` VARCHAR(400) default null,
`City_ReverseLookUp` VARCHAR(400) default null,
`PostalArea_ReverseLookUp` VARCHAR(400) default null,
`StateCode_ReverseLookUp` VARCHAR(400) default null,
`GeoLookupAttempt_ReverseLookUp` VARCHAR(400) default null,
`GeoLookupResult_ReverseLookUp` VARCHAR(400) default null,

`IsDeleted` BIT(1) NULL DEFAULT b'0' ,
PRIMARY KEY (`Id`),
INDEX `fk_MarketingLeadCallDetailV3_marketingLeadCallDetailV2Id_idx` (`MarketingLeadCallDetailId` ASC) ,
CONSTRAINT `fk_MarketingLeadCallDetailV3_marketingLeadCallDetailV2Id`
FOREIGN KEY (`MarketingLeadCallDetailId`)
REFERENCES `marketingLeadCallDetail` (`Id`)
);



Create Table `MarketingLeadCallDetailV4`(
`Id` BIGINT(20) NOT NULL AUTO_INCREMENT,
`MarketingLeadCallDetailId` BIGINT(20) NOT NULL ,
`MissedCall_CallMetrics` VARCHAR(400) default null,
`CallActivities` VARCHAR(400) default null,
`Channel_Attribution` VARCHAR(400) default null,
`Status_Attribution` VARCHAR(400) default null,
`Rank_Attribution` VARCHAR(400) default null,
`Pid_Attribution` VARCHAR(400) default null,
`Bid_Attribution` VARCHAR(400) default null,
`DocumentTitle_FirstTouch` VARCHAR(400) default null,
`DocumentUrl_FirstTouch` VARCHAR(400) default null,
`DocumentPath_FirstTouch` VARCHAR(400) default null,
`DocumentTimeStamp_FirstTouch` VARCHAR(400) default null,
`DocumentTitle_LastTouch` VARCHAR(400) default null,
`DocumentUrl_LastTouch` VARCHAR(400) default null,
`DocumentPath_LastTouch` VARCHAR(400) default null,
`DocumentTimeStamp_LastTouch` VARCHAR(400) default null,
`IPAddress_VisitorData` VARCHAR(400) default null,
`Device_VisitorData` VARCHAR(400) default null,
`Browser_VisitorData` VARCHAR(400) default null,
`BrowserVersion_VisitorData` VARCHAR(400) default null,
`Os_VisitorData` VARCHAR(400) default null,
`OsVersion_VisitorData` VARCHAR(400) default null,
`SearchTerm_VisitorData` VARCHAR(400) default null,
`ActivityValue_VisitorData` VARCHAR(400) default null,

`ActivityTypeId_VisitorData` VARCHAR(400) default null,
`ActivityKeyword_VisitorData` VARCHAR(400) default null,
`ActivityTag_VisitorData` VARCHAR(400) default null,
`Campaign_VisitorData` VARCHAR(400) default null,
`Platform_VisitorData` VARCHAR(400) default null,
`SourceGuard_VisitorData` VARCHAR(400) default null,
`VisitorLogUrl_VisitorData` VARCHAR(400) default null,
`GoogleUaClientId_VisitorData` VARCHAR(400) default null,
`GClid_VisitorData` VARCHAR(400) default null,

`IsDeleted` BIT(1) NULL DEFAULT b'0' ,
PRIMARY KEY (`Id`),
INDEX `fk_MarketingLeadCallDetailV4_marketingLeadCallDetailV4Id_idx` (`MarketingLeadCallDetailId` ASC) ,
CONSTRAINT `fk_MarketingLeadCallDetailV4_marketingLeadCallDetailV4Id`
FOREIGN KEY (`MarketingLeadCallDetailId`)
REFERENCES `marketingLeadCallDetail` (`Id`)
);




Create Table `MarketingLeadCallDetailV5`(
`Id` BIGINT(20) NOT NULL AUTO_INCREMENT,
`MarketingLeadCallDetailId` BIGINT(20) NOT NULL ,
`Sid` varchar(128) DEFAULT NULL,
`UtmSource_DefaultUtmParameters` VARCHAR(400) default null,
`UtmMedium_DefaultUtmParameters` VARCHAR(400) default null,
`UtmCampaign_DefaultUtmParameters` VARCHAR(400) default null,
`UtmTerm_DefaultUtmParameters` VARCHAR(400) default null,
`UtmContent_DefaultUtmParameters` VARCHAR(400) default null,
`VtKeyword_ValueTrackParameters` VARCHAR(400) default null,
`VtMatchType_ValueTrackParameters` VARCHAR(400) default null,
`VtNetwork_ValueTrackParameters` VARCHAR(400) default null,
`VtDevice_ValueTrackParameters` VARCHAR(400) default null,
`VtDeviceModel_ValueTrackParameters` VARCHAR(400) default null,
`VtCreative_ValueTrackParameters` VARCHAR(400) default null,
`VtPlacement_ValueTrackParameters` VARCHAR(400) default null,
`VtTarget_ValueTrackParameters` VARCHAR(400) default null,
`VtParam1_ValueTrackParameters` VARCHAR(400) default null,
`VtParam2_ValueTrackParameters` VARCHAR(400) default null,
`VtRandom_ValueTrackParameters` VARCHAR(400) default null,
`VtAceid_ValueTrackParameters` VARCHAR(400) default null,
`VtAdposition_ValueTrackParameters` VARCHAR(400) default null,
`VtProductTargetId_ValueTrackParameters` VARCHAR(400) default null,
`VtAdType_ValueTrackParameters` VARCHAR(400) default null,
`DomainSetName_SourceIqData` VARCHAR(400) default null,
`DomainSetId_SourceIqData` VARCHAR(400) default null,
`PoolName_SourceIqData` VARCHAR(400) default null,
`LocationName_SourceIqData` VARCHAR(400) default null,
`CustomValue_SourceIqData` VARCHAR(400) default null,
`CustomId_SourceIqData` VARCHAR(400) default null,
`Campaign_PaidSearch` VARCHAR(400) default null,
`CampaignId_PaidSearch` VARCHAR(400) default null,
`Adgroup_PaidSearch` VARCHAR(400) default null,
`AdgroupId_PaidSearch` VARCHAR(400) default null,
`Ads_PaidSearch` VARCHAR(400) default null,
`AdId_PaidSearch` VARCHAR(400) default null,

`Keywords_PaidSearch` VARCHAR(400) default null,
`KeywordId_PaidSearch` VARCHAR(400) default null,
`ClickId_PaidSearch` VARCHAR(400) default null,
`KeyWordMatchType_PaidSearch` VARCHAR(400) default null,
`CallInlyFlag_PaidSearch` VARCHAR(400) default null,
`Type_PaidSearch` VARCHAR(400) default null,
`IsDeleted` BIT(1) NULL DEFAULT b'0' ,
PRIMARY KEY (`Id`),
INDEX `fk_MarketingLeadCallDetailV5_marketingLeadCallDetailV5Id_idx` (`MarketingLeadCallDetailId` ASC) ,
CONSTRAINT `fk_MarketingLeadCallDetailV5_marketingLeadCallDetailV5Id`
FOREIGN KEY (`MarketingLeadCallDetailId`)
REFERENCES `marketingLeadCallDetail` (`Id`)
);



ALTER TABLE `makalu`.`marketingleadcalldetailv3` 
CHANGE COLUMN `RingSeconds_CallFlow` `RingSeconds_CallFlow` BIGINT(20) NULL DEFAULT NULL ,
CHANGE COLUMN `RingCount_CallFlow` `RingCount_CallFlow` BIGINT(20) NULL DEFAULT NULL ;





ALTER TABLE `makalu`.`marketingleadcalldetailv4` 
ADD COLUMN `Sid` VARCHAR(400) NULL DEFAULT NULL AFTER `IsDeleted`;

ALTER TABLE `makalu`.`marketingleadcalldetailv4` 
CHANGE COLUMN `CallActivities` `CallActivities` VARCHAR(1028) NULL DEFAULT NULL ;

UPDATE `makalu`.`technicianworkorder` SET `Name` = 'MaxOut Concentrate Gallon' WHERE (`Id` = '104');
UPDATE `makalu`.`technicianworkorder` SET `Name` = 'Granite Sealer 4 oz Spray' WHERE (`Id` = '93');
UPDATE `makalu`.`technicianworkorder` SET `Name` = 'Marble cleaner Refill Gallon' WHERE (`Id` = '95');
UPDATE `makalu`.`technicianworkorder` SET `Name` = 'MaxOut Grout Cleaner 32 oz' WHERE (`Id` = '103');

INSERT INTO `makalu`.`technicianworkorder` (`Id`, `Name`, `WorkOrderId`, `IsActive`, `IsDeleted`) VALUES ('113', 'Expansion Joint Repair', '279', b'1', b'0');