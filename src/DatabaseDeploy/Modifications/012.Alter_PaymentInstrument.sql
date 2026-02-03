
ALTER TABLE `PaymentInstrument` 
DROP FOREIGN KEY `fk_PaymentInstrument_FranchiseeAutNetProfile`;
ALTER TABLE `PaymentInstrument` 
CHANGE COLUMN `FranchiseeAuthNetProfileId` `FranchiseePaymentProfileId` BIGINT(20) NOT NULL ;
ALTER TABLE `PaymentInstrument` 
ADD CONSTRAINT `fk_PaymentInstrument_FranchiseeAutNetProfile`
  FOREIGN KEY (`FranchiseePaymentProfileId`)
  REFERENCES `FranchiseePaymentProfile` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;


  ALTER TABLE `PaymentInstrument` 
DROP FOREIGN KEY `fk_PaymentInstrument_FranchiseeAutNetProfile`;
ALTER TABLE `PaymentInstrument` 
ADD CONSTRAINT `fk_PaymentInstrument_FranchiseePaymentProfile`
  FOREIGN KEY (`FranchiseePaymentProfileId`)
  REFERENCES `FranchiseePaymentProfile` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;


  ALTER TABLE `PaymentInstrument` 
ADD COLUMN `InstrumentProfileId` VARCHAR(128) NOT NULL;




