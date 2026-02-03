
ALTER TABLE `PaymentInstrument` 
DROP FOREIGN KEY `fk_PaymentInstrument_organization1`,
DROP COLUMN `OrganizationId`,
DROP INDEX `fk_PaymentInstrument_organization1_idx` ;


ALTER TABLE `PaymentInstrument` 
CHANGE COLUMN `Id` `Id` BIGINT(20) NOT NULL AUTO_INCREMENT ;
