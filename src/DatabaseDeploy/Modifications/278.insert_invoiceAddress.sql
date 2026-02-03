insert into invoiceAddress (`InvoiceId`,`TypeId`,`AddressLine1`,`AddressLine2`,`CityId`,`CountryId`,`StateId`,`CityName`,`ZipCode`,`StateName`,`emailid`,`phone`)
(select invoice.Id,address.TypeId,address.AddressLine1,address.AddressLine2,address.CityId,address.CountryId,address.StateId,address.CityName,address.ZipCode
,address.StateName,email.Email,cus.phone
from invoice
left join franchiseesales sales on sales.InvoiceId=invoice.Id
inner join customer cus on cus.Id=sales.CustomerId
inner join customerEmail email on cus.Id=email.CustomerId
inner join address address on address.Id=cus.AddressId
)
