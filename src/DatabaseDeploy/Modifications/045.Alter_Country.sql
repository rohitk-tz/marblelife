
ALTER TABLE `address` 
DROP FOREIGN KEY `fk_Address_Country1`;

ALTER TABLE `address` 
DROP INDEX `fk_Address_Country1_idx` ;


ALTER TABLE `state` 
DROP FOREIGN KEY `fk_State_Country1`;

ALTER TABLE `state` 
DROP INDEX `fk_State_Country1_idx` ;


ALTER TABLE Country
CHANGE COLUMN `Id` `Id` BIGINT(20) NOT NULL ;


ALTER TABLE `address` 
ADD INDEX `fk_Address_Country1_idx` (`CountryId` ASC);

ALTER TABLE `address` 
ADD CONSTRAINT `fk_Address_Country1`
  FOREIGN KEY (`CountryId`)
  REFERENCES `country` (`Id`);

ALTER TABLE `state` 
ADD INDEX `fk_State_Country1_idx` (`CountryId` ASC);

ALTER TABLE `state` 
ADD CONSTRAINT `fk_State_Country1`
  FOREIGN KEY (`CountryId`)
  REFERENCES `country` (`Id`);


Alter Table Country ADD COLUMN `CurrencyCode` varchar(3);

Update Country set CurrencyCode = 'USD' where Id = 1;

ALTER TABLE Country
   MODIFY CurrencyCode  varchar(3) NOT NULL;