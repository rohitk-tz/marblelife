insert into `franchiseedocumenttype` (`FranchiseeId`,`DocumentTypeId`,`IsActive`,`isDeleted`)
select id,13,1,0 from organization where Id not in (


SELECT FranchiseeId FROM makalu.franchiseedocumenttype where documentTypeId = 13

) 
