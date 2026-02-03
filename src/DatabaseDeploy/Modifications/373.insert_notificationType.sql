INSERT INTO `makalu`.`notificationtype` (`Id`, `Title`, `Description`, `IsQueuingEnabled`, `IsServiceEnabled`, `IsDeleted`) VALUES ('37', 'New Customer Mail', 'New Customer Mail', b'1', b'1', b'0');
INSERT INTO `makalu`.`notificationtype` (`Id`, `Title`, `Description`, `IsQueuingEnabled`, `IsServiceEnabled`, `IsDeleted`) VALUES ('38', 'Update Customer Mail', 'Update Customer Mail', b'1', b'1', b'0');

INSERT INTO `makalu`.`emailtemplate` (`Id`, `NotificationTypeId`, `Title`, `Description`, `Subject`, `Body`, `IsDeleted`, `isActive`) VALUES ('37', '37', 'New Job Created', 'New Job Created', 'New @Model.jobType Created', '', b'0', b'1');
INSERT INTO `makalu`.`emailtemplate` (`Id`, `NotificationTypeId`, `Title`, `Description`, `Subject`, `Body`) VALUES ('38', '38', 'Updation in Your Job', 'Updation in Your Job', 'Updation in your @Model.jobType', '');

