USE `makalu`;
DROP procedure IF EXISTS `migration_minRoyalitySlabMigration`;

DELIMITER $$
USE `makalu`$$
Create DEFINER=`root`@`localhost` PROCEDURE `migration_minRoyalitySlabMigration`()
BEGIN

declare _organizationId int;

create table distinctIds
select distinct Id from  makalu.organization where name not like '0%' and id != 1;
  
WHILE Exists(select 1 from distinctIds Limit 1) DO
 
select id into _organizationId from distinctIds Limit 1;
INSERT INTO `minroyaltyfeeslabs` ( `FranchiseeId`, `StartValue`, `EndValue`,`MinRoyality`, `IsDeleted`) VALUES (_organizationId, 0, 6,0, b'0');
INSERT INTO `minroyaltyfeeslabs` ( `FranchiseeId`, `StartValue`, `EndValue`,`MinRoyality`, `IsDeleted`) VALUES (_organizationId, 7, 12,200, b'0');
INSERT INTO `minroyaltyfeeslabs` ( `FranchiseeId`, `StartValue`, `EndValue`,`MinRoyality`, `IsDeleted`) VALUES (_organizationId, 13, 18,400, b'0');
INSERT INTO `minroyaltyfeeslabs` ( `FranchiseeId`, `StartValue`, `EndValue`,`MinRoyality`, `IsDeleted`) VALUES (_organizationId, 19, 24,600, b'0');
INSERT INTO `minroyaltyfeeslabs` ( `FranchiseeId`, `StartValue`, `EndValue`,`MinRoyality`, `IsDeleted`) VALUES (_organizationId, 25, 2900,800, b'0');

SET SQL_SAFE_UPDATES = 0;
DELETE FROM distinctIds WHERE  id = _organizationId;
SET SQL_SAFE_UPDATES = 1;
 END WHILE;
  
  DROP TABLE distinctIds;
END$$

DELIMITER ;

