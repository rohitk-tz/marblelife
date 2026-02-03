Alter table jobscheduler
Add column `IsJobConverted` bit(1) NULL default b'0';

Alter table estimateinvoice
Add column `IsInvoiceChanged` bit(1) NULL default b'0';

