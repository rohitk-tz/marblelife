INSERT INTO `makalu`.`lookuptype` (`Id`, `Name`, `IsDeleted`) VALUES ('34', 'TaskChoice', b'0');
INSERT INTO `makalu`.`lookup` (`Id`, `LookupTypeId`, `Name`, `Alias`, `RelativeOrder`, `IsActive`, `IsDeleted`) VALUES ('235', '34', 'FOLLOW-UP', 'FOLLOWUP', '1', b'1', b'0');
INSERT INTO `makalu`.`lookup` (`Id`, `LookupTypeId`, `Name`, `Alias`, `RelativeOrder`, `IsActive`, `IsDeleted`) VALUES ('236', '34', 'COLD-CALLING', 'COLDCALLING', '2', b'1', b'0');

ALTER TABLE `todofollowuplist` 
ADD COLUMN `TaskChoiceId` BIGINT(20) NULL DEFAULT null AFTER `IsDeleted`,
ADD INDEX `fk_todofollowuplist_lookUp_idx` (`TaskChoiceId` ASC);
ALTER TABLE `todofollowuplist` 
ADD CONSTRAINT `fk_todofollowuplist_lookUp`
  FOREIGN KEY (`TaskChoiceId`)
  REFERENCES `lookup` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;