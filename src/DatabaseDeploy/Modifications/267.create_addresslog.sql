 CREATE TABLE `addresshistrylog` (
  `Id` bigint(20) NOT NULL AUTO_INCREMENT,
  `addressId` bigint(20) NOT NULL,
  `TypeId` bigint(20) NOT NULL,
  `AddressLine1` varchar(512) NOT NULL,
  `AddressLine2` varchar(512) DEFAULT NULL,
  `CityId` bigint(20) DEFAULT NULL,
  `StateId` bigint(20) DEFAULT NULL,
  `CountryId` bigint(20) NOT NULL,
  `ZipId` bigint(20) DEFAULT NULL,
  `CityName` varchar(512) DEFAULT NULL,
  `ZipCode` varchar(128) DEFAULT NULL,
  `IsDeleted` bit(1) NOT NULL DEFAULT b'0',
  `StateName` varchar(512) DEFAULT NULL,
  `EmailId` varchar(512) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `fk_addresslog_Lookup1_idx` (`TypeId`),
  KEY `fk_addresslog_City1_idx` (`CityId`),
  KEY `fk_addresslog_State1_idx` (`StateId`),
  KEY `fk_addresslog_Zip1_idx` (`ZipId`),
  KEY `fk_addresslog_Country1_idx` (`CountryId`),
  KEY `fk_addresslog_address_idx` (`addressId`),
  CONSTRAINT `fk_addresslog_City1` FOREIGN KEY (`CityId`) REFERENCES `city` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `fk_addresslog_Country1` FOREIGN KEY (`CountryId`) REFERENCES `country` (`Id`),
  CONSTRAINT `fk_addresslog_Lookup1` FOREIGN KEY (`TypeId`) REFERENCES `lookup` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `fk_addresslog_State1` FOREIGN KEY (`StateId`) REFERENCES `state` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `fk_addresslog_Zip1` FOREIGN KEY (`ZipId`) REFERENCES `zip` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `fk_addresslog_address` FOREIGN KEY (`addressId`) REFERENCES `address` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB AUTO_INCREMENT=659185 DEFAULT CHARSET=latin1;



ALTER TABLE `addresshistrylog`
ADD COLUMN `franchiseesalesId` BIGINT(20) NULL,
ADD INDEX `fk_addresshistrylog_franchiseeSales_lookup2_idx` (`franchiseesalesId`);
ALTER TABLE `addresshistrylog`
ADD CONSTRAINT `fk_addresshistrylog_franchiseeSales_lookup2`
  FOREIGN KEY (`franchiseesalesId`)
  REFERENCES `franchiseeSales` (`id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;

