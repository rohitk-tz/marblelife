ALTER TABLE `markbeforeafterimageshistry` 
ADD COLUMN `categoryId` BIGINT(20) NULL DEFAULT 228 AFTER `IsDeleted`,
ADD INDEX `fk_markbeforeafterimageshistry_lookup_idx` (`categoryId` ASC);
ALTER TABLE `markbeforeafterimageshistry` 
ADD CONSTRAINT `fk_markbeforeafterimageshistry_lookup`
  FOREIGN KEY (`categoryId`)
  REFERENCES `lookup` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;