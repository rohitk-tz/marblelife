ALTER TABLE `invoice` 
ADD COLUMN `customerInvoiceId` BIGINT(40) NULL DEFAULT 0 AFTER `isDeleted`;


ALTER TABLE `franchiseesales` 
ADD COLUMN `customerInvoiceId` BIGINT(40) NULL DEFAULT 0 AFTER `isDeleted`;


ALTER TABLE `invoice` 
ADD COLUMN `customerInvoiceIdString` varchar(100) NULL DEFAULT 0 AFTER `isDeleted`;

ALTER TABLE `franchiseesales` 
ADD COLUMN `customerInvoiceIdString` varchar(100) NULL DEFAULT 0 AFTER `isDeleted`;

ALTER TABLE `AuditFranchiseeSales` 
ADD COLUMN `customerInvoiceIdString` varchar(100) NULL DEFAULT 0 AFTER `isDeleted`;

ALTER TABLE `AuditFranchiseeSales` 
ADD COLUMN `customerInvoiceId` varchar(100) NULL DEFAULT 0 AFTER `isDeleted`;



ALTER TABLE `franchiseesales` 
ADD COLUMN `customerQbInvoiceId` BIGINT(40) NULL DEFAULT 0 AFTER `isDeleted`;
ALTER TABLE `franchiseesales` 
ADD COLUMN `customerQbInvoiceIdString` varchar(100) NULL DEFAULT 0 AFTER `isDeleted`;

ALTER TABLE `AuditFranchiseeSales` 
ADD COLUMN `customerQbInvoiceIdString` varchar(100) NULL DEFAULT 0 AFTER `isDeleted`;

ALTER TABLE `AuditFranchiseeSales` 
ADD COLUMN `customerQbInvoiceId` BIGINT(40) NULL DEFAULT 0 AFTER `isDeleted`;

ALTER TABLE `invoice` 
ADD COLUMN `customerQbInvoiceIdString` varchar(100) NULL DEFAULT 0 AFTER `isDeleted`;
ALTER TABLE `invoice` 
ADD COLUMN `customerQbInvoiceId` BIGINT(40) NULL DEFAULT 0 AFTER `isDeleted`;


ALTER TABLE `AuditInvoice` 
ADD COLUMN `customerQbInvoiceId` BIGINT(40) NULL DEFAULT null AFTER `isDeleted`;
ALTER TABLE `AuditFranchiseeSales` 
ADD COLUMN `customerQbInvoiceId` BIGINT(40) NULL DEFAULT null AFTER `isDeleted`;