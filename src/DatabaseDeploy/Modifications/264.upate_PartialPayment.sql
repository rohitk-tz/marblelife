ALTER TABLE `partialpaymentemailapirecord` 
ADD COLUMN `statusId` BIGINT(20) DEFAULT '83',
ADD INDEX `fk_partialpaymentemailapirecord_lookup1_idx` (`statusId`);
ALTER TABLE `partialpaymentemailapirecord` 
ADD CONSTRAINT `fk_partialpaymentemailapirecord_lookup1_idx`
  FOREIGN KEY (`statusId`)
  REFERENCES `lookup` (`id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;  