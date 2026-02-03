CREATE TABLE `franchiseeDurationNotesHistry` (
  `Id` bigint(20) NOT NULL AUTO_INCREMENT,
  `franchiseeId` bigint(20) NOT NULL,
  `UserId` bigint(20) NOT NULL,
  `TypeId` bigint(20) NOT NULL,
  `Duration` bigint(20)  NULL,
   `description` varchar(1205)  NULL,
   `DataRecorderMetaDataId` bigint(20) NOT NULL,
   `StatusId` bigint(20) NOT NULL,
   `IsDeleted` bit(1) NOT NULL DEFAULT b'0',
   `RoleId` bigint(20) NOT NULL,
    `ApprovedById` bigint(20) NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `fk_franchiseeNotesHistry_Franchisee_idx` (`FranchiseeId`),
  KEY `fk_franchiseeNotesHistry_Person_idx` (`UserId`),
  KEY `fk_franchiseeNotesHistry_datarecordermetadata_idx` (`DataRecorderMetaDataId`),
  KEY `fk_franchiseeNotesHistry_Lookup_idx` (`TypeId`),
   KEY `fk_franchiseeNotesHistry_Status_Lookup_idx` (`StatusId`),
   KEY `fk_franchiseeNotesHistry_Role_idx` (`RoleId`),
   KEY `fk_franchiseeNotesHistry_ApprovedBy_Person_idx` (`ApprovedById`),
  CONSTRAINT `fk_franchiseeNotesHistry_Franchisee` FOREIGN KEY (`FranchiseeId`) REFERENCES `franchisee` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `fk_franchiseeNotesHistry_Person` FOREIGN KEY (`UserId`) REFERENCES `person` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `fk_franchiseeNotesHistry_datarecordermetadata` FOREIGN KEY (`DataRecorderMetaDataId`) REFERENCES `datarecordermetadata` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `fk_franchiseeNotesHistry_Lookup` FOREIGN KEY (`TypeId`) REFERENCES `Lookup` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
   CONSTRAINT `fk_franchiseeNotesHistry_Status_Lookup` FOREIGN KEY (`StatusId`) REFERENCES `lookup` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
   CONSTRAINT `fk_franchiseeNotesHistry_Role` FOREIGN KEY (`RoleId`) REFERENCES `Role` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
   CONSTRAINT `fk_franchiseeNotesHistry_ApprovedBy_Person` FOREIGN KEY (`ApprovedById`) REFERENCES `person` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB AUTO_INCREMENT=1 DEFAULT CHARSET=latin1;

ALTER TABLE `makalu`.`franchiseedurationnoteshistry` 
CHANGE COLUMN `Duration` `Duration` BIGINT(20) NULL ,
CHANGE COLUMN `description` `description` VARCHAR(1205) NULL ;

ALTER TABLE `makalu`.`franchiseedurationnoteshistry` 
DROP FOREIGN KEY `fk_franchiseeNotesHistry_ApprovedBy_Person`;
ALTER TABLE `makalu`.`franchiseedurationnoteshistry` 
CHANGE COLUMN `ApprovedById` `ApprovedById` BIGINT(20) NULL ;
ALTER TABLE `makalu`.`franchiseedurationnoteshistry` 
ADD CONSTRAINT `fk_franchiseeNotesHistry_ApprovedBy_Person`
  FOREIGN KEY (`ApprovedById`)
  REFERENCES `makalu`.`person` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;
