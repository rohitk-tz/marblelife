CREATE TABLE `termsAndConditionFranchisee` (
  `Id` bigint(20) NOT NULL AUTO_INCREMENT,
  `IsDeleted` bit(1) NOT NULL DEFAULT b'0',
  `TermAndCondition` text NOT NULL,
  `FranchiseeId` bigint(20) DEFAULT NULL,
   `TyepeId` bigint(20) NOT NULL,
  PRIMARY KEY (`Id`),
   KEY `fk_termsAndConditionFranchisee_frachisee_idx` (`FranchiseeId`),
 KEY `fk_termsAndConditionFranchisee_lookup1_idx` (`TyepeId`),
  CONSTRAINT `fk_termsAndConditionFranchisee_lookup1` FOREIGN KEY (`TyepeId`) REFERENCES `lookup` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `fk_termsAndConditionFranchisee_frachisee` FOREIGN KEY (`FranchiseeId`) REFERENCES `franchisee` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION

);

INSERT INTO `makalu`.`lookuptype` (`Id`, `Name`, `IsDeleted`) VALUES ('36', 'TermAndCondition', b'0');

INSERT INTO `makalu`.`lookup` (`Id`, `LookupTypeId`, `Name`, `Alias`, `RelativeOrder`, `IsActive`, `IsDeleted`) VALUES ('240', '36', 'Concrete Terms And Condition', 'Concrete Terms And Condition', '1', b'1', b'0');
INSERT INTO `makalu`.`lookup` (`Id`, `LookupTypeId`, `Name`, `Alias`, `RelativeOrder`, `IsActive`, `IsDeleted`) VALUES ('241', '36', 'Others Terms and Condition', 'Others Terms and Condition', '2', b'1', b'0');