CREATE TABLE `marketingleadcalldetailV2` (
  `Id` bigint(20) NOT NULL AUTO_INCREMENT,
  `FranchiseeId` bigint(20) DEFAULT NULL,
  `SetName` varchar(128) NOT NULL,
  `CallDate` datetime NOT NULL,
 `PhoneLabel` varchar(128) NOT NULL,
  `TransferNumber` varchar(128) NOT NULL,
  `CallerId` varchar(128) NOT NULL,
  `EnteredZipCode` varchar(128) NOT NULL,
  `FirstName` varchar(128) NOT NULL,
   `LastName` varchar(128) NOT NULL,
   `StreetAddress` varchar(128) NOT NULL,
   `City` varchar(128) NOT NULL,
   `State` varchar(128) NOT NULL,
   `ZipCode` varchar(128) NOT NULL,
   `Reroute` varchar(128) NOT NULL,
   `TalkMintues` varchar(128) NOT NULL,
   `TalkSeconds` varchar(128) NOT NULL,
   `TotalMintues` varchar(128) NOT NULL,
   `TotalSeconds` varchar(128) NOT NULL,
   `CallDuration` varchar(128) NOT NULL,
   `Sid` varchar(128) NOT NULL,
   `IvrResults` varchar(128) NOT NULL,
   `Source` varchar(128) NOT NULL,
   `SourceNumber` varchar(128) NOT NULL,
   `Destination` varchar(128) NOT NULL,
   `CallRoute` varchar(128) NOT NULL,
   `CallStatus` varchar(128) NOT NULL,
   `APPState` varchar(128) NOT NULL,
   `RepeaSourceCaller` varchar(128) NOT NULL,
    `SourceCap` varchar(128) NOT NULL,
    `CallRouteQualified` varchar(128) NOT NULL,
	`SourceQualified` varchar(128) NOT NULL,
    `IsDeleted` bit(1) NOT NULL DEFAULT b'0',
  PRIMARY KEY (`Id`),
  
  
  KEY `fk_MarketingLeadCallDetailV2_Franchisee_idx` (`FranchiseeId`),
  CONSTRAINT `fk_MarketingLeadCallDetailV2_Franchisee` FOREIGN KEY (`FranchiseeId`) REFERENCES `franchisee` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB AUTO_INCREMENT=461935 DEFAULT CHARSET=latin1;


ALTER TABLE `beforeafterimages` 
ADD COLUMN `imageUrl` varchar(15534) NULL  AFTER `IsDeleted`;