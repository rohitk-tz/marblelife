ALTER TABLE `servicetype` 
ADD COLUMN `NewOrderBy` varchar(100) NULL;



INSERT INTO `servicetype` (`Id`, `Name`, `ColorCode`, `CategoryId`) VALUES ('41', 'WEB-AMAZONMEXICO', '#00008B', '103');

UPDATE `makalu`.`servicetype` SET `Description` = 'Carpet Cleaning (Non-Royalty)' WHERE (`Id` = '16');
UPDATE `makalu`.`servicetype` SET `Description` = 'Air Purifying Glass Coatings' WHERE (`Id` = '13');
UPDATE `makalu`.`servicetype` SET `Description` = 'Countertop Treatment' WHERE (`Id` = '11');
UPDATE `makalu`.`servicetype` SET `Description` = 'Endurachip, Endurachip, Marbilized' WHERE (`Id` = '33');
UPDATE `makalu`.`servicetype` SET `Description` = 'Countertop services' WHERE (`Id` = '5');
UPDATE `makalu`.`servicetype` SET `Description` = 'Terrazzo and Concrete polishing and Staining' WHERE (`Id` = '2');
UPDATE `makalu`.`servicetype` SET `Description` = 'Colorseal, Tilelok' WHERE (`Id` = '3');
UPDATE `makalu`.`servicetype` SET `Description` = 'Maintenance' WHERE (`Id` = '7');
UPDATE `makalu`.`servicetype` SET `Description` = 'Maintenance' WHERE (`Id` = '6');
UPDATE `makalu`.`servicetype` SET `Description` = 'Maintenance' WHERE (`Id` = '8');
UPDATE `makalu`.`servicetype` SET `Description` = 'Metal Polishing' WHERE (`Id` = '15');
UPDATE `makalu`.`servicetype` SET `Description` = 'Product Sales' WHERE (`Id` = '18');
UPDATE `makalu`.`servicetype` SET `Description` = 'Marble, Granite and Natural Stone Services' WHERE (`Id` = '1');
UPDATE `makalu`.`servicetype` SET `Description` = 'Tile installation and Repair' WHERE (`Id` = '17');
UPDATE `makalu`.`servicetype` SET `Description` = 'Vinyl Services' WHERE (`Id` = '4');
UPDATE `makalu`.`servicetype` SET `Description` = 'Wood Services' WHERE (`Id` = '40');

UPDATE `makalu`.`servicetype` SET `Description` = 'Product sold to Fabricators' WHERE (`Id` = '14');
UPDATE `makalu`.`servicetype` SET `Description` = 'Product sold to Government' WHERE (`Id` = '31');
UPDATE `makalu`.`servicetype` SET `Description` = 'Product sold via Hardware' WHERE (`Id` = '27');
UPDATE `makalu`.`servicetype` SET `Description` = 'Product sold to Hotels' WHERE (`Id` = '32');
UPDATE `makalu`.`servicetype` SET `Description` = 'Product sold to MLFS' WHERE (`Id` = '21');
UPDATE `makalu`.`servicetype` SET `Description` = 'Product internal use' WHERE (`Id` = '30');
UPDATE `makalu`.`servicetype` SET `Description` = 'Product sold to Retail' WHERE (`Id` = '28');
UPDATE `makalu`.`servicetype` SET `Description` = 'Product used in R&D' WHERE (`Id` = '29');
UPDATE `makalu`.`servicetype` SET `Description` = 'Product sold via ML Online Store' WHERE (`Id` = '20');
UPDATE `makalu`.`servicetype` SET `Description` = 'Product sold through Amazon' WHERE (`Id` = '24');
UPDATE `makalu`.`servicetype` SET `Description` = 'Product sold through Jet' WHERE (`Id` = '22');
UPDATE `makalu`.`servicetype` SET `Description` = 'Product sold through Walmart' WHERE (`Id` = '23');
UPDATE `makalu`.`servicetype` SET `Description` = 'Product sold through AMZ Canada' WHERE (`Id` = '26');
UPDATE `makalu`.`servicetype` SET `Description` = 'Product sold via Amazon Merchant' WHERE (`Id` = '25');
UPDATE `makalu`.`servicetype` SET `Description` = 'other' WHERE (`Id` = '37');
UPDATE `makalu`.`servicetype` SET `Description` = 'Product sold through AMZ Mexico (NEW)' WHERE (`Id` = '41');



