ALTER TABLE `annualsalesdataupload` 
ADD COLUMN `WeeklyRoyality` VARCHAR(100) NULL DEFAULT NULL AFTER `PaidAmount`,
ADD COLUMN `AnnualRoyality` VARCHAR(100) NULL DEFAULT NULL AFTER `WeeklyRoyality`;



INSERT INTO `annualreporttype` (`Id`, `ReportTypeName`, `Description`, `isDeleted`) VALUES ('22', 'Type1', 'Perfect Report', b'0');


INSERT INTO `annualreporttype` (`Id`, `ReportTypeName`, `Description`, `isDeleted`) VALUES ('23', 'Type17', 'Other Type', b'0');

SET SQL_SAFE_UPDATES = 0;
ALTER TABLE `annualsalesdataupload` 
CHANGE COLUMN `WeeklyRoyality` `WeeklyRoyality` DECIMAL(10,2) NULL DEFAULT NULL ,
CHANGE COLUMN `AnnualRoyality` `AnnualRoyality` DECIMAL(10,2) NULL DEFAULT NULL ;
SET SQL_SAFE_UPDATES = 1;
