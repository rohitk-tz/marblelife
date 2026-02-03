SET SQL_SAFE_UPDATES = 0;
ALTER TABLE `person` 
ADD COLUMN `CalendarPreference` varchar(20)  DEFAULT 'month';
SET SQL_SAFE_UPDATES = 1;