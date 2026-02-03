
ALTER TABLE `beforeafterimages` 
ADD COLUMN `IsImageCropped` BIT(1) NOT NULL DEFAULT b'0';

ALTER TABLE `beforeafterimages` 
ADD COLUMN `CroppedImageId` bigint(20) NULL DEFAULT NULL;

ALTER TABLE `beforeafterimages` 
ADD COLUMN `CroppedImageThumbId` bigint(20) NULL DEFAULT NULL;