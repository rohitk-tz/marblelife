CREATE TABLE `PaymentItem` (
  `Id` bigint(20) NOT NULL AUTO_INCREMENT,
  `PaymentId` bigint(20) NOT NULL,
  `ItemId` bigint(20) NOT NULL,
  `ItemTypeId` bigint(20) NOT NULL,
  `IsDeleted` bit(1) NOT NULL DEFAULT b'0',
  PRIMARY KEY (`Id`),
  KEY `fk_PaymentItem_Payment1_idx` (`PaymentId`),
  KEY `fk_PaymentItem_lookup1_idx` (`ItemTypeId`),
  KEY `fk_PaymentItem_ServiceType1_idx` (`ItemId`),
  CONSTRAINT `fk_PaymentItem_Payment1` FOREIGN KEY (`PaymentId`) REFERENCES `Payment` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `fk_PaymentItem_ServiceType1` FOREIGN KEY (`ItemId`) REFERENCES `ServiceType` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `fk_PaymentItem_lookup1` FOREIGN KEY (`ItemTypeId`) REFERENCES `Lookup` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
