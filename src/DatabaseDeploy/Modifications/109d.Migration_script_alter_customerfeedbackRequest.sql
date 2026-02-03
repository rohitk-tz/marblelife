ALTER TABLE `customerfeedbackrequest` 
ADD COLUMN `CustomerId` BIGINT(20) NULL DEFAULT NULL AFTER `FranchiseeId`;

SET SQL_SAFE_UPDATES = 0;

update customerfeedbackRequest req
inner join franchiseesales fs
on req.franchiseesalesId = fs.id
set req.franchiseeid = fs.franchiseeid
where req.franchiseeid is null;

update customerfeedbackrequest req
inner join franchiseesales fs
on req.franchiseesalesid = fs.id
set req.customerid = fs.customerid
where req.customerid is null;

SET SQL_SAFE_UPDATES = 1;


ALTER TABLE `customerfeedbackrequest` 
CHANGE COLUMN `FranchiseeId` `FranchiseeId` BIGINT(20) NOT NULL  ,
ADD INDEX `fk_customerFeedbackRequest_franchisee_idx` (`FranchiseeId` ASC)  ;
ALTER TABLE `customerfeedbackrequest` 
ADD CONSTRAINT `fk_customerFeedbackRequest_franchisee`
  FOREIGN KEY (`FranchiseeId`)
  REFERENCES `franchisee` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;

ALTER TABLE `customerfeedbackrequest` 
CHANGE COLUMN `CustomerId` `CustomerId` BIGINT(20) NOT NULL ,
ADD INDEX `fk_customerfeedbackRecord_customer_idx` (`CustomerId` ASC) ;
ALTER TABLE `customerfeedbackrequest` 
ADD CONSTRAINT `fk_customerfeedbackRecord_customer`
  FOREIGN KEY (`CustomerId`)
  REFERENCES `customer` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;


