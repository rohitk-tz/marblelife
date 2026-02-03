CREATE TABLE `job` (
  `Id` BIGINT(20) NOT NULL AUTO_INCREMENT ,
  `JobTypeId` BIGINT(20) NOT NULL ,
  `QBInvoiceNumber` VARCHAR(45) NULL ,
  `Description` VARCHAR(512) NULL ,
  `CustomerId` BIGINT(20) NOT NULL ,
  `IsDeleted` BIT(1) NOT NULL DEFAULT b'0',
  PRIMARY KEY (`Id`) ,
  INDEX `fk_job_jobType_idx` (`JobTypeId` ASC),
  CONSTRAINT `fk_job_jobType`
    FOREIGN KEY (`JobTypeId`)
    REFERENCES `servicetype` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION);


CREATE TABLE `jobestimate` (
  `Id` BIGINT(20) NOT NULL AUTO_INCREMENT,
  `EstimateHour` INT NOT NULL ,
  `Description` VARCHAR(512) NULL ,
  `CustomerId` BIGINT(20) NOT NULL ,
  `IsDeleted` BIT(1) NOT NULL DEFAULT b'0',
  PRIMARY KEY (`Id`));

CREATE TABLE `jobcustomer` (
  `Id` BIGINT(20) NOT NULL AUTO_INCREMENT,
  `CustomerName` VARCHAR(128) NOT NULL ,
  `Email` VARCHAR(128) NOT NULL ,
  `PhoneNumber` VARCHAR(45) NULL DEFAULT NULL ,
  `AddressId` BIGINT(20) NOT NULL ,
  `IsDeleted` BIT(1) NULL DEFAULT b'0' ,
  PRIMARY KEY (`Id`)  ,
  INDEX `fk_jobCustomer_address_idx` (`AddressId` ASC)  ,
  CONSTRAINT `fk_jobCustomer_address`
    FOREIGN KEY (`AddressId`)
    REFERENCES `address` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION);


ALTER TABLE `jobestimate` 
ADD INDEX `fk_jobEstimate_jobCustomer_idx` (`CustomerId` ASC) ;
ALTER TABLE `jobestimate` 
ADD CONSTRAINT `fk_jobEstimate_jobCustomer`
  FOREIGN KEY (`CustomerId`)
  REFERENCES `jobcustomer` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;

ALTER TABLE `job` 
ADD INDEX `fk_job_jobCustomer_idx` (`CustomerId` ASC)  ;
ALTER TABLE `job` 
ADD CONSTRAINT `fk_job_jobCustomer`
  FOREIGN KEY (`CustomerId`)
  REFERENCES `jobcustomer` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;
  
  

