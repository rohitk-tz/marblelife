CREATE TABLE `emailSignatures` (
  `Id` bigint(20) NOT NULL AUTO_INCREMENT,
  `UserId` bigint(20) DEFAULT NULL,
  `OrganizationRoleUserId` bigint(20) DEFAULT NULL,
  `SignatureName` varchar(200) DEFAULT NULL,
  `Signature` text DEFAULT NULL,
  `IsDefault` bit(1) DEFAULT b'0',
  `IsDeleted` bit(1) DEFAULT b'0',
  `IsActive` bit(1) DEFAULT b'0',
  PRIMARY KEY (`Id`),
  KEY `fk_emailSignatures_UserId_idx` (`UserId`),
  KEY `OrganizationRoleUserId` (`OrganizationRoleUserId`),
  CONSTRAINT `fk_emailSignatures_UserId` FOREIGN KEY (`UserId`) REFERENCES `person` (`ID`),
  CONSTRAINT `emailSignatures_ibfk_1` FOREIGN KEY (`OrganizationRoleUserId`) REFERENCES `organizationroleuser` (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
