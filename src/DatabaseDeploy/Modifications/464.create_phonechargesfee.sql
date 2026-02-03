CREATE TABLE `phonechargesfee` (
  `Id` bigint(20) NOT NULL AUTO_INCREMENT,
  `Amount` decimal(10,2) NOT NULL,
  `FranchiseeId` bigint(20) NOT NULL,
  `InvoiceItemId` bigint(20) DEFAULT NULL,
  `Description` varchar(512) DEFAULT NULL,
  `IsDeleted` bit(1) NOT NULL DEFAULT b'0',
  `DateCreated` datetime NOT NULL,
  `CurrencyExchangeRateId` bigint(20) NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `fk_phonechargesfee_franchiseeId_idx` (`FranchiseeId`),
  KEY `fk_phonechargesfee_InvoiceItem_idx` (`InvoiceItemId`),
  KEY `fk_phonechargesfee_currencyExchangerate_idx` (`CurrencyExchangeRateId`),
  CONSTRAINT `fk_phonechargesfee_InvoiceItem` FOREIGN KEY (`InvoiceItemId`) REFERENCES `invoiceitem` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `fk_phonechargesfee_currencyExchangerate` FOREIGN KEY (`CurrencyExchangeRateId`) REFERENCES `currencyexchangerate` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `fk_phonechargesfee_franchiseeId` FOREIGN KEY (`FranchiseeId`) REFERENCES `franchisee` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB AUTO_INCREMENT=569 DEFAULT CHARSET=latin1