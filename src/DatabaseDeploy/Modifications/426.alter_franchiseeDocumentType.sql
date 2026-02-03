ALTER TABLE `franchisedocument` 
ADD COLUMN `IsPerpetuity` bit(1) default false;


ALTER TABLE `franchisedocument` 
ADD COLUMN `IsRejected` bit(1) default false;


ALTER TABLE `franchiseedocumenttype` 
ADD COLUMN `IsPerpetuity` bit(1) default false;

ALTER TABLE `franchiseedocumenttype` 
ADD COLUMN `IsRejected` bit(1) default false;

ALTER TABLE `makalu`.`franchisedocument` 
DROP FOREIGN KEY `fk_franchiseDocument_file`;
ALTER TABLE `makalu`.`franchisedocument` 
CHANGE COLUMN `FileId` `FileId` BIGINT(20) NULL ;
ALTER TABLE `makalu`.`franchisedocument` 
ADD CONSTRAINT `fk_franchiseDocument_file`
  FOREIGN KEY (`FileId`)
  REFERENCES `makalu`.`file` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;
