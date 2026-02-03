ALTER TABLE `marketingclass` 
ADD COLUMN `NewOrderBy` BIGINT(20) NULL;

ALTER TABLE `marketingclass` 
ADD COLUMN `CategoryId` BIGINT(20) NULL DEFAULT null AFTER `ColorCode`,
ADD INDEX `fk_marketingclass_service_idx` (`NewOrderBy` ASC);
ALTER TABLE `marketingclass` 
ADD CONSTRAINT `fk_marketingclass_service`
  FOREIGN KEY (`CategoryId`)
  REFERENCES `Lookup` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;
  
INSERT INTO `makalu`.`lookupType` (`Id`, `Name`, `IsDeleted`) VALUES ('40', 'MarketingClassCategory', b'0');
INSERT INTO `makalu`.`lookup` (`Id`, `LookupTypeId`, `Name`, `Alias`, `RelativeOrder`, `IsActive`, `IsDeleted`) VALUES ('255', '40', 'OFFICE BUILDINGS', 'OFFICEBUILDINGS', '1', b'1', b'0');
INSERT INTO `makalu`.`lookup` (`Id`, `LookupTypeId`, `Name`, `Alias`, `RelativeOrder`, `IsActive`, `IsDeleted`) VALUES ('256', '40', 'RESIDENTIAL BUILDINGS', 'RESIDENTIALBUILDINGS', '2', b'1', b'0');
INSERT INTO `makalu`.`lookup` (`Id`, `LookupTypeId`, `Name`, `Alias`, `RelativeOrder`, `IsActive`, `IsDeleted`) VALUES ('257', '40', 'PUBLIC BUILDINGS', 'PUBLICBUILDINGS', '3', b'1', b'0');
INSERT INTO `makalu`.`lookup` (`Id`, `LookupTypeId`, `Name`, `Alias`, `RelativeOrder`, `IsActive`, `IsDeleted`) VALUES ('258', '40', 'REFERRAL CONTACTS', 'REFERRALCONTACTS', '4', b'1', b'0');
INSERT INTO `makalu`.`lookup` (`Id`, `LookupTypeId`, `Name`, `Alias`, `RelativeOrder`, `IsActive`, `IsDeleted`) VALUES ('259', '40', 'NATIONAL', 'NATIONAL', '5', b'1', b'0');
INSERT INTO `makalu`.`lookup` (`Id`, `LookupTypeId`, `Name`, `Alias`, `RelativeOrder`, `IsActive`, `IsDeleted`) VALUES ('260', '40', 'FRONT OFFICE', 'FRONTOFFICE', '6', b'1', b'0');
INSERT INTO `makalu`.`lookup` (`Id`, `LookupTypeId`, `Name`, `Alias`, `RelativeOrder`, `IsActive`, `IsDeleted`) VALUES ('261', '40', 'NON-PROJECT DATA SOURCES', 'NONPROJECTDATASOURCES', '5', b'1', b'0');  


UPDATE `makalu`.`marketingclass` SET `NewOrderBy` = '1' WHERE (`Id` = '1');
UPDATE `makalu`.`marketingclass` SET `NewOrderBy` = '2' WHERE (`Id` = '30');
UPDATE `makalu`.`marketingclass` SET `NewOrderBy` = '3' WHERE (`Id` = '28');
UPDATE `makalu`.`marketingclass` SET `NewOrderBy` = '4' WHERE (`Id` = '14');
UPDATE `makalu`.`marketingclass` SET `NewOrderBy` = '5' WHERE (`Id` = '23');
UPDATE `makalu`.`marketingclass` SET `NewOrderBy` = '6' WHERE (`Id` = '25');
UPDATE `makalu`.`marketingclass` SET `NewOrderBy` = '7' WHERE (`Id` = '9');
UPDATE `makalu`.`marketingclass` SET `NewOrderBy` = '8' WHERE (`Id` = '35');
UPDATE `makalu`.`marketingclass` SET `CategoryId` = '257' WHERE (`Id` = '1');
UPDATE `makalu`.`marketingclass` SET `CategoryId` = '257' WHERE (`Id` = '9');
UPDATE `makalu`.`marketingclass` SET `CategoryId` = '257' WHERE (`Id` = '14');
UPDATE `makalu`.`marketingclass` SET `CategoryId` = '257' WHERE (`Id` = '23');
UPDATE `makalu`.`marketingclass` SET `CategoryId` = '257' WHERE (`Id` = '25');
UPDATE `makalu`.`marketingclass` SET `CategoryId` = '257' WHERE (`Id` = '28');
UPDATE `makalu`.`marketingclass` SET `CategoryId` = '257' WHERE (`Id` = '30');
UPDATE `makalu`.`marketingclass` SET `CategoryId` = '257' WHERE (`Id` = '35');


