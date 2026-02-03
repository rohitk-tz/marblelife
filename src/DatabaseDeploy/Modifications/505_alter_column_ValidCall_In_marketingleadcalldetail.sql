 ALTER TABLE `marketingleadcalldetail`
ADD COLUMN `ValidCall` bit(1) default false;

UPDATE `makalu`.`servicetype` SET `Description` = 'Terrazzo and Concrete polishing' WHERE (`Id` = '2');




CREATE TABLE `salestaxrates` (
  `Id` bigint(20) NOT NULL AUTO_INCREMENT,
  `City` varchar(255) DEFAULT NULL,
  `CityId` bigint(20) DEFAULT NULL,
  `Country` varchar(255) DEFAULT NULL,
  `CountryId` bigint(20) DEFAULT NULL,
  `State` varchar(255) DEFAULT NULL,
  `StateId` bigint(20) DEFAULT NULL,
  `ZipCode` varchar(255) DEFAULT NULL,
  `ZipId` bigint(20) DEFAULT NULL,
  `CombinedDistrictRate` decimal(18,2) DEFAULT NULL,
  `CombinedRate` decimal(18,2) DEFAULT NULL,
  `CountryRate` decimal(18,2) DEFAULT NULL,
  `DistanceSalesThreshold` decimal(18,2) DEFAULT NULL,
  `FreightTaxable` bit(1) DEFAULT NULL,
  `FranchiseeId` bigint(20) DEFAULT NULL,
  `ParkingRate` decimal(18,2) DEFAULT NULL,
  `ReducedRate` decimal(18,2) DEFAULT NULL,
  `StandardRate` decimal(18,2) DEFAULT NULL,
  `StateRate` decimal(18,2) DEFAULT NULL,
  `SuperReducedRate` decimal(18,2) DEFAULT NULL,
  `IsDeleted` bit(1) DEFAULT b'0',
  PRIMARY KEY (`Id`),
  KEY `fk_salesTaxRates_franchiseeId_idx` (`FranchiseeId`),
  KEY `fk_salesTaxRates_country_idx` (`CountryId`),
  KEY `fk_salesTaxRates_state_idx` (`StateId`),
  KEY `fk_salesTaxRates_city_idx` (`CityId`),
  KEY `fk_salesTaxRates_zip_idx` (`ZipId`),
  CONSTRAINT `fk_salesTaxRates_city` FOREIGN KEY (`CityId`) REFERENCES `city` (`Id`),
  CONSTRAINT `fk_salesTaxRates_country` FOREIGN KEY (`CountryId`) REFERENCES `country` (`Id`),
  CONSTRAINT `fk_salesTaxRates_franchiseeId` FOREIGN KEY (`FranchiseeId`) REFERENCES `franchisee` (`Id`),
  CONSTRAINT `fk_salesTaxRates_state` FOREIGN KEY (`StateId`) REFERENCES `state` (`Id`),
  CONSTRAINT `fk_salesTaxRates_zip` FOREIGN KEY (`ZipId`) REFERENCES `zip` (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=1427 DEFAULT CHARSET=latin1