ALTER TABLE `FranchiseeAuthNetProfile` 
RENAME TO  `FranchiseePaymentProfile` ;


ALTER TABLE `FranchiseePaymentProfile` 
DROP FOREIGN KEY `fk_FranchiseeAuthNetProfile_Franchisee`;
ALTER TABLE `FranchiseePaymentProfile` 
ADD CONSTRAINT `fk_FranchiseePaymentProfile_Franchisee`
  FOREIGN KEY (`MerchantCustomerId`)
  REFERENCES `Franchisee` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;


  ALTER TABLE `FranchiseePaymentProfile` 
DROP FOREIGN KEY `fk_FranchiseePaymentProfile_Franchisee`;
ALTER TABLE `FranchiseePaymentProfile` 
DROP INDEX `fk_FranchiseeAuthNetProfile_Franchisee_idx` ;
ALTER TABLE `FranchiseePaymentProfile` 
ADD CONSTRAINT `fk_FranchiseePaymentProfile_Franchisee`
  FOREIGN KEY (`Id`)
  REFERENCES `Franchisee` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;


  ALTER TABLE `FranchiseePaymentProfile` 
DROP COLUMN `MerchantCustomerId`;

