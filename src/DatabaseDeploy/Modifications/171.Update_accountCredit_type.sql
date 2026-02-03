INSERT INTO `lookup` (`Id`, `LookupTypeId`, `Name`, `Alias`, `RelativeOrder`) VALUES ('163', '20', 'All Sales Credit', 'AllSalesCredit', '3');

update franchiseeaccountcredit set credittypeid = 163 where credittypeid is null and isdeleted = 0;
