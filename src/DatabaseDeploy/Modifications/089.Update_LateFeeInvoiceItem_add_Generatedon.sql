ALTER TABLE `latefeeinvoiceitem` 
ADD COLUMN `GeneratedOn` DATE NULL ;


SET SQL_SAFE_UPDATES = 0;

update latefeeinvoiceitem lin
inner join invoiceitem invt 
on invt.id = lin.id
inner join invoice inv
on inv.id = invt.invoiceid
set lin.generatedon = inv.generatedon
where invt.isdeleted = 0 and inv.isdeleted = 0 and lin.isdeleted = 0 
and lin.latefeetypeid = 125
and inv.generatedon > inv.dueDate;

update latefeeinvoiceitem lin
inner join invoiceitem invt 
on invt.id = lin.id
inner join invoice inv
on inv.id = invt.invoiceid
set lin.generatedon = DATE_ADD(inv.duedate, INTERVAL 1 DAY)
where invt.isdeleted = 0 and inv.isdeleted = 0 and lin.isdeleted = 0 
and lin.latefeetypeid = 125
and inv.generatedon < inv.dueDate;

update latefeeinvoiceitem lin
inner join invoiceitem invt 
on invt.id = lin.id
inner join invoice inv
on inv.id = invt.invoiceid
set lin.generatedon = DATE_ADD(inv.duedate, INTERVAL 1 DAY)
where invt.isdeleted = 0 and inv.isdeleted = 0 and lin.isdeleted = 0 
and lin.latefeetypeid = 125
and inv.generatedon = inv.dueDate;

update latefeeinvoiceitem lin
inner join invoiceitem invt 
on invt.id = lin.id
inner join invoice inv
on inv.id = invt.invoiceid
set lin.generatedon = inv.generatedon
where invt.isdeleted = 0 and inv.isdeleted = 0 and lin.isdeleted = 0 
and lin.latefeetypeid = 126;

SET SQL_SAFE_UPDATES = 1;