UPDATE `makalu`.`marketingclass` SET `CategoryId` = '253', `NewOrderBy` = '9' WHERE (`Id` = '12');
UPDATE `makalu`.`marketingclass` SET `CategoryId` = '253', `NewOrderBy` = '10' WHERE (`Id` = '4');



UPDATE `makalu`.`marketingclass` SET `CategoryId` = '254', `NewOrderBy` = '11' WHERE (`Id` = '22');
UPDATE `makalu`.`marketingclass` SET `CategoryId` = '254', `NewOrderBy` = '12' WHERE (`Id` = '13');
UPDATE `makalu`.`marketingclass` SET `CategoryId` = '254', `NewOrderBy` = '13' WHERE (`Id` = '6');
UPDATE `makalu`.`marketingclass` SET `CategoryId` = '254', `NewOrderBy` = '14' WHERE (`Id` = '7');
UPDATE `makalu`.`marketingclass` SET `CategoryId` = '254', `NewOrderBy` = '15' WHERE (`Id` = '2');
UPDATE `makalu`.`marketingclass` SET `CategoryId` = '254', `NewOrderBy` = '16' WHERE (`Id` = '32');
UPDATE `makalu`.`marketingclass` SET `CategoryId` = '254', `NewOrderBy` = '17' WHERE (`Id` = '3');
UPDATE `makalu`.`marketingclass` SET `CategoryId` = '254', `NewOrderBy` = '18' WHERE (`Id` = '33');
UPDATE `makalu`.`marketingclass` SET `CategoryId` = '254', `NewOrderBy` = '19' WHERE (`Id` = '10');
UPDATE `makalu`.`marketingclass` SET `CategoryId` = '254', `NewOrderBy` = '20' WHERE (`Id` = '20');
UPDATE `makalu`.`marketingclass` SET `CategoryId` = '254', `NewOrderBy` = '21' WHERE (`Id` = '31');
UPDATE `makalu`.`marketingclass` SET `CategoryId` = '254', `NewOrderBy` = '22' WHERE (`Id` = '24');

UPDATE `makalu`.`marketingclass` SET `CategoryId` = '255', `NewOrderBy` = '23' WHERE (`Id` = '5');
UPDATE `makalu`.`marketingclass` SET `CategoryId` = '255', `NewOrderBy` = '24' WHERE (`Id` = '15');
UPDATE `makalu`.`marketingclass` SET `CategoryId` = '255', `NewOrderBy` = '25' WHERE (`Id` = '16');
UPDATE `makalu`.`marketingclass` SET `CategoryId` = '255', `NewOrderBy` = '26' WHERE (`Id` = '26');
UPDATE `makalu`.`marketingclass` SET `CategoryId` = '255', `NewOrderBy` = '27' WHERE (`Id` = '8');

UPDATE `makalu`.`marketingclass` SET `CategoryId` = '256', `NewOrderBy` = '28' WHERE (`Id` = '17');


UPDATE `makalu`.`marketingclass` SET `Description` = 'Multi-Family Homes (Condo, Appartments)' WHERE (`Id` = '12');

UPDATE `makalu`.`marketingclass` SET `Description` = 'Single-Family Homes' WHERE (`Id` = '4');
UPDATE `makalu`.`marketingclass` SET `Description` = 'Auto Dealerships' WHERE (`Id` = '22');
UPDATE `makalu`.`marketingclass` SET `Description` = 'Country Clubs and Spas' WHERE (`Id` = '7');
UPDATE `makalu`.`marketingclass` SET `Description` = 'Schools, Colleges, Universities' WHERE (`Id` = '2');
UPDATE `makalu`.`marketingclass` SET `Description` = 'Yacht Clubs and Marinas' WHERE (`Id` = '24');


UPDATE `makalu`.`marketingclass` SET `Description` = 'Builders and Remodelers' WHERE (`Id` = '5');
UPDATE `makalu`.`marketingclass` SET `Description` = 'Flooring stores and contractors' WHERE (`Id` = '15');
UPDATE `makalu`.`marketingclass` SET `Description` = 'Thse should all be RESIDENTIAL: WOODLIFE' WHERE (`Id` = '29');

UPDATE `makalu`.`marketingclass` SET `CategoryId` = '258' WHERE (`Id` = '18');
UPDATE `makalu`.`marketingclass` SET `CategoryId` = '258' WHERE (`Id` = '34');

UPDATE `makalu`.`marketingclass` SET `NewOrderBy` = '31' WHERE (`Id` = '18');
UPDATE `makalu`.`marketingclass` SET `NewOrderBy` = '32' WHERE (`Id` = '34');
UPDATE `makalu`.`marketingclass` SET `NewOrderBy` = '29' WHERE (`Id` = '19');


