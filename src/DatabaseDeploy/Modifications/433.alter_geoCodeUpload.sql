ALTER TABLE `geocodefileupload` 
ADD COLUMN `countiesCount` BIGINT(20) NULL DEFAULT 0;

ALTER TABLE `geocodefileupload` 
ADD COLUMN `zipCodeCount` BIGINT(20) NULL DEFAULT 0;