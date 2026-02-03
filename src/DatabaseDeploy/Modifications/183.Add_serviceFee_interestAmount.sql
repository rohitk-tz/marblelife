INSERT INTO `lookup` (`Id`, `LookupTypeId`, `Name`, `Alias`, `RelativeOrder`) 
VALUES ('177', '21', 'Interest Amount', 'InterestAmount', '7');

UPDATE `lookup` SET `Alias`='Accounting Services:Acct Min' WHERE `Id`='172';
UPDATE `lookup` SET `Alias`='Recruitment Fees' WHERE `Id`='174';
UPDATE `lookup` SET `Alias`='National Account' WHERE `Id`='176';
UPDATE `lookup` SET `Alias`='Interest – Note Payments' WHERE `Id`='177';
UPDATE `lookup` SET `Alias`='Notes Receiveable' WHERE `Id`='171';
UPDATE `lookup` SET `Alias`='Sales - Franchise Services:One Time Project' WHERE `Id`='175';
UPDATE `lookup` SET `Alias`='Sales - Franchise Services:Payroll Processing' WHERE `Id`='173';


INSERT INTO `lookup` (`Id`, `LookupTypeId`, `Name`, `Alias`, `RelativeOrder`) 
VALUES ('178', '21', 'Book-keeping(var)', 'Accounting Services:Acct Var', '8');

ALTER TABLE `franchiseeloanschedule` 
ADD COLUMN `InterestAmountInvoiceItemId` BIGINT(20) NULL DEFAULT NULL,
ADD INDEX `fk_franchiseeLoanSchedule_invoiceItem1_idx` (`InterestAmountInvoiceItemId` ASC);
ALTER TABLE `franchiseeloanschedule` 
ADD CONSTRAINT `fk_franchiseeLoanSchedule_invoiceItem1`
  FOREIGN KEY (`InterestAmountInvoiceItemId`)
  REFERENCES `invoiceitem` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;

