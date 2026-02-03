INSERT INTO `lookuptype` (`Id`, `Name`, `IsDeleted`) VALUES ('18', 'ScheduleType', 0);

INSERT INTO `lookup` (`Id`, `LookupTypeId`, `Name`, `Alias`, `RelativeOrder`, `IsActive`, `IsDeleted`) VALUES ('145', '18', 'Job', 'Job', '1', 1, 0);
INSERT INTO `lookup` (`Id`, `LookupTypeId`, `Name`, `Alias`, `RelativeOrder`, `IsActive`, `IsDeleted`) VALUES ('146', '18', 'Estimate', 'Estimate', '2', 1, 0);
