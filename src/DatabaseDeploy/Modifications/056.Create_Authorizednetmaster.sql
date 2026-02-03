CREATE TABLE `authorizenetapimaster` (
  `Id` BIGINT NOT NULL,
  `ApiLoginID` VARCHAR(45) NOT NULL,
  `ApiTransactionKey` VARCHAR(45) NOT NULL,
  `AccountTypeId` BIGINT(20) NOT NULL ,
  `IsDeleted` BIT NOT NULL DEFAULT b'0',
  PRIMARY KEY (`Id`),
  INDEX `fk_authorizenetapimaster_accounttype_idx` (`AccountTypeId` ASC),
  CONSTRAINT `fk_authorizenetapimaster_accounttype`
  FOREIGN KEY (`AccountTypeId`)
  REFERENCES `lookup` (`Id`)
  );

INSERT INTO `authorizenetapimaster` (`Id`, `ApiLoginID`, `ApiTransactionKey`,`AccountTypeId`, `IsDeleted`) VALUES ('1', '6aTb6993EkM', '9ZE2Gzt9djY49L4a',130,0);
INSERT INTO `authorizenetapimaster` (`Id`, `ApiLoginID`, `ApiTransactionKey`,`AccountTypeId`, `IsDeleted`) VALUES ('2', '9Ekt39v5U6', '5tg69jhT3RE2xN9R',131,0);


ALTER TABLE `currencyexchangerate` 
CHANGE COLUMN `Date` `DateTime` DATETIME NOT NULL ;