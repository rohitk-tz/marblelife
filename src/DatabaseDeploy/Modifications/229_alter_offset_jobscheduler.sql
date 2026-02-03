SET SQL_SAFE_UPDATES = 0;
 
UPDATE `jobscheduler` SET `Offset`='-240' where `MeetingId` is null;
 
SET SQL_SAFE_UPDATES = 1