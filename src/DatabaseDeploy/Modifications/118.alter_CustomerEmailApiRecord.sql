ALTER TABLE `CustomerEmailapiRecord` 
ADD COLUMN `IsFailed` BIT(1) NOT NULL DEFAULT b'0' AFTER `IsDeleted`;
