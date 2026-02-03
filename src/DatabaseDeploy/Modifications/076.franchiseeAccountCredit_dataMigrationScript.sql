SET SQL_SAFE_UPDATES = 0;
update franchiseeaccountcredit fac
inner join invoice inv on fac.invoiceid = inv.id
inner join invoiceitem invt on inv.id = invt.invoiceid
set fac.currencyexchangerateid = invt.currencyexchangerateid;
SET SQL_SAFE_UPDATES = 1;