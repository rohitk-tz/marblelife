INSERT INTO FranchiseeService(FranchiseeId,ServiceTypeId,CalculateRoyalty,IsActive)
select distinct(franchiseeId), 18, 1, 1 from franchiseeService
where franchiseeId not in 
(
	select distinct(franchiseeId) from franchiseeService where ServiceTypeId = 18 and IsDeleted = 0
) and IsDeleted = 0;