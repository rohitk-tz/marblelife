ALTER TABLE `franchisee` 
ADD COLUMN `isRoyality` Bit(1) NULL default 1;

ALTER TABLE `franchiseeloan` 
ADD COLUMN `isRoyality` Bit(1) NULL default 1;

ALTER TABLE `franchiseeloanschedule` 
ADD COLUMN `isRoyality` Bit(1) NULL default 1;

insert into lookup (`Id`, `LookupTypeId`, `Name`, `Alias`, `RelativeOrder`)  values('208','10','LoanServiceFee','Loan Service Fee','2');
insert into lookup (`Id`, `LookupTypeId`, `Name`, `Alias`, `RelativeOrder`)  values('209','10','LoanInterestRatePerAnnum','Loan Service Fee Interest' ,'3');


ALTER TABLE `franchisee` 
ADD COLUMN `FileId` BIGINT(20) DEFAULT NULL;