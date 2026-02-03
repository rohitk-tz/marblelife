
  ALTER TABLE `franchiseesales` 
ADD COLUMN `SubClassTypeId` BIGINT(20) NULL,
ADD INDEX `fk_franchiseesales_subClassTypeId_idx` (`SubClassTypeId`);
ALTER TABLE `franchiseesales` 
ADD CONSTRAINT `fk_franchiseesales_subClassTypeId`
  FOREIGN KEY (`SubClassTypeId`)
  REFERENCES `subclassmarketingclass` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;