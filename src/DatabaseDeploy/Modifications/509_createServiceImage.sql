create table estimateInvoiceServiceImage(
`Id` BIGINT(20) NOT NULL AUTO_INCREMENT,
`EstimateId` BIGINT(20) NULL ,
`EstimateInvoiceServiceId` BIGINT(20) NULL ,
`FileId` BIGINT(20) NULL ,
`DataRecorderMetaDataId` BigInt(20) null,
`IsDeleted` bit(1) NOT NULL DEFAULT b'0',
PRIMARY KEY (`Id`),
CONSTRAINT `fk_estimateInvoiceServiceImages_estimateId_idx`
FOREIGN KEY (`EstimateId`)
REFERENCES `JobEstimate` (`Id`),
CONSTRAINT `fk_estimateInvoiceServiceImages_estimateInvoiceServiceId_idx`
FOREIGN KEY (`EstimateInvoiceServiceId`)
REFERENCES `EstimateInvoiceService` (`Id`),
CONSTRAINT `fk_estimateInvoiceServiceImages_file_idx`
FOREIGN KEY (`FileId`)
REFERENCES `File` (`Id`),
INDEX `fk_estimateInvoiceServiceImages_datarecordermetadataId_idx` (`DataRecorderMetaDataId` ASC) ,
CONSTRAINT `fk_estimateInvoiceServiceImages_datarecordermetadataId_idx`
FOREIGN KEY (`DataRecorderMetaDataId`)
REFERENCES `DataRecorderMetaData` (`Id`)
);

Alter table estimateInvoiceServiceImage
Add column `EstimateInvoiceId` BIGINT(20) NULL AFTER EstimateId,
Add CONSTRAINT `fk_estimateInvoiceServiceImages_estimateInvoiceId_idx`
FOREIGN KEY (`EstimateInvoiceId`)
REFERENCES `EstimateInvoice` (`Id`);

Alter table estimateInvoiceServiceImage
Add column `IsBeforeAfter`  bit(1) NOT NULL DEFAULT b'0' AFTER FileId;