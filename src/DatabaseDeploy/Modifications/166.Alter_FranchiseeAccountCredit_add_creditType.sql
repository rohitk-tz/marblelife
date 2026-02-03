ALTER TABLE `franchiseeaccountcredit` 
ADD COLUMN `CreditTypeId` BIGINT(20) NULL DEFAULT NULL,
ADD INDEX `fk_FranchiseeAccountCreadit_lookup_idx` (`CreditTypeId` ASC);
ALTER TABLE `franchiseeaccountcredit` 
ADD CONSTRAINT `fk_FranchiseeAccountCreadit_lookup`
  FOREIGN KEY (`CreditTypeId`)
  REFERENCES `lookup` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;
  
INSERT INTO `lookuptype` (`Id`, `Name`) VALUES ('20', 'AccountCreditType');

INSERT INTO `lookup` (`Id`, `LookupTypeId`, `Name`, `Alias`, `RelativeOrder`) VALUES ('161', '20', 'AdFund', 'AdFund', '1');
INSERT INTO `lookup` (`Id`, `LookupTypeId`, `Name`, `Alias`, `RelativeOrder`) VALUES ('162', '20', 'Royalty', 'Royalty', '2');


