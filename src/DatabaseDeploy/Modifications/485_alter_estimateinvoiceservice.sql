Alter table estimateinvoiceservice
Add column InvoiceImageId Bigint(20) default null;

Alter table jobestimateservices
Add column InvoiceNumber bigint(20) NULL DEFAULT Null;