Alter table calldetailsreportnotes
Add column `MarketingLeadId` bigint(20) NULL default Null,
ADD INDEX `fk_calldetailsreportnotes_MarketingLead_idx` (`MarketingLeadId` ASC);
ALTER TABLE `calldetailsreportnotes` 
ADD CONSTRAINT `fk_calldetailsreportnotes_MarketingLead`
  FOREIGN KEY (`MarketingLeadId`)
  REFERENCES `marketingleadcalldetailv2` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;

ALTER TABLE `calldetailsreportnotes` 
ADD COLUMN `UserRole` varchar(128) NULL DEFAULT NULL;

ALTER TABLE `calldetailsreportnotes` 
ADD COLUMN `CreatedBy` varchar(128) NULL DEFAULT NULL;