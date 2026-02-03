CREATE TABLE `Perpetuitydatehistry` (
  `ID` bigint(20) NOT NULL AUTO_INCREMENT,
  `LastDateChecked` datetime NOT NULL,
  `DataRecorderMetaDataId` bigint(20) NOT NULL,
  `IsDeleted` bit(1) NOT NULL DEFAULT b'0',
  `IsPerpetuity` tinyint(1) DEFAULT NULL,
  `FranchiseeId` bigint(20) NOT NULL,
  PRIMARY KEY (`ID`),
  KEY `fk_perpetuitydatehistry_datarecordermetadata_idx` (`DataRecorderMetaDataId`),
  KEY `fk_perpetuitydatehistry_franchisee1_idx` (`FranchiseeId`),
  CONSTRAINT `fk_perpetuitydatehistry_datarecordermetadata` FOREIGN KEY (`DataRecorderMetaDataId`) REFERENCES `datarecordermetadata` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `fk_perpetuitydatehistry_franchisee1` FOREIGN KEY (`FranchiseeId`) REFERENCES `franchisee` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=latin1