CREATE TABLE `auditcustomer` (
  `Id` bigint(20) NOT NULL AUTO_INCREMENT,
  `Name` varchar(128) NOT NULL,
  `ContactPerson` varchar(128) DEFAULT NULL,
  `AuditAddressId` bigint(20) DEFAULT NULL,
  `Phone` varchar(16) DEFAULT NULL,
  `IsDeleted` bit(1) NOT NULL DEFAULT b'0',
  `ClassTypeId` bigint(20) NOT NULL,
  `ReceiveNotification` bit(1) DEFAULT b'0',
  `DateCreated` datetime DEFAULT NULL,
  `TotalSales` decimal(10,2) DEFAULT NULL,
  `NoOfSales` int(11) DEFAULT NULL,
  `AvgSales` decimal(10,2) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `fk_auditcustomer_address1_idx` (`AuditAddressId`),
  CONSTRAINT `fk_auditcustomer_address1` FOREIGN KEY (`AuditAddressId`) REFERENCES `Auditaddress` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB AUTO_INCREMENT=66179 DEFAULT CHARSET=utf8