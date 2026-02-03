SET SQL_SAFE_UPDATES = 0;
Update `holiday` set IsDeleted=b'1';
SET SQL_SAFE_UPDATES = 1;


ALTER TABLE `holiday` 
ADD COLUMN `canSchedule` bool NULL DEFAULT true AFTER `isDeleted`;

INSERT INTO `holiday` (`Id`, `Title`, `StartDate`, `EndDate`, `IsDeleted`,`canSchedule`) VALUES ('44', 'New Year’s Day', '2021-01-01', '2021-01-01', b'0',b'0');
INSERT INTO `holiday` (`Id`, `Title`, `StartDate`, `EndDate`, `IsDeleted`,`canSchedule`) VALUES ('45', 'M L LKing Day', '2021-01-18', '2021-01-18', b'0',b'1');
INSERT INTO `holiday` (`Id`, `Title`, `StartDate`, `EndDate`, `IsDeleted`,`canSchedule`) VALUES ('46', 'Valentine’s Day', '2021-02-14', '2021-02-14', b'0',b'1');
INSERT INTO `holiday` (`Id`, `Title`, `StartDate`, `EndDate`, `IsDeleted`,`canSchedule`) VALUES ('47', 'President’s Day', '2021-02-15', '2021-02-15', b'0',b'1');
INSERT INTO `holiday` (`Id`, `Title`, `StartDate`, `EndDate`, `IsDeleted`,`canSchedule`) VALUES ('48', 'Good Friday', '2021-04-02', '2021-04-02', b'0',b'1');
INSERT INTO `holiday` (`Id`, `Title`, `StartDate`, `EndDate`, `IsDeleted`,`canSchedule`) VALUES ('49', 'Easter Sunday', '2021-04-04', '2021-04-04', b'0',b'1');
INSERT INTO `holiday` (`Id`, `Title`, `StartDate`, `EndDate`, `IsDeleted`,`canSchedule`) VALUES ('50', 'Mother’s Day', '2021-05-09', '2021-05-09', b'0',b'1');
INSERT INTO `holiday` (`Id`, `Title`, `StartDate`, `EndDate`, `IsDeleted`,`canSchedule`) VALUES ('51', 'Memorial Day', '2021-05-31', '2021-05-31', b'0',b'0');
INSERT INTO `holiday` (`Id`, `Title`, `StartDate`, `EndDate`, `IsDeleted`,`canSchedule`) VALUES ('52', 'Father’s Day', '2021-06-20', '2021-06-20', b'0',b'1');
INSERT INTO `holiday` (`Id`, `Title`, `StartDate`, `EndDate`, `IsDeleted`,`canSchedule`) VALUES ('53', 'Independence Day', '2021-07-04', '2021-07-04', b'0',b'0');
INSERT INTO `holiday` (`Id`, `Title`, `StartDate`, `EndDate`, `IsDeleted`,`canSchedule`) VALUES ('54', 'Labor Day', '2021-09-06', '2021-09-06', b'0',b'0');
INSERT INTO `holiday` (`Id`, `Title`, `StartDate`, `EndDate`, `IsDeleted`,`canSchedule`) VALUES ('55', 'Columbus Day', '2021-10-11', '2021-10-11', b'0',b'1');
INSERT INTO `holiday` (`Id`, `Title`, `StartDate`, `EndDate`, `IsDeleted`,`canSchedule`) VALUES ('56', 'Halloween', '2021-10-31', '2021-10-31', b'0',b'1');
INSERT INTO `holiday` (`Id`, `Title`, `StartDate`, `EndDate`, `IsDeleted`,`canSchedule`) VALUES ('57', 'Thanksgiving Day', '2021-11-25', '2021-11-25', b'0',b'0');
INSERT INTO `holiday` (`Id`, `Title`, `StartDate`, `EndDate`, `IsDeleted`,`canSchedule`) VALUES ('58', 'Christmas Day', '2021-12-25', '2021-12-25', b'0',b'0');
INSERT INTO `holiday` (`Id`, `Title`, `StartDate`, `EndDate`, `IsDeleted`,`canSchedule`) VALUES ('59', 'National Donut Day', '2021-06-04', '2021-05-04', b'0',b'1');
INSERT INTO `holiday` (`Id`, `Title`, `StartDate`, `EndDate`, `IsDeleted`,`canSchedule`) VALUES ('60', 'Independence Day Holiday', '2021-07-05', '2021-07-05', b'0',b'0');
