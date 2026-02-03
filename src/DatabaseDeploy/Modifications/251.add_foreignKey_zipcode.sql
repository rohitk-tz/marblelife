
ALTER TABLE `zipcode` 
ADD INDEX `fk_zipcode_county_idx` (`CountyId` ASC);
ALTER TABLE `zipcode` 
ADD CONSTRAINT `fk_zipcode_county`
  FOREIGN KEY (`CountyId`)
  REFERENCES `county` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;