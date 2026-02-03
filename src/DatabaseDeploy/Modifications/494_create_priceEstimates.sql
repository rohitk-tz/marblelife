Insert into lookuptype (Id, Name, Alias, IsDeleted) Values (46,'Units','Units',b'0');
Insert Into lookup (Id, LookupTypeId, Name, Alias, RelativeOrder, IsActive, IsDeleted) Values(291, 46, 'sq. ft.', 'AREA', 1, b'1', b'0');
Insert Into lookup (Id, LookupTypeId, Name, Alias, RelativeOrder, IsActive, IsDeleted) Values(292, 46, 'ft.', 'LINEARFT', 1, b'1', b'0');
Insert Into lookup (Id, LookupTypeId, Name, Alias, RelativeOrder, IsActive, IsDeleted) Values(293, 46, 'per unit', 'PRODUCTPRICE', 1, b'1', b'0');
Insert Into lookup (Id, LookupTypeId, Name, Alias, RelativeOrder, IsActive, IsDeleted) Values(294, 46, 'per shift', 'MAINTAINANCE', 1, b'1', b'0');
Insert Into lookup (Id, LookupTypeId, Name, Alias, RelativeOrder, IsActive, IsDeleted) Values(295, 46, 'per hour', 'TIME', 1, b'1', b'0');

Create Table `priceestimateservices`(
`Id` BIGINT(20) NOT NULL AUTO_INCREMENT,
`FranchiseeId` BIGINT(20) NULL ,
`ServiceTagId` BIGINT(20) default null,
`BulkCorporatePrice` DECIMAL(18,2) Null,
`BulkCorporateAdditionalPrice` DECIMAL(18,2) Null,
`CorporatePrice` DECIMAL(18,2) NULL ,
`CorporateAdditionalPrice` DECIMAL(18,2) NULL ,
`FranchiseePrice` DECIMAL(18,2) NULL ,
`FranchiseeAdditionalPrice` DECIMAL(18,2) NULL ,
`AlternativeSolution` VARCHAR(255) default NULL ,
`IsPriceChangedByFranchisee` BIT(1) NULL DEFAULT b'0',
`IsPriceChangedByAdmin` BIT(1) NULL DEFAULT b'0',
`IsDeleted` BIT(1) NULL DEFAULT b'0' ,
PRIMARY KEY (`Id`),
INDEX `fk_priceestimateservices_franchiseeId_idx` (`FranchiseeId` ASC) ,
CONSTRAINT `fk_ServicesTag_franchiseeid_idx`
FOREIGN KEY (`FranchiseeId`)
REFERENCES `franchisee` (`Id`),
FOREIGN KEY (`ServiceTagId`)
REFERENCES `servicestag` (`Id`)
);

Create Table `taxrates`(
`Id` BIGINT(20) NOT NULL AUTO_INCREMENT,
`FranchiseeId` BIGINT(20) NULL,
`TaxForProducts` DECIMAL(18,2) Null,
`TaxForServices` DECIMAL(18,2) Null,
`IsDeleted` BIT(1) NULL DEFAULT b'0' ,
PRIMARY KEY (`Id`),
INDEX `fk_taxrates_franchiseeId_idx` (`FranchiseeId` ASC) ,
CONSTRAINT `fk_taxrates_franchiseeid_idx`
FOREIGN KEY (`FranchiseeId`)
REFERENCES `franchisee` (`Id`)
);


ALTER TABLE `marketingleadcalldetail` 
ADD COLUMN `calledFranchiseeId` BIGINT(20) NULL DEFAULT null AFTER `isDeleted`,
ADD INDEX `fk_marketingleadcalldetail_marketingleadcalldetail_idx` (`calledFranchiseeId` ASC);
ALTER TABLE `marketingleadcalldetail` 
ADD CONSTRAINT `fk_marketingleadcalldetail_marketingleadcalldetail`
  FOREIGN KEY (`calledFranchiseeId`)
  REFERENCES `franchisee` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;