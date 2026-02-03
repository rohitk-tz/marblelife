INSERT INTO `lookuptype` (`Id`, `Name`, `IsDeleted`) VALUES ('19', 'AuditActionType', 0);

INSERT INTO `lookup` (`Id`, `LookupTypeId`, `Name`, `Alias`, `RelativeOrder`, `IsActive`, `IsDeleted`) VALUES ('151', '19', 'Accepted', 'Accepted', '1', 1, 0);
INSERT INTO `lookup` (`Id`, `LookupTypeId`, `Name`, `Alias`, `RelativeOrder`, `IsActive`, `IsDeleted`) VALUES ('152', '19', 'Rejected', 'Rejected', '2', 1, 0);
INSERT INTO `lookup` (`Id`, `LookupTypeId`, `Name`, `Alias`, `RelativeOrder`, `IsActive`, `IsDeleted`) VALUES ('153', '19', 'Pending', 'Pending', '3', 1, 0);


ALTER TABLE `annualsalesdataupload` 
ADD COLUMN `AuditActionId` BIGINT(20) NULL,
ADD INDEX `fk_annualSalesdataUpload_lookup1_idx` (`AuditActionId` ASC);
ALTER TABLE `annualsalesdataupload` 
ADD CONSTRAINT `fk_annualSalesdataUpload_lookup1`
  FOREIGN KEY (`AuditActionId`)
  REFERENCES `lookup` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;  

update annualsalesdataupload set AuditActionId = 153 where id > 0;

ALTER TABLE `annualsalesdataupload` 
DROP FOREIGN KEY `fk_annualSalesdataUpload_lookup1`;
ALTER TABLE `annualsalesdataupload` 
CHANGE COLUMN `AuditActionId` `AuditActionId` BIGINT(20) NOT NULL ;
ALTER TABLE `annualsalesdataupload` 
ADD CONSTRAINT `fk_annualSalesdataUpload_lookup1`
  FOREIGN KEY (`AuditActionId`)
  REFERENCES `lookup` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;


