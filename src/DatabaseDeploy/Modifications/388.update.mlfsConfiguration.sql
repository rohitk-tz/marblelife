delete from `makalu`.`mlfsconfigurationsetting`;

ALTER TABLE `makalu`.`mlfsconfigurationsetting` 
CHANGE COLUMN `MinValue` `MinValue` DECIMAL(3,1) NOT NULL ,
CHANGE COLUMN `MaxValue` `MaxValue` DECIMAL(3,1) NOT NULL ;
