 CREATE TABLE `jobEstimateServices` (
  `Id` bigint(20) NOT NULL AUTO_INCREMENT,
  `CategoryId` bigint(20) NULL,
  `SurfaceColor` varchar(512) NOT NULL,
  `FinishMaterial` varchar(512) NOT NULL,
  `SurfaceMaterial` varchar(512) DEFAULT NULL,
  `SurfaceType` varchar(512) DEFAULT NULL,
  `DataRecorderMetaDataId` bigint(20)  NULL,
  `PairId` bigint(20)  NULL,
  `BuildingLocation` varchar(512) DEFAULT NULL,
  `ServiceTypeId` bigint(20) NOT NULL,
`IsDeleted` bit(1) NOT NULL DEFAULT b'0',
  PRIMARY KEY (`Id`),
  KEY `fk_jobEstimateServices_jobEstimateimageCategory_idx` (`CategoryId`),
  KEY `fk_jobEstimateServices_dataRecorderMetaData_idx` (`DataRecorderMetaDataId`),
  KEY `fk_jobEstimateServices_PairId_idx` (`PairId`),
  CONSTRAINT `fk_jobEstimateServices_dataRecorderMetaData` FOREIGN KEY (`DataRecorderMetaDataId`) REFERENCES `dataRecorderMetaData` (`Id`),
  CONSTRAINT `fk_jobEstimateServices_PairId` FOREIGN KEY (`PairId`) REFERENCES `jobEstimateServices` (`Id`),
  CONSTRAINT `fk_jobEstimateServices_CategoryId` FOREIGN KEY (`CategoryId`) REFERENCES `jobEstimateimageCategory` (`Id`),
   CONSTRAINT `fk_jobEstimateServices_servicetype` FOREIGN KEY (`ServiceTypeId`) REFERENCES `servicetype` (`Id`)
);




ALTER TABLE `jobEstimateServices` 
ADD COLUMN `description` varchar(1024) NULL DEFAULT NULL;


ALTER TABLE `jobEstimateServices` 
DROP FOREIGN KEY `fk_jobEstimateServices_servicetype`;
ALTER TABLE `jobEstimateServices` 
CHANGE COLUMN `ServiceTypeId` `ServiceTypeId` BIGINT(20) NULL DEFAULT NULL ;
ALTER TABLE `jobEstimateServices` 
ADD CONSTRAINT `fk_jobEstimateImageServices_servicetype`
  FOREIGN KEY (`ServiceTypeId`)
  REFERENCES `servicetype` (`Id`);

