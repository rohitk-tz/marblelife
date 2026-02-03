CREATE TABLE `RoyaltyInvoiceItem` (
  `Id` bigint(20) NOT NULL,
  `Percentage` decimal(10, 2) NULL,
  `StartDate` date NOT NULL,
  `EndDate` date NOT NULL,
  `IsDeleted` bit(1) NOT NULL DEFAULT b'0',
  PRIMARY KEY (`Id`),
  KEY `fk_RoyaltyInvoiceItem_InvoiceItem_idx` (`Id`),
  CONSTRAINT `fk_RoyaltyInvoiceItem_InvoiceItem` FOREIGN KEY (`Id`) REFERENCES `InvoiceItem` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

CREATE TABLE `AdFundInvoiceItem` (
  `Id` bigint(20) NOT NULL,
  `Percentage` decimal(10, 2) Not NULL,
  `StartDate` date NOT NULL,
  `EndDate` date NOT NULL,
  `IsDeleted` bit(1) NOT NULL DEFAULT b'0',
  PRIMARY KEY (`Id`),
  KEY `fk_AdFundInvoiceItem_InvoiceItem_idx` (`Id`),
  CONSTRAINT `fk_AdFundInvoiceItem_InvoiceItem` FOREIGN KEY (`Id`) REFERENCES `InvoiceItem` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
