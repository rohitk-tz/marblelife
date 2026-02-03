CREATE TABLE `honingmeasurement` (
  `Id` bigint(20) NOT NULL AUTO_INCREMENT,
  `EstimateInvoiceServiceId` bigint(20) DEFAULT NULL,
  `Area` decimal(18,2) DEFAULT NULL,
  `Sections` decimal(18,2) DEFAULT NULL,
  `UGC` decimal(18,2) DEFAULT NULL,
  `Thirty` decimal(18,2) DEFAULT NULL,
  `Fifty` decimal(18,2) DEFAULT NULL,
  `Hundred` decimal(18,2) DEFAULT NULL,
  `TwoHundred` decimal(18,2) DEFAULT NULL,
  `FourHundred` decimal(18,2) DEFAULT NULL,
  `EightHundred` decimal(18,2) DEFAULT NULL,
  `FifteenHundred` decimal(18,2) DEFAULT NULL,
  `ThreeThousand` decimal(18,2) DEFAULT NULL,
  `EightThousand` decimal(18,2) DEFAULT NULL,
  `ElevenThousand` decimal(18,2) DEFAULT NULL,
  `ShiftPrice` decimal(18,2) DEFAULT NULL,
  `Caco` decimal(18,2) DEFAULT NULL,
  `Ihg` decimal(18,2) DEFAULT NULL,
  `Dimension` decimal(18,2) DEFAULT NULL,
  `ProdutivityRate` decimal(18,2) DEFAULT NULL,
  `TotalAreaInHour` decimal(18,2) DEFAULT NULL,
  `TotalAreaInShift` decimal(18,2) DEFAULT NULL,
  `TotalMinute` decimal(18,2) DEFAULT NULL,
  `TotalArea` decimal(18,2) DEFAULT NULL,
  `TotalCost` decimal(18,2) DEFAULT NULL,
  `TotalCostPerSquare` decimal(18,2) DEFAULT NULL,
  `MinRestoration` decimal(18,2) DEFAULT NULL,
  `IsDeleted` bit(1) DEFAULT b'0',
  `DataRecorderMetaDataId` bigint(20) DEFAULT NULL,
  `IsActive` bit(1) DEFAULT b'0',
  `ShiftName` varchar(100) DEFAULT '\0',
  `SeventeenBase` decimal(18,2) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `fk_honingMeasurement_estimateInvoice_idx` (`EstimateInvoiceServiceId`),
  KEY `DataRecorderMetaDataId` (`DataRecorderMetaDataId`),
  CONSTRAINT `fk_honingMeasurement_estimateInvoice` FOREIGN KEY (`EstimateInvoiceServiceId`) REFERENCES `estimateinvoiceservice` (`ID`),
  CONSTRAINT `honingmeasurement_ibfk_1` FOREIGN KEY (`DataRecorderMetaDataId`) REFERENCES `datarecordermetadata` (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;



alter table honingmeasurement
Add column StartingPointTechShiftEstimates decimal(18,2) null default null After MinRestoration;