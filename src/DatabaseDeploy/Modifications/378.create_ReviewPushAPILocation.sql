CREATE TABLE `reviewPushAPILocation` (
  `Id` BIGINT(20) NOT NULL AUTO_INCREMENT,
  `Location_Id` BIGINT(20) NOT NULL ,
  `Rp_ID` BIGINT(20) NOT NULL ,
  `Name` varchar(100) NOT NULL ,
  `IsDeleted` BIT Not NULL DEFAULT 0 ,
  PRIMARY KEY (`Id`)
  );