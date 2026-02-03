USE `makalu`;
DROP procedure IF EXISTS `migration_franchiseeServicesMigration`;

DELIMITER $$
USE `makalu`$$
Create DEFINER=`root`@`localhost` PROCEDURE `migration_franchiseeServicesMigration`()
BEGIN

declare _organizationId int;

create table distinctIds
select distinct Id from  makalu.franchisee where Id not in (3, 97, 98, 99, 100);
 
WHILE Exists(select 1 from distinctIds Limit 1) DO
 
select id into _organizationId from distinctIds Limit 1;

INSERT INTO `franchiseeservice` (`FranchiseeId`, `ServiceTypeId`, `CalculateRoyalty`,`IsActive`, `IsDeleted`, `IsCertified`) VALUES (_organizationId, '43', b'1', b'1', b'0', b'0'); 
SET SQL_SAFE_UPDATES = 0;
DELETE FROM distinctIds WHERE  id = _organizationId;
SET SQL_SAFE_UPDATES = 1;
 END WHILE;
  
  DROP TABLE distinctIds;
END$$

DELIMITER ;

SET SQL_SAFE_UPDATES = 0;
CALL migration_franchiseeServicesMigration();
SET SQL_SAFE_UPDATES = 1;