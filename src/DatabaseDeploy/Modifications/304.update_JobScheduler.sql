SET SQL_SAFE_UPDATES = 0;
Update jobscheduler t1, jobscheduler t2
set t1.isActive=b'0'
WHERE t1.PersonId = t2.PersonId AND t1.StartDate = t2.StartDate AND t1.id < t2.id  and (t1.isVacation =b'1' or t1.MeetingId is not null) AND t1.StartDate >=CURDATE();
SET SQL_SAFE_UPDATES = 1;