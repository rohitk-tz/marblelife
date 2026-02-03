alter table `estimateinvoiceservice`
Drop column `Length`,
Drop column `Width`,
Drop column `Area`;

Create Table `estimateinvoicedimension`(
`Id` BIGINT(20) NOT NULL AUTO_INCREMENT,
`EstimateInvoiceServiceId` BIGINT(20) NOT NULL ,
`Length` decimal(10,2) default null,
`Width` decimal(10,2) default null,
`Area` decimal(10,2) default null,
`Description` VARCHAR(400) default null,
`IsDeleted` BIT(1) NULL DEFAULT b'0' ,
PRIMARY KEY (`Id`),
INDEX `fk_EstimateInvoiceDimension_estimateinvoiceserviceid_idx` (`EstimateInvoiceServiceId` ASC) ,
CONSTRAINT `fk_EstimateInvoiceDimension_estimateinvoiceserviceid_idx`
FOREIGN KEY (`EstimateInvoiceServiceId`)
REFERENCES `estimateinvoiceservice` (`Id`)
);

UPDATE `makalu`.`technicianworkorder` SET `Name` = 'Steel-7 inch-FLAT' WHERE (`Id` = '25');
UPDATE `makalu`.`technicianworkorder` SET `Name` = 'Steel-22 inch FLAT' WHERE (`Id` = '26');
UPDATE `makalu`.`technicianworkorder` SET `Name` = 'Steel 7 inch FLUFFY' WHERE (`Id` = '27');
UPDATE `makalu`.`technicianworkorder` SET `Name` = 'Steel 22 inch FLUFFY' WHERE (`Id` = '28');
