SET SQL_SAFE_UPDATES = 0;

update franchiseeSalesPayment fsp 
inner join payment p on fsp.PaymentId = p.Id
inner join franchiseeSales fs on fsp.FranchiseeSalesId = fs.Id
inner join salesDataUpload sdu on fs.FranchiseeId = sdu.FranchiseeId
set fsp.salesdatauploadid = sdu.id
where p.Date >= sdu.PeriodStartDate and p.Date <= sdu.PeriodEndDate
and fsp.SalesDataUploadId is null 
and fsp.isdeleted = 0 and p.isdeleted = 0 and fs.isdeleted = 0 and sdu.isdeleted = 0;

SET SQL_SAFE_UPDATES = 1;