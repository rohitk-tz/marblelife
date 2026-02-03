ALTER TABLE `franchiseeinvoice` 
ADD COLUMN `IsDownloaded` BIT(1) NOT NULL DEFAULT b'0'  AFTER `SalesDataUploadId`;


SET SQL_SAFE_UPDATES = 0;
UPDATE `franchiseeinvoice` SET `IsDownloaded`=0 WHERE `Id`>'0';
SET SQL_SAFE_UPDATES = 1;
