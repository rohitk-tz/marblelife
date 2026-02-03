ALTER TABLE `annualreporttype` 
ADD COLUMN `Alias` VARCHAR(15) NULL AFTER `isDeleted`;


UPDATE `annualreporttype` SET `Alias`='1' WHERE `Id`='3';
UPDATE `annualreporttype` SET `Alias`='1' WHERE `Id`='4';
UPDATE `annualreporttype` SET `Alias`='2' WHERE `Id`='1';
UPDATE `annualreporttype` SET `Alias`='2' WHERE `Id`='2';
UPDATE `annualreporttype` SET `Alias`='2' WHERE `Id`='5';
UPDATE `annualreporttype` SET `Alias`='3' WHERE `Id`='6';
UPDATE `annualreporttype` SET `Alias`='5' WHERE `Id`='7';
UPDATE `annualreporttype` SET `Alias`='5' WHERE `Id`='8';
UPDATE `annualreporttype` SET `Alias`='5' WHERE `Id`='9';
UPDATE `annualreporttype` SET `Alias`='4' WHERE `Id`='10';
UPDATE `annualreporttype` SET `Alias`='6' WHERE `Id`='11';
UPDATE `annualreporttype` SET `Alias`='6' WHERE `Id`='12';
UPDATE `annualreporttype` SET `Alias`='7' WHERE `Id`='13';
UPDATE `annualreporttype` SET `Alias`='7' WHERE `Id`='14';
UPDATE `annualreporttype` SET `Alias`='8' WHERE `Id`='15';
UPDATE `annualreporttype` SET `Alias`='9' WHERE `Id`='16';
UPDATE `annualreporttype` SET `Alias`='9' WHERE `Id`='17';
UPDATE `annualreporttype` SET `Alias`='10' WHERE `Id`='18';
UPDATE `annualreporttype` SET `Alias`='11' WHERE `Id`='19';
UPDATE `annualreporttype` SET `Alias`='11' WHERE `Id`='20';
UPDATE `annualreporttype` SET `Alias`='12' WHERE `Id`='21';
UPDATE `annualreporttype` SET `Alias`='13' WHERE `Id`='22';
UPDATE `annualreporttype` SET `Alias`='14' WHERE `Id`='23';
