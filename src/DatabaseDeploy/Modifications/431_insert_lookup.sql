INSERT INTO `makalu`.`lookuptype` (`Id`, `Name`, `IsDeleted`) VALUES ('35', 'FranchiseeNotes', b'0');

INSERT INTO `makalu`.`lookup` (`Id`, `LookupTypeId`, `Name`, `Alias`, `RelativeOrder`, `IsActive`, `IsDeleted`) VALUES ('237', '35', 'Franchisee Duration', 'Franchisee Duration', '1', b'1', b'0');

INSERT INTO `makalu`.`lookup` (`Id`, `LookupTypeId`, `Name`, `Alias`, `RelativeOrder`, `IsActive`, `IsDeleted`) VALUES ('238', '35', 'Notes From Owner', 'Notes From Owner', '2', b'1', b'0');

INSERT INTO `makalu`.`lookup` (`Id`, `LookupTypeId`, `Name`, `Alias`, `RelativeOrder`, `IsActive`, `IsDeleted`) VALUES ('239', '35', 'Notes from Call Center / Backup', 'Notes from Call Center / Backup', '3', b'1', b'0');
