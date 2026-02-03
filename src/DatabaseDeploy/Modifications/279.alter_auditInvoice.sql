ALTER TABLE `auditinvoice` 
ADD COLUMN `QbInvoiceNumbers` VARCHAR(45) NULL DEFAULT NULL AFTER `ReportTypeId`;
