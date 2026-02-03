CREATE TABLE `county` (
  `Id` bigint(20) NOT NULL AUTO_INCREMENT,
  `CountryId` bigint(20) DEFAULT NULL,
  `StateCode` varchar(10) DEFAULT NULL,
  `FranchiseeName` varchar(16) NOT NULL,
  `FranchseId` bigint(20) DEFAULT NULL,
  `TerritoryCode` varchar(255) DEFAULT NULL,
  `DirectionCode` varchar(200) NOT NULL,
  `DirectionFromOffice` varchar(100) DEFAULT NULL,
  `ReachingTime` varchar(255) DEFAULT NULL,
  `Population` DECIMAL DEFAULT NULL,
  `Status` varchar(255) DEFAULT NULL,
  `FranchiseMLD` varchar(500) DEFAULT NULL,
  `ContractedTerritory` varchar(500) DEFAULT NULL,
  `CoveringLessThan3Hours` varchar(500) DEFAULT NULL,
  `DayTrip` varchar(500) DEFAULT NULL,
  `UnCovered` varchar(500) DEFAULT NULL,
  `IsDeleted` bit(1) DEFAULT b'0',
  `CountyName` varchar(255) DEFAULT NULL,
  `StateCountryCode` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`Id`),
   CONSTRAINT `fk_Zip_Code_Check_country_id` FOREIGN KEY (`CountryId`) REFERENCES `Country` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION
)

