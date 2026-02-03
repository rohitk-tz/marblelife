CREATE TABLE `tag` (
  `Id` BIGINT(20) NOT NULL AUTO_INCREMENT,
  `name` VARCHAR(45) NOT NULL,
  `IsActive` BIT(1) NOT NULL DEFAULT b'1',
  `IsDeleted` BIT(1) NOT NULL DEFAULT b'0',
  PRIMARY KEY (`Id`));
  
ALTER TABLE `routingnumber` 
ADD COLUMN `TagId` BIGINT(20) NULL DEFAULT NULL,
ADD INDEX `fk_routingNumber_tag_idx` (`TagId` ASC);
ALTER TABLE `routingnumber` 
ADD CONSTRAINT `fk_routingNumber_tag`
  FOREIGN KEY (`TagId`)
  REFERENCES `tag` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;

