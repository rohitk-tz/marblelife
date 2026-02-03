CREATE TABLE `franchiseedocumenttype` (
  `Id` bigint(20) NOT NULL AUTO_INCREMENT,
  `FranchiseeId` bigint(20) NOT NULL,
  `DocumentTypeId` bigint(20) NOT NULL,
  `IsActive` bit(1) NOT NULL DEFAULT b'1',
  `IsDeleted` bit(1) NOT NULL DEFAULT b'0',
  PRIMARY KEY (`Id`),
  KEY `fk_FranchiseeDocument_franchisee1_idx` (`FranchiseeId`),
  KEY `fk_FranchiseeDocument_DocumentType1_idx` (`DocumentTypeId`),
  CONSTRAINT `fk_FranchiseeDocument_DocumentType1_idx` FOREIGN KEY (`DocumentTypeId`) REFERENCES `documenttype` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `fk_FranchiseeDocument_franchisee1_idx` FOREIGN KEY (`FranchiseeId`) REFERENCES `organization` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION
)