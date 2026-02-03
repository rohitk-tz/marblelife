CREATE TABLE `mlfsConfigurationSetting`(
  `Id` BIGINT(20) NOT NULL AUTO_INCREMENT,
  `UserId` BIGINT(20) NOT NULL ,
  `Status` VARCHAR(200) NOT NULL ,
   `ColorCode` VARCHAR(200) NOT NULL ,
   `MinValue` BIGINT(20) NOT NULL ,  
   `MaxValue` BIGINT(20) NOT NULL ,  
   `IsDeleted` BIT Not NULL DEFAULT 0 ,
   `IsActive` BIT Not NULL DEFAULT 0 ,
  
  PRIMARY KEY (`Id`),
  KEY `fk_reviewPushCustomerFeedback_franchisee_idx` (`Id`),
  CONSTRAINT `fk_mlfsConfigurationSetting_peron` FOREIGN KEY (`UserId`) REFERENCES `Person` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION
  );

  UPDATE `makalu`.`notificationtype` SET `IsServiceEnabled` = b'0' WHERE (`Id` = '7');
