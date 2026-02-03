INSERT INTO makalu.lookup (Id, LookupTypeId, Name, Alias, RelativeOrder, IsActive, IsDeleted)
VALUES(296, 21, "SEO Charges", "SEOCharges", 5, 1, 0);

INSERT INTO makalu.lookup (Id, LookupTypeId, Name, Alias, RelativeOrder, IsActive, IsDeleted)
VALUES(297, 4, "First Week", "FirstWeek", 5, 1, 0);

INSERT INTO makalu.lookup (Id, LookupTypeId, Name, Alias, RelativeOrder, IsActive, IsDeleted)
VALUES(298, 4, "Third Week", "ThirdWeek", 5, 1, 0);

 ALTER TABLE `franchiseeservicefee`
ADD COLUMN `SaveDateForSeoCost` datetime null default null;

 ALTER TABLE `franchiseeservicefee`
ADD COLUMN `InvoiceDateForSeoCost` datetime null default null;