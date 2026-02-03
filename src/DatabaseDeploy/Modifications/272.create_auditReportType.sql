CREATE TABLE `annualreporttype` (
  `Id` bigint(20) NOT NULL AUTO_INCREMENT,
  `ReportTypeName` varchar(128) NOT NULL,
  `Description` varchar(100) NOT NULL,
  `IsDeleted` bit(1) NOT NULL DEFAULT b'0',
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1
