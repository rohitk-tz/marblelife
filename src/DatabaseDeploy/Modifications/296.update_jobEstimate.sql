SET SQL_SAFE_UPDATES = 0;
UPDATE jobestimate t1
INNER JOIN jobscheduler t2 ON t1.Id = t2. EstimateId
SET t1.StartDate=t2.StartDate,t1.EndDate=t2.EndDate;
SET SQL_SAFE_UPDATES = 1;

