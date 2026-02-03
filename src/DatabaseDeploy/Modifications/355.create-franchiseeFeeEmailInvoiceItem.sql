CREATE TABLE `FranchiseeFeeEmailInvoiceItem` (
  `Id` bigint(20) NOT NULL,
  `Percentage` decimal(10,2) NOT NULL,
  `StartDate` date NOT NULL,
  `EndDate` date NOT NULL,
  `IsDeleted` bit(1) NOT NULL DEFAULT b'0',
  `Amount` decimal(10,2) NOT NULL DEFAULT '0.00',
  PRIMARY KEY (`Id`),
  KEY `fk_FranchiseeFeeEmailInvoiceItem_InvoiceItem_idx` (`Id`),
  CONSTRAINT `fk_FranchiseeFeeEmailInvoiceItem_InvoiceItem` FOREIGN KEY (`Id`) REFERENCES `invoiceitem` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8