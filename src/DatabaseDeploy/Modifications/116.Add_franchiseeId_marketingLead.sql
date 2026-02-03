ALTER TABLE `routingnumber` 
ADD COLUMN `FranchiseeId` BIGINT(20) NULL DEFAULT NULL AFTER `PhoneLabel`,
ADD INDEX `fk_RoutingNumber_Franchisee_idx` (`FranchiseeId` ASC)  ;
ALTER TABLE `routingnumber` 
ADD CONSTRAINT `fk_RoutingNumber_Franchisee`
  FOREIGN KEY (`FranchiseeId`)
  REFERENCES `franchisee` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;
  
  
  ALTER TABLE `marketingleadcalldetail` 
ADD COLUMN `FranchiseeId` BIGINT(20) NULL DEFAULT NULL AFTER `Id`,
ADD INDEX `fk_MarketingLeadCallDetail_Franchisee_idx` (`FranchiseeId` ASC) ;
ALTER TABLE `marketingleadcalldetail` 
ADD CONSTRAINT `fk_MarketingLeadCallDetail_Franchisee`
  FOREIGN KEY (`FranchiseeId`)
  REFERENCES `franchisee` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;

ALTER TABLE `weblead` 
ADD COLUMN `FranchiseeId` BIGINT(20) NULL DEFAULT NULL AFTER `WebLeadFranchiseeId`,
ADD INDEX `fk_webLeads_franchisee_idx` (`FranchiseeId` ASC) ;
ALTER TABLE `weblead` 
ADD CONSTRAINT `fk_webLeads_franchisee`
  FOREIGN KEY (`FranchiseeId`)
  REFERENCES `franchisee` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;

  
  
