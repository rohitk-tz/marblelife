ALTER TABLE `jobscheduler` 
ADD COLUMN `StartDateTimeString` Datetime NOT NULL;

ALTER TABLE `jobscheduler` 
ADD COLUMN `EndDateTimeString` Datetime NOT NULL;

ALTER TABLE `job` 
ADD COLUMN `EndDateTimeString` Datetime NOT NULL;

ALTER TABLE `job` 
ADD COLUMN `StartDateTimeString` Datetime NOT NULL;

ALTER TABLE `jobEstimate` 
ADD COLUMN `EndDateTimeString` Datetime NOT NULL;

ALTER TABLE `jobEstimate` 
ADD COLUMN `StartDateTimeString` Datetime NOT NULL;

ALTER TABLE `jobEstimate` 
ADD COLUMN `Offset` double DEFAULT NULL;

ALTER TABLE `job` 
ADD COLUMN `Offset` double DEFAULT NULL;


ALTER TABLE `meeting` 
ADD COLUMN `EndDateTimeString` Datetime NOT NULL;

ALTER TABLE `meeting` 
ADD COLUMN `StartDateTimeString` Datetime NOT NULL;

ALTER TABLE `meeting` 
ADD COLUMN `Offset` double DEFAULT NULL;