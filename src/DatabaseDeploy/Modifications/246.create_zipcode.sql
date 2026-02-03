CREATE TABLE `zipcode` (
  `Id` bigint(20) NOT NULL AUTO_INCREMENT,
  `ZipCode` varchar(255) NOT NULL,
  `CountyId` bigint(20) DEFAULT NULL,
  `AreaCode` varchar(16) NOT NULL,
  `Direction` bigint(20) DEFAULT NULL,
  `DriveTest` bit(1) DEFAULT NULL,
  `Code` varchar(200) DEFAULT NULL,
  `IsDeleted` bit(1) DEFAULT b'0',
  `StateCode` varchar(16) DEFAULT NULL,
  `Dir` varchar(45) DEFAULT NULL,
  `CityId` bigint(20) DEFAULT NULL,
  `CityName` varchar(255) DEFAULT NULL,
  `CountyName` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  CONSTRAINT `fk_Zip_Code_Check_city` FOREIGN KEY (`CityId`) REFERENCES `City` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION
)