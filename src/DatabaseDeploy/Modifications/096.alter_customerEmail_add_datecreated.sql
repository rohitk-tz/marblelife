ALTER TABLE `customeremail` 
ADD COLUMN `DateCreated` DATETIME NULL AFTER `IsDeleted`;

SET SQL_SAFE_UPDATES = 0;
update customeremail ce
inner join customer c
on ce.customerid = c.id
set ce.datecreated = c.datecreated
where c.isdeleted = 0 and ce.isdeleted = 0;
SET SQL_SAFE_UPDATES = 1;