UPDATE `makalu`.`servicetype` SET `NewOrderBy` = '17' WHERE (`Id` = '1');
UPDATE `makalu`.`servicetype` SET `NewOrderBy` = '1' WHERE (`Id` = '16');
UPDATE `makalu`.`servicetype` SET `NewOrderBy` = '2' WHERE (`Id` = '13');
UPDATE `makalu`.`servicetype` SET `NewOrderBy` = '3' WHERE (`Id` = '11');
UPDATE `makalu`.`servicetype` SET `NewOrderBy` = '4' WHERE (`Id` = '33');
UPDATE `makalu`.`servicetype` SET `NewOrderBy` = '5' WHERE (`Id` = '34');
UPDATE `makalu`.`servicetype` SET `NewOrderBy` = '6' WHERE (`Id` = '35');
UPDATE `makalu`.`servicetype` SET `NewOrderBy` = '7' WHERE (`Id` = '38');
UPDATE `makalu`.`servicetype` SET `NewOrderBy` = '8' WHERE (`Id` = '5');
UPDATE `makalu`.`servicetype` SET `NewOrderBy` = '9' WHERE (`Id` = '2');
UPDATE `makalu`.`servicetype` SET `NewOrderBy` = '10' WHERE (`Id` = '3');
UPDATE `makalu`.`servicetype` SET `NewOrderBy` = '11' WHERE (`Id` = '7');
UPDATE `makalu`.`servicetype` SET `NewOrderBy` = '12' WHERE (`Id` = '6');
UPDATE `makalu`.`servicetype` SET `NewOrderBy` = '13' WHERE (`Id` = '8');
UPDATE `makalu`.`servicetype` SET `NewOrderBy` = '15' WHERE (`Id` = '15');
UPDATE `makalu`.`servicetype` SET `NewOrderBy` = '16' WHERE (`Id` = '18');
UPDATE `makalu`.`servicetype` SET `NewOrderBy` = '18' WHERE (`Id` = '17');
UPDATE `makalu`.`servicetype` SET `NewOrderBy` = '19' WHERE (`Id` = '4');
UPDATE `makalu`.`servicetype` SET `NewOrderBy` = '20' WHERE (`Id` = '40');
UPDATE `makalu`.`servicetype` SET `NewOrderBy` = '21' WHERE (`Id` = '19');


UPDATE `makalu`.`servicetype` SET `NewOrderBy` = 'MLD-1' WHERE (`Id` = '36');
UPDATE `makalu`.`servicetype` SET `NewOrderBy` = 'MLD-2' WHERE (`Id` = '14');
UPDATE `makalu`.`servicetype` SET `NewOrderBy` = 'MLD-3' WHERE (`Id` = '31');
UPDATE `makalu`.`servicetype` SET `NewOrderBy` = 'MLD-4' WHERE (`Id` = '27');
UPDATE `makalu`.`servicetype` SET `NewOrderBy` = 'MLD-5' WHERE (`Id` = '32');
UPDATE `makalu`.`servicetype` SET `NewOrderBy` = 'MLD-6' WHERE (`Id` = '21');
UPDATE `makalu`.`servicetype` SET `NewOrderBy` = 'MLD-7' WHERE (`Id` = '30');
UPDATE `makalu`.`servicetype` SET `NewOrderBy` = 'MLD-8' WHERE (`Id` = '28');
UPDATE `makalu`.`servicetype` SET `NewOrderBy` = 'MLD-9' WHERE (`Id` = '29');
UPDATE `makalu`.`servicetype` SET `NewOrderBy` = 'MLD-10' WHERE (`Id` = '20');
UPDATE `makalu`.`servicetype` SET `NewOrderBy` = 'MLD-11' WHERE (`Id` = '24');
UPDATE `makalu`.`servicetype` SET `NewOrderBy` = 'MLD-12' WHERE (`Id` = '22');
UPDATE `makalu`.`servicetype` SET `NewOrderBy` = 'MLD-13' WHERE (`Id` = '23');
UPDATE `makalu`.`servicetype` SET `NewOrderBy` = 'MLD-14' WHERE (`Id` = '26');
UPDATE `makalu`.`servicetype` SET `NewOrderBy` = 'MLD-15' WHERE (`Id` = '41');
UPDATE `makalu`.`servicetype` SET `NewOrderBy` = 'MLD-16' WHERE (`Id` = '25');
UPDATE `makalu`.`servicetype` SET `NewOrderBy` = 'MLD-17' WHERE (`Id` = '37');

UPDATE `makalu`.`servicetype` SET `NewOrderBy` = 'FO-1' WHERE (`Id` = '39');


INSERT INTO `makalu`.`lookup` (`Id`, `LookupTypeId`, `Name`, `Alias`, `RelativeOrder`, `IsActive`, `IsDeleted`) VALUES ('252', '11', ' ', ' ', '4', b'1', b'0');
INSERT INTO `makalu`.`lookup` (`Id`, `LookupTypeId`, `Name`, `Alias`, `RelativeOrder`, `IsActive`, `IsDeleted`) VALUES ('253', '11', 'ML-DISTRIBUTION CLASSES', 'ML-DISTRIBUTIONCLASSES', '5', b'1', b'0');
INSERT INTO `makalu`.`lookup` (`Id`, `LookupTypeId`, `Name`, `Alias`, `RelativeOrder`, `IsActive`, `IsDeleted`) VALUES ('254', '11', 'FRONT OFFICE CALL MANAGEMENT', 'FRONT OFFICECALLMANAGEMENT', '6', b'1', b'0');

