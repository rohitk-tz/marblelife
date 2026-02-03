SET SQL_SAFE_UPDATES = 0;

UPDATE `jobscheduler` job
SET startDate = ADDTIME(startDate,'1:00:00')
WHERE job.DataRecorderMetaDataId In (select Id from `datarecordermetadata` where CreatedBy in (SELECT Id FROM makalu.organizationroleuser where UserId in (85,87)) ) 
and job.startDate>'2018-11-04 02:00:00' and job.IsActive=b'1' and job.IsDeleted=b'0';


UPDATE `jobscheduler` job
SET endDate = ADDTIME(endDate,'1:00:00')
WHERE job.DataRecorderMetaDataId In (select Id from `datarecordermetadata` where CreatedBy in (SELECT Id FROM makalu.organizationroleuser where UserId in (85,87)) ) 
and job.endDate>'2018-11-04 02:00:00' and job.IsActive=b'1' and job.IsDeleted=b'0';


SET SQL_SAFE_UPDATES = 1;




SET SQL_SAFE_UPDATES = 0;
UPDATE `jobscheduler` job
SET startDate = ADDTIME(startDate,'2:00:00')
WHERE job.DataRecorderMetaDataId In (select Id from `datarecordermetadata` where CreatedBy in (SELECT Id FROM makalu.organizationroleuser where UserId in (73)) ) 
and job.startDate>'2018-11-04 02:00:00' and job.IsActive=b'1' and job.IsDeleted=b'0' and  job.FranchiseeId = 22 ;


UPDATE `jobscheduler` job
SET endDate = ADDTIME(endDate,'2:00:00')
WHERE job.DataRecorderMetaDataId In (select Id from `datarecordermetadata` where CreatedBy in (SELECT Id FROM makalu.organizationroleuser where UserId in (73)) ) 
and job.endDate>'2018-11-04 02:00:00' and job.IsActive=b'1' and job.IsDeleted=b'0' and  job.FranchiseeId = 22 ;

SET SQL_SAFE_UPDATES = 1;


SET SQL_SAFE_UPDATES = 0;

UPDATE `jobscheduler` job
SET startDate = ADDTIME(startDate,'1:00:00')
WHERE job.DataRecorderMetaDataId In (select Id from `datarecordermetadata` where CreatedBy in (SELECT Id FROM makalu.organizationroleuser where UserId in (112)) ) 
and job.startDate>'2018-11-04 02:00:00' and job.IsActive=b'1' and job.IsDeleted=b'0' ; 


UPDATE `jobscheduler` job
SET endDate = ADDTIME(endDate,'1:00:00')
WHERE   job.DataRecorderMetaDataId In (select Id from `datarecordermetadata` where CreatedBy in (SELECT Id FROM makalu.organizationroleuser where UserId in (112)) ) 
and job.endDate>'2018-11-04 02:00:00' and job.IsActive=b'1' and job.IsDeleted=b'0' ;

SET SQL_SAFE_UPDATES = 1;


SET SQL_SAFE_UPDATES = 0;

UPDATE `jobscheduler` job
SET startDate = ADDTIME(startDate,'1:00:00')
WHERE job.DataRecorderMetaDataId In (select Id from `datarecordermetadata` where CreatedBy in (SELECT Id FROM makalu.organizationroleuser where UserId in (58)) ) 
and job.startDate>'2018-11-04 02:00:00' and job.IsActive=b'1' and job.IsDeleted=b'0' ; 


UPDATE `jobscheduler` job
SET endDate = ADDTIME(endDate,'1:00:00')
WHERE   job.DataRecorderMetaDataId In (select Id from `datarecordermetadata` where CreatedBy in (SELECT Id FROM makalu.organizationroleuser where UserId in (58)) ) 
and job.endDate>'2018-11-04 02:00:00' and job.IsActive=b'1' and job.IsDeleted=b'0' ;

SET SQL_SAFE_UPDATES = 1;

