UPDATE `makalu`.`servicetype` SET `Name` = 'MAINTENANCE:OTHER' WHERE (`Id` = '42');
UPDATE `makalu`.`servicetype` SET `Name` = 'MAINTENANCE:QUARTERLY' WHERE (`Id` = '8');
UPDATE `makalu`.`servicetype` SET `Name` = 'MAINTENANCE:BI-MONTHLY' WHERE (`Id` = '7');
UPDATE `makalu`.`servicetype` SET `Name` = 'MAINTENANCE:MONTHLY' WHERE (`Id` = '6');

INSERT INTO makalu.servicestag (ServiceTypeId, CategoryId, MaterialType, Service, IsDeleted, IsActive)
VALUES (33, 285, "Concrete", "Concrete Floor Prep - Grind", 0, 0);

ALTER TABLE franchiseedurationnoteshistry
MODIFY COLUMN Duration decimal(10,2);

ALTER TABLE franchisee
MODIFY COLUMN Duration decimal(10,2);

UPDATE `makalu`.`marketingclass` SET `CategoryId` = '256' WHERE (`Id` = '21');
UPDATE `makalu`.`marketingclass` SET `NewOrderBy` = '32' WHERE (`Id` = '21');

