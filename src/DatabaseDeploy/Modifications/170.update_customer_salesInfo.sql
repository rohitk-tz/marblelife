
SET SQL_SAFE_UPDATES=0;

update customer c
inner join 
(
select fs.customerid as customerid, sum(fs.amount) as TotalSales,
count(fs.customerid) as NoOfSales,
CAST(sum(fs.amount)/count(fs.customerid) as decimal(10,2)) as AvgSales
from franchiseesales fs
where fs.invoiceid is not null and fs.isdeleted = 0
group by fs.customerid) temp on c.id = temp.customerid
set c.NoOfSales = temp.NoOfSales
, c.TotalSales = temp.TotalSales
, c.AvgSales = temp.AvgSales
where c.isdeleted = 0;

SET SQL_SAFE_UPDATES=1;

