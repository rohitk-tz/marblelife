
alter table beforeafterimages
Add column S3BucketURL varchar(1200) default null;

alter table beforeafterimages
Add column S3BucketThumbURL varchar(1200) default null;

Alter table customerfeedbackrequest
Add column `StatusId` bigint(20) NULL default Null,
ADD INDEX `fk_customerfeedbackrequest_LookUp_idx` (`StatusId` ASC);
ALTER TABLE `customerfeedbackrequest` 
ADD CONSTRAINT `fk_customerfeedbackrequest_LookUp`
  FOREIGN KEY (`StatusId`)
  REFERENCES `LookUp` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;
  
Alter table customerfeedbackresponse
Add column `StatusId` bigint(20) NULL default Null,
ADD INDEX `fk_customerfeedbackresponse_LookUp_idx` (`StatusId` ASC);
ALTER TABLE `customerfeedbackresponse` 
ADD CONSTRAINT `fk_customerfeedbackresponse_LookUp`
  FOREIGN KEY (`StatusId`)
  REFERENCES `LookUp` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;