update `franchiseedocumenttype` set `isDeleted`=b'1' where `DocumentTypeId` = 13

insert into `franchiseedocumenttype` (`FranchiseeId`,`DocumentTypeId`,`IsActive`,`isDeleted`)
select `id`,13,1,0 from `organization` where `Id` not in (
SELECT `FranchiseeId` FROM `franchiseedocumenttype` where `DocumentTypeId` = 13 
) 
and  Id   in (
SELECT `FranchiseeId` FROM `franchiseedocumenttype`  
) 