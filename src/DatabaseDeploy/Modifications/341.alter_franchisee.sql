 ALTER TABLE `franchisee` 
ADD COLUMN `CategoryId` BIGINT(20) NULL DEFAULT null,
ADD INDEX `fk_frannchisee_lookup_idx` (`CategoryId` ASC);
ALTER TABLE `franchisee` 
ADD CONSTRAINT `fk_frannchisee_lookup`
  FOREIGN KEY (`CategoryId`)
  REFERENCES `Lookup` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;


   ALTER TABLE `franchisee` 
ADD COLUMN `CategoryNotes` varchar(512) NULL DEFAULT NULL;