SET SQL_SAFE_UPDATES = 0;
UPDATE `jobscheduler` SET `EndDate`='2019-03-14 23:00:00' WHERE `Id`='8056';
UPDATE `jobscheduler` SET `EndDate`='2019-03-14 23:00:00' WHERE `Id`='8057';
UPDATE `jobscheduler` SET `EndDate`='2019-03-14 23:00:00' WHERE `Id`='8058';
UPDATE `jobscheduler` SET `EndDate`='2019-03-12 01:00:00' WHERE `Id`='8026';
UPDATE `jobscheduler` SET `EndDate`='2019-03-12 01:00:00' WHERE `Id`='8027';
SET SQL_SAFE_UPDATES = 1;

SET SQL_SAFE_UPDATES = 0;
update jobscheduler js set js.startDateTimeString =DATE_ADD(js.StartDate, INTERVAL  js.Offset MINUTE),
js.endDateTimeString =DATE_ADD(js.EndDate, INTERVAL  js.Offset MINUTE) where js.Offset is not null and js.EndDate <> '0001-01-01 00:00:00' and js.StartDate <>'0001-01-01 00:00:00';
SET SQL_SAFE_UPDATES = 1;



SET SQL_SAFE_UPDATES = 0;
update jobscheduler set StartDateTimeString='1000-01-01 00:00:00'  where StartDateTimeString='0000-00-00 00:00:00';
update jobscheduler set EndDateTimeString='1000-01-01 00:00:00'  where EndDateTimeString='0000-00-00 00:00:00';
SET SQL_SAFE_UPDATES = 1;

SET SQL_SAFE_UPDATES = 0;
update job jb 
INNER JOIN  
jobscheduler js  
ON jb.Id = js.JobId 
set jb.offset =js.offset;
SET SQL_SAFE_UPDATES = 1;

SET SQL_SAFE_UPDATES = 0;
update meeting 
INNER JOIN  
jobscheduler js  
ON meeting.Id = js.MeetingId 
set Meeting.Offset =js.offset;
SET SQL_SAFE_UPDATES = 1;

SET SQL_SAFE_UPDATES = 0;
update jobEstimate je 
INNER JOIN  
jobscheduler js  
ON je.Id = js.EstimateId 
set je.Offset =js.offset;
SET SQL_SAFE_UPDATES = 1;

SET SQL_SAFE_UPDATES = 0;
update job js set js.startDateTimeString =DATE_ADD(js.StartDate, INTERVAL  js.Offset MINUTE),
js.endDateTimeString =DATE_ADD(js.EndDate, INTERVAL  js.Offset MINUTE) where js.Offset is not null and js.EndDate <> '0001-01-01 00:00:00' and js.StartDate <>'0001-01-01 00:00:00';
SET SQL_SAFE_UPDATES = 1;


SET SQL_SAFE_UPDATES = 0;
update meeting js set js.startDateTimeString =DATE_ADD(js.StartDate, INTERVAL  js.Offset MINUTE),
js.endDateTimeString =DATE_ADD(js.EndDate, INTERVAL  js.Offset MINUTE) where js.Offset is not null and js.EndDate <> '0001-01-01 00:00:00' and js.StartDate <>'0001-01-01 00:00:00';
SET SQL_SAFE_UPDATES = 1;

SET SQL_SAFE_UPDATES = 0;
update JobEstimate js set js.startDateTimeString =DATE_ADD(js.StartDate, INTERVAL  js.Offset MINUTE),
js.endDateTimeString =DATE_ADD(js.EndDate, INTERVAL  js.Offset MINUTE) where js.Offset is not null and js.EndDate <> '0001-01-01 00:00:00' and js.StartDate <>'0001-01-01 00:00:00';
SET SQL_SAFE_UPDATES = 1;



