ALTER TABLE `weblead` 
ADD COLUMN `InvoiceId` BIGINT(20) NULL DEFAULT NULL AFTER `FEmail`;

ALTER TABLE `marketingleadcalldetail` 
ADD COLUMN `InvoiceId` BIGINT(20) NULL DEFAULT NULL AFTER `CallDuration`;


