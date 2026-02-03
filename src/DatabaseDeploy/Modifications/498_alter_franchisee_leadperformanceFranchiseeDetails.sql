 ALTER TABLE `franchisee`
ADD COLUMN `IsSEOActive` bit(1) default false;

 ALTER TABLE `leadPerformanceFranchiseeDetails`
ADD COLUMN `week` int null default null;

 ALTER TABLE `leadPerformanceFranchiseeDetails`
ADD COLUMN `IsSEOActive` bit(1) default true;