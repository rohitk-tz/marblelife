ALTER TABLE `franchiseeaccountcredit` 
DROP FOREIGN KEY `FK_FranchiseeAccountCredit_Invoice`;

ALTER TABLE `franchiseeaccountcredit` 
CHANGE COLUMN `InvoiceId` `InvoiceId` BIGINT(20) NULL DEFAULT NULL ;

