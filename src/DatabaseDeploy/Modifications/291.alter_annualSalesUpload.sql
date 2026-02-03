ALTER TABLE `annualsalesdataupload` 
ADD COLUMN `IsAuditAddressParsing` BIT(1) NOT NULL DEFAULT b'1' AFTER `AuditActionId`;