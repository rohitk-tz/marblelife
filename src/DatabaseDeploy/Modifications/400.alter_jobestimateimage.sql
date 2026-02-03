ALTER TABLE `jobestimateimage` 
ADD COLUMN `AddToGalleryDateTime` Datetime NULL;


ALTER TABLE `jobestimateimage` 
ADD COLUMN `IsAddToLocalGallery` BIGINT(20) NULL DEFAULT False;