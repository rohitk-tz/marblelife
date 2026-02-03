ALTER TABLE `emailtemplate` 
ADD COLUMN `languageId` BIGINT(20) NULL DEFAULT 249 AFTER `isDeleted`,
ADD INDEX `fk_emailtemplate_lookups_idx` (`languageId` ASC);
ALTER TABLE `emailtemplate` 
ADD CONSTRAINT `fk_emailtemplate_lookups`
  FOREIGN KEY (`languageId`)
  REFERENCES `lookup` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;


  ALTER TABLE `franchisee` 
ADD COLUMN `languageId` BIGINT(20) NULL DEFAULT 249 AFTER `isDeleted`,
ADD INDEX `fk_franchisee_lookups_idx` (`languageId` ASC);
ALTER TABLE `franchisee` 
ADD CONSTRAINT `fk_franchisee_lookups`
  FOREIGN KEY (`languageId`)
  REFERENCES `lookup` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;