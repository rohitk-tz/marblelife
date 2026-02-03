ALTER TABLE `franchiseetechmailemail` 
ADD COLUMN `DateForCharges` Datetime NULL DEFAULT null AFTER `isDeleted`;



ALTER TABLE `phonechargesfee` 
ADD COLUMN `franchiseetechmailserviceId` BIGINT(20) NULL DEFAULT 249 AFTER `isDeleted`,
ADD INDEX `fk_franchiseetechmailemail_phonechargesfee_idx` (`franchiseetechmailserviceId` ASC);
ALTER TABLE `phonechargesfee` 
ADD CONSTRAINT `fk_franchiseetechmailemail_phonechargesfee`
  FOREIGN KEY (`franchiseetechmailserviceId`)
  REFERENCES `franchiseetechmailemail` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;

  ALTER TABLE `phonechargesfee` 
ADD COLUMN `IsInvoiceGenerated` bit(1) default false;

ALTER TABLE `phonechargesfee` 
ADD COLUMN `IsInvoiceInQueue` bit(1) default false;

INSERT INTO `makalu`.`lookup` (`Id`, `LookupTypeId`, `Name`, `Alias`, `RelativeOrder`, `IsActive`, `IsDeleted`) VALUES ('251', '21', 'Phone Call Charges', 'PhoneCallCharges', '1', b'1', b'0');


ALTER TABLE `phonechargesfee` 
ADD COLUMN `franchiseeservicefeeId` BIGINT(20) NULL DEFAULT null AFTER `isDeleted`,
ADD INDEX `fk_franchiseeservicefee_phonechargesfee_idx` (`franchiseeservicefeeId` ASC);
ALTER TABLE `phonechargesfee` 
ADD CONSTRAINT `fk_franchiseeservicefee_phonechargesfee`
  FOREIGN KEY (`franchiseeservicefeeId`)
  REFERENCES `franchiseeservicefee` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;
