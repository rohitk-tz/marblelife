CREATE TABLE `jobscheduler` (
  `Id` BIGINT(20) NOT NULL AUTO_INCREMENT ,
  `JobId` BIGINT(20) NULL DEFAULT NULL ,
  `Title` VARCHAR(512) NOT NULL ,
  `EstimateId` BIGINT(20) NULL DEFAULT NULL ,
  `FranchiseeId` BIGINT(20) NOT NULL ,
  `SalesRepId` BIGINT(20) NULL DEFAULT NULL ,
  `AssigneeId` BIGINT(20) NULL DEFAULT NULL ,
  `IsActive` BIT(1) NOT NULL DEFAULT b'1' ,
  `IsDeleted` BIT(1) NOT NULL DEFAULT b'0' ,
  PRIMARY KEY (`Id`)  ,
  INDEX `fk_jobScheduler_job_idx` (`JobId` ASC)  ,
  INDEX `fk_jobScheduler_jobEstimate_idx` (`EstimateId` ASC)  ,
  INDEX `fk_jobScheduler_Franchisee_idx` (`FranchiseeId` ASC)  ,
  INDEX `fk_JobScheduler_OrgRoleUserId_idx` (`AssigneeId` ASC)  ,
  CONSTRAINT `fk_jobScheduler_job`
    FOREIGN KEY (`JobId`)
    REFERENCES `job` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_jobScheduler_jobEstimate`
    FOREIGN KEY (`EstimateId`)
    REFERENCES `jobestimate` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_jobScheduler_Franchisee`
    FOREIGN KEY (`FranchiseeId`)
    REFERENCES `franchisee` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_JobScheduler_OrgRoleUserId`
    FOREIGN KEY (`AssigneeId`)
    REFERENCES `organizationroleuser` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION);
    
    
CREATE TABLE `jobstatus` (
  `Id` BIGINT(20) NOT NULL AUTO_INCREMENT ,
  `Name` VARCHAR(145) NOT NULL ,
  `ColorCode` VARCHAR(45) NULL DEFAULT NULL ,
  `IsDeleted` BIT(1) NOT NULL DEFAULT b'0' ,
   PRIMARY KEY (`Id`) );
  
ALTER TABLE `job` 
ADD COLUMN `StatusId` BIGINT(20) NOT NULL ,
ADD INDEX `fk_job_jobStatus_idx` (`StatusId` ASC)  ;
ALTER TABLE `job` 
ADD CONSTRAINT `fk_job_jobStatus`
  FOREIGN KEY (`StatusId`)
  REFERENCES `jobstatus` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;
  
INSERT INTO `jobstatus` (`Id`, `Name`, `ColorCode`, `IsDeleted`) VALUES ('1', 'Created', '#a9a9a9', 0);
INSERT INTO `jobstatus` (`Id`, `Name`, `ColorCode`, `IsDeleted`) VALUES ('2', 'Assigned', '#3737a9', 0);
INSERT INTO `jobstatus` (`Id`, `Name`, `ColorCode`, `IsDeleted`) VALUES ('3', 'InProgress', '#cfd655', 0);
INSERT INTO `jobstatus` (`Id`, `Name`, `ColorCode`, `IsDeleted`) VALUES ('4', 'Completed', '#3a8b3b', 0);
INSERT INTO `jobstatus` (`Id`, `Name`, `ColorCode`, `IsDeleted`) VALUES ('5', 'Canceled', '#e55646', 0);

ALTER TABLE `jobscheduler` 
ADD COLUMN `Startdate` DATETIME NOT NULL,
ADD COLUMN `EndDate` DATETIME NOT NULL;

ALTER TABLE `jobscheduler` 
ADD COLUMN `DataRecorderMetaDataId` BIGINT(20) NOT NULL ,
ADD INDEX `fk_jobScheduler_datarecordermetadata_idx` (`DataRecorderMetaDataId` ASC);
ALTER TABLE `jobscheduler` 
ADD CONSTRAINT `fk_jobScheduler_datarecordermetadata`
  FOREIGN KEY (`DataRecorderMetaDataId`)
  REFERENCES `datarecordermetadata` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;

  ALTER TABLE `jobscheduler` 
ADD INDEX `fk_jobScheduler_salesRep_idx` (`SalesRepId` ASC)  ;
ALTER TABLE `jobscheduler` 
ADD CONSTRAINT `fk_jobScheduler_salesRep`
  FOREIGN KEY (`SalesRepId`)
  REFERENCES `organizationroleuser` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;





