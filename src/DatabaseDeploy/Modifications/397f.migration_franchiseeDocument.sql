USE `makalu`;
DROP procedure IF EXISTS `migration_franchiseeDocumentMigration2`;

DELIMITER $$
USE `makalu`$$
Create DEFINER=`root`@`localhost` PROCEDURE `migration_franchiseeDocumentMigration2`()
BEGIN

declare _organizationId int;

create table distinctIds
select distinct Id from  makalu.organization;
 
WHILE Exists(select 1 from distinctIds Limit 1) DO
 
select id into _organizationId from distinctIds Limit 1;

INSERT INTO `franchiseedocumenttype` ( `FranchiseeId`, `DocumentTypeId`, `IsActive`, `IsDeleted`) VALUES (_organizationId, '20', b'1', b'0');
INSERT INTO `franchiseedocumenttype` ( `FranchiseeId`, `DocumentTypeId`, `IsActive`, `IsDeleted`) VALUES (_organizationId, '21', b'1', b'0');
INSERT INTO `franchiseedocumenttype` ( `FranchiseeId`, `DocumentTypeId`, `IsActive`, `IsDeleted`) VALUES (_organizationId, '22', b'1', b'0');

SET SQL_SAFE_UPDATES = 0;
DELETE FROM distinctIds WHERE  id = _organizationId;
SET SQL_SAFE_UPDATES = 1;
 END WHILE;
  
  DROP TABLE distinctIds;
END$$

DELIMITER ;

UPDATE `makalu`.`documenttype` SET `order`='9' WHERE `Id`='22';


call migration_franchiseeDocumentMigration2();