ALTER TABLE `servicetype` 
ADD COLUMN `SubCategoryId` BIGINT(20) NULL DEFAULT null AFTER `CategoryId`,
ADD INDEX `fk_servicetype_service_idx` (`SubCategoryId` ASC);
ALTER TABLE `servicetype` 
ADD CONSTRAINT `fk_servicetype_service`
  FOREIGN KEY (`SubCategoryId`)
  REFERENCES `Lookup` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;
  
  
UPDATE `makalu`.`servicetype` SET `SubCategoryId` = '254' WHERE (`Id` = '14');
UPDATE `makalu`.`servicetype` SET `SubCategoryId` = '254' WHERE (`Id` = '20');
UPDATE `makalu`.`servicetype` SET `SubCategoryId` = '254' WHERE (`Id` = '21');
UPDATE `makalu`.`servicetype` SET `SubCategoryId` = '254' WHERE (`Id` = '22');
UPDATE `makalu`.`servicetype` SET `SubCategoryId` = '254' WHERE (`Id` = '23');
UPDATE `makalu`.`servicetype` SET `SubCategoryId` = '254' WHERE (`Id` = '24');
UPDATE `makalu`.`servicetype` SET `SubCategoryId` = '254' WHERE (`Id` = '25');
UPDATE `makalu`.`servicetype` SET `SubCategoryId` = '254' WHERE (`Id` = '26');
UPDATE `makalu`.`servicetype` SET `SubCategoryId` = '254' WHERE (`Id` = '27');
UPDATE `makalu`.`servicetype` SET `SubCategoryId` = '254' WHERE (`Id` = '28');
UPDATE `makalu`.`servicetype` SET `SubCategoryId` = '254' WHERE (`Id` = '29');
UPDATE `makalu`.`servicetype` SET `SubCategoryId` = '254' WHERE (`Id` = '30');
UPDATE `makalu`.`servicetype` SET `SubCategoryId` = '254' WHERE (`Id` = '31');
UPDATE `makalu`.`servicetype` SET `SubCategoryId` = '254' WHERE (`Id` = '32');
UPDATE `makalu`.`servicetype` SET `SubCategoryId` = '254' WHERE (`Id` = '36');
UPDATE `makalu`.`servicetype` SET `SubCategoryId` = '254' WHERE (`Id` = '37');
UPDATE `makalu`.`servicetype` SET `SubCategoryId` = '254' WHERE (`Id` = '41');

UPDATE `makalu`.`servicetype` SET `SubCategoryId` = '252' WHERE (`Id` = '1');
UPDATE `makalu`.`servicetype` SET `SubCategoryId` = '252' WHERE (`Id` = '2');
UPDATE `makalu`.`servicetype` SET `SubCategoryId` = '252' WHERE (`Id` = '3');
UPDATE `makalu`.`servicetype` SET `SubCategoryId` = '252' WHERE (`Id` = '4');
UPDATE `makalu`.`servicetype` SET `SubCategoryId` = '252' WHERE (`Id` = '5');
UPDATE `makalu`.`servicetype` SET `SubCategoryId` = '252' WHERE (`Id` = '6');
UPDATE `makalu`.`servicetype` SET `SubCategoryId` = '252' WHERE (`Id` = '7');
UPDATE `makalu`.`servicetype` SET `SubCategoryId` = '252' WHERE (`Id` = '8');
UPDATE `makalu`.`servicetype` SET `SubCategoryId` = '252' WHERE (`Id` = '11');
UPDATE `makalu`.`servicetype` SET `SubCategoryId` = '252' WHERE (`Id` = '13');
UPDATE `makalu`.`servicetype` SET `SubCategoryId` = '252' WHERE (`Id` = '15');
UPDATE `makalu`.`servicetype` SET `SubCategoryId` = '252' WHERE (`Id` = '16');
UPDATE `makalu`.`servicetype` SET `SubCategoryId` = '252' WHERE (`Id` = '17');
UPDATE `makalu`.`servicetype` SET `SubCategoryId` = '252' WHERE (`Id` = '18');
UPDATE `makalu`.`servicetype` SET `SubCategoryId` = '252' WHERE (`Id` = '19');
UPDATE `makalu`.`servicetype` SET `SubCategoryId` = '252' WHERE (`Id` = '33');
UPDATE `makalu`.`servicetype` SET `SubCategoryId` = '252' WHERE (`Id` = '34');
UPDATE `makalu`.`servicetype` SET `SubCategoryId` = '252' WHERE (`Id` = '35');
UPDATE `makalu`.`servicetype` SET `SubCategoryId` = '252' WHERE (`Id` = '38');
UPDATE `makalu`.`servicetype` SET `SubCategoryId` = '252' WHERE (`Id` = '39');
UPDATE `makalu`.`servicetype` SET `SubCategoryId` = '252' WHERE (`Id` = '40');


