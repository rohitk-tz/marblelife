TRUNCATE `calldetaildata`;

ALTER TABLE `calldetaildata` 
ADD COLUMN `PhoneNumber` VARCHAR(45) NOT NULL ;
