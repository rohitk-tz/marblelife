INSERT INTO `marketingclass` (`Id`, `Name`, `ColorCode`) VALUES ('18', '0-MLD', '#52a752');

INSERT INTO `lookup` (`Id`, `LookupTypeId`, `Name`, `Alias`, `RelativeOrder`) VALUES ('103', '11', 'Product Channel', 'ProductChannel', '3');

INSERT INTO `servicetype` (`Id`, `Name`, `ColorCode`, `CategoryId`, `Alias`) VALUES ('20', 'WEB - MLD', '#58739e', '103', 'Internet');
INSERT INTO `servicetype` (`Id`, `Name`, `ColorCode`, `CategoryId`, `Alias`) VALUES ('21', 'MLFS - Franchisee Sales', '#58739e', '103', 'MLFS');
INSERT INTO `servicetype` (`Id`, `Name`, `ColorCode`, `CategoryId`, `Alias`) VALUES ('22', 'WEB - JET', '#634a10', '103', 'Jet');
INSERT INTO `servicetype` (`Id`, `Name`, `ColorCode`, `CategoryId`, `Alias`) VALUES ('23', 'WEB - WALMART', '#e5cf9c', '103', 'Walmart');
INSERT INTO `servicetype` (`Id`, `Name`, `ColorCode`, `CategoryId`, `Alias`) VALUES ('24', 'WEB - AMAZON', '#ced85b', '103', 'Amazon');
INSERT INTO `servicetype` (`Id`, `Name`, `ColorCode`, `CategoryId`, `Alias`) VALUES ('25', 'WEB - AMAZON PRIME', '#82161b', '103', 'Amazon FBA');
INSERT INTO `servicetype` (`Id`, `Name`, `ColorCode`, `CategoryId`, `Alias`) VALUES ('26', 'WEB - AMAZON CANADA', '#59a8a8', '103', 'Amazon-Canada');
INSERT INTO `servicetype` (`Id`, `Name`, `ColorCode`, `CategoryId`, `Alias`) VALUES ('27', 'HARDWARE', '#512a7a', '103', 'Hardware:ACE,Hardware:ACE:ACE Stores, Hardware:ACE:Westlake ACE Stores,Hardware:Do It Best,Hardware:Do IT Best :DIB Warehouse, Hardware:Do It Best:Do It Best Store,Hardware:Orgill,Dropship ');


UPDATE `servicetype` SET `Alias`='MLFS,Convention,Franchisee Start Up' WHERE `Id`='21';