UPDATE `makalu`.`servicetype` SET `SubCategoryId` = '253' WHERE (`Id` = '14');
UPDATE `makalu`.`servicetype` SET `SubCategoryId` = '253' WHERE (`Id` = '20');
UPDATE `makalu`.`servicetype` SET `SubCategoryId` = '253' WHERE (`Id` = '21');
UPDATE `makalu`.`servicetype` SET `SubCategoryId` = '253' WHERE (`Id` = '22');
UPDATE `makalu`.`servicetype` SET `SubCategoryId` = '253' WHERE (`Id` = '23');
UPDATE `makalu`.`servicetype` SET `SubCategoryId` = '253' WHERE (`Id` = '24');
UPDATE `makalu`.`servicetype` SET `SubCategoryId` = '253' WHERE (`Id` = '25');
UPDATE `makalu`.`servicetype` SET `SubCategoryId` = '253' WHERE (`Id` = '26');
UPDATE `makalu`.`servicetype` SET `SubCategoryId` = '253' WHERE (`Id` = '27');
UPDATE `makalu`.`servicetype` SET `SubCategoryId` = '253' WHERE (`Id` = '28');
UPDATE `makalu`.`servicetype` SET `SubCategoryId` = '253' WHERE (`Id` = '29');
UPDATE `makalu`.`servicetype` SET `SubCategoryId` = '253' WHERE (`Id` = '30');
UPDATE `makalu`.`servicetype` SET `SubCategoryId` = '253' WHERE (`Id` = '32');
UPDATE `makalu`.`servicetype` SET `SubCategoryId` = '253' WHERE (`Id` = '31');
UPDATE `makalu`.`servicetype` SET `SubCategoryId` = '253' WHERE (`Id` = '37');
UPDATE `makalu`.`servicetype` SET `SubCategoryId` = '253' WHERE (`Id` = '36');
UPDATE `makalu`.`servicetype` SET `SubCategoryId` = '253' WHERE (`Id` = '41');

UPDATE `makalu`.`servicetype` SET `SubCategoryId` = '254' WHERE (`Id` = '39');


UPDATE `makalu`.`servicetype` SET `NewOrderBy` = '1' WHERE (`Id` = '36');
UPDATE `makalu`.`servicetype` SET `NewOrderBy` = '10' WHERE (`Id` = '20');
UPDATE `makalu`.`servicetype` SET `NewOrderBy` = '11' WHERE (`Id` = '24');
UPDATE `makalu`.`servicetype` SET `NewOrderBy` = '12' WHERE (`Id` = '22');
UPDATE `makalu`.`servicetype` SET `NewOrderBy` = '13' WHERE (`Id` = '23');
UPDATE `makalu`.`servicetype` SET `NewOrderBy` = '14' WHERE (`Id` = '26');
UPDATE `makalu`.`servicetype` SET `NewOrderBy` = '15' WHERE (`Id` = '41');
UPDATE `makalu`.`servicetype` SET `NewOrderBy` = '16' WHERE (`Id` = '25');
UPDATE `makalu`.`servicetype` SET `NewOrderBy` = '17' WHERE (`Id` = '37');
UPDATE `makalu`.`servicetype` SET `NewOrderBy` = '2' WHERE (`Id` = '14');
UPDATE `makalu`.`servicetype` SET `NewOrderBy` = '3' WHERE (`Id` = '31');
UPDATE `makalu`.`servicetype` SET `NewOrderBy` = '4' WHERE (`Id` = '27');
UPDATE `makalu`.`servicetype` SET `NewOrderBy` = '5' WHERE (`Id` = '32');
UPDATE `makalu`.`servicetype` SET `NewOrderBy` = '6' WHERE (`Id` = '21');
UPDATE `makalu`.`servicetype` SET `NewOrderBy` = '7' WHERE (`Id` = '30');
UPDATE `makalu`.`servicetype` SET `NewOrderBy` = '8' WHERE (`Id` = '28');
UPDATE `makalu`.`servicetype` SET `NewOrderBy` = '9' WHERE (`Id` = '29');
  
UPDATE `makalu`.`servicetype` SET `NewOrderBy` = '1' WHERE (`Id` = '39');

ALTER TABLE `makalu`.`servicetype` 
CHANGE COLUMN `NewOrderBy` `NewOrderBy` BIGINT(20) NULL DEFAULT NULL ;
