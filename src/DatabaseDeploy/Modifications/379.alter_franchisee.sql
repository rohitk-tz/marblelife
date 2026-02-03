ALTER TABLE `franchisee` 
ADD COLUMN `reviewpushId` BIGINT(20) NULL DEFAULT NULL AFTER `website`,
ADD INDEX `fk_franchisee_reivewpush_idx` (`reviewpushId` ASC);
ALTER TABLE `franchisee` 
ADD CONSTRAINT `fk_franchisee_reivewpush`
  FOREIGN KEY (`reviewpushId`)
  REFERENCES `reviewpushapilocation` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;