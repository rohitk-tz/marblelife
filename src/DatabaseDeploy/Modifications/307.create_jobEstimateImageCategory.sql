 CREATE TABLE `jobestimateimagecategory` (
  `Id` bigint(20) NOT NULL AUTO_INCREMENT,
  `JobId` bigint(20)  NULL,
  `EstimateId` bigint(20)  NULL,
   `MarkertingClassId` bigint(20) NOT NULL,
   `SchedulerId` bigint(20)  NULL,
  `IsDeleted` bit(1) NOT NULL DEFAULT b'0',
  PRIMARY KEY (`Id`),
  KEY `fk_jobEstimateImagePairing_job_idx` (`JobId`),
  KEY `fk_jobEstimateImagePairing_estimate_idx` (`EstimateId`),
  KEY `fk_jobEstimateimageCategory_marketingclass_idx` (`MarkertingClassId`),
   KEY `fk_jobEstimateimageCategory_jobScheduler_idx` (`SchedulerId`),
  CONSTRAINT `fk_jobEstimateimageCategory_marketingclass` FOREIGN KEY (`MarkertingClassId`) REFERENCES `marketingclass` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `fk_jobEstimateImagePairing_jobScheduler1` FOREIGN KEY (`SchedulerId`) REFERENCES `jobScheduler` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `fk_jobEstimateImagePairing_job` FOREIGN KEY (`JobId`) REFERENCES `jobScheduler` (`JobId`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `fk_jobEstimateImagePairing_estimate` FOREIGN KEY (`EstimateId`) REFERENCES `jobScheduler` (`EstimateId`)
);