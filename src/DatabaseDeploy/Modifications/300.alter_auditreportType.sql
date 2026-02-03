ALTER TABLE `annualreporttype` 
CHANGE COLUMN `Description` `Description` VARCHAR(200) NOT NULL ;

INSERT INTO `annualreporttype` (`Id`, `ReportTypeName`, `Description`, `isDeleted`, `Alias`) VALUES ('24', 'Type17A', 'Final Payment in Cash or Invoice Reduced to match payments', b'0', '15');
INSERT INTO `annualreporttype` (`Id`, `ReportTypeName`, `Description`, `isDeleted`, `Alias`) VALUES ('25', 'Type17B', ' INVOICE MODIFIED TO ADD SERVICES AFTER WEEKLY REPORT, VERSUS CREATING AN ADDITIONAL INVOICE - PAYMENTS REPORTED OK', b'0', '15');
INSERT INTO `annualreporttype` (`Id`, `ReportTypeName`, `Description`, `isDeleted`, `Alias`) VALUES ('26', 'Type17C', 'PAYMENT FOR ADDITIONAL SERVICE RECORDED AND THEN LATER ERASED, PRIOR INVOICE MODIFIED INSTEAD OF NEW INVOICE CREATED', b'0', '15');
INSERT INTO `annualreporttype` (`Id`, `ReportTypeName`, `Description`, `isDeleted`, `Alias`) VALUES ('27', 'Type17D', 'ADDITIONAL WORK ADDED TO INVOICE - BUT PAYMENT REPORTED CONSISTENT', b'0', '15');
INSERT INTO `annualreporttype` (`Id`, `ReportTypeName`, `Description`, `isDeleted`, `Alias`) VALUES ('28', 'Type17E', '', b'0', '15');
insert into `annualreporttype` (`Id`, `ReportTypeName`, `Description`, `isDeleted`, `Alias`) VALUES ('29', 'Type18A', 'COLLECTED MORE PAYMENT THAN IN INVOICE', b'0', '15');
insert into `annualreporttype` (`Id`, `ReportTypeName`, `Description`, `isDeleted`, `Alias`) VALUES ('30', 'Type18B', ' ADDITIONAL WORK ADDED TO ORIGINAL INVOICE - INITIAL PAYMENT REPORTED > FINAL REPORT', b'0', '15');
insert into `annualreporttype` (`Id`, `ReportTypeName`, `Description`, `isDeleted`, `Alias`) VALUES ('31', 'Type18C', ' ADDITIONAL WORK DONE, AND PAYMENT MATCHES INVOICES', b'0', '15');
insert into `annualreporttype` (`Id`, `ReportTypeName`, `Description`, `isDeleted`, `Alias`) VALUES ('32', 'Type18D', 'ORIGINAL INVOICE REDUCED - DROPPED SERVICE - OR CASH PAYMENT', b'0', '15');
insert into `annualreporttype` (`Id`, `ReportTypeName`, `Description`, `isDeleted`, `Alias`) VALUES ('33', 'Type18E', '', b'0', '15');
insert into `annualreporttype` (`Id`, `ReportTypeName`, `Description`, `isDeleted`, `Alias`) VALUES ('34', 'Type1H', ' NON-ROYALTY CLASS - These are all METAL which is non-royalty bearing and was not included in Weekly - Has no impact if reported or not reported.', b'0', '15');
insert into `annualreporttype` (`Id`, `ReportTypeName`, `Description`, `isDeleted`, `Alias`) VALUES ('35', 'Type5A', 'MISSING JOB REPORTED WITH PAYMENTS EXCEEDING INVOICE', b'0', '15');
insert into `annualreporttype` (`Id`, `ReportTypeName`, `Description`, `isDeleted`, `Alias`) VALUES ('36', 'Type4B', 'MISSING SERVICE FROM FINAL INVOICE - BUT PAYMENT COLLECTED AND REPORTED - POSSIBLE OVER PAYMENT', b'0', '15');
UPDATE `annualreporttype` SET `Description`=' NON-ROYALTY CLASS ' WHERE `Id`='34';
INSERT INTO `annualreporttype` (`Id`, `ReportTypeName`, `Description`, `isDeleted`, `Alias`) VALUES ('37', 'Type5B', 'MISSING JOB REPORTED IN WEEKLY WITH PAYMENTS LESS THAN INVOICE', b'0', '15');
INSERT INTO `annualreporttype` (`Id`, `ReportTypeName`, `Description`) VALUES ('38', 'Type4A', 'DUP PAYMENT ENTRY IN WEEKLY');
