ALTER TABLE calldetailsreportnotes
MODIFY Notes
varchar(5000);

ALTER TABLE `calldetailsreportnotes` 
ADD COLUMN `PreferredContactNumber` bigint(20) NULL DEFAULT NULL;

ALTER TABLE `calldetailsreportnotes` 
ADD COLUMN `FirstName` varchar(128) NULL DEFAULT NULL;

ALTER TABLE `calldetailsreportnotes` 
ADD COLUMN `LastName` varchar(128) NULL DEFAULT NULL;

ALTER TABLE `calldetailsreportnotes` 
ADD COLUMN `Company` varchar(256) NULL DEFAULT NULL;

ALTER TABLE `calldetailsreportnotes` 
ADD COLUMN `Office` varchar(128) NULL DEFAULT NULL;

ALTER TABLE `calldetailsreportnotes` 
ADD COLUMN `ZipCode` varchar(128) NULL DEFAULT NULL;

ALTER TABLE `calldetailsreportnotes` 
ADD COLUMN `ResultingAction` varchar(256) NULL DEFAULT NULL;

ALTER TABLE `calldetailsreportnotes` 
ADD COLUMN `HouseNumber` varchar(128) NULL DEFAULT NULL;

ALTER TABLE `calldetailsreportnotes` 
ADD COLUMN `Street` varchar(128) NULL DEFAULT NULL;

ALTER TABLE `calldetailsreportnotes` 
ADD COLUMN `City` varchar(128) NULL DEFAULT NULL;

ALTER TABLE `calldetailsreportnotes` 
ADD COLUMN `State` varchar(128) NULL DEFAULT NULL;

ALTER TABLE `calldetailsreportnotes` 
ADD COLUMN `Country` varchar(128) NULL DEFAULT NULL;