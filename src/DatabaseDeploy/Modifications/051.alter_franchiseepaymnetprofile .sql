truncate table paymentinstrument;

DELETE FROM `franchiseepaymentprofile` WHERE Id>0;

ALTER TABLE `paymentinstrument` 
DROP FOREIGN KEY `fk_PaymentInstrument_FranchiseePaymentProfile`;

ALTER TABLE `franchiseepaymentprofile` 
DROP FOREIGN KEY `fk_FranchiseePaymentProfile_Franchisee`;

ALTER TABLE `franchiseepaymentprofile` 
CHANGE COLUMN `Id` `Id` BIGINT(20) NOT NULL AUTO_INCREMENT ,
ADD COLUMN `FranchiseeId` BIGINT(20) NOT NULL AFTER `Id`,
ADD INDEX `fk_franchiseepaymentprofile_franchisee_idx` (`FranchiseeId` ASC);
ALTER TABLE `franchiseepaymentprofile` 
ADD CONSTRAINT `fk_franchiseepaymentprofile_franchisee`
  FOREIGN KEY (`FranchiseeId`)
  REFERENCES `franchisee` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;

  
ALTER TABLE `paymentinstrument` 
ADD CONSTRAINT `fk_PaymentInstrument_FranchiseePaymentProfile`
  FOREIGN KEY (`FranchiseePaymentProfileId`)
  REFERENCES `franchiseepaymentprofile` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;


 
Set @lookuptypeId = 16;

call createlookuptype(@lookuptypeId, 'AuthorizeNetAccountType','');

call createlookup(130, @lookuptypeId, 'AdFund','AdFund');
call createlookup(131, @lookuptypeId, 'Royality','Royality');

ALTER TABLE `franchiseepaymentprofile` 
ADD COLUMN `ProfileTypeId` BIGINT(20) NOT NULL AFTER `IsActive`,
ADD INDEX `fk_franchiseepaymentprofile_lokup_idx` (`ProfileTypeId` ASC);
ALTER TABLE `franchiseepaymentprofile` 
ADD CONSTRAINT `fk_franchiseepaymentprofile_lokup`
  FOREIGN KEY (`ProfileTypeId`)
  REFERENCES `lookup` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;
