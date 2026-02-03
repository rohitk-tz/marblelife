ALTER TABLE `franchisee` 
ADD COLUMN `Currency` VARCHAR(45) NOT NULL AFTER `IsDeleted`;

UPDATE `franchisee` SET `Currency`='USD' WHERE `Id`>0;

ALTER TABLE `address` 
ADD COLUMN `StateName` VARCHAR(512) NULL AFTER `IsDeleted`;


ALTER TABLE  `salesdataupload` 
ADD COLUMN `CurrencyExchangeRateId` BIGINT(20) NULL AFTER `IsDeleted`;

update `salesdataupload` set CurrencyExchangeRateId=1 where Id>0;

ALTER TABLE  `salesdataupload` 
CHANGE COLUMN `CurrencyExchangeRateId` `CurrencyExchangeRateId` BIGINT(20) NOT NULL ;



ALTER TABLE  `franchiseesales` 
ADD COLUMN `CurrencyExchangeRateId` BIGINT(20) NULL AFTER `IsDeleted`;

update `franchiseesales` set CurrencyExchangeRateId=1 where Id>0;

ALTER TABLE  `franchiseesales` 
CHANGE COLUMN `CurrencyExchangeRateId` `CurrencyExchangeRateId` BIGINT(20) NOT NULL ;



ALTER TABLE  `accountcredititem` 
ADD COLUMN `CurrencyExchangeRateId` BIGINT(20) NULL AFTER `IsDeleted`;

update `accountcredititem` set CurrencyExchangeRateId=1 where Id>0;

ALTER TABLE  `accountcredititem` 
CHANGE COLUMN `CurrencyExchangeRateId` `CurrencyExchangeRateId` BIGINT(20) NOT NULL ;



