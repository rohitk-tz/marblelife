
ALTER TABLE `estimateinvoiceservice` 
ADD COLUMN `PriceNotes` text NULL  AFTER `IsDeleted`;


ALTER TABLE `estimateinvoice` 
ADD COLUMN `Notes` text NULL  AFTER `IsDeleted`;




ALTER TABLE `EstimateServiceInvoiceNotes` 
ADD COLUMN `InvoiceNumber` bigint(20) NOT NULL NULL  AFTER `EstimateInvoiceId`;


ALTER TABLE `makalu`.`estimateinvoice` 
CHANGE COLUMN `Notes` `Notes` VARCHAR(1024) NULL DEFAULT NULL ;


ALTER TABLE `makalu`.`estimateinvoiceservice` 
CHANGE COLUMN `PriceNotes` `PriceNotes` VARCHAR(1024) NULL DEFAULT NULL ;


ALTER TABLE `estimateinvoice` 
ADD COLUMN `Option1` text NULL  AFTER `IsDeleted`;

ALTER TABLE `estimateinvoice` 
ADD COLUMN `Option2` text NULL  AFTER `IsDeleted`;

ALTER TABLE `estimateinvoice` 
ADD COLUMN `Option3` text NULL  AFTER `IsDeleted`;