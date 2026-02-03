ALTER TABLE calldetailsreportnotes
ADD EditTimestamp datetime NULL default NULL;

ALTER TABLE `calldetailsreportnotes`
ADD COLUMN `EmailId` Varchar(128) Null DEFAULT Null;