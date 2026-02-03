insert into servicestag(ServiceTypeId, CategoryId, MaterialType, Service, IsDeleted) values (1,285,'Granite','Hardening',b'0');
insert into servicestag(ServiceTypeId, CategoryId, MaterialType, Service, IsDeleted) values (1,285,'Marble','Hardening',b'0');


Alter table honingmeasurement
Add column `Length` decimal(18,2) default Null After `EstimateInvoiceServiceId`;

Alter table honingmeasurement
Add column `Width` decimal(18,2) default Null After `Length`;

UPDATE `makalu`.`servicestag` SET `IsActive` = b'0' WHERE Service='Hardening' And Id>0;