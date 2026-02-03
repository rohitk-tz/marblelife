
ALTER TABLE `Chargecardpayment` 
DROP FOREIGN KEY `fk_ChargeCardPayment_ChargeCard1`;

ALTER TABLE `chargecard` 
CHANGE COLUMN `Id` `Id` BIGINT(20) NOT NULL AUTO_INCREMENT ;

ALTER TABLE `ChargeCardPayment` 
ADD CONSTRAINT `fk_ChargeCardPayment_ChargeCard1`
  FOREIGN KEY (`ChargeCardId`)
  REFERENCES `ChargeCard` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;


ALTER TABLE `PaymentInstrument` 
ADD COLUMN `FranchiseeAuthNetProfileId` BIGINT(20) NOT NULL AFTER `IsDeleted`,
ADD INDEX `fk_PaymentInstrument_FranchiseeAutNetProfile_idx` (`FranchiseeAuthNetProfileId` ASC) ;
ALTER TABLE `paymentInstrument` 
ADD CONSTRAINT `fk_PaymentInstrument_FranchiseeAutNetProfile`
  FOREIGN KEY (`FranchiseeAuthNetProfileId`)
  REFERENCES `FranchiseeAuthNetProfile` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;



