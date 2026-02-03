/* For Date Modified is Null and  for PST*/
SET SQL_SAFE_UPDATES = 0;
update jobscheduler js
INNER JOIN datarecordermetadata db ON js.DataRecorderMetaDataId =db.Id
INNER JOIN organizationroleuser orgRoleUser ON  orgRoleUser.Id=db.CreatedBy
INNER JOIN userLogin pr ON  pr.Id=orgRoleUser.UserId
INNER JOIN person per ON  orgRoleUser.UserId=per.Id 
Set js.Offset=-480
where ((db.DateCreated>='2018-11-04 12:00:00' and db.DateCreated<='2019-03-10 12:00:00')
 or (db.DateCreated>='2017-11-11 12:00:00' and db.DateCreated<='2018-03-11 12:00:00')
)
and db.CreatedBy in (select Id from organizationroleuser where userId in (51)) and db.DateModified is null;
SET SQL_SAFE_UPDATES = 1;

/* For Date Modified is not Null and  for PST*/
SET SQL_SAFE_UPDATES = 0;
update jobscheduler js
INNER JOIN datarecordermetadata db ON js.DataRecorderMetaDataId =db.Id
INNER JOIN organizationroleuser orgRoleUser ON  orgRoleUser.Id=db.ModifiedBy
INNER JOIN userLogin pr ON  pr.Id=orgRoleUser.UserId
INNER JOIN person per ON  orgRoleUser.UserId=per.Id 
Set js.Offset=-480
where ((db.DateModified>='2018-11-04 12:00:00' and db.DateModified<='2019-03-10 12:00:00')
or (db.DateModified>='2017-11-4 12:00:00' and db.DateModified<='2018-03-11 12:00:00')) 
and db.ModifiedBy in (select Id from organizationroleuser where userId in (51)) and db.DateModified is  not null;
SET SQL_SAFE_UPDATES = 1;



/* For Date Modified is Null and  for PDT*/
SET SQL_SAFE_UPDATES = 0;
update jobscheduler js
INNER JOIN datarecordermetadata db ON js.DataRecorderMetaDataId =db.Id
INNER JOIN organizationroleuser orgRoleUser ON  orgRoleUser.Id=db.CreatedBy
INNER JOIN userLogin pr ON  pr.Id=orgRoleUser.UserId
INNER JOIN person per ON  orgRoleUser.UserId=per.Id 
Set js.Offset=-420
where ((db.DateCreated>='2018-03-11 12:00:00' and db.DateCreated<='2018-11-04 12:00:00')
or (db.DateCreated>='2019-03-10 12:00:00' and db.DateCreated<='2019-11-11 12:00:00'))
and db.CreatedBy in (select Id from organizationroleuser where userId in (51)) and db.DateModified is null;
SET SQL_SAFE_UPDATES = 1;

/* For Date Modified is not Null and  for PDT*/
SET SQL_SAFE_UPDATES = 0;
update jobscheduler js
INNER JOIN datarecordermetadata db ON js.DataRecorderMetaDataId =db.Id
INNER JOIN organizationroleuser orgRoleUser ON  orgRoleUser.Id=db.ModifiedBy
INNER JOIN userLogin pr ON  pr.Id=orgRoleUser.UserId
INNER JOIN person per ON  orgRoleUser.UserId=per.Id 
Set js.Offset=-420
where ((db.DateModified>='2018-03-11 12:00:00' and db.DateModified<='2018-11-04 12:00:00')
or (db.DateModified>='2019-03-10 12:00:00' and db.DateModified<='2019-11-11 12:00:00'))
and db.ModifiedBy in (select Id from organizationroleuser where userId in (51)) and db.DateModified is  not null;
SET SQL_SAFE_UPDATES = 1;



