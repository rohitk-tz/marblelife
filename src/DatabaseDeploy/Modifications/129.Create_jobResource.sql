CREATE TABLE `jobresource` (
  `Id` BIGINT(20) NOT NULL AUTO_INCREMENT ,
  `JobId` BIGINT(20) NOT NULL ,
  `FileId` BIGINT(20) NULL ,
  `IsDeleted` BIT(1) NOT NULL DEFAULT b'0' ,
  `StatusId` BIGINT(20) NOT NULL ,
  PRIMARY KEY (`Id`)  COMMENT '',
  INDEX `jobResource_job_idx` (`JobId` ASC)  ,
  INDEX `JobResource_file_idx` (`FileId` ASC)  ,
  INDEX `JobResource_jobStatus_idx` (`StatusId` ASC)  ,
  CONSTRAINT `jobResource_job`
    FOREIGN KEY (`JobId`)
    REFERENCES `job` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `JobResource_file`
    FOREIGN KEY (`FileId`)
    REFERENCES `file` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `JobResource_jobStatus`
    FOREIGN KEY (`StatusId`)
    REFERENCES `jobstatus` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION);
