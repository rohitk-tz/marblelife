SET SQL_SAFE_UPDATES = 0;
UPDATE jobscheduler t1
INNER JOIN
organizationroleuser t2
ON t1.AssigneeId=t2.Id
SET t1.PersonId = t2.UserId
 where t1.IsDeleted=b'0' and t1.isActive=b'1';
 SET SQL_SAFE_UPDATES = 1;