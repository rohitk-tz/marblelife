set sql_safe_updates=0;
SET foreign_key_checks = 0;

delete from UserLogin;
ALTER TABLE UserLogin AUTO_INCREMENT = 1;

delete from Person;
ALTER TABLE Person AUTO_INCREMENT = 1;

delete from DataRecorderMetaData;
ALTER TABLE DataRecorderMetaData AUTO_INCREMENT = 1;

delete from OrganizationAddress;
ALTER TABLE OrganizationAddress AUTO_INCREMENT = 1;

delete from OrganizationRoleUser;
ALTER TABLE OrganizationRoleUser AUTO_INCREMENT = 1;

delete from Organization;
ALTER TABLE Organization AUTO_INCREMENT = 1;

set sql_safe_updates=1;
SET foreign_key_checks = 1;

/* ------------------------------------------------------------------ */
/* ------------------------ creating admin -------------------------- */

insert into DataRecorderMetaData (DateCreated) values (now());
set @drm = last_insert_id();

insert into Person (FirstName, LastName, Email, DataRecorderMetaDataId) values ('Super', 'Admin', 'super.admin@yopmail.com', @drm);
set @personid = last_insert_id();


insert into UserLogin (Id, UserName, `Password`, Salt)
values (@personid,'super.admin@yopmail.com', '43D2m+aKFAhYZmP6Gkhy1vh5/hQvA0BZ', 'pAAX8Bjb/e8bXKIX8wS6Ft/vs1KXtKvb'); /*Password : pass123 */

set @userLoginId =  last_insert_id();

/*Organization */
insert into DataRecorderMetaData (DateCreated) values (now());
set @drm = last_insert_id();
INSERT INTO `Organization`(`Id`,`TypeId`,`Name`,`Description`,`DataRecorderMetaDataId`)
VALUES(1, 21, 'Marble Life', 'The Franchisor', @drm);

INSERT INTO `Address`(`TypeId`,`AddressLine1`,`AddressLine2`,`CityId`,`StateId`,`CountryId`,`ZipId`)
VALUES(11, 'Line 1', 'Line 2', 27519, 51, 1, 24618);

set @organizationAddress = last_insert_id();

INSERT INTO `OrganizationAddress`(`OrganizationId`,`AddressId`)
VALUES(1, @organizationAddress);

INSERT INTO `OrganizationRoleUser`(`Id`,`UserId`,`RoleId`,`OrganizationId`)
VALUES(1, @userLoginId, 1, 1);


