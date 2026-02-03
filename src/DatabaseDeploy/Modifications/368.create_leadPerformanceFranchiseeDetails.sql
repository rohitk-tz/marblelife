CREATE TABLE `leadPerformanceFranchiseeDetails` (
  `Id` bigint(20) NOT NULL AUTO_INCREMENT,
  `franchiseeId` bigint(20) NOT NULL,
  `categoryId` bigint(20) NOT NULL,
  `Amount` decimal(10,2) NOT NULL,
  `DateTime` date NOT NULL,  
  `Month` bigint(20) DEFAULT NULL,
  `IsActive` bit(1) NOT NULL DEFAULT b'1',
  `IsDeleted` bit(1) NOT NULL DEFAULT b'0',
   `DataRecorderMetaDataId` bigint(20) NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `fk_leadPerformanceFranchiseeDetails_Franchisee_idx` (`FranchiseeId`),
  CONSTRAINT `fk_leadPerformanceFranchiseeDetails_Franchisee` FOREIGN KEY (`FranchiseeId`) REFERENCES `franchisee` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `fk_leadPerformanceFranchiseeDetails_datarecordermetadata` FOREIGN KEY (`DataRecorderMetaDataId`) REFERENCES `datarecordermetadata` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB AUTO_INCREMENT=1 DEFAULT CHARSET=latin1