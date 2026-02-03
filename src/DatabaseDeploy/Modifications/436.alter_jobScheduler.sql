ALTER TABLE `jobscheduler` 
ADD COLUMN `InvoiceId` BIGINT(20) NULL DEFAULT null AFTER `ParentJobId`,
ADD INDEX `fk_jobScheduler_invoice_idx` (`InvoiceId` ASC);
ALTER TABLE `jobscheduler` 
ADD CONSTRAINT `fk_jobScheduler_invoice`
  FOREIGN KEY (`InvoiceId`)
  REFERENCES `Invoice` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;

  UPDATE `makalu`.`emailtemplate` SET `Subject` = 'RPID Missing for @Model.FranchiseeName' WHERE (`Id` = '49');
