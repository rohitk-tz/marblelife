CREATE TABLE `allCustomerFeedback` (
  `Id` bigint(20) NOT NULL AUTO_INCREMENT,
  `CustomerId` bigint(20) DEFAULT NULL,
  `CustomerName` Varchar(256) DEFAULT NULL,
  `CustomerEmail` Varchar(128) DEFAULT NULL,
  `ResponseReceivedDate` datetime default NULL,
  `ResponseSyncingDate` datetime default NULL,
  `ResponseContent` text DEFAULT NULL,
  `FranchiseeId` bigint(20) DEFAULT NULL,
  `FranchiseeName` Varchar(128) DEFAULT NULL,
  `Rating` decimal(4, 2) DEFAULT NULL,
  `Recommend` bigint(20) DEFAULT NULL,
  `ContactPerson` Varchar(128) DEFAULT NULL,
  `CustomerNameFromAPI` Varchar(128) DEFAULT NULL,
  `AuditStatusId` bigint(20) DEFAULT NULL,
  `From` Varchar(128) DEFAULT NULL,
  `FromTable` Varchar(128) DEFAULT NULL,
  `ReviewPushCustomerFeedbackId` bigint(20) DEFAULT NULL,
  `CustomerFeedbackRequestId` bigint(20) DEFAULT NULL,
  `CustomerFeedbackResponseId` bigint(20) DEFAULT NULL,
  `IsSentToMarketingWebsite` bit(1) DEFAULT b'0',
  `IsEmailSent` bit(1) DEFAULT b'0',
  `IsDeleted` bit(1) DEFAULT b'0',
  `IsActive` bit(1) DEFAULT b'0',
  PRIMARY KEY (`Id`),
  
  KEY `fk_allCustomerFeedback_customer_idx` (`CustomerId`),
  CONSTRAINT `fk_allCustomerFeedback_customer` 
  FOREIGN KEY (`CustomerId`)
  REFERENCES `customer` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  
  KEY `fk_allCustomerFeedback_franchisee_idx` (`FranchiseeId`),
  CONSTRAINT `fk_allCustomerFeedback_franchisee` 
  FOREIGN KEY (`FranchiseeId`)
  REFERENCES `franchisee` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  
  KEY `fk_allCustomerFeedback_lookup_idx` (`AuditStatusId`),
  CONSTRAINT `fk_allCustomerFeedback_lookup` 
  FOREIGN KEY (`AuditStatusId`)
  REFERENCES `lookup` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  
  KEY `fk_allCustomerFeedback_reviewpushcustomerfeedback_idx` (`ReviewPushCustomerFeedbackId`),
  CONSTRAINT `fk_allCustomerFeedback_reviewpushcustomerfeedback` 
  FOREIGN KEY (`ReviewPushCustomerFeedbackId`)
  REFERENCES `reviewpushcustomerfeedback` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  
  KEY `fk_allCustomerFeedback_customerfeedbackrequest_idx` (`CustomerFeedbackRequestId`),
  CONSTRAINT `fk_allCustomerFeedback_customerfeedbackrequest` 
  FOREIGN KEY (`CustomerFeedbackRequestId`)
  REFERENCES `customerfeedbackrequest` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  
  KEY `fk_allCustomerFeedback_customerfeedbackresponse_idx` (`CustomerFeedbackResponseId`),
  CONSTRAINT `fk_allCustomerFeedback_customerfeedbackresponse` 
  FOREIGN KEY (`CustomerFeedbackResponseId`)
  REFERENCES `customerfeedbackresponse` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=latin1;