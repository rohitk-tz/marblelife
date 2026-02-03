USE `makalu`;
DROP procedure IF EXISTS `migration_franchiseeDocumentMigration`;

DELIMITER $$
USE `makalu`$$
Create DEFINER=`root`@`localhost` PROCEDURE `migration_franchiseeDocumentMigration`()
BEGIN

declare _organizationId int;

create table distinctIds
select distinct Id from  makalu.organization;
 
WHILE Exists(select 1 from distinctIds Limit 1) DO
 
select id into _organizationId from distinctIds Limit 1;

INSERT INTO `franchiseedocumenttype` ( `FranchiseeId`, `DocumentTypeId`, `IsActive`, `IsDeleted`) VALUES (_organizationId, '15', b'1', b'0');
INSERT INTO `franchiseedocumenttype` ( `FranchiseeId`, `DocumentTypeId`, `IsActive`, `IsDeleted`) VALUES (_organizationId, '16', b'1', b'0');
INSERT INTO `franchiseedocumenttype` ( `FranchiseeId`, `DocumentTypeId`, `IsActive`, `IsDeleted`) VALUES (_organizationId, '17', b'1', b'0');
INSERT INTO `franchiseedocumenttype` ( `FranchiseeId`, `DocumentTypeId`, `IsActive`, `IsDeleted`) VALUES (_organizationId, '18', b'1', b'0');
INSERT INTO `franchiseedocumenttype` ( `FranchiseeId`, `DocumentTypeId`, `IsActive`, `IsDeleted`) VALUES (_organizationId, '19', b'1', b'0');       
SET SQL_SAFE_UPDATES = 0;
DELETE FROM distinctIds WHERE  id = _organizationId;
SET SQL_SAFE_UPDATES = 1;
 END WHILE;
  
  DROP TABLE distinctIds;
END$$

DELIMITER ;

