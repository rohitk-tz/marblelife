ALTER TABLE `addresshistrylog` 
DROP FOREIGN KEY `fk_addresslog_Lookup1`;
ALTER TABLE `addresshistrylog` 
CHANGE COLUMN `TypeId` `TypeId` BIGINT(20) NULL DEFAULT NULL ;
ALTER TABLE `addresshistrylog` 
ADD CONSTRAINT `fk_addresslog_Lookup1`
  FOREIGN KEY (`TypeId`)
  REFERENCES `lookup` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;
