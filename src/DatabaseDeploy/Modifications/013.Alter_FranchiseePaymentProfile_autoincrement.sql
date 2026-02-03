
ALTER TABLE `PaymentInstrument` 
DROP FOREIGN KEY `fk_PaymentInstrument_FranchiseePaymentProfile`;

ALTER TABLE `FranchiseePaymentProfile` 
DROP FOREIGN KEY `fk_FranchiseePaymentProfile_Franchisee`;

ALTER TABLE `FranchiseePaymentProfile` 
CHANGE COLUMN `Id` `Id` BIGINT(20) NOT NULL ;

ALTER TABLE `FranchiseePaymentProfile` 
ADD CONSTRAINT `fk_FranchiseePaymentProfile_Franchisee`
  FOREIGN KEY (`Id`)
  REFERENCES `Franchisee` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;

ALTER TABLE `PaymentInstrument` 
ADD CONSTRAINT `fk_PaymentInstrument_FranchiseePaymentProfile`
  FOREIGN KEY (`FranchiseePaymentProfileId`)
  REFERENCES `FranchiseePaymentProfile` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;
  

