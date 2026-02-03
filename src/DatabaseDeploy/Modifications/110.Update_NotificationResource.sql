ALTER TABLE `notificationresource` 
DROP FOREIGN KEY `fk_notificationResource_file`;
ALTER TABLE `makalu`.`notificationresource` 
CHANGE COLUMN `resourceId` `ResourceId` BIGINT(20) NOT NULL;
ALTER TABLE `notificationresource` 
ADD CONSTRAINT `fk_notificationResource_file`
  FOREIGN KEY (`ResourceId`)
  REFERENCES `file` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;
