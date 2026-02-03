
ALTER TABLE `franchisedocument` 
ADD COLUMN `UploadFor` varchar(40) NULL;


ALTER TABLE `franchisee` 
ADD COLUMN `IsMinRoyalityFixed` bit(1) default false;

ALTER TABLE `franchisee` 
ADD COLUMN `RegistrationDate`DateTime default null;

