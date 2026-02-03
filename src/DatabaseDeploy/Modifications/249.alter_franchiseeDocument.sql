ALTER TABLE `franchisedocument` 
ADD COLUMN `UserId` BIGINT(20) NULL,
ADD INDEX `fk_franchiseedocument_lookup1_idx` (`UserId`);
ALTER TABLE `franchisedocument` 
ADD CONSTRAINT `fk_franchiseedocument_lookup1_idx`
  FOREIGN KEY (`UserId`)
  REFERENCES `organizationroleuser` (`Userid`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;