USE `makalu` ;




insert into DataRecorderMetaData (DateCreated) values (now());
set @drm = last_insert_id();

INSERT INTO `Organization`(,`TypeId`,`Name`, `Email`, `DataRecorderMetaDataId`)
VALUES( 22, 'Delaware Valley', 'delawarevalley@marblelife.com',  @drm);

INSERT INTO `Franchisee`(`Id`,`OwnerName`,`QuickBookIdentifier`)
VALUES(2, '', '');