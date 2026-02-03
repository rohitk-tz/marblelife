
ALTER TABLE `batchuploadrecord` 
ADD COLUMN `IsCorrectUploaded` BIT(1) NOT NULL DEFAULT b'1' AFTER `UploadedOn`;

ALTER TABLE `batchuploadrecord` 
CHANGE COLUMN `UploadedOn` `UploadedOn` DATE NULL ;
