CREATE TABLE `WeeklyNotification` (
  `ID` BIGINT(20) NOT NULL AUTO_INCREMENT,
  `NotificationDate` DATE NOT NULL,
  `NotificationTypeId` BIGINT(20) NOT NULL,
  `IsDeleted` BIT(1) NOT NULL DEFAULT b'0',
  PRIMARY KEY (`Id`) ,
  INDEX `FK_weeklyNotification_type_idx` (`NotificationTypeId` ASC) ,
  CONSTRAINT `FK_weeklyNotification_type`
    FOREIGN KEY (`NotificationTypeId`)
    REFERENCES `NotificationType` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION
);
