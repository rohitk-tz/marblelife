INSERT INTO auditfranchiseesales (`FranchiseeId`, `CustomerId`, `InvoiceId`,`ClassTypeId`,`SalesRep`,`QbInvoiceNumber`,`CurrencyExchangeRateId`,`AccountCreditId`,`auditInvoiceId`)
SELECT B.`FranchiseeId`,C.`CustomerId`,A.`InvoiceId`,C.`ClassTypeId`,C.`SalesRep`,A.`QbInvoiceNumber`,C.`CurrencyExchangeRateId`,C.`AccountCreditId`,A.`Id`
FROM `auditinvoice`AS A
INNER JOIN `annualsalesdataupload` AS B
ON B.`Id` = A.`annualUploadId`
INNER JOIN `franchiseesales` AS C
ON A.`QbInvoiceNumber` = C.`QbInvoiceNumber` and C.`FranchiseeId` = B.`FranchiseeId`;
  
  



