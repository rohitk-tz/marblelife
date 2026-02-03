ALTER TABLE `marketingleadcalldetail` 
ADD COLUMN `TagId` BIGINT(20) NULL DEFAULT NULL AFTER `IsDeleted`,
ADD INDEX `fk_MarketingLeadCallDetail_tag_idx` (`TagId` ASC);
ALTER TABLE `marketingleadcalldetail` 
ADD CONSTRAINT `fk_MarketingLeadCallDetail_tag`
  FOREIGN KEY (`TagId`)
  REFERENCES `tag` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;
  
  
update marketingleadcalldetail set tagId = 2  where franchiseeid is null;
update marketingleadcalldetail set tagId = 1 where franchiseeid is not null;


ALTER TABLE `marketingleadcalldetail` 
DROP FOREIGN KEY `fk_MarketingLeadCallDetail_tag`;
ALTER TABLE `marketingleadcalldetail` 
CHANGE COLUMN `TagId` `TagId` BIGINT(20) NOT NULL ;
ALTER TABLE `marketingleadcalldetail` 
ADD CONSTRAINT `fk_MarketingLeadCallDetail_tag`
  FOREIGN KEY (`TagId`)
  REFERENCES `tag` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;
