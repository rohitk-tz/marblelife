ALTER TABLE `LateFeeInvoiceItem` 
CHANGE COLUMN `ExpectedDate` `StartDate` DATE NOT NULL ,
ADD COLUMN `EndDate` DATE NOT NULL ;
