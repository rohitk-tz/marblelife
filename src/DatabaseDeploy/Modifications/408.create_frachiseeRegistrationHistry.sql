CREATE TABLE `franchiseeRegistrationHistry` (
  `Id` bigint(20) NOT NULL AUTO_INCREMENT,
  `franchiseeId` bigint(20) NOT NULL,
  `RegistrationDate` date NOT NULL,
   `DataRecorderMetaDataId` bigint(20) NOT NULL,
   `IsDeleted` bit(1) NOT NULL DEFAULT b'0',
  PRIMARY KEY (`Id`),
  KEY `fk_frachiseeRegistrationHistry_Franchisee_idx` (`FranchiseeId`),
  CONSTRAINT `fk_frachiseeRegistrationHistry_Franchisee` FOREIGN KEY (`FranchiseeId`) REFERENCES `franchisee` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `fk_frachiseeRegistrationHistry_datarecordermetadata` FOREIGN KEY (`DataRecorderMetaDataId`) REFERENCES `datarecordermetadata` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB AUTO_INCREMENT=1 DEFAULT CHARSET=latin1