Alter Table replacementcharges
Add Column `Order` int(11) null After Material;

Update replacementcharges
Set `Order` = 1 Where Material = 'Marble' And Id>0;

Update replacementcharges
Set `Order` = 2 Where Material = 'Granite' And Id>0;

Update replacementcharges
Set `Order` = 3 Where Material = 'Ceramic' And Id>0;

Update replacementcharges
Set `Order` = 4 Where Material = 'Concrete' And Id>0;

Update replacementcharges
Set `Order` = 5 Where Material = 'Terrazzo' And Id>0;

Update replacementcharges
Set `Order` = 6 Where Material = 'Vinyl' And Id>0;

Update replacementcharges
Set `Order` = 7 Where Material = 'Terracotta' And Id>0;

Alter Table maintenancecharges
Add Column `Order` int(11) null After Material;

Update maintenancecharges
Set `Order` = 1 Where Material = 'Marble' And Id>0;

Update maintenancecharges
Set `Order` = 2 Where Material = 'Granite' And Id>0;

Update maintenancecharges
Set `Order` = 3 Where Material = 'Ceramic' And Id>0;

Update maintenancecharges
Set `Order` = 4 Where Material = 'Concrete' And Id>0;

Update maintenancecharges
Set `Order` = 5 Where Material = 'Terrazzo' And Id>0;

Update maintenancecharges
Set `Order` = 6 Where Material = 'Vinyl' And Id>0;

Update maintenancecharges
Set `Order` = 7 Where Material = 'Terracotta' And Id>0;

Alter Table maintenancecharges
Add Column `Notes` varchar(255) null After UOM;

Create Table `floorgrindingadjustmentnotes`(
`Id` BIGINT(20) NOT NULL AUTO_INCREMENT,
`FranchiseeId` BIGINT(20) NULL ,
`Note` varchar(500) Null,
`IsChangedByFranchisee` BIT(1) NULL DEFAULT b'0',
`IsActive` BIT(1) NULL DEFAULT b'0',
`IsDeleted` BIT(1) NULL DEFAULT b'0' ,
`DataRecorderMetaDataId` BigInt(20) null,
PRIMARY KEY (`Id`),
INDEX `fk_floorgrindingadjustmentnotes_franchiseeId_idx` (`FranchiseeId` ASC) ,
CONSTRAINT `fk_floorgrindingadjustmentnotes_franchiseeid_idx`
FOREIGN KEY (`FranchiseeId`)
REFERENCES `franchisee` (`Id`),
INDEX `fk_floorgrindingadjustmentnotes_datarecordermetadataId_idx` (`DataRecorderMetaDataId` ASC) ,
CONSTRAINT `fk_floorgrindingadjustmentnotes_datarecordermetadataId_idx`
FOREIGN KEY (`DataRecorderMetaDataId`)
REFERENCES `DataRecorderMetaData` (`Id`)
);

Update floorgrindingadjustment
SET DiameterOfGrindingPlate = '3X32"' Where AdjustmentFactor = 10.63 ANd Id>0;


Alter Table estimateinvoicedimension
Add Column `areaTime` decimal(10,2) DEFAULT NULL null After IncrementedPrice;