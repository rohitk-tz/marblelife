CREATE TABLE `partialcustomeremailapirecord` (
  `ID` bigint(20) NOT NULL AUTO_INCREMENT,
  `CustomerId` bigint(20) NOT NULL,
  `FranchiseeId` bigint(20) NOT NULL,
  `CustomerEmail` varchar(512) NOT NULL,
  `apiCustomerId` varchar(512) DEFAULT NULL,
  `apiListId` varchar(512) DEFAULT NULL,
  `apiEmailId` varchar(512) DEFAULT NULL,
  `ErrorResponse` text,
  `status` varchar(218) DEFAULT NULL,
  `DateCreated` datetime DEFAULT NULL,
  `IsSynced` bit(1) NOT NULL DEFAULT b'0',
  `IsDeleted` bit(1) NOT NULL DEFAULT b'0',
  `IsFailed` bit(1) NOT NULL DEFAULT b'0',
  `InvoiceId` bigint(20) NOT NULL,
  PRIMARY KEY (`ID`),
  KEY `fk_customerEmailapiRecord_customers_idx` (`CustomerId`),
  KEY `fk_customerEmailApiRecord_Franchisees_idx` (`FranchiseeId`),
  CONSTRAINT `fk_customerEmailApisRecord_Franchisees` FOREIGN KEY (`FranchiseeId`) REFERENCES `franchisee` (`Id`),
  CONSTRAINT `fk_customerEmailapisRecord_customers` FOREIGN KEY (`CustomerId`) REFERENCES `customer` (`Id`),
  CONSTRAINT `fk_customerEmailapisRecord_Invoices` FOREIGN KEY (`InvoiceId`) REFERENCES `Invoice` (`Id`)
   ON DELETE NO ACTION
   ON UPDATE NO ACTION);