/*  For Boundary Condition DateModified is null*/
SET SQL_SAFE_UPDATES = 0;
update jobscheduler js
INNER JOIN datarecordermetadata db ON js.DataRecorderMetaDataId =db.Id
INNER JOIN organizationroleuser orgRoleUser ON  orgRoleUser.Id=db.CreatedBy
INNER JOIN userLogin pr ON  pr.Id=orgRoleUser.UserId
INNER JOIN person per ON  orgRoleUser.UserId=per.Id 
Set js.Offset=-480
where ((db.DateCreated>='2018-03-11 12:00:00' and db.DateCreated<'2018-11-04 02:00:00')
or (db.DateCreated>='2019-03-10 12:00:00' and db.DateCreated<='2019-11-11 12:00:00'))
and db.CreatedBy in (select Id from organizationroleuser where userId in (51)) and db.DateModified is null
and js.startdate > '2018-11-04 02:00:00' and js.startdate < '2019-03-10 12:00:00';
SET SQL_SAFE_UPDATES = 1;




/*  For Boundary Condition DateModified is  not null*/
SET SQL_SAFE_UPDATES = 0;
update jobscheduler js
INNER JOIN datarecordermetadata db ON js.DataRecorderMetaDataId =db.Id
INNER JOIN organizationroleuser orgRoleUser ON  orgRoleUser.Id=db.ModifiedBy
INNER JOIN userLogin pr ON  pr.Id=orgRoleUser.UserId
INNER JOIN person per ON  orgRoleUser.UserId=per.Id 
Set js.Offset=-480
where ((db.DateModified>='2018-03-11 12:00:00' and db.DateModified<='2018-11-04 12:00:00')
or (db.DateModified>='2019-03-10 12:00:00' and db.DateModified<='2019-11-11 12:00:00'))
and db.ModifiedBy in (select Id from organizationroleuser where userId in (51)) and db.DateModified is  not null
and js.startdate > '2018-11-04 02:00:00' and js.startdate < '2019-03-10 12:00:00';
SET SQL_SAFE_UPDATES = 1;


/* For Date Modified is not Null and  for EST*/


SET SQL_SAFE_UPDATES = 0;
update jobscheduler js
INNER JOIN datarecordermetadata db ON js.DataRecorderMetaDataId =db.Id
INNER JOIN organizationroleuser orgRoleUser ON  orgRoleUser.Id=db.ModifiedBy
INNER JOIN userLogin pr ON  pr.Id=orgRoleUser.UserId
INNER JOIN person per ON  orgRoleUser.UserId=per.Id 
Set js.Offset=-420
where ((db.DateModified>='2018-11-04 12:00:00' and db.DateModified<='2019-03-10 12:00:00')
or (db.DateModified>='2017-11-04 12:00:00' and db.DateModified<='2018-03-11 12:00:00')) 
and db.ModifiedBy in (select Id from organizationroleuser where userId in (51)) and db.DateModified is  not null
and js.startdate > '2019-03-10 12:00:00' and js.startdate < '2019-11-11 12:00:00';

SET SQL_SAFE_UPDATES = 1;


SET SQL_SAFE_UPDATES = 0;
update jobscheduler js
INNER JOIN datarecordermetadata db ON js.DataRecorderMetaDataId =db.Id
INNER JOIN organizationroleuser orgRoleUser ON  orgRoleUser.Id=db.CreatedBy
INNER JOIN userLogin pr ON  pr.Id=orgRoleUser.UserId
INNER JOIN person per ON  orgRoleUser.UserId=per.Id 
Set js.Offset=-420
where ((db.DateCreated>='2018-11-04 12:00:00' and db.DateCreated<='2019-03-10 12:00:00')
 or (db.DateCreated>='2017-11-11 12:00:00' and db.DateCreated<='2018-03-11 12:00:00')
)
and db.CreatedBy in (select Id from organizationroleuser where userId in (51)) and db.DateModified is null
and js.startdate > '2019-03-10 12:00:00' and js.startdate < '2019-11-11 12:00:00';
SET SQL_SAFE_UPDATES = 1;