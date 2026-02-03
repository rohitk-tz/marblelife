ALTER TABLE `estimateinvoiceservice` 
ADD COLUMN `IsBundle` bit(1) default false;



ALTER TABLE `estimateinvoiceservice` 
ADD COLUMN `isActive` bit(1) default true;



ALTER TABLE `estimateinvoiceservice` 
ADD COLUMN `bundleName` varchar(300) NULL;

ALTER TABLE `estimateinvoiceservice` 
ADD COLUMN `IsMainBundle` bit(1) default false;