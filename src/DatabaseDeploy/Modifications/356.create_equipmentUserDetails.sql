CREATE TABLE `equipmentUserDetails` (
  `Id` BIGINT(20) NOT NULL AUTO_INCREMENT ,
  `UserId` BIGINT(20) NULL ,
  `IsActive` BIT(1) NOT NULL DEFAULT b'0' ,
  `IsLock` BIT(1) NOT NULL DEFAULT b'0'  ,
  `IsDeleted` BIT(1) NOT NULL DEFAULT b'0' ,
  PRIMARY KEY (`Id`),
  INDEX `fk_equipmentUserDetails_Person_idx` (`UserId` ASC),
  CONSTRAINT `fk_equipmentUserDetails_Person`
 FOREIGN KEY (`UserId`)
 REFERENCES `Person` (`Id`)
 ON DELETE NO ACTION
 ON UPDATE NO ACTION);