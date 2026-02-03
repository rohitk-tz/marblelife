ALTER TABLE `FranchiseeAccountCredit` 
ADD COLUMN `CurrencyExchangeRateId` BIGINT(20) ;

UPDATE `franchiseeaccountcredit` SET `CurrencyExchangeRateId`='1' WHERE `Id`>'0';

ALTER TABLE `FranchiseeAccountCredit` 
ADD CONSTRAINT `FK_FranchiseeAccountCredit_Currencyexchange`
  FOREIGN KEY (`CurrencyExchangeRateId`)
  REFERENCES `CurrencyExchangeRate` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;