ALTER TABLE `franchisee` 
ADD COLUMN `RoyalityLateFee` DECIMAL(10,2) NULL AFTER `IsDeleted`,
ADD COLUMN `RoyalityWaitPeriodInDays` INT NULL AFTER `RoyalityLateFee`,
ADD COLUMN `RoyalityInterestRatePercentagePerAnnum` DECIMAL(10,2) NULL AFTER `RoyalityWaitPeriodInDays`,
ADD COLUMN `SalesDataLateFee` DECIMAL(10,2) NULL AFTER `RoyalityInterestRatePercentagePerAnnum`,
ADD COLUMN `SalesDataWaitPeriodInDays` INT NULL AFTER `SalesDataLateFee`;



update franchisee set RoyalityLateFee=50 where Id>0;
update franchisee set RoyalityWaitPeriodInDays=2 where Id>0;
update franchisee set RoyalityInterestRatePercentagePerAnnum=18 where Id>0;

update franchisee set SalesDataLateFee=50 where Id>0;
update franchisee set SalesDataWaitPeriodInDays=1 where Id>0;


Set @lookuptypeId = 10;
call createlookup(123, @lookuptypeId, 'Royality Late Fee', 'RoyalityLateFee');

Set @lookuptypeId = 10;
call createlookup(124, @lookuptypeId, 'Sales Data Late Fee', 'SalesDataLateFee');



ALTER TABLE `franchisee` 
DROP COLUMN `SalesDataWaitPeriodInDays`,
DROP COLUMN `SalesDataLateFee`,
DROP COLUMN `RoyalityInterestRatePercentagePerAnnum`,
DROP COLUMN `RoyalityWaitPeriodInDays`,
DROP COLUMN `RoyalityLateFee`;



CREATE TABLE `latefee` (
  `Id` BIGINT NOT NULL,
  `RoyalityLateFee` DECIMAL(10,2) NULL DEFAULT 50,
  `RoyalityWaitPeriodInDays` INT NULL DEFAULT 2,
  `RoyalityInterestRatePercentagePerAnnum` DECIMAL(10,2) NULL DEFAULT 18,
  `SalesDataLateFee` DECIMAL(10,2) NULL DEFAULT '50',
  `SalesDataWaitPeriodInDays` INT NULL DEFAULT 1,
 `IsDeleted` BIT(1) NOT NULL DEFAULT b'0',
  PRIMARY KEY (`Id`),
  CONSTRAINT `fk_latefee_franchisee`
    FOREIGN KEY (`Id`)
    REFERENCES `franchisee` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION);


insert into latefee(Id,RoyalityLateFee,RoyalityWaitPeriodInDays,RoyalityInterestRatePercentagePerAnnum,SalesDataLateFee,SalesDataWaitPeriodInDays)
(select Id,50,2,18,50,1  from franchisee);


CREATE TABLE `latefeeinvoiceitem` (
  `Id` BIGINT NOT NULL,
  `LateFeeTypeId` BIGINT  NULL,
  `LateFee` DECIMAL(10,2)  NULL,
  `WaitPeriod` INT NOT NULL,
  `InterestRate` DECIMAL(10,2)  NULL,
  `ExpectedDate` DATE NOT NULL, 
  `IsDeleted` BIT(1) NOT NULL DEFAULT b'1',
  PRIMARY KEY (`Id`),
  INDEX `fk_latefeeinvoiceitem_lookup_idx` (`LateFeeTypeId` ASC),
  CONSTRAINT `fk_latefeeInvoiceItem_InvoiceItem`
    FOREIGN KEY (`Id`)
    REFERENCES `invoiceitem` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_latefeeinvoiceitem_lookup`
    FOREIGN KEY (`LateFeeTypeId`)
    REFERENCES `lookup` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION);


UPDATE `lookup` SET `Name`='Late Fee', `Alias`='LateFee' WHERE `Id`='123';
UPDATE `lookup` SET `Name`='Interest Rate', `Alias`='InterestRatePerAnnum' WHERE `Id`='124';

Set @lookuptypeId = 14;

call createlookuptype(@lookuptypeId, 'Late Fee','');

call createlookup(125, @lookuptypeId, 'Royality','Royality');
call createlookup(126, @lookuptypeId, 'Sales Data','SalesData');







