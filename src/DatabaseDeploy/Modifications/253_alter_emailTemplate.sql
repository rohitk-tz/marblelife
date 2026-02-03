ALTER TABLE `emailtemplate` 
ADD COLUMN `isActive` BIT(1) NOT NULL DEFAULT b'1' AFTER `IsDeleted`;



UPDATE `emailtemplate` SET `Title`='Document Upload To Franchisee' WHERE `Id`='24';
