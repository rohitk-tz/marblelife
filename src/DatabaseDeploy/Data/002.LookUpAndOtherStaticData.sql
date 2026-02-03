
set sql_safe_updates=0;
delete from LookupType ;
ALTER TABLE LookupType AUTO_INCREMENT = 1;
delete from Lookup;
ALTER TABLE Lookup AUTO_INCREMENT = 1;
set sql_safe_updates=1;

SET foreign_key_checks = 0;

/* --------------------------------------- */
/* look up type - phone */
Set @lookuptypeId = 1;

call createlookuptype(@lookuptypeId, 'PhoneType','');

call createlookup(1, @lookuptypeId, 'Office','office');
call createlookup(2, @lookuptypeId, 'Cell','cell');
call createlookup(3, @lookuptypeId, 'Fax','fax');


/* look up type - address */
Set @lookuptypeId = 2;

call createlookuptype(@lookuptypeId, 'AddressType','');

call createlookup(11, @lookuptypeId, 'Primary','primary');
call createlookup(12, @lookuptypeId, 'Billing','billing');
call createlookup(13, @lookuptypeId, 'Shipping','shipping');


/* look up type - OrganizationType*/
Set @lookuptypeId = 3;

call createlookuptype(@lookuptypeId, 'OrganizationType','');

call createlookup(21, @lookuptypeId, 'Franchisor', 'Franchisor');
call createlookup(22, @lookuptypeId, 'Franchisee', 'Franchisee');



/*all roles*/
set sql_safe_updates=0;
delete from Role;
ALTER TABLE Role AUTO_INCREMENT = 1;
set sql_safe_updates=1;

INSERT INTO `Role` (`Id`, `Name`, `Alias`, OrganizationTypeId) VALUES (1,'Super Admin','superadmin', 21);
INSERT INTO `Role` (`Id`, `Name`, `Alias`, OrganizationTypeId) VALUES (2,'Franchisee Admin','franchisee', 22);
INSERT INTO `Role` (`Id`, `Name`, `Alias`, OrganizationTypeId) VALUES (3,'SalesRep','rep', 22);
INSERT INTO `Role` (`Id`, `Name`, `Alias`, OrganizationTypeId) VALUES (4,'Technician','tech', 22);

/* look up type - PaymentFrequency*/
Set @lookuptypeId = 4;

call createlookuptype(@lookuptypeId, 'PaymentFrequency','');

call createlookup(31, @lookuptypeId, 'Weekly', 'Weekly');
call createlookup(32, @lookuptypeId, 'Monthly', 'Monthly');


/* look up type - InstrumentType*/
Set @lookuptypeId = 5;

call createlookuptype(@lookuptypeId, 'InstrumentType','');

call createlookup(41, @lookuptypeId, 'Charge Card', 'ChargeCard');
call createlookup(42, @lookuptypeId, 'ECheck', 'ECheck');
call createlookup(43, @lookuptypeId, 'Cash', 'Cash');


/* look up type - ChargeCardType*/
Set @lookuptypeId = 6;

call createlookuptype(@lookuptypeId, 'ChargeCardType','');

call createlookup(51, @lookuptypeId, 'VISA', 'visa');
call createlookup(52, @lookuptypeId, 'MASTER CARD', 'mastercard');
call createlookup(53, @lookuptypeId, 'DISCOVER', 'discover');
call createlookup(54, @lookuptypeId, 'AMERICAN EXPRESS', 'amex');

/* look up type - AccountType*/
Set @lookuptypeId = 7;

call createlookuptype(@lookuptypeId, 'AccountType','');

call createlookup(61, @lookuptypeId, 'Checking', 'Checking');
call createlookup(62, @lookuptypeId, 'Saving', 'Saving');


/* look up type - SalesDataUploadStatus*/
Set @lookuptypeId = 8;

call createlookuptype(@lookuptypeId, 'SalesDataUploadStatus','');

