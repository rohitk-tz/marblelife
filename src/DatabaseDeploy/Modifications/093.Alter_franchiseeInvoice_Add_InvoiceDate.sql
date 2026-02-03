ALTER TABLE `FranchiseeInvoice` 
ADD COLUMN `InvoiceDate` DATETIME NULL DEFAULT NULL AFTER `id`;

SET SQL_SAFE_UPDATES = 0;
update franchiseeinvoice fi
inner join salesdataupload sd
on fi.salesdatauploadid = sd.id
set fi.invoiceDate = sd.periodEnddate
where sd.isdeleted = 0 and fi.isdeleted = 0;
SET SQL_SAFE_UPDATES = 1;