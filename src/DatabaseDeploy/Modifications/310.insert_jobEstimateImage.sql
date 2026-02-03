 CREATE TABLE `jobEstimateImage` (
  `Id` bigint(20) NOT NULL AUTO_INCREMENT,
  `ServiceId` bigint(20) NULL,
  `FileId` bigint(20)  NULL,
  `IsDeleted` bit(1) NOT NULL DEFAULT b'0',
   `TypeId` bigint(20)  NULL,
  PRIMARY KEY (`Id`),
  KEY `fk_jobEstimateImage_file_idx` (`FileId`),
  KEY `fk_jobEstimateImage_Lookup_idx` (`TypeId`),
  KEY `fk_jobEstimateImage_jobEstimateServices_idx` (`ServiceId`),
  CONSTRAINT `fk_jobEstimateImage_TypeId` FOREIGN KEY (`TypeId`) REFERENCES `lookup` (`Id`),
   CONSTRAINT `fk_jobEstimateImage_file` FOREIGN KEY (`FileId`) REFERENCES `File` (`Id`),
   CONSTRAINT `fk_jobEstimateImage_servicetype` FOREIGN KEY (`ServiceId`) REFERENCES `jobEstimateServices` (`Id`),
     CONSTRAINT `fk_jobEstimateImage_Looku[` FOREIGN KEY (`TypeId`) REFERENCES `Lookup` (`Id`)
);



ALTER TABLE `jobestimateservices` 
ADD COLUMN `TypeId` BIGINT(20) NULL,
ADD INDEX `fk_jobestimateservices_Lookup_idx` (`TypeId`);
ALTER TABLE `jobestimateservices` 
ADD CONSTRAINT `fk_jobestimateservices_Lookup`
  FOREIGN KEY (`TypeId`)
  REFERENCES `Lookup` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;

  ALTER TABLE `jobestimateservices` 
CHANGE COLUMN `SurfaceColor` `SurfaceColor` VARCHAR(512) NULL DEFAULT NULL ,
CHANGE COLUMN `FinishMaterial` `FinishMaterial` VARCHAR(512) NULL DEFAULT NULL ;