call createlookup(71, @lookuptypeId, 'Uploaded', 'Uploaded');
call createlookup(72, @lookuptypeId, 'Parsed', 'Parsed');
call createlookup(73, @lookuptypeId, 'Failed', 'Failed');
call createlookup(74, @lookuptypeId, 'ParseInProgress', 'ParseInProgress');


/* look up type - InvoiceStatus*/
Set @lookuptypeId = 9;

call createlookuptype(@lookuptypeId, 'InvoiceStatus','');

call createlookup(81, @lookuptypeId, 'Paid', 'paid');
call createlookup(82, @lookuptypeId, 'Unpaid', 'unpaid');
call createlookup(83, @lookuptypeId, 'Partial Paid', 'partial');
call createlookup(84, @lookuptypeId, 'Canceled', 'canceled');

/* look up type - InvoiceItemType*/
Set @lookuptypeId = 10;

call createlookuptype(@lookuptypeId, 'InvoiceItemType','');

call createlookup(91, @lookuptypeId, 'Service', 'Service');
call createlookup(92, @lookuptypeId, 'Royalty Fee', 'Royalty');
call createlookup(93, @lookuptypeId, 'Ad Fund', 'AdFund');
call createlookup(94, @lookuptypeId, 'Discount', 'discount');

/* look up type - ServiceTypeCategory*/
Set @lookuptypeId = 11;

call createlookuptype(@lookuptypeId, 'ServiceTypeCategory','');

call createlookup(101, @lookuptypeId, 'Restoration', 'restoration');
call createlookup(102, @lookuptypeId, 'Maintenance', 'maintenance');

/* look up type - ServiceStatus*/
Set @lookuptypeId = 12;

call createlookuptype(@lookuptypeId, 'ServiceStatus','');

call createlookup(111, @lookuptypeId, 'Pending', 'Pending');
call createlookup(112, @lookuptypeId, 'Success', 'Success');
call createlookup(113, @lookuptypeId, 'Failed', 'Failed');

/* look up type - NotificationResourceType*/
Set @lookuptypeId = 13;

call createlookuptype(@lookuptypeId, 'NotificationResourceType','');

call createlookup(121, @lookuptypeId, 'Attachment', 'Attachment');
call createlookup(122, @lookuptypeId, 'Embedded Resource', 'EmbeddedResource');


/* All Service Types*/
set sql_safe_updates=0;
delete from ServiceType;
ALTER TABLE ServiceType AUTO_INCREMENT = 1;
set sql_safe_updates=1;

INSERT INTO `ServiceType` ( `Name`, `CategoryId`) VALUES ('Stonelife', 101);
INSERT INTO `ServiceType` ( `Name`, `CategoryId`) VALUES ('Enduracrete', 101);
INSERT INTO `ServiceType` ( `Name`, `CategoryId`) VALUES ('Groutelife/Tilelok', 101);
INSERT INTO `ServiceType` ( `Name`, `CategoryId`) VALUES ('Vinylguard', 101);
INSERT INTO `ServiceType` ( `Name`, `CategoryId`) VALUES ('Counterlife', 101);
INSERT INTO `ServiceType` ( `Name`, `CategoryId`) VALUES ('Monthly', 102);
INSERT INTO `ServiceType` ( `Name`, `CategoryId`) VALUES ('Bi-Monthly',102);
INSERT INTO `ServiceType` ( `Name`, `CategoryId`) VALUES ('Quarterly',102);
INSERT INTO `ServiceType` ( `Name`, `CategoryId`) VALUES ('Other',102);


/* All Marketing Classes*/
set sql_safe_updates=0;
delete from MarketingClass;
ALTER TABLE MarketingClass AUTO_INCREMENT = 1;
set sql_safe_updates=1;

INSERT INTO `MarketingClass` (`Id`, `Name`) VALUES (1, 'Commercial');
INSERT INTO `MarketingClass` (`Id`, `Name`) VALUES (2, 'Education');
INSERT INTO `MarketingClass` (`Id`, `Name`) VALUES (3, 'Hotel');
INSERT INTO `MarketingClass` (`Id`, `Name`) VALUES (4, 'Residential');







