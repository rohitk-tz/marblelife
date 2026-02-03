
ALTER TABLE `reviewmarketingimagelastdatehistry` 
ADD COLUMN `StartDate`DateTime NULL DEFAULT null AFTER `IsDeleted`;

ALTER TABLE `reviewmarketingimagelastdatehistry` 
ADD COLUMN `EndDate`DateTime NULL DEFAULT null AFTER `IsDeleted`;


ALTER TABLE `reviewmarketingimagelastdatehistry` 
ADD COLUMN `IsReview`bool NULL DEFAULT null AFTER `IsDeleted`;