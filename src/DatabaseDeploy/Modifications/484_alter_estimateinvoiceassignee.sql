Alter table jobestimateservices
Add column IsInvoiceForJob Bit(1) NULL DEFAULT b'0';

Alter Table estimateinvoiceassignee
Add column Label varchar(400) Null After InvoiceNumber;