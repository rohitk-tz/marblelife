ALTER TABLE `franchiseetechmailemail` 
ADD COLUMN `CallCount` BIGINT(20) NULL DEFAULT 0 AFTER `isDeleted`;