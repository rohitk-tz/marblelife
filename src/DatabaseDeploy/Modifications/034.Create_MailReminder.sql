CREATE TABLE `paymentmailreminder` (
  `Id` BIGINT NOT NULL AUTO_INCREMENT,
  `InvoiceId` BIGINT NOT NULL,
  `FranchiseeId` BIGINT NOT NULL,
  `Date` DATE NOT NULL,
  `IsDeleted` BIT(1) NOT NULL DEFAULT b'0',
  PRIMARY KEY (`Id`),
 INDEX `fk_paymentmailreminder_invoiceid_idx` (`InvoiceId` ASC),
 INDEX `fk_paymentmailreminder_franchisee_idx` (`FranchiseeId` ASC), 
 CONSTRAINT `fk_paymentmailreminder_invoiceid`
  FOREIGN KEY (`InvoiceId`)
  REFERENCES `invoice` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION,
 CONSTRAINT `fk_paymentmailreminder_franchisee`
  FOREIGN KEY (`FranchiseeId`)
  REFERENCES `franchisee` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION)ENGINE=InnoDB DEFAULT CHARSET=utf8;


  CREATE TABLE `salesdatamailreminder` (
  `Id` BIGINT NOT NULL AUTO_INCREMENT, 
  `FranchiseeId` BIGINT NOT NULL,
  `Date` DATE NOT NULL,
  `IsDeleted` BIT(1) NOT NULL DEFAULT b'0',
  PRIMARY KEY (`Id`),
 INDEX `fk_salesdatamailreminder_franchisee_idx` (`FranchiseeId` ASC),  
 CONSTRAINT `fk_salesdatamailreminder_franchisee`
  FOREIGN KEY (`FranchiseeId`)
  REFERENCES `franchisee` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION)ENGINE=InnoDB DEFAULT CHARSET=utf8;



