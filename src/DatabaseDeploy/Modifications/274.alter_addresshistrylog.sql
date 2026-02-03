ALTER TABLE `addresshistrylog` 
ADD COLUMN `phoneNumber` VARCHAR(100) NULL DEFAULT NULL AFTER `franchiseesalesId`;



ALTER TABLE `auditaddressdiscrepancy` 
ADD COLUMN `phoneNumber` VARCHAR(100) NULL DEFAULT NULL AFTER `annualsalesdatauploadid`;
