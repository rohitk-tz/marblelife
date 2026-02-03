USE `makalu`;
DROP procedure IF EXISTS `migration_jobDetails_Estimate`;

DELIMITER $$
USE `makalu`$$
CREATE PROCEDURE `migration_jobDetails_Estimate` ()
BEGIN
declare _estimateHour int;
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
select distinct Id from  jobEstimate;
 
WHILE Exists(select 1 from distinctIds Limit 1) DO
 
select id into _index from distinctIds Limit 1;

		Select id,EstimateHour , Description, CustomerId,StartDate,EndDate,StartDateTimeString,
        EndDateTimeString,Offset,GeoCode
		into 
		_EstimateId, _estimateHour , _description,  _customerId,_startDateTime,_endDateTime,_startDateTimeString,
        _endDateTimeString,_offset,_geoCode
		from jobEstimate
        where id = _index
        Limit 1;
        
        create table distinctIdsForScheduler
select distinct Id from  jobScheduler where EstimateId = _index ;
        
        WHILE Exists(select 1 from distinctIdsForScheduler Limit 1) DO

select id into _index1 from distinctIdsForScheduler Limit 1;

		INSERT INTO `jobDetails`
		(`JobId`,`JobTypeId`,`QBInvoiceNumber`,`Description`,`CustomerId`,`IsDeleted`,`StatusId`,`StartDate`,`EndDate`,`EstimateId`,`GeoCode`,
        `EndDateTimeString`,`StartDateTimeString`,`Offset`,`SchedulerId`)
		VALUES
		(
        null,
		null,
		'',
		_description,
		_customerId,
		b'0',
        null,
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

