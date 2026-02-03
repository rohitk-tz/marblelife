ALTER TABLE `adfundinvoiceitem` 
ADD COLUMN `Amount` DECIMAL(10,2) NOT NULL Default 0;


ALTER TABLE `royaltyinvoiceitem` 
ADD COLUMN `Amount` DECIMAL(10,2) NOT NULL Default 0;