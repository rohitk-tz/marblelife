ALTER TABLE `jobestimate` 
ADD COLUMN `GeoCode` VARCHAR(128) NULL DEFAULT NULL,
ADD COLUMN `TypeId` BIGINT(20) NULL DEFAULT NULL ,
ADD INDEX `fk_jobEstimate_marketingClass_idx` (`TypeId` ASC);
ALTER TABLE `jobestimate` 
ADD CONSTRAINT `fk_jobEstimate_marketingClass`
  FOREIGN KEY (`TypeId`)
  REFERENCES `marketingclass` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;