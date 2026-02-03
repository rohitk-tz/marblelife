CREATE TABLE `customerlog` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `CustomerId` bigint(20) default NULL,
  `SessionId` varchar(45) NOT NULL,
  `DeviceKey` varchar(100) DEFAULT NULL,
  `Browser` varchar(100) NOT NULL,
  `ClientIp` varchar(15) NOT NULL,
  `LoginAttemptCount` int(11) NOT NULL,
  `LoggedInAt` datetime NOT NULL,
  `LoggedOutAt` datetime DEFAULT NULL,
  `IsDeleted` bit(1) NOT NULL DEFAULT b'0',
   `EstimateCustomerId` bigint(20) default NULL,
  PRIMARY KEY (`Id`),
  KEY `fk_customerlog_customer_idx` (`CustomerId`),
  KEY `fk_customerlog_estimatecustomerid_idx` (`EstimateCustomerId`),
  CONSTRAINT `fk_customerlog_customer` FOREIGN KEY (`CustomerId`) REFERENCES `customer` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `fk_customerlog_estimatecustomerid` FOREIGN KEY (`EstimateCustomerId`) REFERENCES `estimateinvoicecustomer` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB AUTO_INCREMENT=123273 DEFAULT CHARSET=latin1;




  ALTER TABLE `makalu`.`customersignature` 
CHANGE COLUMN `Name` `Name` VARCHAR(128) NULL ;

ALTER TABLE `customerlog` 
ADD COLUMN `EstimateInvoiceId` BIGINT(40) NULL DEFAULT null AFTER `isDeleted`,
ADD INDEX `fk_customerlog_service_idx` (`EstimateInvoiceId` ASC);
ALTER TABLE `customerlog` 
ADD CONSTRAINT `fk_customerlog_service`
  FOREIGN KEY (`EstimateInvoiceId`)
  REFERENCES `estimateInvoice` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;




  ALTER TABLE `makalu`.`customersignature` 
DROP FOREIGN KEY `fk_CustomerSignature_customerId`;
ALTER TABLE `makalu`.`customersignature` 
CHANGE COLUMN `CustomerId` `CustomerId` BIGINT(20) NULL ;
ALTER TABLE `makalu`.`customersignature` 
ADD CONSTRAINT `fk_CustomerSignature_customerId`
  FOREIGN KEY (`CustomerId`)
  REFERENCES `makalu`.`jobcustomer` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;



  ALTER TABLE `customersignature` 
ADD COLUMN `SignedDateTime` datetime default null;




SET SQL_SAFE_UPDATES = 0;
Update `holiday` set IsDeleted=b'1';
SET SQL_SAFE_UPDATES = 1;


INSERT INTO `holiday` (`Id`, `Title`, `StartDate`, `EndDate`, `IsDeleted`,`canSchedule`) VALUES ('61', 'New Year’s Day', '2022-01-01', '2022-01-01', b'0',b'0');
INSERT INTO `holiday` (`Id`, `Title`, `StartDate`, `EndDate`, `IsDeleted`,`canSchedule`) VALUES ('62', 'M L LKing Day', '2022-01-17', '2022-01-17', b'0',b'1');
INSERT INTO `holiday` (`Id`, `Title`, `StartDate`, `EndDate`, `IsDeleted`,`canSchedule`) VALUES ('63', 'Valentine’s Day', '2022-02-14', '2022-02-14', b'0',b'1');
INSERT INTO `holiday` (`Id`, `Title`, `StartDate`, `EndDate`, `IsDeleted`,`canSchedule`) VALUES ('64', 'President’s Day', '2022-02-21', '2022-02-21', b'0',b'1');
INSERT INTO `holiday` (`Id`, `Title`, `StartDate`, `EndDate`, `IsDeleted`,`canSchedule`) VALUES ('65', 'Good Friday', '2022-04-14', '2022-04-15', b'0',b'1');
INSERT INTO `holiday` (`Id`, `Title`, `StartDate`, `EndDate`, `IsDeleted`,`canSchedule`) VALUES ('66', 'Easter Sunday', '2022-04-17', '2022-04-17', b'0',b'1');
INSERT INTO `holiday` (`Id`, `Title`, `StartDate`, `EndDate`, `IsDeleted`,`canSchedule`) VALUES ('67', 'Mother’s Day', '2022-05-08', '2022-05-08', b'0',b'1');
INSERT INTO `holiday` (`Id`, `Title`, `StartDate`, `EndDate`, `IsDeleted`,`canSchedule`) VALUES ('68', 'Memorial Day', '2022-05-30', '2022-05-30', b'0',b'0');
INSERT INTO `holiday` (`Id`, `Title`, `StartDate`, `EndDate`, `IsDeleted`,`canSchedule`) VALUES ('69', 'Father’s Day', '2022-06-19', '2022-06-19', b'0',b'1');
INSERT INTO `holiday` (`Id`, `Title`, `StartDate`, `EndDate`, `IsDeleted`,`canSchedule`) VALUES ('70', 'Independence Day', '2022-07-04', '2022-07-04', b'0',b'0');
INSERT INTO `holiday` (`Id`, `Title`, `StartDate`, `EndDate`, `IsDeleted`,`canSchedule`) VALUES ('71', 'Labor Day', '2022-09-05', '2022-09-05', b'0',b'0');
INSERT INTO `holiday` (`Id`, `Title`, `StartDate`, `EndDate`, `IsDeleted`,`canSchedule`) VALUES ('72', 'Columbus Day', '2022-10-10', '2022-10-10', b'0',b'1');
INSERT INTO `holiday` (`Id`, `Title`, `StartDate`, `EndDate`, `IsDeleted`,`canSchedule`) VALUES ('73', 'Halloween', '2022-10-31', '2022-10-31', b'0',b'1');
INSERT INTO `holiday` (`Id`, `Title`, `StartDate`, `EndDate`, `IsDeleted`,`canSchedule`) VALUES ('74', 'Thanksgiving Day', '2022-11-24', '2022-11-24', b'0',b'0');
INSERT INTO `holiday` (`Id`, `Title`, `StartDate`, `EndDate`, `IsDeleted`,`canSchedule`) VALUES ('75', 'Christmas Day', '2022-12-25', '2022-12-25', b'0',b'0');
INSERT INTO `holiday` (`Id`, `Title`, `StartDate`, `EndDate`, `IsDeleted`,`canSchedule`) VALUES ('76', 'National Donut Day', '2022-06-03', '2022-06-03', b'0',b'1');
