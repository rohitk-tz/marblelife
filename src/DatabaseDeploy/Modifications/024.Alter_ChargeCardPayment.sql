ALTER TABLE `ChargeCardPayment` 
DROP COLUMN `RawResponse`,
ADD COLUMN `Amount` DECIMAL(10,2) NOT NULL;
