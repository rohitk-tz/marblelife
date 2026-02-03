SET SQL_SAFE_UPDATES=0;
UPDATE `jobscheduler` js JOIN `organizationroleuser` org ON js.AssigneeId = org.Id
  SET js.IsDeleted = b'1'
WHERE org.RoleId='2' and js.isVacation;
SET SQL_SAFE_UPDATES=1;