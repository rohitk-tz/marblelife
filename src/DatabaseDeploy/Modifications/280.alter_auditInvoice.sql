SET SQL_SAFE_UPDATES = 0;
ALTER TABLE `auditinvoice` 
ADD COLUMN `isActive` BIT(1) NULL DEFAULT b'1' AFTER `QbInvoiceNumbers`;
SET SQL_SAFE_UPDATES = 1;


UPDATE `annualreporttype` SET `Description`='Split Invoice-Weekly Payment Exceeds Weekly' WHERE `Id`='19';


UPDATE `annualreporttype` SET `ReportTypeName`='Type 1B' WHERE `Id`='4';
