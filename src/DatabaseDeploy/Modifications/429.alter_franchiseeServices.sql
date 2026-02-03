ALTER TABLE `franchiseeservice` 
ADD COLUMN `IsCertified` bit(1) default false;



ALTER TABLE `franchisee` 
ADD COLUMN `notesFromCallCenter` VARCHAR(1024) default Null;

ALTER TABLE `franchisee` 
ADD COLUMN `notesFromOwner` VARCHAR(1024) default Null;

ALTER TABLE `franchisee` 
ADD COLUMN `duration` bigInt(20) default 2;