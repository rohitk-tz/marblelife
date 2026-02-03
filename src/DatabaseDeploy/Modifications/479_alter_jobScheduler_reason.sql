ALTER TABLE `jobscheduler` 
ADD COLUMN `InvoiceReason` VARCHAR(9200) default null;

ALTER TABLE `jobestimateservices` 
ADD COLUMN `IsFromInvoiceAttach` bit(1) default false;