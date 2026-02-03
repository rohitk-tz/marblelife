ALTER TABLE `bulkUploadImages` 
ADD COLUMN `JobId` BIGINT(20) NULL DEFAULT NULL,
ADD INDEX `fk_bulkUploadImages_jobschedulerJob_idx` (`JobId`);
ALTER TABLE `bulkUploadImages` 
ADD CONSTRAINT `fk_bulkUploadImages_jobschedulerJob`
  FOREIGN KEY (`JobId`)
  REFERENCES `jobscheduler` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;
  
ALTER TABLE `bulkUploadImages` 
ADD COLUMN `EstimateId` BIGINT(20) NULL DEFAULT NULL,
ADD INDEX `fk_bulkUploadImages_jobschedulerEstimate_idx` (`EstimateId`);
ALTER TABLE `bulkUploadImages` 
ADD CONSTRAINT `fk_bulkUploadImages_jobschedulerEstimate`
  FOREIGN KEY (`EstimateId`)
  REFERENCES `jobscheduler` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;
  
ALTER TABLE `bulkUploadImages` 
ADD COLUMN `SchedulerId` BIGINT(20) NULL DEFAULT NULL,
ADD INDEX `fk_bulkUploadImages_jobschedulerScheduler_idx` (`SchedulerId`);
ALTER TABLE `bulkUploadImages` 
ADD CONSTRAINT `fk_bulkUploadImages_jobschedulerScheduler`
  FOREIGN KEY (`SchedulerId`)
  REFERENCES `jobscheduler` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;
  
ALTER TABLE `bulkUploadImages` 
ADD COLUMN `BeforeAfterImageId` BIGINT(20) NULL DEFAULT NULL,
ADD INDEX `fk_bulkUploadImages_beforeafterimages_idx` (`BeforeAfterImageId`);
ALTER TABLE `bulkUploadImages` 
ADD CONSTRAINT `fk_bulkUploadImages_beforeafterimages`
  FOREIGN KEY (`BeforeAfterImageId`)
  REFERENCES `beforeafterimages` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;
  
ALTER TABLE bulkUploadImages
ADD IsCalendarImage bit(1) DEFAULT b'0';
