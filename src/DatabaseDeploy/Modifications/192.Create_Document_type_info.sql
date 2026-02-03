INSERT INTO `lookuptype` (`Id`, `Name`) VALUES ('24', 'DocumentCategory');
INSERT INTO `lookup` (`Id`, `LookupTypeId`, `Name`, `Alias`, `RelativeOrder`) VALUES ('201', '24', 'Franchisee Management Document', 'FranchiseeManagementDocument', '1');
INSERT INTO `lookup` (`Id`, `LookupTypeId`, `Name`, `Alias`, `RelativeOrder`) VALUES ('202', '24', 'National Account Documents', 'NationalAccountDocuments', '2');

CREATE TABLE `documenttype` (
  `Id` BIGINT(20) NOT NULL,
  `Name` VARCHAR(128) NOT NULL,
  `CategoryId` BIGINT(20) NOT NULL,
  `IsDeleted` BIT(1) NOT NULL DEFAULT b'0',
  PRIMARY KEY (`Id`),
  INDEX `fk_DocumentType_idx` (`CategoryId` ASC),
  CONSTRAINT `fk_DocumentType`
    FOREIGN KEY (`CategoryId`)
    REFERENCES `lookup` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION);
    
INSERT INTO `documenttype` (`Id`, `Name`, `CategoryId`) VALUES ('1', 'W9', '202');
INSERT INTO `documenttype` (`Id`, `Name`, `CategoryId`) VALUES ('2', 'Loan Agreement', '201');
INSERT INTO `documenttype` (`Id`, `Name`, `CategoryId`) VALUES ('3', 'Annual Tax Filling', '201');
INSERT INTO `documenttype` (`Id`, `Name`, `CategoryId`) VALUES ('4', 'Franchisee Contract', '201');
INSERT INTO `documenttype` (`Id`, `Name`, `CategoryId`) VALUES ('5', 'COI', '202');


ALTER TABLE `franchisedocument` 
ADD COLUMN `DocumentTypeId` BIGINT(20) NULL DEFAULT NULL,
ADD INDEX `fk_franchiseDocument_documentType_idx` (`DocumentTypeId` ASC);
ALTER TABLE `franchisedocument` 
ADD CONSTRAINT `fk_franchiseDocument_documentType`
  FOREIGN KEY (`DocumentTypeId`)
  REFERENCES `documenttype` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;

