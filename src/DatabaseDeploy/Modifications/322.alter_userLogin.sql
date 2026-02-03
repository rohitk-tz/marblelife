ALTER TABLE `userlogin` 
DROP COLUMN `Offset`;


ALTER TABLE `userlogin` 
ADD COLUMN `ESTOffset` double DEFAULT NULL;

ALTER TABLE `userlogin` 
ADD COLUMN `EDTOffset` double DEFAULT NULL;