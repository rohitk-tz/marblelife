CREATE TABLE `Check` (
  `Id` bigint(20) NOT NULL AUTO_INCREMENT,
  `CheckNumber` varchar(64) NOT NULL,
  `AccountTypeId` bigint(20) NOT NULL,
  `Name` varchar(128) DEFAULT NULL,
  `AccountNumber` varchar(64) NOT NULL,
  `IsDeleted` bit(1) NOT NULL DEFAULT b'0',
  PRIMARY KEY (`Id`),
  KEY `fk_Check_lookup1_idx` (`AccountTypeId`),
  CONSTRAINT `fk_Check_lookup1` FOREIGN KEY (`AccountTypeId`) REFERENCES `lookup` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION
);
