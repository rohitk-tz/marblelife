CREATE TABLE `MinRoyaltyFeeSlabs` (
  `Id` bigint(20) NOT NULL AUTO_INCREMENT,
   `FranchiseeId` bigint(20) NOT NULL,
  `StartValue` decimal(18,2) DEFAULT NULL,
  `EndValue` decimal(18,2) DEFAULT NULL,
  `MinRoyality` decimal(10,2) NOT NULL,
  `IsDeleted` bit(1) NOT NULL DEFAULT b'0',
  PRIMARY KEY (`Id`),
  KEY `fk_MinRoyaltyFeeSlabs_franchisee1_idx` (`FranchiseeId`),
 CONSTRAINT `fk_MinRoyaltyFeeSlabs_franchisee1` FOREIGN KEY (`FranchiseeId`) REFERENCES `franchisee` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB AUTO_INCREMENT=234 DEFAULT CHARSET=latin1