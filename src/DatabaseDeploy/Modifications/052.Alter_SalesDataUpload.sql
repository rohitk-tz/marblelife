ALTER TABLE `SalesdataUpload` 
ADD COLUMN `IsActive` BIT(1) NOT NULL DEFAULT b'1';

UPDATE `SalesdataUpload` SET `IsActive`= 1 WHERE `Id`> 0;