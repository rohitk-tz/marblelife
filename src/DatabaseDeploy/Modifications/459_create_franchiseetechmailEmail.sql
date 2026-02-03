CREATE TABLE `franchiseetechmailEmail` (
  `ID` bigint(20) NOT NULL AUTO_INCREMENT,
  `FranchiseeId` bigint(20) NOT NULL,
  `ChargesForPhone` decimal(10,2) DEFAULT '15.00',
  `IsDeleted` bit(1) NOT NULL DEFAULT b'0',
  `UserId` bigint(20) NOT NULL,
  `DataRecorderMetaDataId` bigint(20) NOT NULL,
  PRIMARY KEY (`ID`),
  KEY `fk_franchiseetechmailEmail_datarecordermetadata_idx` (`DataRecorderMetaDataId`),
  KEY `FK_franchiseetechmailEmail_type_idx` (`ID`),
  KEY `FK_franchiseetechmailEmail_franchisee_idx` (`FranchiseeId`),
   KEY `FK_franchiseetechmailEmail_person_idx` (`UserId`),
  CONSTRAINT `FK_franchiseetechmailEmail_franchisee` FOREIGN KEY (`FranchiseeId`) REFERENCES `franchisee` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `FK_franchiseetechmailEmail_person` FOREIGN KEY (`UserId`) REFERENCES `person` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
   CONSTRAINT `fk_franchiseetechmailEmail_datarecordermetadata` FOREIGN KEY (`DataRecorderMetaDataId`) REFERENCES `datarecordermetadata` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=latin1