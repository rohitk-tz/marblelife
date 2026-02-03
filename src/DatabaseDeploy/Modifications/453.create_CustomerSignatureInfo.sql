CREATE TABLE `CustomerSignatureInfo` (
  `ID` bigint(20) NOT NULL AUTO_INCREMENT,
  `code` bigint(20) NOT NULL,
  `IsDeleted` bit(1) NOT NULL DEFAULT b'0',
   `IsActive` bit(1) NOT NULL DEFAULT b'1',
  `estimateinvoiceId` bigint(20) NOT NULL,
  PRIMARY KEY (`ID`),
  KEY `fk_CustomerSignatureInfo_estimateinvoice_idx` (`estimateinvoiceId`),
  CONSTRAINT `k_CustomerSignatureInfo_estimateinvoice` FOREIGN KEY (`estimateinvoiceId`) REFERENCES `estimateinvoice` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=latin1;


ALTER TABLE `makalu`.`customersignatureinfo` 
CHANGE COLUMN `code` `code` VARCHAR(100) NOT NULL ;
