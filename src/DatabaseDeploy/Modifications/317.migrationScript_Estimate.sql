USE `makalu`;
DROP procedure IF EXISTS `new_procedure_Estimate`;

DELIMITER $$
USE `makalu`$$
CREATE DEFINER=`root`@`localhost` PROCEDURE `new_procedure_Estimate`()
BEGIN

declare _index int;
declare _id int;
declare _jobid int;
declare _servicetypeid int;
declare _FileId int;
declare _DataRecorderMetaDataId int;
declare _DateCreated DateTime ;
declare _DateModified dateTime;
declare _createdby int;
declare _ModifiedBy int;
declare _EstimateId int;
declare _categoryId int;
declare _schedulerId int;
declare _jobEstimateServiceId int;
declare _drmdId int;

drop table IF EXISTS distinctIds;
create table distinctIds
select distinct estimateId from  BeforeAfter where estimateId is not null ;
 
WHILE Exists(select 1 from distinctIds Limit 1) DO
 
		select estimateId into _index from distinctIds Limit 1;

		select id, jobid , servicetypeid,   EstimateId,SchedulerId
		into 
		_id, _jobid , _servicetypeid,  _EstimateId,_schedulerId
		from BeforeAfter
        where estimateId = _index
        Limit 1;

		INSERT INTO `jobestimateimagecategory`
		(`JobId`,`EstimateId`,`MarkertingClassId`,`SchedulerId`,`IsDeleted`)
		VALUES
		(
		_jobid,
		_EstimateId,
		_servicetypeid,
		_schedulerId,
		b'0');

		set _categoryId=LAST_INSERT_ID();

        drop table IF EXISTS BeforeAfterINNER;
        
        create table BeforeAfterINNER
		select * from  BeforeAfter where estimateId = _index ;
 
            select id, jobid , servicetypeid,  FileId,  DateCreated,DateModified,createdby,ModifiedBy,EstimateId,SchedulerId
			into 
			_id, _jobid , _servicetypeid,  _FileId,  _DateCreated,_DateModified,_createdby,_ModifiedBy,_EstimateId,_schedulerId
			from BeforeAfterINNER Where BeforeAfterINNER.EstimateId= _index Limit 1;
		

	     	INSERT INTO `datarecordermetadata`(`DateCreated`,`DateModified`,`CreatedBy`,`ModifiedBy`,`IsDeleted`)
			VALUES(_DateCreated,_DateModified,_createdby,_ModifiedBy,b'0');
			set _drmdId=LAST_INSERT_ID();
		
			INSERT INTO `jobestimateservices`(`CategoryId`,`DataRecorderMetaDataId`,`IsDeleted`,`IsBeforeImage`,`TypeId`)
			VALUES(_categoryId,_drmdId,b'0',true,203);
			set _jobEstimateServiceId=LAST_INSERT_ID();



			INSERT INTO `datarecordermetadata`(`DateCreated`,`DateModified`,`CreatedBy`,`ModifiedBy`,`IsDeleted`)
			VALUES(_DateCreated,_DateModified,_createdby,_ModifiedBy,b'0');
			set _drmdId=LAST_INSERT_ID();
			
			INSERT INTO `jobestimateservices`(`CategoryId`,`DataRecorderMetaDataId`,`PairId`,`IsDeleted`,`IsBeforeImage`,`TypeId`)
			VALUES(_categoryId,_drmdId,_jobEstimateServiceId,b'0',b'1',204);

        WHILE Exists(select 1 from BeforeAfterINNER Limit 1) DO
        
			select id, jobid , servicetypeid,  FileId,  DateCreated,DateModified,createdby,ModifiedBy,EstimateId,SchedulerId
			into 
			_id, _jobid , _servicetypeid,  _FileId,  _DateCreated,_DateModified,_createdby,_ModifiedBy,_EstimateId,_schedulerId
			from BeforeAfterINNER Limit 1;


			 INSERT INTO `datarecordermetadata`(`DateCreated`,`DateModified`,`CreatedBy`,`ModifiedBy`,`IsDeleted`)
			VALUES(_DateCreated,_DateModified,_createdby,_ModifiedBy,b'0');

			set _drmdId=LAST_INSERT_ID();

			 INSERT INTO `jobestimateimage`
			(`ServiceId`,`FileId`,`IsDeleted`,`TypeId`,`DataRecorderMetaDataId`)
			VALUES
			(
			_jobEstimateServiceId,
			_fileId,
			b'0',
			203,
			_drmdId);
			
            DELETE FROM BeforeAfterINNER WHERE    id = _id;
           DELETE FROM distinctIds WHERE    EstimateId = _index;
           DELETE FROM BeforeAfterINNER WHERE    FileId = _FileId;
           
       END WHILE;      
       DROP TABLE BeforeAfterINNER;
 END WHILE;
  
  DROP TABLE distinctIds;
 
 END$$

DELIMITER ;

