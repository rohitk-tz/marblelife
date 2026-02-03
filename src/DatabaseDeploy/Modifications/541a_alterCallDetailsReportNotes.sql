Alter table calldetailsreportnotes
Add column `MarketingLeadIdFromCallDetailsReport` bigint(20) NULL default Null,
ADD INDEX `fk_calldetailsreportnotes_MarketingLead2_idx` (`MarketingLeadIdFromCallDetailsReport` ASC);
ALTER TABLE `calldetailsreportnotes` 
ADD CONSTRAINT `fk_calldetailsreportnotes_MarketingLead2`
  FOREIGN KEY (`MarketingLeadIdFromCallDetailsReport`)
  REFERENCES `marketingleadcalldetail` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;