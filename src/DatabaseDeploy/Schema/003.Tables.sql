


USE `makalu` ;


-- -----------------------------------------------------
-- Table `LookupType`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `LookupType` (
  `Id` BIGINT(20) NOT NULL,
  `Name` VARCHAR(128) NOT NULL,
  `Alias` VARCHAR(128) NULL DEFAULT NULL,
  `IsDeleted` BIT NOT NULL DEFAULT 0,
  PRIMARY KEY (`Id`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = latin1;


-- -----------------------------------------------------
-- Table `Lookup`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `Lookup` (
  `Id` BIGINT(20) NOT NULL,
  `LookupTypeId` BIGINT(20) NOT NULL,
  `Name` VARCHAR(512) NOT NULL,
  `Alias` VARCHAR(512) NULL DEFAULT NULL,
  `RelativeOrder` TINYINT(4) NOT NULL,
  `IsActive` BIT(1) NOT NULL DEFAULT b'1',
  `IsDeleted` BIT NOT NULL DEFAULT 0,
  PRIMARY KEY (`Id`),
  UNIQUE INDEX `UNIQUE_lookup` (`LookupTypeId` ASC, `Name` ASC),
  INDEX `fk_Lookup_LookupType1_idx` (`LookupTypeId` ASC),
  CONSTRAINT `fk_Lookup_LookupType1`
    FOREIGN KEY (`LookupTypeId`)
    REFERENCES `LookupType` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
DEFAULT CHARACTER SET = latin1;


-- -----------------------------------------------------
-- Table `DataRecorderMetaData`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `DataRecorderMetaData` (
  `Id` BIGINT(20) NOT NULL AUTO_INCREMENT,
  `DateCreated` DATETIME NOT NULL,
  `DateModified` DATETIME NULL DEFAULT NULL,
  `CreatedBy` BIGINT(20) NULL DEFAULT NULL,
  `ModifiedBy` BIGINT(20) NULL DEFAULT NULL,
  `IsDeleted` BIT NOT NULL DEFAULT 0,
  PRIMARY KEY (`Id`) )
ENGINE = InnoDB
DEFAULT CHARACTER SET = latin1;


-- -----------------------------------------------------
-- Table `Organization`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `Organization` (
  `Id` BIGINT(20) NOT NULL AUTO_INCREMENT,
  `TypeId` BIGINT(20) NOT NULL,
  `Name` VARCHAR(512) NOT NULL,
  `Email` VARCHAR(256) NULL DEFAULT NULL,
  `About` VARCHAR(512) NULL DEFAULT NULL,
  `Description` VARCHAR(1024) NULL DEFAULT NULL,
  `DataRecorderMetaDataId` BIGINT(20) NOT NULL,
  `IsActive` BIT(1) NOT NULL DEFAULT b'1',
  `IsDeleted` BIT NOT NULL DEFAULT 0,
  PRIMARY KEY (`Id`),
  INDEX `fk_Organization_DataRecorderMetaData1_idx` (`DataRecorderMetaDataId` ASC),
  INDEX `fk_Organization_Lookup_Id_idx` (`TypeId` ASC),
  CONSTRAINT `fk_Organization_DataRecorderMetaData1`
    FOREIGN KEY (`DataRecorderMetaDataId`)
    REFERENCES `DataRecorderMetaData` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_Organization_Lookup_Id`
    FOREIGN KEY (`TypeId`)
    REFERENCES `Lookup` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
DEFAULT CHARACTER SET = latin1;


-- -----------------------------------------------------
-- Table `Person`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `Person` (
  `Id` BIGINT(20) NOT NULL AUTO_INCREMENT,
  `FirstName` VARCHAR(128) NULL DEFAULT NULL,
  `LastName` VARCHAR(128) NOT NULL,
  `MiddleName` VARCHAR(128) NULL DEFAULT NULL,
  `Email` VARCHAR(512) NULL DEFAULT NULL,
  `DataRecorderMetaDataId` BIGINT(20) NOT NULL,
  `IsDeleted` BIT NOT NULL DEFAULT 0,
  PRIMARY KEY (`Id`),
  INDEX `fk_Person_DataRecorderMetaData1_idx` (`DataRecorderMetaDataId` ASC),
  CONSTRAINT `fk_Person_DataRecorderMetaData1`
    FOREIGN KEY (`DataRecorderMetaDataId`)
    REFERENCES `DataRecorderMetaData` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
DEFAULT CHARACTER SET = latin1;


-- -----------------------------------------------------
-- Table `Role`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `Role` (
  `Id` BIGINT(20) NOT NULL,
  `Name` VARCHAR(128) NOT NULL,
  `Alias` VARCHAR(128) NULL DEFAULT NULL,
  `OrganizationTypeId` BIGINT(20) NOT NULL,
  `IsDeleted` BIT NOT NULL DEFAULT 0,
  PRIMARY KEY (`Id`),
  INDEX `fk_Role_Lookup_idx` (`OrganizationTypeId` ASC),
  CONSTRAINT `fk_Role_Lookup`
    FOREIGN KEY (`OrganizationTypeId`)
    REFERENCES `Lookup` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
DEFAULT CHARACTER SET = latin1;


-- -----------------------------------------------------
-- Table `OrganizationRoleUser`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `OrganizationRoleUser` (
  `Id` BIGINT(20) NOT NULL AUTO_INCREMENT,
  `UserId` BIGINT(20) NOT NULL,
  `RoleId` BIGINT(20) NOT NULL,
  `OrganizationId` BIGINT(20) NOT NULL,
  `IsDefault` BIT(1) NOT NULL DEFAULT b'1',
  `IsActive` BIT(1) NOT NULL DEFAULT b'1',
  `IsDeleted` BIT NOT NULL DEFAULT 0,
  PRIMARY KEY (`Id`),
  INDEX `fk_OrganizationRoleUser_Role1_idx` (`RoleId` ASC),
  INDEX `fk_OrganizationRoleUser_Person1_idx` (`UserId` ASC),
  INDEX `fk_OrganizationRoleUser_Organization1_idx` (`OrganizationId` ASC),
  CONSTRAINT `fk_OrganizationRoleUser_Organization`
    FOREIGN KEY (`OrganizationId`)
    REFERENCES `Organization` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_OrganizationRoleUser_Person`
    FOREIGN KEY (`UserId`)
    REFERENCES `Person` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_OrganizationRoleUser_Role`
    FOREIGN KEY (`RoleId`)
    REFERENCES `Role` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
DEFAULT CHARACTER SET = latin1;


Alter Table DataRecorderMetaData Add INDEX `fk_DataRecorderMetaData_OrganizationRoleUser1_idx` (`CreatedBy` ASC);

Alter Table DataRecorderMetaData Add INDEX `fk_DataRecorderMetaData_OrganizationRoleUser2_idx` (`ModifiedBy` ASC);

Alter Table DataRecorderMetaData Add CONSTRAINT `fk_DataRecorderMetaData_OrganizationRoleUser1`
    FOREIGN KEY (`CreatedBy`)
    REFERENCES `OrganizationRoleUser` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION;

Alter Table DataRecorderMetaData Add CONSTRAINT `fk_DataRecorderMetaData_OrganizationRoleUser2`
    FOREIGN KEY (`ModifiedBy`)
    REFERENCES `OrganizationRoleUser` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION;


-- -----------------------------------------------------
-- Table `UserLogin`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `UserLogin` (
  `Id` BIGINT(20) NOT NULL,
  `UserName` VARCHAR(128) NOT NULL,
  `Password` VARCHAR(128) NOT NULL,
  `Salt` VARCHAR(64) NOT NULL,
  `IsLocked` BIT(1) NOT NULL DEFAULT b'0',
  `LoginAttemptCount` INT(11) NULL DEFAULT NULL,
  `LastLoggedInDate` DATETIME NULL DEFAULT NULL,
  `ResetToken` VARCHAR(128) NULL DEFAULT NULL,
  `ResetTokenIssueDate` DATETIME NULL DEFAULT NULL,
  `IsActive` BIT(1) NOT NULL DEFAULT b'1',
  `IsDeleted` BIT NOT NULL DEFAULT 0,
  PRIMARY KEY (`Id`),
  UNIQUE INDEX `UserName_UNIQUE` (`UserName` ASC),
  CONSTRAINT `fk_UserLogin_Person1`
    FOREIGN KEY (`Id`)
    REFERENCES `Person` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
DEFAULT CHARACTER SET = latin1;


-- -----------------------------------------------------
-- Table `Country`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `Country` (
  `Id` BIGINT(20) NOT NULL AUTO_INCREMENT,
  `Name` VARCHAR(128) NOT NULL,
  `ShortName` VARCHAR(5) NULL DEFAULT NULL,
  `IsDeleted` BIT NOT NULL DEFAULT 0,
  PRIMARY KEY (`Id`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = latin1;


-- -----------------------------------------------------
-- Table `State`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `State` (
  `Id` BIGINT(20) NOT NULL AUTO_INCREMENT,
  `CountryId` BIGINT(20) NOT NULL,
  `Name` VARCHAR(128) NOT NULL,
  `ShortName` VARCHAR(5) NULL DEFAULT NULL,
  `IsDeleted` BIT NOT NULL DEFAULT 0,
  PRIMARY KEY (`Id`),
  UNIQUE INDEX `Name_UNIQUE` (`CountryId` ASC, `Name` ASC),
  INDEX `fk_State_Country1_idx` (`CountryId` ASC),
  CONSTRAINT `fk_State_Country1`
    FOREIGN KEY (`CountryId`)
    REFERENCES `Country` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
DEFAULT CHARACTER SET = latin1;


-- -----------------------------------------------------
-- Table `City`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `City` (
  `Id` BIGINT(20) NOT NULL AUTO_INCREMENT,
  `StateId` BIGINT(20) NOT NULL,
  `Name` VARCHAR(128) NOT NULL,
  `IsDeleted` BIT NOT NULL DEFAULT 0,
  PRIMARY KEY (`Id`),
  INDEX `fk_City_State1_idx` (`StateId` ASC),
  CONSTRAINT `fk_City_State1`
    FOREIGN KEY (`StateId`)
    REFERENCES `State` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
DEFAULT CHARACTER SET = latin1;


-- -----------------------------------------------------
-- Table `Zip`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `Zip` (
  `Id` BIGINT(20) NOT NULL AUTO_INCREMENT,
  `Code` VARCHAR(128) NOT NULL,
  `Latitude` DECIMAL(18,8) NULL DEFAULT NULL,
  `Longitude` DECIMAL(18,8) NULL DEFAULT NULL,
  `IsDeleted` BIT NOT NULL DEFAULT 0,
  PRIMARY KEY (`Id`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = latin1;


-- -----------------------------------------------------
-- Table `Address`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `Address` (
  `Id` BIGINT(20) NOT NULL AUTO_INCREMENT,
  `TypeId` BIGINT(20) NOT NULL,
  `AddressLine1` VARCHAR(512) NOT NULL,
  `AddressLine2` VARCHAR(512) NULL DEFAULT NULL,
  `CityId` BIGINT(20) NULL DEFAULT NULL,
  `StateId` BIGINT(20) NULL DEFAULT NULL,
  `CountryId` BIGINT(20) NOT NULL,
  `ZipId` BIGINT(20) NULL DEFAULT NULL,
  `CityName` VARCHAR(512) NULL DEFAULT NULL,
  `ZipCode` VARCHAR(128) NULL DEFAULT NULL,
  `IsDeleted` BIT NOT NULL DEFAULT 0,
  PRIMARY KEY (`Id`),
  INDEX `fk_Address_Lookup1_idx` (`TypeId` ASC),
  INDEX `fk_Address_City1_idx` (`CityId` ASC),
  INDEX `fk_Address_State1_idx` (`StateId` ASC),
  INDEX `fk_Address_Country1_idx` (`CountryId` ASC),
  INDEX `fk_Address_Zip1_idx` (`ZipId` ASC),
  CONSTRAINT `fk_Address_City1`
    FOREIGN KEY (`CityId`)
    REFERENCES `City` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_Address_Country1`
    FOREIGN KEY (`CountryId`)
    REFERENCES `Country` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_Address_Lookup1`
    FOREIGN KEY (`TypeId`)
    REFERENCES `Lookup` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_Address_State1`
    FOREIGN KEY (`StateId`)
    REFERENCES `State` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_Address_Zip1`
    FOREIGN KEY (`ZipId`)
    REFERENCES `Zip` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
DEFAULT CHARACTER SET = latin1;


-- -----------------------------------------------------
-- Table `CityZip`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `CityZip` (
  `CityId` BIGINT(20) NOT NULL,
  `ZipId` BIGINT(20) NOT NULL,
  `IsDeleted` BIT NOT NULL DEFAULT 0,
  PRIMARY KEY (`CityId`, `ZipId`),
  INDEX `fk_CityZip_City1_idx` (`CityId` ASC),
  INDEX `fk_CityZip_Zip1_idx` (`ZipId` ASC),
  CONSTRAINT `fk_CityZip_City1`
    FOREIGN KEY (`CityId`)
    REFERENCES `City` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_CityZip_Zip1`
    FOREIGN KEY (`ZipId`)
    REFERENCES `Zip` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
DEFAULT CHARACTER SET = latin1;


-- -----------------------------------------------------
-- Table `Franchisee`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `Franchisee` (
  `Id` BIGINT(20) NOT NULL,
  `OwnerName` VARCHAR(128) NOT NULL,
  `QuickBookIdentifier` VARCHAR(32) NULL DEFAULT NULL,
  `IsDeleted` BIT NOT NULL DEFAULT 0,
  PRIMARY KEY (`Id`),
  INDEX `fk_franchisee_organization_idx` (`Id` ASC),
  CONSTRAINT `fk_franchisee_organization`
    FOREIGN KEY (`Id`)
    REFERENCES `Organization` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
DEFAULT CHARACTER SET = latin1;


-- -----------------------------------------------------
-- Table `FeeProfile`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `FeeProfile` (
  `Id` BIGINT(20) NOT NULL,
  `PaymentFrequencyId` BIGINT(20) NULL,
  `MinimumRoyaltyPerMonth` DECIMAL(10,2) NULL,
  `SalesBasedRoyalty` BIT NOT NULL DEFAULT 1,
  `FixedAmount` Decimal(10, 2) Null,
  `AdFundPercentage` DECIMAL(10,2) NULL,
  `IsDeleted` BIT NOT NULL DEFAULT 0,
  PRIMARY KEY (`Id`),
  INDEX `fk_feeprofile_lookup` (`PaymentFrequencyId` ASC),
  INDEX `fk_feeprofile_franchisee` (`Id` ASC),
  CONSTRAINT `fk_feeprofile_franchisee`
    FOREIGN KEY (`Id`)
    REFERENCES `Franchisee` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_feeprofile_lookup`
    FOREIGN KEY (`PaymentFrequencyId`)
    REFERENCES `Lookup` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
DEFAULT CHARACTER SET = latin1;


-- -----------------------------------------------------
-- Table `RoyaltyFeeSlabs`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `RoyaltyFeeSlabs` (
  `Id` BIGINT(20) NOT NULL AUTO_INCREMENT,
  `RoyaltyFeeProfileId` BIGINT(20) NOT NULL,
  `MinValue` DECIMAL(18,2) NULL DEFAULT NULL,
  `MaxValue` DECIMAL(18,2) NULL DEFAULT NULL,
  `ChargePercentage` DECIMAL(10,2) NOT NULL,
  `IsDeleted` BIT NOT NULL DEFAULT 0,
  PRIMARY KEY (`Id`),
  INDEX `fk_royaltyfeeslabs_feeprofile_idx` (`RoyaltyFeeProfileId` ASC),
  CONSTRAINT `fk_royaltyfeeslabs_feeprofile`
    FOREIGN KEY (`RoyaltyFeeProfileId`)
    REFERENCES `FeeProfile` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
DEFAULT CHARACTER SET = latin1;


-- -----------------------------------------------------
-- Table `Phone`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `Phone` (
  `Id` BIGINT(20) NOT NULL AUTO_INCREMENT,
  `TypeId` BIGINT(20) NOT NULL,
  `CountryCode` VARCHAR(2) NULL DEFAULT NULL,
  `AreaCode` VARCHAR(2) NULL DEFAULT NULL,
  `Number` VARCHAR(10) NOT NULL,
  `Extension` VARCHAR(5) NULL DEFAULT NULL,
  `IsDeleted` BIT NOT NULL DEFAULT 0,
  PRIMARY KEY (`Id`),
  INDEX `fk_Phone_Lookup1_idx` (`TypeId` ASC),
  CONSTRAINT `fk_Phone_Lookup1`
    FOREIGN KEY (`TypeId`)
    REFERENCES `Lookup` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
DEFAULT CHARACTER SET = latin1;



-- -----------------------------------------------------
-- Table `PersonPhone`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `PersonPhone` (
  `PersonId` BIGINT(20) NOT NULL,
  `PhoneId` BIGINT(20) NOT NULL,
  `IsDeleted` BIT NOT NULL DEFAULT 0,
  PRIMARY KEY (`PersonId`, `PhoneId`),
  INDEX `fk_PersonPhone_Person1_idx` (`PersonId` ASC),
  INDEX `fk_PersonPhone_Phone1_idx` (`PhoneId` ASC),
  CONSTRAINT `fk_PersonPhone_Phone1_idx`
    FOREIGN KEY (`PhoneId`)
    REFERENCES `Phone` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_PersonPhone_Person1_idx`
    FOREIGN KEY (`PersonId`)
    REFERENCES `Person` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
DEFAULT CHARACTER SET = latin1;


-- -----------------------------------------------------
-- Table `OrganizationPhone`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `OrganizationPhone` (
  `OrganizationId` BIGINT(20) NOT NULL,
  `PhoneId` BIGINT(20) NOT NULL,
  `IsDeleted` BIT NOT NULL DEFAULT 0,
  PRIMARY KEY (`OrganizationId`, `PhoneId`),
  INDEX `fk_OrganizationPhone_Organization1_idx` (`OrganizationId` ASC),
  INDEX `fk_OrganizationPhone_Phone1_idx` (`PhoneId` ASC),
  CONSTRAINT `fk_OrganizationPhone_Phone1_idx`
    FOREIGN KEY (`PhoneId`)
    REFERENCES `Phone` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_OrganizationPhone_Organization1_idx`
    FOREIGN KEY (`OrganizationId`)
    REFERENCES `Organization` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
DEFAULT CHARACTER SET = latin1;



-- -----------------------------------------------------
-- Table `OrganizationAddress`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `OrganizationAddress` (
  `OrganizationId` BIGINT(20) NOT NULL,
  `AddressId` BIGINT(20) NOT NULL,
  `IsDeleted` BIT NOT NULL DEFAULT 0,
  PRIMARY KEY (`OrganizationId`, `AddressId`),
  INDEX `fk_OrganizationAddress_Organization1_idx` (`OrganizationId` ASC),
  INDEX `fk_OrganizationAddress_Address1_idx` (`AddressId` ASC),
  CONSTRAINT `fk_AccountAddress_Address1`
    FOREIGN KEY (`AddressId`)
    REFERENCES `Address` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_OrganizationAddress_Organization`
    FOREIGN KEY (`OrganizationId`)
    REFERENCES `Organization` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
DEFAULT CHARACTER SET = latin1;


-- -----------------------------------------------------
-- Table `PersonAddress`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `PersonAddress` (
  `PersonId` BIGINT(20) NOT NULL,
  `AddressId` BIGINT(20) NOT NULL,
  `IsDeleted` BIT NOT NULL DEFAULT 0,
  PRIMARY KEY (`PersonId`, `AddressId`),
  INDEX `fk_PersonAddress_Person1_idx` (`PersonId` ASC),
  INDEX `fk_PersonAddress_Address1_idx` (`AddressId` ASC),
  CONSTRAINT `fk_PersonAddress_Address1`
    FOREIGN KEY (`AddressId`)
    REFERENCES `Address` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_PersonAddress_Person1`
    FOREIGN KEY (`PersonId`)
    REFERENCES `Person` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
DEFAULT CHARACTER SET = latin1;


-- -----------------------------------------------------
-- Table `UserLog`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `UserLog` (
  `Id` INT(11) NOT NULL AUTO_INCREMENT,
  `UserId` BIGINT(20) NOT NULL,
  `SessionId` VARCHAR(45) NOT NULL,
  `DeviceKey` VARCHAR(100) NULL DEFAULT NULL,
  `Browser` VARCHAR(100) NOT NULL,
  `ClientIp` VARCHAR(15) NOT NULL,
  `LoginAttemptCount` INT(11) NOT NULL,
  `LoggedInAt` DATETIME NOT NULL,
  `LoggedOutAt` DATETIME NULL DEFAULT NULL,
  `IsDeleted` BIT NOT NULL DEFAULT 0,
  PRIMARY KEY (`Id`),
  INDEX `fk_UserLog_UserLogin1_idx` (`UserId` ASC),
  CONSTRAINT `fk_UserLog_UserLogin1`
    FOREIGN KEY (`UserId`)
    REFERENCES `UserLogin` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
DEFAULT CHARACTER SET = latin1;


-- -----------------------------------------------------
-- Table `ServiceType`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `ServiceType` (
  `Id` BIGINT NOT NULL AUTO_INCREMENT,
  `Name` VARCHAR(128) NOT NULL,
  `Description` VARCHAR(1024) NULL,
  `CategoryId` BIGINT NOT NULL,
  `IsActive` BIT NOT NULL DEFAULT 1,
  `IsDeleted` BIT NOT NULL DEFAULT 0,
  PRIMARY KEY (`Id`),
  INDEX `fk_ServiceType_lookup1_idx` (`CategoryId` ASC),
  CONSTRAINT `fk_ServiceType_lookup1`
    FOREIGN KEY (`CategoryId`)
    REFERENCES `Lookup` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `FranchiseeService`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `FranchiseeService` (
  `Id` BIGINT NOT NULL AUTO_INCREMENT,
  `FranchiseeId` BIGINT NOT NULL,
  `ServiceTypeId` BIGINT NOT NULL,
  `CalculateRoyalty` BIT NOT NULL DEFAULT 1,
  `IsActive` BIT NOT NULL DEFAULT 1,
  `IsDeleted` BIT NOT NULL DEFAULT 0,
  PRIMARY KEY (`Id`),
  INDEX `fk_FranchiseeService_franchisee1_idx` (`FranchiseeId` ASC),
  INDEX `fk_FranchiseeService_ServiceType1_idx` (`ServiceTypeId` ASC),
  CONSTRAINT `fk_FranchiseeService_franchisee1`
    FOREIGN KEY (`FranchiseeId`)
    REFERENCES `Franchisee` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_FranchiseeService_ServiceType1`
    FOREIGN KEY (`ServiceTypeId`)
    REFERENCES `ServiceType` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `PaymentInstrument`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `PaymentInstrument` (
  `Id` BIGINT NOT NULL,
  `OrganizationId` BIGINT NOT NULL,
  `InstrumentTypeId` BIGINT NOT NULL,
  `InstrumentEntityId` BIGINT NOT NULL,
  `IsPrimary` BIT NOT NULL DEFAULT 1,
  `IsActive` BIT NOT NULL DEFAULT 1,
  `DataRecorderMetaDataId` BIGINT NOT NULL,
  `IsDeleted` BIT NOT NULL DEFAULT 0,
  PRIMARY KEY (`Id`),
  INDEX `fk_PaymentInstrument_organization1_idx` (`OrganizationId` ASC),
  INDEX `fk_PaymentInstrument_lookup1_idx` (`InstrumentTypeId` ASC),
  INDEX `fk_PaymentInstrument_datarecordermetadata1_idx` (`DataRecorderMetaDataId` ASC),
  CONSTRAINT `fk_PaymentInstrument_organization1`
    FOREIGN KEY (`OrganizationId`)
    REFERENCES `Organization` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_PaymentInstrument_lookup1`
    FOREIGN KEY (`InstrumentTypeId`)
    REFERENCES `Lookup` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_PaymentInstrument_datarecordermetadata1`
    FOREIGN KEY (`DataRecorderMetaDataId`)
    REFERENCES `DataRecorderMetaData` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `ChargeCard`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `ChargeCard` (
  `Id` BIGINT NOT NULL,
  `NameOnCard` VARCHAR(128) NOT NULL,
  `TypeId` BIGINT NOT NULL,
  `Number` VARCHAR(4) NOT NULL,
  `ExpiryMonth` INT NOT NULL,
  `ExpiryYear` INT NOT NULL,
  `IsDeleted` BIT NOT NULL DEFAULT 0,
  PRIMARY KEY (`Id`),
  INDEX `fk_ChargeCard_lookup1_idx` (`TypeId` ASC),
  CONSTRAINT `fk_ChargeCard_lookup1`
    FOREIGN KEY (`TypeId`)
    REFERENCES `Lookup` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `ECheck`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `ECheck` (
  `Id` BIGINT NOT NULL AUTO_INCREMENT,
  `RoutingNumber` VARCHAR(64) NOT NULL,
  `AccountTypeId` BIGINT NOT NULL,
  `Name` VARCHAR(128) NULL,
  `AccountNumber` VARCHAR(64) NOT NULL,
  `BankName` VARCHAR(256) NULL,
  `IsDeleted` BIT NOT NULL DEFAULT 0,
  PRIMARY KEY (`Id`),
  INDEX `fk_ECheck_lookup1_idx` (`AccountTypeId` ASC),
  CONSTRAINT `fk_ECheck_lookup1`
    FOREIGN KEY (`AccountTypeId`)
    REFERENCES `Lookup` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `Payment`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `Payment` (
  `Id` BIGINT NOT NULL AUTO_INCREMENT,
  `Date` DATE NOT NULL,
  `Amount` DECIMAL(10,2) NOT NULL,
  `InstrumentTypeId` BIGINT NOT NULL,
  `IsDeleted` BIT NOT NULL DEFAULT 0,
  PRIMARY KEY (`Id`),
  INDEX `fk_Payment_lookup1_idx` (`InstrumentTypeId` ASC),
  CONSTRAINT `fk_Payment_lookup1`
    FOREIGN KEY (`InstrumentTypeId`)
    REFERENCES `Lookup` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `ChargeCardPayment`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `ChargeCardPayment` (
  `Id` BIGINT NOT NULL,
  `ChargeCardId` BIGINT NOT NULL,
  `TransactionId` VARCHAR(64) NULL,
  `RawResponse` TEXT NOT NULL,
  `IsDeleted` BIT NOT NULL DEFAULT 0,
  PRIMARY KEY (`Id`),
  INDEX `fk_ChargeCardPayment_ChargeCard1_idx` (`ChargeCardId` ASC),
  CONSTRAINT `fk_ChargeCardPayment_Payment1`
    FOREIGN KEY (`Id`)
    REFERENCES `Payment` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_ChargeCardPayment_ChargeCard1`
    FOREIGN KEY (`ChargeCardId`)
    REFERENCES `ChargeCard` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `ECheckPayment`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `ECheckPayment` (
  `Id` BIGINT NOT NULL,
  `ECheckId` BIGINT NOT NULL,
  `TransactionId` VARCHAR(64) NULL,
  `RawResponse` TEXT NOT NULL,
  `IsDeleted` BIT NOT NULL DEFAULT 0,
  PRIMARY KEY (`Id`),
  INDEX `fk_ECheckPayemnt_ECheck1_idx` (`ECheckId` ASC),
  CONSTRAINT `fk_ECheckPayemnt_Payment1`
    FOREIGN KEY (`Id`)
    REFERENCES `Payment` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_ECheckPayemnt_ECheck1`
    FOREIGN KEY (`ECheckId`)
    REFERENCES `ECheck` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `File`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `File` (
  `Id` BIGINT NOT NULL AUTO_INCREMENT,
  `Name` VARCHAR(64) NOT NULL,
  `Caption` VARCHAR(64) NOT NULL,
  `Size` DECIMAL(10,2) NOT NULL,
  `RelativeLocation` VARCHAR(256) NOT NULL,
  `MimeType` VARCHAR(128) NOT NULL,
  `DataRecorderMetaDataId` BIGINT NOT NULL,
  `IsDeleted` BIT NOT NULL DEFAULT 0,
  PRIMARY KEY (`Id`),
  INDEX `fk_File_datarecordermetadata1_idx` (`DataRecorderMetaDataId` ASC),
  CONSTRAINT `fk_File_datarecordermetadata1`
    FOREIGN KEY (`DataRecorderMetaDataId`)
    REFERENCES `DataRecorderMetaData` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `SalesDataUpload`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `SalesDataUpload` (
  `Id` BIGINT NOT NULL AUTO_INCREMENT,
  `FranchiseeId` BIGINT NOT NULL,
  `FileId` BIGINT NOT NULL,
  `PeriodStartDate` DATE NOT NULL,
  `PeriodEndDate` DATE NOT NULL,
  `StatusId` BIGINT NOT NULL,
  `ParsedLogFileId` BIGINT NULL,
  `NumberOfCustomers` INT NULL,
  `NumberOfInvoices` INT NULL,
  `NumberOfFailedRecords` INT NULL,
  `NumberOfParsedRecords` INT NULL,
  `TotalAmount` DECIMAL(10,2) NULL,
  `PaidAmount` DECIMAL(10,2) NULL,
  `AccruedAmount` DECIMAL(10,2) NULL,
  `DataRecorderMetaDataId` BIGINT NOT NULL,
  `IsDeleted` BIT NOT NULL DEFAULT 0,
  PRIMARY KEY (`Id`),
  INDEX `fk_SalesDataUpload_datarecordermetadata1_idx` (`DataRecorderMetaDataId` ASC),
  INDEX `fk_SalesDataUpload_franchisee1_idx` (`FranchiseeId` ASC),
  INDEX `fk_SalesDataUpload_File1_idx` (`FileId` ASC),
  INDEX `fk_SalesDataUpload_File2_idx` (`ParsedLogFileId` ASC),
  INDEX `fk_SalesDataUpload_lookup1_idx` (`StatusId` ASC),
  CONSTRAINT `fk_SalesDataUpload_datarecordermetadata1`
    FOREIGN KEY (`DataRecorderMetaDataId`)
    REFERENCES `DataRecorderMetaData` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_SalesDataUpload_franchisee1`
    FOREIGN KEY (`FranchiseeId`)
    REFERENCES `Franchisee` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_SalesDataUpload_File1`
    FOREIGN KEY (`FileId`)
    REFERENCES `File` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_SalesDataUpload_File2`
    FOREIGN KEY (`ParsedLogFileId`)
    REFERENCES `File` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_SalesDataUpload_lookup1`
    FOREIGN KEY (`StatusId`)
    REFERENCES `Lookup` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `MarketingClass`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `MarketingClass` (
  `Id` BIGINT NOT NULL,
  `Name` VARCHAR(64) NOT NULL,
  `Description` VARCHAR(1024) NULL,
  `IsDeleted` BIT NOT NULL DEFAULT 0,
  PRIMARY KEY (`Id`))
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `Customer`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `Customer` (
  `Id` BIGINT NOT NULL AUTO_INCREMENT,
  `Name` VARCHAR(128) NOT NULL,
  `ContactPerson` VARCHAR(128) NULL,
  `AddressId` BIGINT NULL,
  `Phone` VARCHAR(16) NULL,
  `DataRecorderMetaDataId` BIGINT NOT NULL,
  `IsDeleted` BIT NOT NULL DEFAULT 0,
  PRIMARY KEY (`Id`),
  INDEX `fk_Customer_datarecordermetadata1_idx` (`DataRecorderMetaDataId` ASC),
  INDEX `fk_Customer_address1_idx` (`AddressId` ASC),
  CONSTRAINT `fk_Customer_datarecordermetadata1`
    FOREIGN KEY (`DataRecorderMetaDataId`)
    REFERENCES `DataRecorderMetaData` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_Customer_address1`
    FOREIGN KEY (`AddressId`)
    REFERENCES `Address` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;

-- -----------------------------------------------------
-- Table `CustomerEmail`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `CustomerEmail` (
  `Id` BIGINT NOT NULL AUTO_INCREMENT,
  `CustomerId` BIGINT NOT NULL,
  `Email` VARCHAR(256) NOT NULL,
  `IsDeleted` BIT NOT NULL DEFAULT 0,
  PRIMARY KEY (`Id`),
  INDEX `fk_Customer1_idx` (`CustomerId` ASC),
  CONSTRAINT `fk_CustomerEmail_Customer1`
    FOREIGN KEY (`CustomerId`)
    REFERENCES `Customer` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `Invoice`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `Invoice` (
  `Id` BIGINT NOT NULL AUTO_INCREMENT,
  `GeneratedOn` DATETIME NOT NULL,
  `DueDate` DATE NOT NULL,
  `StatusId` BIGINT NOT NULL,
  `DataRecorderMetaDataId` BIGINT NOT NULL,
  `IsDeleted` BIT NOT NULL DEFAULT 0,
  PRIMARY KEY (`Id`),
  INDEX `fk_Invoice_lookup1_idx` (`StatusId` ASC),
  INDEX `fk_Invoice_datarecordermetadata1_idx` (`DataRecorderMetaDataId` ASC),
  CONSTRAINT `fk_Invoice_lookup1`
    FOREIGN KEY (`StatusId`)
    REFERENCES `Lookup` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_Invoice_datarecordermetadata1`
    FOREIGN KEY (`DataRecorderMetaDataId`)
    REFERENCES `DataRecorderMetaData` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `FranchiseeSales`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `FranchiseeSales` (
  `Id` BIGINT NOT NULL AUTO_INCREMENT,
  `FranchiseeId` BIGINT NOT NULL,
  `CustomerId` BIGINT NOT NULL,
  `InvoiceId` BIGINT NOT NULL,
  `ClassTypeId` BIGINT NOT NULL,
  `SalesRep` VARCHAR(5) NULL,
  `Amount` DECIMAL(10,2) NOT NULL,
  `QbInvoiceNumber` VARCHAR(4) NULL,
  `SalesDataUploadId` BIGINT NULL,
  `DataRecorderMetaDataId` BIGINT NOT NULL,
  `IsDeleted` BIT NOT NULL DEFAULT 0,
  PRIMARY KEY (`Id`),
  INDEX `fk_FranchiseeSales_Invoice1_idx` (`InvoiceId` ASC),
  INDEX `fk_FranchiseeSales_Customer1_idx` (`CustomerId` ASC),
  INDEX `fk_FranchiseeSales_datarecordermetadata1_idx` (`DataRecorderMetaDataId` ASC),
  INDEX `fk_FranchiseeSales_SalesDataUpload1_idx` (`SalesDataUploadId` ASC),
  INDEX `fk_FranchiseeSales_franchisee1_idx` (`FranchiseeId` ASC),
  INDEX `fk_FranchiseeSales_MarketingClass1_idx` (`ClassTypeId` ASC),
  CONSTRAINT `fk_FranchiseeSales_Invoice1`
    FOREIGN KEY (`InvoiceId`)
    REFERENCES `Invoice` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_FranchiseeSales_Customer1`
    FOREIGN KEY (`CustomerId`)
    REFERENCES `Customer` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_FranchiseeSales_datarecordermetadata1`
    FOREIGN KEY (`DataRecorderMetaDataId`)
    REFERENCES `DataRecorderMetaData` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_FranchiseeSales_SalesDataUpload1`
    FOREIGN KEY (`SalesDataUploadId`)
    REFERENCES `SalesDataUpload` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_FranchiseeSales_franchisee1`
    FOREIGN KEY (`FranchiseeId`)
    REFERENCES `Franchisee` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_FranchiseeSales_MarketingClass1`
    FOREIGN KEY (`ClassTypeId`)
    REFERENCES `MarketingClass` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `InvoiceItem`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `InvoiceItem` (
  `Id` BIGINT NOT NULL AUTO_INCREMENT,
  `InvoiceId` BIGINT NOT NULL,
  `ItemId` BIGINT NULL,
  `ItemTypeId` BIGINT NOT NULL,
  `Description` VARCHAR(1024) NULL,
  `Quantity` INT NOT NULL,
  `Rate` DECIMAL(10,2) NOT NULL,
  `Amount` DECIMAL(10,2) NOT NULL,
  `IsDeleted` BIT NOT NULL DEFAULT 0,
  PRIMARY KEY (`Id`),
  INDEX `fk_InvoiceItem_Invoice1_idx` (`InvoiceId` ASC),
  INDEX `fk_InvoiceItem_lookup1_idx` (`ItemTypeId` ASC),
  INDEX `fk_InvoiceItem_ServiceType1_idx` (`ItemId` ASC),
  CONSTRAINT `fk_InvoiceItem_Invoice1`
    FOREIGN KEY (`InvoiceId`)
    REFERENCES `Invoice` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_InvoiceItem_lookup1`
    FOREIGN KEY (`ItemTypeId`)
    REFERENCES `Lookup` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_InvoiceItem_ServiceType1`
    FOREIGN KEY (`ItemId`)
    REFERENCES `ServiceType` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `InvoicePayment`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `InvoicePayment` (
  `InvoiceId` BIGINT NOT NULL,
  `PaymentId` BIGINT NOT NULL,
  `IsDeleted` BIT NOT NULL DEFAULT 0,
  PRIMARY KEY (`InvoiceId`, `PaymentId`),
  INDEX `fk_InvoicePayment_Payment1_idx` (`PaymentId` ASC),
  CONSTRAINT `fk_InvoicePayment_Payment1`
    FOREIGN KEY (`PaymentId`)
    REFERENCES `Payment` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_InvoicePayment_Invoice1`
    FOREIGN KEY (`InvoiceId`)
    REFERENCES `Invoice` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;

 -- Table `NotificationType`
		 
	CREATE TABLE IF NOT EXISTS `NotificationType` (
  `Id` BIGINT(20) NOT NULL AUTO_INCREMENT COMMENT '',
  `Title` VARCHAR(512) NOT NULL COMMENT '',
  `Description` VARCHAR(512) NULL COMMENT '',
  `IsServiceEnabled` BIT(1) NOT NULL DEFAULT b'1' COMMENT '',
  `IsDeleted` BIT(1) NOT NULL DEFAULT b'0' COMMENT '',
  PRIMARY KEY (`Id`)  COMMENT '');

-- Table `EmailTemplate`

 CREATE TABLE IF NOT EXISTS `EmailTemplate` (
  `Id` BIGINT(20) NOT NULL AUTO_INCREMENT COMMENT '',
  `NotificationTypeId` BIGINT(20) NOT NULL COMMENT '',
  `Title` VARCHAR(200) NOT NULL COMMENT '',
  `Description` VARCHAR(512) NULL COMMENT '',
  `Subject` VARCHAR(512) NOT NULL COMMENT '',
  `Body` TEXT NOT NULL COMMENT '',
  `IsDeleted` BIT(1) NOT NULL DEFAULT b'0' COMMENT '',
  PRIMARY KEY (`Id`)  COMMENT '',
  INDEX `fk_EmailTemplate_NotificationType_idx` (`NotificationTypeId` ASC)  COMMENT '',
  CONSTRAINT `fk_EmailTemplate_NotificationType`
    FOREIGN KEY (`NotificationTypeId`)
    REFERENCES `NotificationType` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION);
   
  	-- Table `NotificationQueue`
	
  CREATE TABLE IF NOT EXISTS `NotificationQueue` (
  `Id` BIGINT(20) NOT NULL AUTO_INCREMENT COMMENT '',
  `NotificationTypeId` BIGINT(20) NOT NULL COMMENT '',
  `NotificationDate` DATETIME NOT NULL COMMENT '',
  `ServiceStatusId` BIGINT(20) NOT NULL COMMENT '',
  `ServicedAt` DATETIME NULL COMMENT '',
  `AttemptCount` INT(11) NULL COMMENT '',
  `Source` VARCHAR(512) NOT NULL COMMENT '',
  `DataRecorderMetaDataId` BIGINT(20) NOT NULL COMMENT '',
  `IsDeleted` BIT(1) NOT NULL DEFAULT b'0' COMMENT '',
  PRIMARY KEY (`Id`)  COMMENT '',
  INDEX `fk_NotificationQueue_DatarecorderMetadata_idx` (`DataRecorderMetaDataId` ASC)  COMMENT '',
  INDEX `fk_NotificationQueue_NotificationType_idx` (`NotificationTypeId` ASC)  COMMENT '',
  INDEX `fk_NotificationQueue_Lookup_idx` (`ServiceStatusId` ASC)  COMMENT '',
  CONSTRAINT `fk_NotificationQueue_DatarecorderMetadata`
    FOREIGN KEY (`DataRecorderMetaDataId`)
    REFERENCES `DataRecorderMetaData` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_NotificationQueue_NotificationType`
    FOREIGN KEY (`NotificationTypeId`)
    REFERENCES `NotificationType` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
 CONSTRAINT `fk_NotificationQueue_Lookup`
    FOREIGN KEY (`ServiceStatusId`)
    REFERENCES `lookup` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION);
  
 -- Table `NotificationEmail`
 
  CREATE TABLE IF NOT EXISTS `NotificationEmail` (
  `Id` BIGINT(20) NOT NULL AUTO_INCREMENT COMMENT '',
  `EmailTemplateId` BIGINT(20) NOT NULL COMMENT '',
  `FromEmail` VARCHAR(512) NOT NULL COMMENT '',
  `FromName` VARCHAR(512) NOT NULL COMMENT '',
  `Subject` VARCHAR(512) NOT NULL COMMENT '',
  `Body` TEXT NOT NULL COMMENT '',
  `IsDeleted` BIT(1) NOT NULL DEFAULT b'0' COMMENT '',
  PRIMARY KEY (`Id`)  COMMENT '',
  INDEX `fk_NotificationEmail_EmailTemplate_idx` (`EmailTemplateId` ASC)  COMMENT '',
  CONSTRAINT `fk_NotificationEmail_EmailTemplate`
    FOREIGN KEY (`EmailTemplateId`)
    REFERENCES `EmailTemplate` (`Id`) 
	ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_NotificationEmail_NotificationQueue`
    FOREIGN KEY (`Id`)
    REFERENCES `NotificationQueue` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION);
	
	 -- Table `NotificationEmailRecipient`
 
	CREATE TABLE IF NOT EXISTS `NotificationEmailRecipient` (
  `Id` BIGINT(20) NOT NULL AUTO_INCREMENT COMMENT '',
  `NotificationId` BIGINT(20) NOT NULL COMMENT '',
  `OrganizationRoleUserId` BIGINT(20) NOT NULL COMMENT '',
  `RecipientEmail` VARCHAR(512) NOT NULL COMMENT '',
  `IsDeleted` BIT(1) NOT NULL DEFAULT b'0' COMMENT '',
  PRIMARY KEY (`Id`)  COMMENT '',
  INDEX `fk_NotificationEmailRecipient_NotificationEmail_idx` (`NotificationId` ASC)  COMMENT '',
  INDEX `fk_NotificationEmailRecipient_OrganizationRoleUser_idx` (`OrganizationRoleUserId` ASC)  COMMENT '',
  CONSTRAINT `fk_NotificationEmailRecipient_NotificationEmail`
    FOREIGN KEY (`NotificationId`)
    REFERENCES `NotificationEmail` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_NotificationEmailRecipient_OrganizationRoleUser`
    FOREIGN KEY (`OrganizationRoleUserId`)
    REFERENCES `OrganizationRoleUser` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION);	
		

-- Table `FranchiseeInvoice`

 CREATE TABLE IF NOT EXISTS `FranchiseeInvoice` (
  `Id` BIGINT(20) NOT NULL AUTO_INCREMENT,
  `FranchiseeId` BIGINT(20) NOT NULL,
  `InvoiceId` BIGINT(20) NOT NULL,
  `SalesDataUploadId` BIGINT(20) NOT NULL,
  `IsDeleted` BIT(1) NOT NULL DEFAULT b'0',
  PRIMARY KEY (`Id`) ,
  INDEX `fk_FranchiseeInvoice_Franchisee_idx` (`FranchiseeId` ASC),
  INDEX `fk_FranchiseeInvoice_Invoice_idx` (`InvoiceId` ASC),
  INDEX `fk_FranchiseeInvoice_SalesDataUpload_idx` (`SalesDataUploadId` ASC),
  CONSTRAINT `fk_FranchiseeInvoice_Franchisee`
    FOREIGN KEY (`FranchiseeId`)
    REFERENCES `Franchisee` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_FranchiseeInvoice_Invoice`
    FOREIGN KEY (`InvoiceId`)
    REFERENCES `Invoice` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_FranchiseeInvoice_SalesDataUpload`
    FOREIGN KEY (`SalesDataUploadId`)
    REFERENCES `SalesDataUpload` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION);

	-- Table `SalesRep`

  CREATE TABLE `SalesRep` (
   `Id` BIGINT(20) NOT NULL,  
   `Alias` VARCHAR(5) NULL DEFAULT NULL ,
   `IsDeleted` BIT(1) NOT NULL DEFAULT b'0',
  PRIMARY KEY (`Id`),
  INDEX `fk_SalesRep_OrganizationRoleUser_idx` (`Id` ASC),
  CONSTRAINT `fk_SalesRep_OrganizationRoleUser`
  FOREIGN KEY (`Id`)
   REFERENCES `OrganizationRoleUser` (`Id`)
   ON DELETE NO ACTION
   ON UPDATE NO ACTION);
  
