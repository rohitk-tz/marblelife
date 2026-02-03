ALTER TABLE `salesdataupload` 
ADD COLUMN `IsInvoiceGenerated` BIT(1) NOT NULL DEFAULT b'0' ;

UPDATE `salesdataupload` SET `IsInvoiceGenerated` = 1 WHERE `Id`>'0' and statusId = 72;