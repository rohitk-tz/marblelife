ALTER TABLE `onetimeprojectfee` 
ADD COLUMN `CurrencyExchangeRateId` BIGINT(20) NOT NULL,
ADD INDEX `fk_oneTimeProjectfee_currencyExchangerate_idx` (`CurrencyExchangeRateId` ASC);
ALTER TABLE `onetimeprojectfee` 
ADD CONSTRAINT `fk_oneTimeProjectfee_currencyExchangerate`
  FOREIGN KEY (`CurrencyExchangeRateId`)
  REFERENCES `currencyexchangerate` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;
  
ALTER TABLE `franchiseeloan` 
ADD COLUMN `CurrencyExchangeRateId` BIGINT(20) NOT NULL,
ADD INDEX `fk_franchiseeloan_currencyExchangeRate_idx` (`CurrencyExchangeRateId` ASC);
ALTER TABLE `franchiseeloan` 
ADD CONSTRAINT `fk_franchiseeloan_currencyExchangeRate`
  FOREIGN KEY (`CurrencyExchangeRateId`)
  REFERENCES `currencyexchangerate` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;
