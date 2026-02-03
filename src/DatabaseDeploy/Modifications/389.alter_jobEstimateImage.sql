

ALTER TABLE `jobestimateservices` 
ADD COLUMN `IsFromEstimate` BIGINT(20) NULL DEFAULT False AFTER `FloorNumber`;

ALTER TABLE `jobestimateimage` 
ADD COLUMN `IsBestImage` BIGINT(20) NULL DEFAULT False AFTER `TypeId`;