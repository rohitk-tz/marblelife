Alter table beforeafterimages
Add column `BestPairMarkedBy` bigint(20) NULL default Null,
ADD INDEX `fk_beforeafterimages_person_idx` (`BestPairMarkedBy`);
ALTER TABLE `beforeafterimages` 
ADD CONSTRAINT `fk_beforeafterimages_person`
  FOREIGN KEY (`BestPairMarkedBy`)
  REFERENCES `person` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;
