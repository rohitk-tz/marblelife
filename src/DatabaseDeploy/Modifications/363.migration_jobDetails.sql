USE `makalu`;
DROP procedure IF EXISTS `migration_jobDetails`;

DELIMITER $$
USE `makalu`$$
CREATE DEFINER=`root`@`localhost` PROCEDURE `migration_jobDetails`()
BEGIN
declare _index1 int;
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
declare _qaInvoiceId int;
declare _jobEstimateServiceId int;
declare _drmdId int;
declare _jobTypeid int;
declare _customerId int;
declare _statusId int;
declare _description varchar(1024);
declare _startDateTime DateTime ;
declare _endDateTime dateTime;
declare _startDateTimeString DateTime ;
declare _endDateTimeString dateTime;
declare _offset int;
declare _geoCode varchar(200);

create table distinctIds
select distinct id from  job;
 
WHILE Exists(select 1 from distinctIds Limit 1) DO
 
select id into _index from distinctIds Limit 1;

		Select id, JobTypeId , Description, CustomerId,QBInvoiceNumber,StatusId,StartDate,EndDate,StartDateTimeString,
        EndDateTimeString,EstimateId,Offset,GeoCode
		into 
		_jobid, _jobTypeid , _description,  _customerId,_qaInvoiceId,_statusId,_startDateTime,_endDateTime,_startDateTimeString,
        _endDateTimeString,_EstimateId,_offset,_geoCode
		from job
        where id = _index
        Limit 1;
        
        create table distinctIdsForScheduler
select distinct Id from  jobScheduler where JobId = _index ;
        
        WHILE Exists(select 1 from distinctIdsForScheduler Limit 1) DO

select id into _index1 from distinctIdsForScheduler Limit 1;

		INSERT INTO `jobDetails`
		(`JobId`,`JobTypeId`,`QBInvoiceNumber`,`Description`,`CustomerId`,`IsDeleted`,`StatusId`,`StartDate`,`EndDate`,`EstimateId`,`GeoCode`,
        `EndDateTimeString`,`StartDateTimeString`,`Offset`,`SchedulerId`)
		VALUES
		(
        _jobid,
		_jobTypeid,
		_qaInvoiceId,
		_description,
		_customerId,
		b'0',
        _statusId,
        _startDateTime,
        _endDateTime,
        _EstimateId,
        _geoCode,
        _endDateTimeString,
        _startDateTimeString,
        _offset,
        _index1
		);
        
		DELETE FROM distinctIdsForScheduler WHERE    id = _index1;
        END WHILE;    
		DROP TABLE distinctIdsForScheduler;
	    DELETE FROM distinctIds WHERE    Id = _index;
                   
			
       
 END WHILE;
  
  DROP TABLE distinctIds;
END$$

DELIMITER ;

