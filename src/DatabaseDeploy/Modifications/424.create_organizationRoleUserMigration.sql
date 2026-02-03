USE `makalu`;
DROP procedure IF EXISTS `migration_organizationRoleUserMigration`;

DELIMITER $$
USE `makalu`$$
Create PROCEDURE `migration_organizationRoleUserMigration`()
BEGIN

declare _organizationRoleUserId int;
declare _userColor varchar(45);
create table distinctIds
select distinct Id from  makalu.organizationroleuser where roleId =3;
  
WHILE Exists(select 1 from distinctIds Limit 1) DO
 
select id into _organizationRoleUserId from distinctIds Limit 1;
select ColorCode into _userColor from organizationroleuser where Id=_organizationRoleUserId;
SET SQL_SAFE_UPDATES = 0;
update makalu.organizationroleuser set ColorCodeSale=_userColor where Id=_organizationRoleUserId;
update makalu.organizationroleuser set ColorCode=null where Id=_organizationRoleUserId;
DELETE FROM distinctIds WHERE  id = _organizationRoleUserId;
SET SQL_SAFE_UPDATES = 1;
 END WHILE;
  
  DROP TABLE distinctIds;
END$$

DELIMITER ;$$

DELIMITER ;

SET SQL_SAFE_UPDATES = 0;
CALL migration_organizationRoleUserMigration();
SET SQL_SAFE_UPDATES = 1;