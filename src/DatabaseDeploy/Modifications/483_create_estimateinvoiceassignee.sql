Create Table `estimateinvoiceassignee`(
`Id` BIGINT(20) NOT NULL AUTO_INCREMENT,
`EstimateId` BIGINT(20) not null,
`EstimateInvoiceId` BIGINT(20) NOT NULL ,
`SchedulerId` BIGINT(20) default null,
`InvoiceNumber` BIGINT(20) default null,
`AssigneeId` BIGINT(20) default null,
`IsDeleted` BIT(1) NULL DEFAULT b'0' ,
PRIMARY KEY (`Id`),
INDEX `fk_EstimateInvoiceAssignee_estimateid_idx` (`EstimateId` ASC) ,
CONSTRAINT `fk_EstimateInvoiceAssignee_estimateid_idx`
FOREIGN KEY (`EstimateId`)
REFERENCES `jobestimate` (`Id`),
FOREIGN KEY (`AssigneeId`)
REFERENCES `organizationroleuser` (`Id`)
);