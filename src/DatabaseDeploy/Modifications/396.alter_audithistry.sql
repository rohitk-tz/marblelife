
INSERT INTO `makalu`.`lookuptype` (`Id`, `Name`, `IsDeleted`) VALUES ('30', 'Before After Images', b'0');




INSERT INTO `makalu`.`lookup` (`Id`, `LookupTypeId`, `Name`, `Alias`, `RelativeOrder`, `IsActive`, `IsDeleted`) VALUES ('226', '30', 'BEST PAIR', 'BEST PAIR', '4', b'1', b'0');

INSERT INTO `makalu`.`lookup` (`Id`, `LookupTypeId`, `Name`, `Alias`, `RelativeOrder`, `IsActive`, `IsDeleted`) VALUES ('227', '30', 'REVIEW MARKETING IMAGES', 'REVIEW MARKETING IMAGES', '4', b'1', b'0');


ALTER TABLE `markbeforeafterimageshistry` 
ADD COLUMN `bestTypeId` BIGINT(20) NULL DEFAULT 226 AFTER `IsDeleted`,
ADD INDEX `fk_markbeforeafterimageshistry_bestType_idx` (`bestTypeId` ASC);
ALTER TABLE `markbeforeafterimageshistry` 
ADD CONSTRAINT `fk_markbeforeafterimageshistry_bestType`
  FOREIGN KEY (`bestTypeId`)
  REFERENCES `lookup` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;