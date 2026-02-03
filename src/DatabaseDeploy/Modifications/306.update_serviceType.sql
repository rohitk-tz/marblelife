UPDATE `servicetype` SET `Alias`='Amazon FBA,WEB-Amazon FBA' WHERE `Id`='25';

INSERT INTO `servicetype` (`Id`, `Name`, `ColorCode`, `CategoryId`, `IsActive`, `IsDeleted`, `Alias`,`OrderBy`) VALUES ('36', 'Admin', '#E9967A', '103', b'1', b'0', 'admin','34');
INSERT INTO `servicetype` (`Id`, `Name`, `ColorCode`, `CategoryId`, `IsActive`, `IsDeleted`, `Alias`,`OrderBy`) VALUES ('37', 'Other', '#F08080', '101', b'1', b'0', 'other','35');



UPDATE `servicetype` SET `CategoryId`='103' WHERE `Id`='37';

UPDATE `annualreporttype` SET `isDeleted`=b'0' WHERE `Id`='38';