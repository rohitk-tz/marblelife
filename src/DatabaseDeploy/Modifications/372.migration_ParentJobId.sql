USE `makalu`;
DROP procedure IF EXISTS `migration_lead_performance_franchisee_deteatils`;

DELIMITER $$
USE `makalu`$$
CREATE PROCEDURE `migration_jobScheduler_ParentJobId` ()
BEGIN

declare _index int;
declare _id int;

create table distinctIds
select distinct ParentJobId from  `makalu`.`jobscheduler` where ParentJobId is not null;
 
 
 
WHILE Exists(select 1 from distinctIds Limit 1) DO
 select ParentJobId into _index from distinctIds Limit 1;

 
update `jobscheduler`  set isRepeat=b'1' where `ParentJobId` =_index;



DELETE FROM distinctIds WHERE  ParentJobId = _index;
	   
 END WHILE;
  
  DROP TABLE distinctIds;
END$$

DELIMITER ;

