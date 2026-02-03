SET SQL_SAFE_UPDATES = 0;
update jobscheduler js
INNER JOIN datarecordermetadata db ON js.DataRecorderMetaDataId =db.Id
INNER JOIN organizationroleuser orgRoleUser ON  orgRoleUser.Id=db.CreatedBy
INNER JOIN userLogin pr ON  pr.Id=orgRoleUser.UserId
INNER JOIN person per ON  orgRoleUser.UserId=per.Id 
Set pr.EDTOffset=js.Offset
where js.StartDate<='2018-11-11 12:00:00' and js.StartDate>='2018-05-01 12:00:00' 
and js.offSet is not null;
SET SQL_SAFE_UPDATES = 1;


SET SQL_SAFE_UPDATES = 0;
update jobscheduler js
INNER JOIN datarecordermetadata db ON js.DataRecorderMetaDataId =db.Id
INNER JOIN organizationroleuser orgRoleUser ON  orgRoleUser.Id=db.CreatedBy
INNER JOIN userLogin pr ON  pr.Id=orgRoleUser.UserId
INNER JOIN person per ON  orgRoleUser.UserId=per.Id 
Set pr.ESTOffset=pr.EDTOffset-60
where js.StartDate>='2018-11-11 12:00:00' and js.StartDate<='2019-05-01 12:00:00' 
and js.offSet is not null;
SET SQL_SAFE_UPDATES = 1;