CREATE TABLE `EstimateInvoiceServiceDescription` (
  `ID` bigint(20) NOT NULL AUTO_INCREMENT,
  `ServiceType` bigint(20) NOT NULL,
  `IsDeleted` bit(1) NOT NULL DEFAULT b'0',
  `Description`  varchar(1024) DEFAULT NULL,
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=latin1;


ALTER TABLE `makalu`.`franchisee` 
ADD COLUMN `lessDeposit` DECIMAL(10,2) NULL DEFAULT 50 ;

UPDATE `makalu`.`franchisee` SET `lessDeposit` = '10.00' WHERE (`Id` = '5');
UPDATE `makalu`.`franchisee` SET `lessDeposit` = '10.00' WHERE (`Id` = '6');
UPDATE `makalu`.`franchisee` SET `lessDeposit` = '10.00' WHERE (`Id` = '10');
UPDATE `makalu`.`franchisee` SET `lessDeposit` = '10.00' WHERE (`Id` = '76');
UPDATE `makalu`.`franchisee` SET `lessDeposit` = '10.00' WHERE (`Id` = '9');






ALTER TABLE `makalu`.`estimateinvoicecustomer` 
CHANGE COLUMN `StateName` `StateName` VARCHAR(100) NULL ;
