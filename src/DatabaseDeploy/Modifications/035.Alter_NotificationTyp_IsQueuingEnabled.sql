ALTER TABLE `notificationtype` 
ADD COLUMN `IsQueuingEnabled` BIT(1) NOT NULL DEFAULT b'1' AFTER `Description`;


ALTER TABLE `check` 
DROP FOREIGN KEY `fk_Check_lookup1`;
ALTER TABLE `check` 
CHANGE COLUMN `AccountTypeId` `AccountTypeId` BIGINT(20) NULL ,
CHANGE COLUMN `AccountNumber` `AccountNumber` VARCHAR(64) NULL ;
ALTER TABLE `check` 
ADD CONSTRAINT `fk_Check_lookup1`
  FOREIGN KEY (`AccountTypeId`)
  REFERENCES `lookup` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;


  UPDATE `lookup` SET `Name`='Charge Card On File' WHERE `Id`='45';
UPDATE `lookup` SET `Name`='ECheck On File' WHERE `Id`='46';


ALTER TABLE `notificationemailrecipient` 
DROP FOREIGN KEY `fk_NotificationEmailRecipient_OrganizationRoleUser`;
ALTER TABLE `notificationemailrecipient` 
CHANGE COLUMN `OrganizationRoleUserId` `OrganizationRoleUserId` BIGINT(20) NULL ;
ALTER TABLE `notificationemailrecipient` 
ADD CONSTRAINT `fk_NotificationEmailRecipient_OrganizationRoleUser`
  FOREIGN KEY (`OrganizationRoleUserId`)
  REFERENCES `organizationroleuser` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;

  

Set @lookuptypeId = 15;

call createlookuptype(@lookuptypeId, 'RecipientType','');

call createlookup(127, @lookuptypeId, 'TO','TO');
call createlookup(128, @lookuptypeId, 'CC','CC');
call createlookup(129, @lookuptypeId, 'BCC','BCC');


ALTER TABLE `notificationemailrecipient` 
ADD COLUMN `RecipientTypeId` BIGINT(20) NULL AFTER `RecipientEmail`;

update makalu.notificationemailrecipient set RecipientTypeId=127 where Id>0;


ALTER TABLE `notificationemailrecipient` 
CHANGE COLUMN `RecipientTypeId` `RecipientTypeId` BIGINT(20) NOT NULL  DEFAULT '127';