UPDATE `makalu`.`marketingclass` SET `Description` = 'Interior Designers' WHERE (`Id` = '16');


UPDATE `makalu`.`marketingclass` SET `CategoryId` = '255' WHERE (`Id` = '1');
UPDATE `makalu`.`marketingclass` SET `CategoryId` = '255' WHERE (`Id` = '9');
UPDATE `makalu`.`marketingclass` SET `CategoryId` = '255' WHERE (`Id` = '14');
UPDATE `makalu`.`marketingclass` SET `CategoryId` = '255' WHERE (`Id` = '23');
UPDATE `makalu`.`marketingclass` SET `CategoryId` = '255' WHERE (`Id` = '25');
UPDATE `makalu`.`marketingclass` SET `CategoryId` = '255' WHERE (`Id` = '28');
UPDATE `makalu`.`marketingclass` SET `CategoryId` = '255' WHERE (`Id` = '30');
UPDATE `makalu`.`marketingclass` SET `CategoryId` = '255' WHERE (`Id` = '35');


UPDATE `makalu`.`marketingclass` SET `CategoryId` = '256' WHERE (`Id` = '4');
UPDATE `makalu`.`marketingclass` SET `CategoryId` = '256' WHERE (`Id` = '12');


UPDATE `makalu`.`marketingclass` SET `CategoryId` = '257' WHERE (`Id` = '2');
UPDATE `makalu`.`marketingclass` SET `CategoryId` = '257' WHERE (`Id` = '3');
UPDATE `makalu`.`marketingclass` SET `CategoryId` = '257' WHERE (`Id` = '6');
UPDATE `makalu`.`marketingclass` SET `CategoryId` = '257' WHERE (`Id` = '7');
UPDATE `makalu`.`marketingclass` SET `CategoryId` = '257' WHERE (`Id` = '10');
UPDATE `makalu`.`marketingclass` SET `CategoryId` = '257' WHERE (`Id` = '13');
UPDATE `makalu`.`marketingclass` SET `CategoryId` = '257' WHERE (`Id` = '20');
UPDATE `makalu`.`marketingclass` SET `CategoryId` = '257' WHERE (`Id` = '22');
UPDATE `makalu`.`marketingclass` SET `CategoryId` = '257' WHERE (`Id` = '24');
UPDATE `makalu`.`marketingclass` SET `CategoryId` = '257' WHERE (`Id` = '31');
UPDATE `makalu`.`marketingclass` SET `CategoryId` = '257' WHERE (`Id` = '32');
UPDATE `makalu`.`marketingclass` SET `CategoryId` = '257' WHERE (`Id` = '33');


UPDATE `makalu`.`marketingclass` SET `CategoryId` = '258' WHERE (`Id` = '15');
UPDATE `makalu`.`marketingclass` SET `CategoryId` = '258' WHERE (`Id` = '16');
UPDATE `makalu`.`marketingclass` SET `CategoryId` = '258' WHERE (`Id` = '26');
UPDATE `makalu`.`marketingclass` SET `CategoryId` = '258' WHERE (`Id` = '8');


UPDATE `makalu`.`marketingclass` SET `CategoryId` = '259' WHERE (`Id` = '17');

UPDATE `makalu`.`marketingclass` SET `CategoryId` = '261' WHERE (`Id` = '18');
UPDATE `makalu`.`marketingclass` SET `CategoryId` = '261' WHERE (`Id` = '34');

UPDATE `makalu`.`marketingclass` SET `CategoryId` = '258' WHERE (`Id` = '5');


INSERT INTO `makalu`.`servicetype` (`Id`, `Name`, `Description`, `CategoryId`, `SubCategoryId`, `IsActive`, `IsDeleted`, `OrderBy`, `NewOrderBy`) VALUES ('42', 'MAINTAINCE:OTHER', 'Maintenance', '102', '252', b'1', b'0', b'10', '16');


UPDATE `makalu`.`serviceType` SET `NewOrderBy` = '15' WHERE (`Id` = '9');
UPDATE `makalu`.`serviceType` SET `NewOrderBy` = '14' WHERE (`Id` = '42');

UPDATE `makalu`.`marketingclass` SET `CategoryId` = '260' WHERE (`Id` = '19');


INSERT INTO `makalu`.`marketingclass` (`Id`, `Name`, `ColorCode`, `IsDeleted`, `NewOrderBy`) VALUES ('36', 'OTHER', '#FFBF00', b'0', b'1');
UPDATE `makalu`.`marketingclass` SET `CategoryId` = '257', `Description` = 'Other' WHERE (`Id` = '36');
UPDATE `makalu`.`marketingclass` SET `NewOrderBy` = '20' WHERE (`Id` = '36');