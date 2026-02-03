CREATE TABLE `todoFollowUpList` (
  `Id` bigint(20) NOT NULL AUTO_INCREMENT,
  `FranchiseeId` bigint(20) NULL,
  `Comment` varchar(1058) NULL,
  `Task` varchar(1058) DEFAULT NULL,
  `StatusId` bigint(20)  NULL,
  `UserId` bigint(20)  NULL,
`IsDeleted` bit(1) NOT NULL DEFAULT b'0',
 `Date` datetime DEFAULT NULL,
 `CustomerId` bigint(20)  NULL,
  `CustomerName` varchar(512) NULL,
   `PhoneNumber` varchar(512) NULL,
    `Email` varchar(512) NULL,
    `TypeId` bigint(20) NULL,
    `SchedulerId` bigint(20) NULL,
  PRIMARY KEY (`Id`),
  KEY `fk_todoList_franchisee1_idx` (`FranchiseeId`),
  KEY `fk_todoList_status_idx` (`StatusId`),
  KEY `fk_todoList_person_idx` (`UserId`),
   KEY `fk_todoList_LookUp_idx` (`TypeId`),
    KEY `fk_todoList_JobScheduler_idx` (`SchedulerId`),
  CONSTRAINT `fk_todoList_status` FOREIGN KEY (`StatusId`) REFERENCES `lookup` (`Id`),
  CONSTRAINT `fk_todoList_franchisee1` FOREIGN KEY (`FranchiseeId`) REFERENCES `franchisee` (`Id`),
  CONSTRAINT `fk_todoList_Person` FOREIGN KEY (`UserId`) REFERENCES `Person` (`Id`),
  CONSTRAINT `fk_todoList_Customer` FOREIGN KEY (`CustomerId`) REFERENCES `JobCustomer` (`Id`),
  CONSTRAINT `fk_todoList_LookUp` FOREIGN KEY (`TypeId`) REFERENCES `LookUpType` (`Id`),
   CONSTRAINT `fk_todoList_JobScheduler` FOREIGN KEY (`SchedulerId`) REFERENCES `jobscheduler` (`Id`)
);

INSERT INTO `makalu`.`lookuptype` (`Id`, `Name`, `IsDeleted`) VALUES ('32', 'ToDoList', b'0');
INSERT INTO `makalu`.`lookuptype` (`Id`, `Name`, `IsDeleted`) VALUES ('33', 'FollowUp', b'0');

INSERT INTO `makalu`.`lookup` (`Id`, `LookupTypeId`, `Name`, `Alias`, `RelativeOrder`, `IsActive`, `IsDeleted`) VALUES ('231', '32', 'OPEN', 'OPEN', '1', b'1', b'0');
INSERT INTO `makalu`.`lookup` (`Id`, `LookupTypeId`, `Name`, `Alias`, `RelativeOrder`, `IsActive`, `IsDeleted`) VALUES ('232', '32', 'IN PROGRESS', 'IN PROGRESS', '2', b'1', b'0');
INSERT INTO `makalu`.`lookup` (`Id`, `LookupTypeId`, `Name`, `Alias`, `RelativeOrder`, `IsActive`, `IsDeleted`) VALUES ('233', '32', 'COMPLETED', 'COMPLETED', '3', b'1', b'0');
INSERT INTO `makalu`.`lookup` (`Id`, `LookupTypeId`, `Name`, `Alias`, `RelativeOrder`, `IsActive`, `IsDeleted`) VALUES ('234', '32', 'RETIRED', 'RETIRED', '4', b'1', b'0');

CREATE TABLE `todoFollowUpComment` (
 `Id` bigint(20) NOT NULL AUTO_INCREMENT,
 `TodoId` bigint(20) NULL,
 `Comment` varchar(1058) NULL,
 `DataRecorderMetaDataId` BIGINT(20) NOT NULL,
 `IsDeleted` BIT(1) NOT NULL DEFAULT b'0',
  PRIMARY KEY (`Id`),
  KEY `fk_todocomment_todoList_idx` (`TodoId`),
  INDEX `fk_todocomment_datarecordermetadata_idx` (`DataRecorderMetaDataId` ASC) ,
   CONSTRAINT `fk_todocomment_todoList` FOREIGN KEY (`TodoId`) REFERENCES `todoFollowUpList` (`Id`),
   CONSTRAINT `fk_todocomment_datarecordermetadata` FOREIGN KEY (`DataRecorderMetaDataId`) REFERENCES `DataRecorderMetaData` (`Id`)
)




UPDATE `makalu`.`emailtemplate` SET `Body` = '<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">\n     <html xmlns=\"http://www.w3.org/1999/xhtml\">\n     <head>\n         <meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\" />\n         <title>@Model.Base.ApplicationName</title>\n         <style type=\"text/css\">\n             body {\n                 padding-left: 10px !important;\n                 padding-right: 10px !important;\n                 padding-top: 10px !important;\n                 padding-bottom: 10px !important;\n                 margin: 20px !important;\n                 width: 600px;\n                 border: solid 1px #B2C8D9;\n                 -webkit-text-size-adjust: 100% !important;\n                 -ms-text-size-adjust: 100% !important;\n                 -webkit-font-smoothing: antialiased !important;\n                 font-size: 16px;\n             }\n             a {\n                 color: #382F2E;\n             }\n     \n             p, h1 {\n                 margin: 0;\n             }\n     \n             p {\n                 text-align: left;\n                 font-weight: normal;\n                 line-height: 19px;\n             }  \n                \n             h2 {\n                 text-align: left;\n                 color: #222222;\n                 font-size: 19px;\n                 font-weight: normal;\n             }\n     \n             div, p, ul, h1 {\n                 margin: 0;\n             }\n     \n             .bgBody {\n                 background: #ffffff;\n             }\n     \n             table thead tr th {\n                 padding: 5px;\n                 margin: 2px;\n                 text-align: left;\n             }\n     \n             table tbody tr td {\n                 padding: 5px;\n                 margin: 2px;\n                 text-align: left;\n             }\n         </style>\n     </head>\n     <body style=\"background-repeat: repeat; -webkit-text-size-adjust: 100%; -ms-text-size-adjust: 100%; -webkit-font-smoothing: antialiased;\">\n             <div class=\"contentEditableContainer contentTextEditable\">\n                 <div class=\"contentEditable\">\n                     <font color=\"#454545\"><span>Dear @Model.FullName,</span>\n                     <br>\n                     <br />\n    				  <p>\n  				  Please find attached the list of feedback requests sent and responses received during the period <b>@Model.StartDate - @Model.EndDate</b>.\n  				   <br />\n       				<br />       				   		                      \n     					</p>					 \n  				   Regards,\n                         <br /><br />\n     					<p>\n                      @Model.Base.CompanyName <br>\n     					@Model.Base.Phone <br>\n  					</font>  \n                 </div>\n             </div>\n     </body>\n     </html>' WHERE (`Id` = '13');
