ALTER TABLE `jobestimate` 
ADD COLUMN  `StartDate` datetime NOT NULL;


ALTER TABLE `jobestimate` 
ADD COLUMN `EndDate` datetime NOT NULL;
ALTER TABLE `jobestimate` 
ADD COLUMN `ParentEstimateId` BIGINT(20) NULL DEFAULT NULL,
ADD INDEX `fk_jobestimate_jobestimate_idx` (`ParentEstimateId`);
ALTER TABLE `jobestimate` 
ADD CONSTRAINT `fk_jobestimate_jobestimate`
  FOREIGN KEY (`ParentEstimateId`)
  REFERENCES `jobestimate` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;
  
