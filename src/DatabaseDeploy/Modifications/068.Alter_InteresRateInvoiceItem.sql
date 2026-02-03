ALTER TABLE `InterestRateInvoiceItem` 
CHANGE COLUMN `ExpectedDate` `StartDate` DATE NULL ,
ADD COLUMN `EndDate` DATE NULL DEFAULT NULL ;
