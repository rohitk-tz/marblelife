CREATE TABLE `annualroyality` (
  `Id` bigint(20) NOT NULL AUTO_INCREMENT,
  `FranchiseeId` bigint(20) NOT NULL,
  `date` datetime NOT NULL,
  `royality` decimal(10,2) default NULL,
  `sales` decimal(10,2) default NULL,
  `isMinRoyalityReached` BIT(1) NULL DEFAULT NULL,
  `IsDeleted` BIT(1) NULL DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `fk_annualroyality_franchiseeId_idx` (`FranchiseeId`),
  CONSTRAINT `fk_annualroyality_franchiseeId` FOREIGN KEY (`FranchiseeId`) REFERENCES `franchisee` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB AUTO_INCREMENT=106770 DEFAULT CHARSET=utf8

