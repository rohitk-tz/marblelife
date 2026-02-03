ALTER TABLE `customer` 
ADD COLUMN `DateCreated` DATETIME NULL DEFAULT NULL AFTER `ReceiveNotification`;


SET SQL_SAFE_UPDATES = 0;
update customer c
inner join
(
select fs.customerid, min(i.generatedon) dateCreated 
from invoice i
inner join franchiseesales fs 
on fs.invoiceid = i.id
group by fs.customerid
) r on c.id = r.customerid
set c.datecreated = r.dateCreated;
SET SQL_SAFE_UPDATES = 1;

SET SQL_SAFE_UPDATES = 0;
update customer c
inner join datarecordermetadata dmr
on c.datarecordermetadataid = dmr.id
set c.datecreated = dmr.datecreated
where c.datecreated is null and c.isdeleted = 0 and dmr.isdeleted = 0;
SET SQL_SAFE_UPDATES = 1;