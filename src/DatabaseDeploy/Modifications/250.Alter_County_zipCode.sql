ALTER TABLE `county` 
CHANGE COLUMN `FranchseId` `FranchiseeId` BIGINT(20) NULL DEFAULT NULL ,
ADD INDEX `fk_county_franchisee_idx` (`FranchiseeId` ASC);
ALTER TABLE `county` 
ADD CONSTRAINT `fk_county_franchisee`
  FOREIGN KEY (`FranchiseeId`)
  REFERENCES `franchisee` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;

 ALTER TABLE `county`
CHANGE COLUMN `FranchiseeName` `FranchiseeName` VARCHAR(512) NOT NULL ;

ALTER TABLE `zipcode` 
CHANGE COLUMN `ZipCode` `Zip` VARCHAR(255) NOT NULL ;

truncate table ZipCode;
truncate table County;
truncate table geocodefileupload;

ALTER TABLE `zipcode` 
DROP FOREIGN KEY `fk_Zip_Code_Check_city`;
ALTER TABLE `zipcode` 
DROP INDEX `fk_Zip_Code_Check_city` ,
ADD INDEX `fk_ZipCode_city` (`CityId` ASC);
ALTER TABLE `zipcode` 
ADD CONSTRAINT `fk_ZipCode_city`
  FOREIGN KEY (`CityId`)
  REFERENCES `city` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;


 
