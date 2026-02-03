CREATE TABLE `organizationroleuserfranchisee` (
  `Id` BIGINT(20) NOT NULL AUTO_INCREMENT,
  `OrganizationRoleUserId` BIGINT(20) NOT NULL,
  `franchiseeId` BIGINT(20) NOT NULL,
  `IsActive` BIT(1) NOT NULL DEFAULT b'1',
  `IsDeleted` BIT(1) NOT NULL DEFAULT b'0',
  `IsDefault` BIT(1) NOT NULL DEFAULT b'1',
  PRIMARY KEY (`Id`),
  INDEX `fk_organizationRoleUserId_idx` (`OrganizationRoleUserId` ASC),
  INDEX `fk_OrganizationRoleUserFranchisee_franchisee_idx` (`franchiseeId` ASC),
  CONSTRAINT `fk_OrganizationRoleUserFranchisee_organizationRoleUser`
    FOREIGN KEY (`OrganizationRoleUserId`)
    REFERENCES `organizationroleuser` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_OrganizationRoleUserFranchisee_franchisee`
    FOREIGN KEY (`franchiseeId`)
    REFERENCES `franchisee` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION);		


INSERT INTO organizationroleuserfranchisee (OrganizationRoleUserId,franchiseeId,IsActive,IsDefault)
SELECT id,OrganizationId,IsActive,IsDefault FROM organizationroleuser where roleid = 5;


SET SQL_SAFE_UPDATES = 0;

update organizationroleuserfranchisee set organizationroleuserId = 94 where organizationroleuserId in (SELECT id FROM organizationroleuser where roleid = 5 and userid  = 85 and isdefault = 0);
update organizationroleuserfranchisee set organizationroleuserId = 102 where organizationroleuserId in(SELECT id FROM organizationroleuser where roleid = 5 and userid  = 87 and isdefault = 0);
update organizationroleuserfranchisee set organizationroleuserId = 134 where organizationroleuserId in (SELECT id FROM organizationroleuser where roleid = 5 and userid  = 102 and isdefault = 0);

update datarecordermetadata set createdby = 94 where createdby in (SELECT id FROM organizationroleuser where roleid = 5 and userid  = 85 and isdefault = 0);
update datarecordermetadata set createdby = 102 where createdby in (SELECT id FROM organizationroleuser where roleid = 5 and userid  = 87 and isdefault = 0);
update datarecordermetadata set createdby = 134 where createdby in (SELECT id FROM organizationroleuser where roleid = 5 and userid  = 102 and isdefault = 0);

update datarecordermetadata set modifiedby = 94 where modifiedby in (SELECT id FROM organizationroleuser where roleid = 5 and userid  = 85 and isdefault = 0);
update datarecordermetadata set modifiedby = 102 where modifiedby in (SELECT id FROM organizationroleuser where roleid = 5 and userid  = 87 and isdefault = 0);
update datarecordermetadata set modifiedby = 134 where modifiedby in (SELECT id FROM organizationroleuser where roleid = 5 and userid  = 102 and isdefault = 0);

update organizationroleuser set isdeleted = 1 where roleid = 5 and userid  = 85 and isdefault = 0;
update organizationroleuser set isdeleted = 1 where roleid = 5 and userid  = 87 and isdefault = 0;
update organizationroleuser set isdeleted = 1 where roleid = 5 and userid  = 102 and isdefault = 0;

update organizationroleuser set organizationId = 1 where roleid = 5 and userid  = 85 and isdefault = 1;
update organizationroleuser set organizationId = 1 where roleid = 5 and userid  = 87 and isdefault = 1;
update organizationroleuser set organizationId = 1 where roleid = 5 and userid  = 102 and isdefault = 1;

SET SQL_SAFE_UPDATES = 1;SET SQL_SAFE_UPDATES = 1;
