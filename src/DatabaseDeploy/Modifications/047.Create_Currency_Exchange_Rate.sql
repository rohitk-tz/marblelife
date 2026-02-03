CREATE TABLE `CurrencyExchangeRate` (
  `Id` bigint(20) NOT NULL AUTO_INCREMENT,
  `CountryId` bigint(20) Not NULL,
  `Rate` decimal(10,4) NOT NULL,
  `Date` DATE NOT NULL ,
  `IsDeleted` bit(1) NOT NULL DEFAULT b'0',
  PRIMARY KEY (`Id`),
  KEY `fk_CurrencyExchangeRate_Country1_idx` (`CountryId`),
  CONSTRAINT `fk_CurrencyExchangeRate_Country1` FOREIGN KEY (`CountryId`) REFERENCES `Country` (`Id`) 
  ON DELETE NO ACTION 
  ON UPDATE NO ACTION)
ENGINE=InnoDB DEFAULT CHARSET=utf8;


INSERT INTO CurrencyExchangeRate(CountryId, Rate,Date) values(1,1,'2016-11-16');


ALTER TABLE  `country` 
ADD COLUMN `IsDefault` BIT NULL AFTER `CurrencyCode`;

UPDATE `country` SET `IsDefault`=1 WHERE `Id`='1';
UPDATE `country` SET `IsDefault`=0 WHERE `Id`>1;

