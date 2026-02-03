INSERT INTO `lookuptype` (`Id`, `Name`) VALUES ('22', 'RepeatFrequency');

INSERT INTO `lookup` (`Id`, `LookupTypeId`, `Name`, `Alias`, `RelativeOrder`) VALUES ('181', '22', 'Weekly', 'Weekly', '2');
INSERT INTO `lookup` (`Id`, `LookupTypeId`, `Name`, `Alias`, `RelativeOrder`) VALUES ('182', '22', 'Monthly', 'Monthly', '3');
INSERT INTO `lookup` (`Id`, `LookupTypeId`, `Name`, `Alias`, `RelativeOrder`) VALUES ('183', '22', 'Daily', 'Daily', '1');
INSERT INTO `lookup` (`Id`, `LookupTypeId`, `Name`, `Alias`, `RelativeOrder`) VALUES ('184', '22', 'Custom', 'Custom', '4');

